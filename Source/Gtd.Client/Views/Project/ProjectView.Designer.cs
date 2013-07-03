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
            this._projectName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _actionList
            // 
            this._actionList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._actionList.FormattingEnabled = true;
            this._actionList.IntegralHeight = false;
            this._actionList.Location = new System.Drawing.Point(0, 45);
            this._actionList.Name = "_actionList";
            this._actionList.Size = new System.Drawing.Size(479, 322);
            this._actionList.TabIndex = 0;
            // 
            // _projectName
            // 
            this._projectName.AutoSize = true;
            this._projectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._projectName.Location = new System.Drawing.Point(3, 18);
            this._projectName.Name = "_projectName";
            this._projectName.Size = new System.Drawing.Size(68, 24);
            this._projectName.TabIndex = 1;
            this._projectName.Text = "Project";
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._projectName);
            this.Controls.Add(this._actionList);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(479, 367);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox _actionList;
        private System.Windows.Forms.Label _projectName;
    }
}
