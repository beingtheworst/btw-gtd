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
    public partial class AddStuffView : MvxPhonePage
    {
        public AddStuffView()
        {
            InitializeComponent();
        }

        // TODO: COde behind Hack to set the focus on the textbox initially when page loads
        void AddStuffView_OnLoaded(object sender, RoutedEventArgs e)
        {
            tbDesc.Focus();
        }
    }
}