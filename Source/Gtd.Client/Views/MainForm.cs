using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Gtd.Client.Controllers;
using Gtd.Shell.Filters;

namespace Gtd.Client
{
    public partial class MainForm : Form, ILogView, INavigateBackView, IMainMenu
    {
        
        public readonly Region MainRegion;
        public readonly Region NavigationRegion;

        public MainForm()
        {
            InitializeComponent();

            MainRegion = new Region(splitContainer1.Panel2);
            NavigationRegion = new Region(splitContainer1.Panel1);

            DisplayFilters(FilterCriteria.LoadAllFilters().ToList());
        }

        public void ShowInboxMenu()
        {
            this.Sync(() =>
                {
                    _menuGoToInbox.Visible = false;
                    _menuCaptureThought.Visible = true;
                    _menuDefineProject.Visible = true;
                });
        }

        public void SubscribeToNavigateBack(Action handler)
        {
            _menuGoBack.Click += (sender, args) => handler();
        }

        sealed class FilterDisplay
        {
            public readonly IFilterCriteria Criteria;

            public FilterDisplay(IFilterCriteria criteria)
            {
                Criteria = criteria;
            }
            public override string ToString()
            {
                return Criteria.Title;
            }
        }

        public void DisplayFilters(ICollection<IFilterCriteria> filters)
        {
            if (filters.Count == 0)
                throw new ArgumentException("Filters can't be empty","filters");
            this.Sync(() =>
                {
                    foreach (var filterCriteria in filters)
                    {
                        _filter.Items.Add(new FilterDisplay(filterCriteria));
                    }
                    _filter.SelectedIndex = 0;
                });
            
        }


        public void ShowProjectMenu(ProjectId id)
        {
            this.Sync(() =>
                {
                    _menuCaptureThought.Visible = true;
                    _menuGoToInbox.Visible = true;
                    _menuDefineProject.Visible = true;

                });
        }

        public void Log(string toString)
        {
            // _log is the name property of the RichTextBox on the bottom of MainForm
            // Sync is an extension method we added to this MainForm control (below) that takes a C# Action delegate
            _log.Sync(() =>
                {    
                    var format = string.Format("{0:HH:mm:ss} {1}{2}", DateTime.UtcNow, toString, Environment.NewLine);
                    // append the formatted string the RichTextBox and scroll down
                    _log.AppendText(format);
                    _log.ScrollToCaret();
                });
        }

        public void EnableNavigateBackButton(bool enabled)
        {
            this.Sync(() => _menuGoBack.Enabled = enabled);
        }

        public void SubscribeToAddStuffClicked(Action callback)
        {
            _menuCaptureThought.Click += (sender, args) => callback();
        }

        public void SubscribeToDefineProjectClicked(Action callback)
        {
            _menuDefineProject.Click += (sender, args) => callback();
        }

        public void SubscribeToGotoInboxClicked(Action callback)
        {
            _menuGoToInbox.Click += (sender, args) => callback();
        }

        public void SubscribeToSelectedFilterChanged(Action<IFilterCriteria> callback)
        {
            var item = (FilterDisplay)_filter.SelectedItem;
            callback(item.Criteria);
        }
    }

    // Extension method we added to this MainForm control that takes a C# Action delegate
    // The MainForm itself, as well as "child View forms", can call this Sync method
    // to invoke a View action they provide on the same UI thread as the MainForm (mandatory Windows thing).
    public static class ExtendControl
    {
        public static void Sync(this Control self, Action act)
        {
            if (self.InvokeRequired)
            {
                self.Invoke(act);
            }
            else
            {
                act();
            }
        }
    }

}
