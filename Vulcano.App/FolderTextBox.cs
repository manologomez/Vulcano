using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomizedFileDialog;

namespace Vulcano.App {

	public partial class FolderTextBox : UserControl {
		public FolderTextBox() {
			InitializeComponent();
		}

		public event Action<FolderTextBox, FolderSelectedEventArgs> OnSelectFolder;

		public TextBox MyTextbox {
			get { return TxtFolder; }
		}

		public Button MyButton {
			get { return BtnSelect; }
		}

		public string SelectedFolder {
			get { return TxtFolder.Text; }
			set { TxtFolder.Text = value; }
		}

		public string InitialPath { get; set; }
		public string Title { get; set; }
		public bool AcceptFiles { get; set; }

		public string GetPath() {
			var init = GetInitialPath();
			using (var dialog = new OpenFileOrFolderDialog()) {
				dialog.AcceptFiles = AcceptFiles;
				dialog.Path = init;
				if (!string.IsNullOrEmpty(Title))
					dialog.Title = Title;
				if (dialog.ShowDialog() != DialogResult.OK)
					return "";
				var path = dialog.Path;
				if (!Directory.Exists(path))
					return "";
				TxtFolder.Text = path;
				InitialPath = path;
				return path;
			}
		}

		private string GetInitialPath() {
			if (!string.IsNullOrEmpty(SelectedFolder) && Directory.Exists(SelectedFolder))
				return SelectedFolder;
			if (!string.IsNullOrEmpty(InitialPath) && Directory.Exists(InitialPath))
				return InitialPath;
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			//return Environment.CurrentDirectory;
			return path;
		}

		private void FolderTextBox_Load(object sender, EventArgs e) {

		}

		private void BtnSelect_Click(object sender, EventArgs e) {
			var path = GetPath();
			var args = new FolderSelectedEventArgs { Path = path };
			args.Tag = string.Format("{0}", Tag);
			if (OnSelectFolder != null)
				OnSelectFolder(this, args);
		}

	}

	public class FolderSelectedEventArgs : EventArgs {
		public string Path { get; set; }
		public string Tag { get; set; }
	}
}
