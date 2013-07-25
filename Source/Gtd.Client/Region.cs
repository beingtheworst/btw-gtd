using System.Collections.Generic;
using System.Windows.Forms;

namespace Gtd.Client
{
    public sealed class Region 
    {
        readonly Control _container;
        readonly IDictionary<string,Control> _controls= new Dictionary<string, Control>();
        Control _activeControl;

        public Region(Control container)
        {
            _container = container;
        }

        public void RegisterDock(UserControl control, string key)
        {
            _container.Sync(() =>
                {
                    control.Visible = false;
                    control.Dock = DockStyle.Fill;
                    _container.Controls.Add(control);

                    _controls.Add(key, control);
                });
        }

        public void SwitchTo(string key)
        {
            var requestedControl = _controls[key];
            if (requestedControl == _activeControl)
                return;

            _container.Sync(() =>
                {
                    

                    if (_activeControl != null)
                    {
                        _activeControl.Visible = false;
                    }

                    requestedControl.BringToFront();
                    requestedControl.Visible = true;
                    
                    _activeControl = requestedControl;
                    
                });
        }
    }
}