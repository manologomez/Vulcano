namespace Vulcano.App {
	partial class FolderTextBox {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.TxtFolder = new System.Windows.Forms.TextBox();
			this.BtnSelect = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TxtFolder
			// 
			this.TxtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TxtFolder.Location = new System.Drawing.Point(3, 4);
			this.TxtFolder.Name = "TxtFolder";
			this.TxtFolder.Size = new System.Drawing.Size(361, 20);
			this.TxtFolder.TabIndex = 4;
			// 
			// BtnSelect
			// 
			this.BtnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnSelect.Location = new System.Drawing.Point(370, 4);
			this.BtnSelect.Name = "BtnSelect";
			this.BtnSelect.Size = new System.Drawing.Size(23, 20);
			this.BtnSelect.TabIndex = 5;
			this.BtnSelect.Tag = "work";
			this.BtnSelect.Text = "...";
			this.BtnSelect.UseVisualStyleBackColor = true;
			this.BtnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
			// 
			// FolderTextBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.TxtFolder);
			this.Controls.Add(this.BtnSelect);
			this.Name = "FolderTextBox";
			this.Size = new System.Drawing.Size(396, 30);
			this.Load += new System.EventHandler(this.FolderTextBox_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TxtFolder;
		private System.Windows.Forms.Button BtnSelect;
	}
}
