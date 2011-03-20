namespace MSR.Tools.Predictor
{
	partial class PredictorView
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
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.commandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.predictToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.predictAndEvaluateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.releaseList = new System.Windows.Forms.CheckedListBox();
			this.outputText = new System.Windows.Forms.TextBox();
			this.modelList = new System.Windows.Forms.CheckedListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.releaseSetAll = new System.Windows.Forms.CheckBox();
			this.releaseSetSize = new System.Windows.Forms.NumericUpDown();
			this.menuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.releaseSetSize)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.commandToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(737, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConfigToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openConfigToolStripMenuItem
			// 
			this.openConfigToolStripMenuItem.Name = "openConfigToolStripMenuItem";
			this.openConfigToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.openConfigToolStripMenuItem.Text = "Open config...";
			this.openConfigToolStripMenuItem.Click += new System.EventHandler(this.openConfigToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showFilesToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// showFilesToolStripMenuItem
			// 
			this.showFilesToolStripMenuItem.Name = "showFilesToolStripMenuItem";
			this.showFilesToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
			this.showFilesToolStripMenuItem.Text = "Show files";
			this.showFilesToolStripMenuItem.Click += new System.EventHandler(this.showFilesToolStripMenuItem_Click);
			// 
			// commandToolStripMenuItem
			// 
			this.commandToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.predictToolStripMenuItem,
            this.predictAndEvaluateToolStripMenuItem});
			this.commandToolStripMenuItem.Name = "commandToolStripMenuItem";
			this.commandToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
			this.commandToolStripMenuItem.Text = "Command";
			// 
			// predictToolStripMenuItem
			// 
			this.predictToolStripMenuItem.Name = "predictToolStripMenuItem";
			this.predictToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.predictToolStripMenuItem.Text = "Predict";
			this.predictToolStripMenuItem.Click += new System.EventHandler(this.predictToolStripMenuItem_Click);
			// 
			// predictAndEvaluateToolStripMenuItem
			// 
			this.predictAndEvaluateToolStripMenuItem.Name = "predictAndEvaluateToolStripMenuItem";
			this.predictAndEvaluateToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.predictAndEvaluateToolStripMenuItem.Text = "Predict and Evaluate";
			this.predictAndEvaluateToolStripMenuItem.Click += new System.EventHandler(this.predictAndEvaluateToolStripMenuItem_Click);
			// 
			// releaseList
			// 
			this.releaseList.FormattingEnabled = true;
			this.releaseList.Location = new System.Drawing.Point(8, 32);
			this.releaseList.Name = "releaseList";
			this.releaseList.Size = new System.Drawing.Size(192, 274);
			this.releaseList.TabIndex = 1;
			// 
			// outputText
			// 
			this.outputText.Location = new System.Drawing.Point(216, 136);
			this.outputText.Multiline = true;
			this.outputText.Name = "outputText";
			this.outputText.ReadOnly = true;
			this.outputText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.outputText.Size = new System.Drawing.Size(504, 240);
			this.outputText.TabIndex = 3;
			// 
			// modelList
			// 
			this.modelList.FormattingEnabled = true;
			this.modelList.Location = new System.Drawing.Point(216, 32);
			this.modelList.Name = "modelList";
			this.modelList.Size = new System.Drawing.Size(504, 94);
			this.modelList.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.releaseSetSize);
			this.groupBox1.Controls.Add(this.releaseSetAll);
			this.groupBox1.Location = new System.Drawing.Point(8, 312);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(192, 72);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Release set size";
			// 
			// releaseSetAll
			// 
			this.releaseSetAll.AutoSize = true;
			this.releaseSetAll.Location = new System.Drawing.Point(16, 32);
			this.releaseSetAll.Name = "releaseSetAll";
			this.releaseSetAll.Size = new System.Drawing.Size(37, 17);
			this.releaseSetAll.TabIndex = 0;
			this.releaseSetAll.Text = "All";
			this.releaseSetAll.UseVisualStyleBackColor = true;
			this.releaseSetAll.CheckedChanged += new System.EventHandler(this.releaseSetAll_CheckedChanged);
			// 
			// releaseSetSize
			// 
			this.releaseSetSize.Location = new System.Drawing.Point(112, 32);
			this.releaseSetSize.Name = "releaseSetSize";
			this.releaseSetSize.Size = new System.Drawing.Size(64, 20);
			this.releaseSetSize.TabIndex = 1;
			// 
			// PredictorView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(737, 406);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.modelList);
			this.Controls.Add(this.outputText);
			this.Controls.Add(this.releaseList);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "PredictorView";
			this.Text = "Predictor";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.releaseSetSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openConfigToolStripMenuItem;
		private System.Windows.Forms.CheckedListBox releaseList;
		private System.Windows.Forms.ToolStripMenuItem commandToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem predictToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem predictAndEvaluateToolStripMenuItem;
		private System.Windows.Forms.TextBox outputText;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showFilesToolStripMenuItem;
		private System.Windows.Forms.CheckedListBox modelList;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown releaseSetSize;
		private System.Windows.Forms.CheckBox releaseSetAll;
	}
}

