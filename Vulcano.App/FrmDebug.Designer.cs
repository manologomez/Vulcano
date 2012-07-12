namespace Vulcano.App {
	partial class FrmDebug {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.TxtContexto = new System.Windows.Forms.TextBox();
			this.TxtDebug = new System.Windows.Forms.TextBox();
			this.BtnClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Contexto";
			// 
			// TxtContexto
			// 
			this.TxtContexto.Location = new System.Drawing.Point(67, 6);
			this.TxtContexto.Name = "TxtContexto";
			this.TxtContexto.Size = new System.Drawing.Size(545, 20);
			this.TxtContexto.TabIndex = 1;
			// 
			// TxtDebug
			// 
			this.TxtDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TxtDebug.Location = new System.Drawing.Point(12, 32);
			this.TxtDebug.Multiline = true;
			this.TxtDebug.Name = "TxtDebug";
			this.TxtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TxtDebug.Size = new System.Drawing.Size(636, 385);
			this.TxtDebug.TabIndex = 2;
			// 
			// BtnClose
			// 
			this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.BtnClose.Location = new System.Drawing.Point(12, 426);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(75, 23);
			this.BtnClose.TabIndex = 3;
			this.BtnClose.Text = "Cerrar";
			this.BtnClose.UseVisualStyleBackColor = true;
			this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
			// 
			// FrmDebug
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(660, 461);
			this.Controls.Add(this.BtnClose);
			this.Controls.Add(this.TxtDebug);
			this.Controls.Add(this.TxtContexto);
			this.Controls.Add(this.label1);
			this.MinimizeBox = false;
			this.Name = "FrmDebug";
			this.Text = "Volcado";
			this.Load += new System.EventHandler(this.FrmDebug_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox TxtContexto;
		private System.Windows.Forms.TextBox TxtDebug;
		private System.Windows.Forms.Button BtnClose;
	}
}