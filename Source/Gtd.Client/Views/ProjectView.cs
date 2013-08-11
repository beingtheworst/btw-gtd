using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Windows.Forms;
using Gtd.Client.Models;
using System.Linq;
using Gtd.ClientCore.Controllers;

namespace Gtd.Client.Views.Project
{
    public partial class ProjectView : UserControl, IProjectView
    {
        

        public ProjectView()
        {
            InitializeComponent();
            _grid.DataSource = _source;

            _addAction.Click += (sender, args) => _whenAddActionClicked(_project);

        }

        Region _region;

        public void SubscribeToDragStart(Action<DragActions> callback)
        {
            _grid.MouseDown += (sender, args) =>
                {
                    
                    if (args.Button != MouseButtons.Left)
                        return;
                    var index = _grid.HitTest(args.X, args.Y);
                    var rowIndex = index.RowIndex;
                    if (rowIndex == -1)
                        return;

                    //var selectedRows = _grid.SelectedRows;
                    DragActions subject;
                    var selectedRows = _grid.SelectedRows.Cast<DataGridViewRow>().ToArray();
                    if (selectedRows.Select(v => v.Index).Contains(rowIndex))
                    {
                        // our hit test includes selection.
                        // drag entire selection
                        subject = DragActions.CreateRequest(selectedRows
                            .Select(r => r.DataBoundItem as ActionDisplay)
                            .Select(a => a.Model));
                    }
                    else
                    {
                        var display = (ActionDisplay)_grid.Rows[index.RowIndex].DataBoundItem;
                        subject = DragActions.CreateRequest(new[] { display.Model });
                    }



                    callback(subject);
                    DoDragDrop(subject.Request, DragDropEffects.Move);
                };
        }

        public void AttachTo(Region region)
        {
            region.RegisterDock(this, "project-view");

            _region = region;
        }

        readonly BindingSource _source = new BindingSource();


        void IProjectView.ShowView(FilteredProject project)
        {
            _region.SwitchTo("project-view");

            this.Sync(() =>
                {
                    _project = project.Info.ProjectId;
                    _projectName.Text = string.Format("{0} ({1})", project.Info.Outcome, project.ActionCount);

                    // TODO: smarter update for the case when we remove item
                    if (_source.Count == project.FilteredActions.Count)
                    {
                        for (int i = 0; i < project.FilteredActions.Count; i++)
                        {
                            _source[i] = new ActionDisplay(project.FilteredActions[i], this);
                        }
                        return;
                    }

                    _source.Clear();
                    foreach (var action in project.FilteredActions)
                    {
                        _source.Add(new ActionDisplay(action, this));
                    }
                });

            
        }

        Action<ActionId, string> _whenOutcomeChanged = (id, s) => { };
        Action<ActionId> _whenActionCompleted = id => { };
        Action<ProjectId> _whenAddActionClicked = id => { };
        

        public void SubscribeOutcomeChanged(Action<ActionId, string> e)
        {
            _whenOutcomeChanged = e;
        }

        public void SubscribeActionCompleted(Action<ActionId> e)
        {
            _whenActionCompleted = e;
        }

        public void SubscribeAddActionClicked(Action<ProjectId> e)
        {
            _whenAddActionClicked = e;
        }

        ProjectId _project;


        public sealed class ActionDisplay
        {
            public readonly ImmutableAction Model;
            readonly ProjectView _controller;

            public ActionDisplay(ImmutableAction model, ProjectView controller)
            {
                Model = model;
                _controller = controller;
            }


            public string Outcome
            {
                get { return Model.Outcome; }
                set
                {
                    _controller._whenOutcomeChanged(Model.ActionId, value);
                }
            }

            public bool Completed
            {
                get { return Model.Completed; }
                set
                {
                    if (value)
                        _controller._whenActionCompleted(Model.ActionId);
                }
            }

            public override string ToString()
            {
                return Model.Outcome;
            }
        }
    }
}
