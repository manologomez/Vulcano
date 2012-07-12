using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Vulcano.App {
	public partial class FrmDebug : Form {
		public FrmDebug() {
			InitializeComponent();
		}

		private void FrmDebug_Load(object sender, EventArgs e) {

		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (keyData == Keys.Escape) this.Close();
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void BtnClose_Click(object sender, EventArgs e) {
			Close();
		}

		public void SetData(string ctx, dynamic data, bool toJson = false) {
			TxtContexto.Text = ctx;
			if (data == null)
				return;
			TxtDebug.Text = toJson ? JsonConvert.SerializeObject(data, Formatting.Indented) : data.ToString();
		}

	}
}
