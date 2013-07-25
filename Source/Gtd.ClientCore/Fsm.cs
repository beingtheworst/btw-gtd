using System;
using System.Collections.Generic;
using System.Linq;

namespace Gtd.Client
{
    /// <summary>
    /// Generic finite state machine and builder with the fluent API.
    /// It is used by the server to manage different SEDA messages
    /// differently based on its state.
    /// <p>
    /// http://en.wikipedia.org/wiki/Finite-state_machine
    /// </p>
    /// </summary>
    /// <remarks>
    /// This code is based on FSM from EventStore:
    /// https://github.com/EventStore/EventStore/tree/master/src/EventStore/EventStore.Core/Services/VNode 
    /// </remarks>
    /// <typeparam name="TEnum"></typeparam>
    public sealed class FiniteStateMachine<TEnum> : IHandle<Message>
       where TEnum : struct
    {
        readonly Func<int> _getState;
        readonly TEnum[] _states;

        readonly IDictionary<Type, Action<TEnum, Message>>[] _handlersByState;
        readonly Action<TEnum, Message>[] _defaultHandlersByState;

        public FiniteStateMachine(
            TEnum[] states,
            Func<int> getState,
            IDictionary<Type, Action<TEnum, Message>>[] handlersByState,
            Action<TEnum, Message>[] defaultHandlersByState)
        {
            _getState = getState;
            _states = states;
            _handlersByState = handlersByState;
            _defaultHandlersByState = defaultHandlersByState;
        }

        public void Handle(Message message)
        {
            var stateNum = _getState();
            var state = _states[stateNum];

            var handlers = _handlersByState[stateNum];

            var type = message.GetType();

            if (TryHandle(state, handlers, message, type))
                return;
            do
            {
                if (null == type)
                    return;
                type = type.BaseType;
                if (TryHandle(state, handlers, message, type))
                    return;
            } while (type != typeof(Message));

            if (_defaultHandlersByState[stateNum] != null)
            {
                _defaultHandlersByState[stateNum](state, message);
                return;
            }
            throw new InvalidOperationException(
                string.Format("Unexpected message {0} in state {1}", message, state));
        }

        static bool TryHandle(
            TEnum state,
            IDictionary<Type, Action<TEnum, Message>> dictionary,
            Message msg,
            Type usedType)
        {
            if (null == dictionary)
                return false;

            Action<TEnum, Message> value;
            if (!dictionary.TryGetValue(usedType, out value))
                return false;

            value(state, msg);
            return true;
        }
    }

    public sealed class FsmBuilder<T> where T : struct
    {
        readonly T[] _allStates;

        readonly Dictionary<Type, Action<T, Message>>[] _handlersByState;
        readonly Action<T, Message>[] _defaultHandlersByState;


        public FsmBuilder()
        {
            _allStates = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            var count = _allStates.Cast<int>().Max() + 1;

            _handlersByState = new Dictionary<Type, Action<T, Message>>[count];
            _defaultHandlersByState = new Action<T, Message>[count];
        }

        internal void AddDefaultHandler(T state, Action<T, Message> handler)
        {
            var num = Convert.ToInt32(state);
            if (null != _defaultHandlersByState[num])
                throw new InvalidOperationException(string.Format("Default handler already defined for state {0}", state));
            _defaultHandlersByState[num] = handler;
        }
        internal void AddHandler<TActualMessage>(T state, Action<T, Message> handler)
            where TActualMessage : Message
        {
            var num = Convert.ToInt32(state);
            var dict = _handlersByState[num] ?? (_handlersByState[num] = new Dictionary<Type, Action<T, Message>>());
            var type = typeof(TActualMessage);
            if (dict.ContainsKey(type))
                throw new InvalidOperationException(
                    string.Format("{0} handler already defined for state {1}", type, state));
            dict.Add(type, handler);
        }

        public FsmStatesDefinition<T> InState(T state)
        {
            return new FsmStatesDefinition<T>(this, state);
        }
        public FsmStatesDefinition<T> InStates(params T[] states)
        {
            return new FsmStatesDefinition<T>(this, states);
        }
        public FsmStatesDefinition<T> InAllStates()
        {
            return new FsmStatesDefinition<T>(this, _allStates);
        }

        public FsmStatesDefinition<T> InOtherStates()
        {
            var unused = _handlersByState
                .Select((x, i) => x == null ? i : -1)
                .Where(x => x >= 0)
                .Select(i => _allStates[i]).ToArray();
            return new FsmStatesDefinition<T>(this, unused);
        }

        public FiniteStateMachine<T> Build(Func<int> getState)
        {
            return new FiniteStateMachine<T>(_allStates, getState, _handlersByState, _defaultHandlersByState);
        }
    }

    public sealed class FsmStatesDefinition<T> where T : struct
    {
        public FsmBuilder<T> Builder { get; private set; }
        public T[] States { get; private set; }


        internal FsmStatesDefinition(FsmBuilder<T> builder, params T[] states)
        {
            Builder = builder;
            States = states;
        }

        public FsmStatesHandler<Message, T> WhenOther()
        {
            return new FsmStatesHandler<Message, T>(this, true);
        }
        public FsmStatesDefinition<T> InState(T state)
        {
            return Builder.InState(state);
        }
        public FsmStatesHandler<TMessage, T> When<TMessage>() where TMessage : Message
        {
            return new FsmStatesHandler<TMessage, T>(this, false);
        }



        public FiniteStateMachine<T> Build(Func<int> getState)
        {
            return Builder.Build(getState);
        }
    }

    public sealed class FsmStatesHandler<TMessage, T>
        where TMessage : Message
        where T : struct
    {
        readonly FsmStatesDefinition<T> _definition;
        readonly bool _isDefault;

        public FsmStatesHandler(FsmStatesDefinition<T> definition, bool isDefault = true)
        {
            _definition = definition;
            _isDefault = isDefault;
        }

        public FsmStatesDefinition<T> Do(Action<TMessage> handler)
        {
            foreach (var state in _definition.States)
            {
                if (_isDefault)
                {
                    _definition.Builder.AddDefaultHandler(state, (_, m) => handler((TMessage)m));
                }
                else
                {
                    _definition.Builder.AddHandler<TMessage>(state, (_, m) => handler((TMessage)m));
                }
            }
            return _definition;
        }
        public FsmStatesDefinition<T> Ignore()
        {
            foreach (var state in _definition.States)
            {
                if (_isDefault)
                {
                    _definition.Builder.AddDefaultHandler(state, (_, __) => { });
                }
                else
                {
                    _definition.Builder.AddHandler<TMessage>(state, (_, __) => { });
                }
            }
            return _definition;
        }

        public FsmStatesDefinition<T> Do(Action<T, TMessage> handler)
        {
            foreach (var state in _definition.States)
            {
                if (_isDefault)
                {
                    _definition.Builder.AddDefaultHandler(state, (s, m) => handler(s, (TMessage)m));
                }
                else
                {
                    _definition.Builder.AddHandler<TMessage>(state, (s, m) => handler(s, (TMessage)m));
                }
            }
            return _definition;
        }
    }
}