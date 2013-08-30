using System;
using System.Windows.Controls;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Gtd.Client.Core.ViewModels;

namespace Gtd.Client.WindowsPhone.Views
{
    public partial class InboxView : MvxPhonePage
    {
        public InboxView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: This is part of the Code-Behind temp hack as described in InboxView.xaml

            // when the selection changes...

            if (e.AddedItems.Count == 1)
            {
                // means somebody selected something

                // nav to the selected item by just calling ViewModel method directly
                // using the first things selected in the list we are passed in
                //((InboxViewModel)ViewModel).MakeStuffActionableCommand.Execute(e.AddedItems[0]);
                //((InboxViewModel)ViewModel).MoveStuffToProject.Execute(null);

                ((InboxViewModel)ViewModel).MakeStuffActionableCommand.Execute(e.AddedItems[0]);


                // clear the ListBox selection (sender) that's been passed in
                ((ListBox)sender).SelectedIndex = -1;
            }
        }


        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            // TODO: Code-Behind Hack again.  See bindable note at bottom of DetailView.xaml

            // when Add Stuff icon is tapped...
            // get a hold of the associated ViewModel again via the ViewModel property
            // and call the AddStuffCommand
            ((InboxViewModel)ViewModel).AddStuffCommand.Execute(null);
        }
    }
}