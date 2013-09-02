using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Gtd.Client.WindowsPhone.Views
{
    public partial class CreateNewProjectView : MvxPhonePage
    {
        public CreateNewProjectView()
        {
            InitializeComponent();
        }

        // TODO: Code-behind hack to set focus to text box on Page Load
        void CreateNewProjectView_OnLoaded(object sender, RoutedEventArgs e)
        {
            tbProjectName.Focus();
            // should could be some initial text in there , so move cursor to end of it
            tbProjectName.Select(tbProjectName.Text.Length, 0);
        }
    }
}