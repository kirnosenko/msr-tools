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
			this.mainMenu = new System.Windows.Forms.MenuStrip();
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
			this.cbLimitReleaseSetSize = new System.Windows.Forms.CheckBox();
			this.releaseSetSize = new System.Windows.Forms.NumericUpDown();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.mainMenu.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.releaseSetSize)).BeginInit();
			this.statusBar.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.commandToolStripMenuItem});
			this.mainMenu.Location = new System.Drawing.Point(0, 0);
			this.mainMenu.Name = "mainMenu";
			this.mainMenu.Size = new System.Drawing.Size(737, 24);
			this.mainMenu.TabIndex = 0;
			this.mainMenu.Text = "menuStrip1";
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
			this.releaseList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.releaseList.FormattingEnabled = true;
			this.releaseList.Location = new System.Drawing.Point(3, 16);
			this.releaseList.Name = "releaseList";
			this.releaseList.Size = new System.Drawing.Size(194, 274);
			this.releaseList.TabIndex = 1;
			// 
			// outputText
			// 
			this.outputText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputText.Location = new System.Drawing.Point(3, 16);
			this.outputText.Multiline = true;
			this.outputText.Name = "outputText";
			this.outputText.ReadOnly = true;
			this.outputText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.outputText.Size = new System.Drawing.Size(526, 200);
			this.outputText.TabIndex = 3;
			// 
			// modelList
			// 
			this.modelList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modelList.FormattingEnabled = true;
			this.modelList.Location = new System.Drawing.Point(3, 16);
			this.modelList.Name = "modelList";
			this.modelList.Size = new System.Drawing.Size(526, 109);
			this.modelList.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbLimitReleaseSetSize);
			this.groupBox1.Controls.Add(this.releaseSetSize);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox1.Location = new System.Drawing.Point(0, 296);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 64);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Release set size";
			// 
			// cbLimitReleaseSetSize
			// 
			this.cbLimitReleaseSetSize.AutoSize = true;
			this.cbLimitReleaseSetSize.Location = new System.Drawing.Point(16, 24);
			this.cbLimitReleaseSetSize.Name = "cbLimitReleaseSetSize";
			this.cbLimitReleaseSetSize.Size = new System.Drawing.Size(47, 17);
			this.cbLimitReleaseSetSize.TabIndex = 2;
			this.cbLimitReleaseSetSize.Text = "Limit";
			this.cbLimitReleaseSetSize.UseVisualStyleBackColor = true;
			this.cbLimitReleaseSetSize.CheckedChanged += new System.EventHandler(this.cbLimitReleaseSetSize_CheckedChanged);
			// 
			// releaseSetSize
			// 
			this.releaseSetSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.releaseSetSize.Location = new System.Drawing.Point(80, 24);
			this.releaseSetSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.releaseSetSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.releaseSetSize.Name = "releaseSetSize";
			this.releaseSetSize.Size = new System.Drawing.Size(104, 20);
			this.releaseSetSize.TabIndex = 1;
			this.releaseSetSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// statusBar
			// 
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
			this.statusBar.Location = new System.Drawing.Point(0, 384);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(737, 22);
			this.statusBar.TabIndex = 6;
			this.statusBar.Text = "statusStrip1";
			// 
			// statusText
			// 
			this.statusText.Name = "statusText";
			this.statusText.Size = new System.Drawing.Size(0, 17);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(200, 360);
			this.panel1.TabIndex = 7;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.releaseList);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 296);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Releases";
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 24);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(5, 360);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.groupBox4);
			this.panel2.Controls.Add(this.splitter2);
			this.panel2.Controls.Add(this.groupBox3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(205, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(532, 360);
			this.panel2.TabIndex = 9;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.outputText);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox4.Location = new System.Drawing.Point(0, 141);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(532, 219);
			this.groupBox4.TabIndex = 7;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Output";
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter2.Location = new System.Drawing.Point(0, 136);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(532, 5);
			this.splitter2.TabIndex = 6;
			this.splitter2.TabStop = false;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.modelList);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox3.Location = new System.Drawing.Point(0, 0);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(532, 136);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Prediction models";
			// 
			// PredictorView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(737, 406);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.mainMenu);
			this.MainMenuStrip = this.mainMenu;
			this.Name = "PredictorView";
			this.Text = "Predictor";
			this.mainMenu.ResumeLayout(false);
			this.mainMenu.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.releaseSetSize)).EndInit();
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip mainMenu;
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
		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.ToolStripStatusLabel statusText;
		private System.Windows.Forms.CheckBox cbLimitReleaseSetSize;
	}
}

