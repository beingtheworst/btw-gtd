namespace Gtd.Client
{
    partial class InboxView
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this._toProject = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._capture = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(13, 14);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(436, 277);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(3, 308);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Archive";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _toProject
            // 
            this._toProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._toProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._toProject.FormattingEnabled = true;
            this._toProject.Location = new System.Drawing.Point(328, 308);
            this._toProject.Name = "_toProject";
            this._toProject.Size = new System.Drawing.Size(121, 21);
            this._toProject.TabIndex = 2;
            this._toProject.DropDown += new System.EventHandler(this._toProject_DropDown);
            this._toProject.SelectedIndexChanged += new System.EventHandler(this._toProject_SelectedIndexChanged);
            this._toProject.SelectionChangeCommitted += new System.EventHandler(this._toProject_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(276, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Move to";
            // 
            // _capture
            // 
            this._capture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._capture.Location = new System.Drawing.Point(84, 308);
            this._capture.Name = "_capture";
            this._capture.Size = new System.Drawing.Size(75, 23);
            this._capture.TabIndex = 4;
            this._capture.Text = "Capture";
            this._capture.UseVisualStyleBackColor = true;
            this._capture.Click += new System.EventHandler(this._capture_Click);
            // 
            // InboxView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._capture);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._toProject);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Name = "InboxView";
            this.Size = new System.Drawing.Size(463, 344);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox _toProject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _capture;
    }
}
