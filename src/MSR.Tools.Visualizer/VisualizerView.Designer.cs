namespace MSR.Tools.Visualizer
{
	partial class VisualizerView
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
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.openConfigMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.scaleMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.logXMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.logYMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.cleanUpMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.automaticallyMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.cleanUpNowMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.visualizationsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.blackAndWhiteMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.statusBar.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
			this.statusBar.Location = new System.Drawing.Point(0, 440);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(784, 22);
			this.statusBar.TabIndex = 0;
			// 
			// statusText
			// 
			this.statusText.Name = "statusText";
			this.statusText.Size = new System.Drawing.Size(0, 17);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.viewMenu,
            this.visualizationsMenu});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(784, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileMenu
			// 
			this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConfigMenu});
			this.fileMenu.Name = "fileMenu";
			this.fileMenu.Size = new System.Drawing.Size(37, 20);
			this.fileMenu.Text = "File";
			// 
			// openConfigMenu
			// 
			this.openConfigMenu.Name = "openConfigMenu";
			this.openConfigMenu.Size = new System.Drawing.Size(152, 22);
			this.openConfigMenu.Text = "Open config...";
			this.openConfigMenu.Click += new System.EventHandler(this.openConfigToolStripMenuItem_Click);
			// 
			// viewMenu
			// 
			this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleMenu,
            this.cleanUpMenu,
            this.blackAndWhiteMenu});
			this.viewMenu.Name = "viewMenu";
			this.viewMenu.Size = new System.Drawing.Size(44, 20);
			this.viewMenu.Text = "View";
			// 
			// scaleMenu
			// 
			this.scaleMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logXMenu,
            this.logYMenu});
			this.scaleMenu.Name = "scaleMenu";
			this.scaleMenu.Size = new System.Drawing.Size(157, 22);
			this.scaleMenu.Text = "Scale";
			// 
			// logXMenu
			// 
			this.logXMenu.Name = "logXMenu";
			this.logXMenu.Size = new System.Drawing.Size(152, 22);
			this.logXMenu.Text = "Log X";
			this.logXMenu.Click += new System.EventHandler(this.logXToolStripMenuItem_Click);
			// 
			// logYMenu
			// 
			this.logYMenu.Name = "logYMenu";
			this.logYMenu.Size = new System.Drawing.Size(152, 22);
			this.logYMenu.Text = "Log Y";
			this.logYMenu.Click += new System.EventHandler(this.logYToolStripMenuItem_Click);
			// 
			// cleanUpMenu
			// 
			this.cleanUpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.automaticallyMenu,
            this.cleanUpNowMenu});
			this.cleanUpMenu.Name = "cleanUpMenu";
			this.cleanUpMenu.Size = new System.Drawing.Size(157, 22);
			this.cleanUpMenu.Text = "Clean up";
			// 
			// automaticallyMenu
			// 
			this.automaticallyMenu.Name = "automaticallyMenu";
			this.automaticallyMenu.Size = new System.Drawing.Size(152, 22);
			this.automaticallyMenu.Text = "Automatically";
			this.automaticallyMenu.Click += new System.EventHandler(this.automaticallyToolStripMenuItem_Click);
			// 
			// cleanUpNowMenu
			// 
			this.cleanUpNowMenu.Name = "cleanUpNowMenu";
			this.cleanUpNowMenu.Size = new System.Drawing.Size(152, 22);
			this.cleanUpNowMenu.Text = "Clean up now";
			this.cleanUpNowMenu.Click += new System.EventHandler(this.cleanUpNowToolStripMenuItem_Click);
			// 
			// visualizationsMenu
			// 
			this.visualizationsMenu.Name = "visualizationsMenu";
			this.visualizationsMenu.Size = new System.Drawing.Size(90, 20);
			this.visualizationsMenu.Text = "Visualizations";
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(784, 416);
			this.panel1.TabIndex = 2;
			// 
			// blackAndWhiteMenu
			// 
			this.blackAndWhiteMenu.Name = "blackAndWhiteMenu";
			this.blackAndWhiteMenu.Size = new System.Drawing.Size(157, 22);
			this.blackAndWhiteMenu.Text = "Black and white";
			this.blackAndWhiteMenu.Click += new System.EventHandler(this.blackAndWhiteMenu_Click);
			// 
			// VisualizerView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 462);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "VisualizerView";
			this.Text = "Visualizer";
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ToolStripStatusLabel statusText;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileMenu;
		private System.Windows.Forms.ToolStripMenuItem openConfigMenu;
		private System.Windows.Forms.ToolStripMenuItem viewMenu;
		private System.Windows.Forms.ToolStripMenuItem scaleMenu;
		private System.Windows.Forms.ToolStripMenuItem logXMenu;
		private System.Windows.Forms.ToolStripMenuItem logYMenu;
		private System.Windows.Forms.ToolStripMenuItem cleanUpMenu;
		private System.Windows.Forms.ToolStripMenuItem automaticallyMenu;
		private System.Windows.Forms.ToolStripMenuItem cleanUpNowMenu;
		private System.Windows.Forms.ToolStripMenuItem visualizationsMenu;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripMenuItem blackAndWhiteMenu;
	}
}