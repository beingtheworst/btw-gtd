﻿namespace Gtd.Client.Views.AddStuff
{
    public sealed class AddStuffController : IHandle<Ui.AddStuffClicked>
    {
        readonly AddStuffForm _form;
        readonly IMessageQueue _queue;

        AddStuffController(AddStuffForm form, IMessageQueue queue)
        {
            _form = form;
            _queue = queue;
        }

        #region Static Wire method to link together the UI presentation layer (View) with...
        // kind of the "Application's (App's) UI domain MODEL language" defined in Ui.cs
        // All AddStuffController knows is that it gets handed a View
        // that it controls (AddStuffForm) and then it has two other injections points,
        // a Bus and a MainQueue. In this case its ISubscriber event source to tell the 
        // "uiBus" to call its Hanlde method for specific events it cares about,
        // and its IMessageQueue, the target it notifies when its stuff/events happen, is "mainQueue".
        #endregion
        public static void Wire(AddStuffForm form, ISubscriber source, IMessageQueue target)
        {
            // when setup code calls this static Wire method
            // we create a new instance of this controller
            var controller = new AddStuffController(form, target);

            // and tell the uiBus to register this new controller instance
            // as something that should be called when Ui.CaptureThoughtClicked events happen.
            // "Hey, I implement IHandle<Ui.CaptureThoughtClicked>, so have something call me when it
            // happens so that I can react to it with the code inside of my Handle method below."
            source.Subscribe<Ui.AddStuffClicked>(controller);
        }

        // we registered to be called when Ui.CaptureThoughtClicked happens, so when it does,
        // this Handle method gets called to perform the work related to handling the event.
        public void Handle(Ui.AddStuffClicked message)
        {
            // tell the AddStuffForm I control to use its TryGetUserInput method and
            // if it is able to get input, call me back with this method I provide =>
            _form.TryGetUserInput(s => _queue.Enqueue(new Ui.AddStuffWizardCompleted(s)));
        }
    }
}
