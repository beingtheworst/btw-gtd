namespace Gtd.Client
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this._menuGoToInbox = new System.Windows.Forms.ToolStripMenuItem();
            this._menuCaptureThought = new System.Windows.Forms.ToolStripMenuItem();
            this._menuDefineProject = new System.Windows.Forms.ToolStripMenuItem();
            this._filter = new System.Windows.Forms.ToolStripComboBox();
            this._log = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuGoToInbox,
            this._menuCaptureThought,
            this._menuDefineProject,
            this._filter});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(693, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // _menuGoToInbox
            // 
            this._menuGoToInbox.Name = "_menuGoToInbox";
            this._menuGoToInbox.Size = new System.Drawing.Size(83, 23);
            this._menuGoToInbox.Text = "Go To Inbox";
            // 
            // _menuCaptureThought
            // 
            this._menuCaptureThought.Name = "_menuCaptureThought";
            this._menuCaptureThought.Size = new System.Drawing.Size(69, 23);
            this._menuCaptureThought.Text = "Add Stuff";
            // 
            // _menuDefineProject
            // 
            this._menuDefineProject.Name = "_menuDefineProject";
            this._menuDefineProject.Size = new System.Drawing.Size(93, 23);
            this._menuDefineProject.Text = "Define Project";
            // 
            // _filter
            // 
            this._filter.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._filter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._filter.IntegralHeight = false;
            this._filter.Name = "_filter";
            this._filter.Size = new System.Drawing.Size(121, 23);
            this._filter.SelectedIndexChanged += new System.EventHandler(this._filter_SelectedIndexChanged);
            // 
            // _log
            // 
            this._log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._log.Dock = System.Windows.Forms.DockStyle.Fill;
            this._log.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._log.Location = new System.Drawing.Point(0, 0);
            this._log.Name = "_log";
            this._log.ReadOnly = true;
            this._log.Size = new System.Drawing.Size(691, 98);
            this._log.TabIndex = 2;
            this._log.Text = "";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this._log);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 377);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(693, 100);
            this.panel1.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(693, 350);
            this.splitContainer1.SplitterDistance = 231;
            this.splitContainer1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 477);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        protected internal System.Windows.Forms.ToolStripMenuItem _menuCaptureThought;
        private System.Windows.Forms.ToolStripMenuItem _menuDefineProject;
        private System.Windows.Forms.RichTextBox _log;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem _menuGoToInbox;
        private System.Windows.Forms.ToolStripComboBox _filter;
    }
}

