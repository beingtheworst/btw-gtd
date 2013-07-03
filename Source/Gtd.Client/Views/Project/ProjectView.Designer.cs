namespace Gtd.Client.Views.Actions
{
    partial class ProjectView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._actionList = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // _actionList
            // 
            this._actionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._actionList.FormattingEnabled = true;
            this._actionList.Location = new System.Drawing.Point(0, 0);
            this._actionList.Name = "_actionList";
            this._actionList.Size = new System.Drawing.Size(479, 367);
            this._actionList.TabIndex = 0;
            // 
            // ActionsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._actionList);
            this.Name = "ActionsView";
            this.Size = new System.Drawing.Size(479, 367);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox _actionList;
    }
}
