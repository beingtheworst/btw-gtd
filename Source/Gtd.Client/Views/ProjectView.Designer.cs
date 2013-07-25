namespace Gtd.Client.Views.Project
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
            this._projectName = new System.Windows.Forms.Label();
            this.Outcome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CompletedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this._grid = new System.Windows.Forms.DataGridView();
            this._addAction = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.SuspendLayout();
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
            // Outcome
            // 
            this.Outcome.DataPropertyName = "Outcome";
            this.Outcome.HeaderText = "Outcome";
            this.Outcome.Name = "Outcome";
            // 
            // CompletedColumn
            // 
            this.CompletedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CompletedColumn.DataPropertyName = "Completed";
            this.CompletedColumn.FillWeight = 1F;
            this.CompletedColumn.HeaderText = "";
            this.CompletedColumn.Name = "CompletedColumn";
            this.CompletedColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CompletedColumn.Width = 25;
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AllowUserToOrderColumns = true;
            this._grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CompletedColumn,
            this.Outcome});
            this._grid.Location = new System.Drawing.Point(0, 45);
            this._grid.Name = "_grid";
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(479, 282);
            this._grid.TabIndex = 2;
            // 
            // _addAction
            // 
            this._addAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._addAction.Location = new System.Drawing.Point(7, 333);
            this._addAction.Name = "_addAction";
            this._addAction.Size = new System.Drawing.Size(98, 23);
            this._addAction.TabIndex = 3;
            this._addAction.Text = "Add Action";
            this._addAction.UseVisualStyleBackColor = true;
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._addAction);
            this.Controls.Add(this._grid);
            this.Controls.Add(this._projectName);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(479, 367);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _projectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Outcome;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CompletedColumn;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.Button _addAction;
    }
}
