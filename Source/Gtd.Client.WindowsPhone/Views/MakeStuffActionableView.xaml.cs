using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Gtd.Client.Core.Models;
using Gtd.Client.Core.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Gtd.Client.WindowsPhone.Views
{
    public partial class MakeStuffActionableView : MvxPhonePage
    {
        public MakeStuffActionableView()
        {
            InitializeComponent();
        }

        // TODO: Code-Behind Hacks for now to wire up app bar on page
        private void AppBarTrashItButton_OnClick(object sender, EventArgs e)
        {
            // when Trash It icon is tapped...
            // get a hold of the associated ViewModel again via the ViewModel property
            // and call the TrashStuffCommand
            ((MakeStuffActionableViewModel)ViewModel).TrashStuffCommand.Execute(null);
        }

        private void AppBarNewProjectButton_OnClick(object sender, EventArgs e)
        {
            ((MakeStuffActionableViewModel)ViewModel).NewProjectCommand.Execute(null);
        }

        // TODO: More code-behind app bar hacks
        private void AppBarSaveActionButton_OnClick(object sender, EventArgs e)
        {
            ((MakeStuffActionableViewModel)ViewModel).SaveNewAction.Execute(null);
        }

        void LongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (LLS_projectList != null && LLS_projectList.SelectedItem != null)
            {
                var selectedItem = (Project)LLS_projectList.SelectedItem;

                ((MakeStuffActionableViewModel) ViewModel).Project = selectedItem;
            }
        }
    }
}