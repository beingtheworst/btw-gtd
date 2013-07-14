using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Gtd.Shell;

namespace Gtd.Client
{
    // this file contains classes of a simple in-memory
    // messaging infrastructure needed for SEDA architecture

    
    /// <summary>
    /// Defines instance of the message handling object (class which
    /// contains code to be executed when some message arrives)
    /// </summary>
    internal interface IMessageHandler
    {
        /// <summary>
        /// Name of this handler (for debugging purposes)
        /// </summary>
        string HandlerName { get; }
        /// <summary>
        /// Attempts to handle the message
        /// </summary>
        /// <param name="message">actuall message to handle</param>
        /// <returns>True if handled</returns>
        bool TryHandle(Message message);
        /// <summary>
        /// Explicit comparison between multiple handlers (to allow
        /// preventing multiple calls in complex subscription scenarios)
        /// </summary>
        /// <param name="handler">another handler object</param>
        /// <returns>True if both handlers are the same</returns>
        bool IsSame(object handler);
    }
    /// <summary>
    /// Marks the class with the capability to handle message <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="TMessage">type of the message this class can handle</typeparam>
    public interface IHandle<TMessage> where TMessage : Message
    {
        void Handle(TMessage message);
    }

    /// <summary>
    /// Inheritors can manage subscriptions for various handlers
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Subscribes handler (capable of handling messages of <typeparamref name="T"/>)
        /// to appropriate messages
        /// </summary>
        /// <typeparam name="T">type of message to subscribe to</typeparam>
        /// <param name="handler">instance of the handler to subscribe</param>
        void Subscribe<T>(IHandle<T> handler) where T : Message;
        /// <summary>
        /// Unsubscribes handler from all messages
        /// </summary>
        /// <typeparam name="T">type of the messages this handler can deal with</typeparam>
        /// <param name="handler">actual instance of handler to unsubscribe</param>
        void Unsubscribe<T>(IHandle<T> handler) where T : Message;
    }
    /// <summary>
    /// Provides publishing interface (can accept message and send them around)
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Publishes instance of the message
        /// </summary>
        void Publish(Message message);
    }

    public interface IQueue
    {
        void Enqueue(Message message);
    }

    


    /// <summary>
    /// Named instance, which can both publish messages and sibscribe handlers to them
    /// </summary>
    public interface IBus : IPublisher, ISubscriber
    {
        /// <summary>
        /// Human-readable name for debugging purposes
        /// </summary>
        string BusName { get; }
    }

    /// <summary>
    /// Possible implementation of typed <see cref="IMessageHandler"/>
    /// which can handle only a single type of message and allows providing 
    /// human-readable name
    /// </summary>
    /// <typeparam name="T">type of the message this class can deal with</typeparam>
    public sealed class MessageHandler<T> : IMessageHandler where T : Message
    {

        readonly IHandle<T> _handler;
        public MessageHandler(IHandle<T> handler, string handlerName)
        {
            Contract.Requires(handler != null);
            HandlerName = handlerName ?? "";
            _handler = handler;
        }

        public string HandlerName { get; private set; }

        /// <summary>
        /// Attempts to handle the message
        /// </summary>
        /// <param name="message">actuall message to handle</param>
        /// <returns>True if handled</returns>
        public bool TryHandle(Message message)
        {
            var msg = message as T;

            if (msg != null)
            {
                _handler.Handle(msg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Explicit comparison between multiple handlers (to allow
        /// preventing multiple calls in complex subscription scenarios)
        /// </summary>
        /// <param name="handler">another handler object</param>
        /// <returns>True if both handlers are the same</returns>
        public bool IsSame(object handler)
        {
            return ReferenceEquals(_handler, handler);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(HandlerName) ? _handler.ToString() : HandlerName;
        }
    }

    /// <summary>
    /// Implementation of <see cref="IEnvelope"/> which simply provides
    /// specific publisher as recipient for replies.
    /// </summary>
    public class PublishEnvelope : IEnvelope
    {
        private readonly IPublisher _publisher;

        public PublishEnvelope(IPublisher publisher)
        {
            _publisher = publisher;
        }

        /// <summary>
        /// Sends back message of type <typeparamref name="TMessage"/> back to the original sender
        /// </summary>
        /// <typeparam name="TMessage">type of the message to send back</typeparam>
        /// <param name="message">message that is being sent as reply</param>
        public void ReplyWith<TMessage>(TMessage message) where TMessage : Message
        {
            _publisher.Publish(message);
        }
    }


    /// <summary>
    /// Allows sending messages back to the original sender.
    /// </summary>
    public interface IEnvelope
    {
        /// <summary>
        /// Sends back message of type <typeparamref name="TMessage"/> back to the original sender
        /// </summary>
        /// <typeparam name="TMessage">type of the message to send back</typeparam>
        /// <param name="message">message that is being sent as reply</param>
        void ReplyWith<TMessage>(TMessage message) where TMessage : Message;
    }

    /// <summary>
    /// This helper class dispatches incoming messages to subscribed message handlers.
    /// Dispatching is simply passing a message to a handling method on an available instance.
    /// This allows us to send messages to the bus so that interested classes can 
    /// subscribe to the messages they care about, and the bus will tell them (publish) when they happen.
    /// This is an in-memory structure that all "approved" event messages go through.
    /// In our case, after the AppController reads an event from the queue, it places the event in this Bus
    /// if it has determined that subscribers of the bus should care about the current event right now.
    /// (ex: tell View/UI subscribed controllers like: CaptureThoughtController, InboxController about it)
    /// This class is ported from Event Store LLP's Event Store and is a more polished implementation
    /// of "RedirectToWhen" from Lokad.CQRS
    /// </summary>
    public sealed class InMemoryBus : IBus, IPublisher, ISubscriber, IHandle<Message>
    {

        private readonly Dictionary<Type, List<IMessageHandler>> _typeLookup = new Dictionary<Type, List<IMessageHandler>>();

        /// <summary>
        /// Subscribes handler (capable of handling messages of <typeparamref name="T"/>)
        /// to appropriate messages that go through this bus
        /// </summary>
        /// <typeparam name="T">type of message to subscribe to</typeparam>
        /// <param name="handler">instance of the handler to subscribe</param>
        public void Subscribe<T>(IHandle<T> handler) where T : Message
        {
            Ensure.NotNull(handler, "handler");
            List<IMessageHandler> handlers;
            var type = typeof(T);
            if (!_typeLookup.TryGetValue(type, out handlers))
            {
                _typeLookup.Add(type, handlers = new List<IMessageHandler>());
            }
            if (!handlers.Any(h => h.IsSame(handler)))
            {
                handlers.Add(new MessageHandler<T>(handler, handler.GetType().Name));
            }
        }

        /// <summary>
        /// Unsubscribes handler from all messages
        /// </summary>
        /// <typeparam name="T">type of the messages this handler can deal with</typeparam>
        /// <param name="handler">actual instance of handler to unsubscribe</param>
        public void Unsubscribe<T>(IHandle<T> handler) where T : Message
        {
            Ensure.NotNull(handler, "handler");
            List<IMessageHandler> list;
            if (_typeLookup.TryGetValue(typeof(T), out list))
            {
                list.RemoveAll(x => x.IsSame(handler));
            }
        }

        /// <summary>
        /// Human-readable name for debugging purposes
        /// </summary>
        public string BusName { get; private set; }

        public InMemoryBus(string name)
        {
            BusName = name;
        }


        // TODO: Rinat, why do Publish/Handle have same code with diff names?

        /// <summary>
        /// Publishes instance of the message to all subscribers
        /// </summary>
        public void Publish(Message message)
        {
            Ensure.NotNull(message, "message");
            DispatchByType(message);
        }
        /// <summary>
        /// Publishes instance of the message to all subscribers
        /// </summary>
        public void Handle(Message message)
        {
            Ensure.NotNull(message, "message");
            DispatchByType(message);
        }

        void DispatchByType(Message message)
        {
            var type = message.GetType();
            do
            {
                DispatchByType(message, type);
                type = type.BaseType;
            } while (type != typeof(object));
        }

        void DispatchByType(Message message, Type type)
        {
            List<IMessageHandler> list;
            if (!_typeLookup.TryGetValue(type, out list)) return;

            if (typeof(IFormCommand).IsAssignableFrom(type))
            {
                foreach (var handler in list)
                {
                    var copy = handler;

                    Form.ActiveForm.Invoke(new Action(() => copy.TryHandle(message)));

                }
            }
            else
            {
                foreach (var handler in list)
                {
                    handler.TryHandle(message);
                }
            }
        }
    }

    /// <summary>
    /// Collect or synchronize all domain/system events that we have declared that we care about.
    /// This special handler maintains an in-memory queue, and sequentially passes
    /// messages to the specified message handler (AppController in this case).
    /// This is all done in a separate thread.    
    /// Some of its received events may be "domain events" related to "TrustedSystemAggregate",
    /// and others may be system events that we defined like "Ui.CaptureThoughtClicked".
    /// These are just different event message types that belong to different contexts (ex: Gtd vs Ui).
    /// Whenever somebody does something, that event goes into this in-memory queue to await processing.
    /// All event messages are captured and accumulated in this queue until some code picks up
    /// each message and processes it. (ex: AppController will do that in this case).
    /// </summary>
    public sealed class QueuedHandler : IHandle<Message>, IPublisher, IMessageQueue
    {
        readonly IHandle<Message> _consumer;
        readonly ConcurrentQueue<Message> _queue = new ConcurrentQueue<Message>();
        private static readonly ILogger Log = LogManager.GetLoggerFor<QueuedHandler>();
        Thread _thread;

        readonly int _waitToStopThreadMs;
        private readonly ManualResetEventSlim _stopped = new ManualResetEventSlim(false);

        readonly string _name;
        volatile bool _selfDestruct;


        public QueuedHandler(IHandle<Message> consumer, string name, int waitToStopThreadMs = 10000)
        {
            _consumer = consumer;
            _name = name;
            _waitToStopThreadMs = waitToStopThreadMs;
        }

        /// <summary>
        /// Starts processing of the queue
        /// </summary>
        public void Start()
        {
            if (null != _thread)
                throw new InvalidOperationException("Thread is already running");

            _thread = new Thread(ReadMessagesFromQueue)
            {
                IsBackground = true,
                Name = _name
            };

            _thread.Start();
        }

        /// <summary>
        /// Stops processing of the queue
        /// </summary>
        public void Stop()
        {
            _selfDestruct = true;
            if (null == _thread) return;


            if (!_stopped.Wait(_waitToStopThreadMs))
            {
                throw new InvalidOperationException("Failed to stop thread ");
            }

        }

        void IHandle<Message>.Handle(Message message)
        {
            Ensure.NotNull(message, "message");
            _queue.Enqueue(message);
        }

        void ReadMessagesFromQueue()
        {
            while (!_selfDestruct)
            {
                Message result;

                if (_queue.TryDequeue(out result))
                {
                    try
                    {
                        _consumer.Handle(result);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException(ex, "Error while processing message {0} in queued handler '{1}'.", result, _name);
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
            _stopped.Set();
        }

        void IPublisher.Publish(Message message)
        {
            Ensure.NotNull(message, "message");
            _queue.Enqueue(message);
        }
        /// <summary>
        /// Allows adding messages to the queue
        /// </summary>
        /// <param name="message"></param>
        public void Enqueue(Message message)
        {
            Ensure.NotNull(message, "message");
            _queue.Enqueue(message);
        }
    }

    public interface IMessageQueue
    {
        void Enqueue(Message message);
    }

    /// <summary>
    /// Set of helper methods that are slightly more readable than
    /// plain exception throws (but are equivalent to  them)
    /// </summary>
    public static class Ensure
    {
        public static void NotNull<T>(T argument, string argumentName) where T : class
        {
            Contract.Requires(argument != null);
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        public static void Nonnegative(long number, string argumentName)
        {
            Contract.Requires(number >= 0);
            if (number < 0)
                throw new ArgumentOutOfRangeException(argumentName, argumentName + " should be non negative.");
        }
        public static void Positive(long number, string argumentName)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(argumentName, argumentName + " should be positive.");
        }
    }

}