using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;

namespace Vulcano.App {
	public partial class FrmCiudad : Form {
		public FrmCiudad() {
			InitializeComponent();
		}

		private void btnCrear_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtCodigo.Text)) {
				MessageBox.Show("Debe escribir el nombre y código de la ciudad");
				return;
			}
			var ciudad = new Ciudad {
				Codigo = txtCodigo.Text,
				Nombre = txtNombre.Text,
				Observaciones = txtObservaciones.Text
			};

			var res = RepoGeneral.CrearCiudad(ciudad);
			if (string.IsNullOrEmpty(res)) {
				DialogResult = DialogResult.OK;
				MessageBox.Show("Ciudad '" + ciudad.Nombre + "' Creada");
				Close();
			} else {
				MessageBox.Show(res);
			}
		}
	}
}
