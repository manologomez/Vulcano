using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomizedFileDialog;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using Newtonsoft.Json;
using Vulcano.App.Redes;
using Vulcano.App.Reportes;
using Vulcano.Engine;
using Vulcano.Engine.Carga;
using Vulcano.Engine.Dominio;

namespace Vulcano.App {
	public partial class FrmMain : Form {
		public FrmMain() {
			InitializeComponent();
			configFile = Path.Combine(Environment.CurrentDirectory, "VulcanoConfig.json");
			config = new ConfigApp();
			if (File.Exists(configFile))
				config = ConfigApp.Cargar(configFile);
			folderTextBoxWork.InitialPath = Directory.Exists(config.CarpetaTrabajo)
												? config.CarpetaTrabajo
												: Environment.CurrentDirectory;
			folderTextBoxOut.InitialPath = Directory.Exists(config.CarpetaSalida)
												? config.CarpetaSalida
												: Environment.CurrentDirectory;
			folderTextBoxWork.MyTextbox.DataBindings.Add("Text", config, "CarpetaTrabajo", false, DataSourceUpdateMode.OnPropertyChanged);
			folderTextBoxOut.MyTextbox.DataBindings.Add("Text", config, "CarpetaSalida", false, DataSourceUpdateMode.OnPropertyChanged);
			ComboCiudad.ComboBox.ValueMember = "Nombre";
			ComboCiudad.ComboBox.DisplayMember = "Nombre";

			//comboCiudad1.ValueMember = "Nombre";
			//comboCiudad1.DisplayMember = "Nombre";
			Bootstrapper.Init();
			fuentesFactory = Bootstrapper.Container.Resolve<FuentesFactory>();
			procesosFactory = Bootstrapper.Container.Resolve<ProcesosFactory>();
			//TxtScript.DataBindings.Add("Text", config, "ScriptProceso", false, DataSourceUpdateMode.OnPropertyChanged);
			TxtScript.Text = config.ScriptProceso;
			AppNotifier.OnMessage += LogMessageEvent;
		}

		private string configFile = "";
		private ConfigApp config;
		DatosProceso datosProceso = new DatosProceso();
		private FuentesFactory fuentesFactory;
		private ProcesosFactory procesosFactory;

		private OpenFileOrFolderDialog _dialog = new OpenFileOrFolderDialog();

		private void FrmMain_FormClosed(object sender, FormClosedEventArgs e) {
			_dialog.Dispose();
			config.Save(configFile);
			Bootstrapper.Dispose();
		}

		private void FrmMain_Load(object sender, EventArgs e) {
			datosProceso.Ciudades = RepoGeneral.Lista();
			ComboCiudad.ComboBox.DataSource = datosProceso.Ciudades;
			foreach (var fuente in fuentesFactory.Fuentes) {
				listViewFuentes.Items.Add(fuente.Name);
			}
			foreach (var proc in procesosFactory.Procesos) {
				listBoxProcesos.Items.Add(proc.Nombre);
			}

			openFileDialog1.InitialDirectory = config.CarpetaTrabajo;
			FillScripts();

			var fsmProvider = new FileSyntaxModeProvider(Environment.CurrentDirectory); // Create new provider with the highlighting directory.
			HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider); // Attach to the text editor.
			editorScript.SetHighlighting("Boo"); // Activate the highlighting, use the name from the SyntaxDefinition node.
			var font = editorScript.TextEditorProperties.Font;
			editorScript.TextEditorProperties.Font = new Font(font.FontFamily, 8);
			editorScript.ActiveTextAreaControl.TextArea.DoProcessDialogKey += TextArea_DoProcessDialogKey;
			comboFont.Text = "8";

			listViewFichas.SelectedIndexChanged += ListaSelectedIndexChanged;
			listViewCatalogos.SelectedIndexChanged += ListaSelectedIndexChanged;
			listViewMapeos.SelectedIndexChanged += ListaSelectedIndexChanged;
			listViewFuentes.SelectedIndexChanged += ListaSelectedIndexChanged;
		}

		void ListaSelectedIndexChanged(object sender, EventArgs e) {
			var tpl = "Ficha:'{0}' -> Catalogo:'{1}' -> Mapeo:'{2}' -> Fuente:'{3}'";
			var ficha = GetSelected(listViewFichas);
			var cat = GetSelected(listViewCatalogos);
			var mapeo = GetSelected(listViewMapeos);
			var fuente = GetSelected(listViewFuentes);
			txtProceso.Text = string.Format(tpl, ficha, cat, mapeo, fuente);
		}

		public bool esControl = false;
		bool TextArea_DoProcessDialogKey(Keys keyData) {
			if (keyData == Keys.F5) {
				TestScript();
				return true;
			}
			return false;
		}

		public void LogMessage(string msg, Exception error = null) {
			var ev = new LogEvent(msg, this) { Error = error };
			LogMessageEvent(ev);
		}

		public void LogMessageEvent(LogEvent e) {
			if (InvokeRequired) {
				Invoke(new MethodInvoker(() => PushMessage(e)));
			} else {
				PushMessage(e);
			}
		}

		public void PushMessage(LogEvent e) {
			if (!string.IsNullOrEmpty(e.Mensaje)) {
				var nl = (e.Mensaje.EndsWith(Environment.NewLine) || e.Mensaje.EndsWith("\r\n"))
							? ""
							: Environment.NewLine;
				var msg = string.Format("{0}{1}", e.Mensaje.Trim(), nl);
				txtLog.AppendText(msg);
			}
			if (e.Error != null) {
				//var sb = new StringBuilder(e.Mensaje).AppendLine(e.Error.StackTrace);
				var sb = new StringBuilder(e.Mensaje).AppendLine(e.Error.ToString());
				if (e.Error.InnerException != null) {
					sb.AppendLine("-- inner exception")
						.AppendLine(e.Error.InnerException.Message)
						.AppendLine(e.Error.InnerException.StackTrace);
				}
				txtErrores.Text = sb.ToString();
			}
			// mejor un listview?
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void BtnSaveConfig_Click(object sender, EventArgs e) {
			config.Save(configFile);
			LogMessage("Guardado");
		}

		private void BtnCargaBasicos_Click(object sender, EventArgs e) {
			var task = new Task(() => {
				try {
					var bas = config.CarpetaTrabajo;
					config.ArchivoFichas = Path.Combine(bas, "Fichas_indicadores.xlsx");
					config.ArchivoCatalogos = Path.Combine(bas, "Catalogos_sistema.xlsx");
					config.ArchivoMapeos = Path.Combine(bas, "Mapeos_fuentes.xlsx");
					LogMessage("Cargando fichas");
					datosProceso.Fichas = CargadorFichas.CargarFichas(config.ArchivoFichas);
					LogMessage("Cargando catalogos");
					datosProceso.CatalogosFuentes = CargadorCatalogos.Cargar(config.ArchivoCatalogos);
					LogMessage("Cargando mapeos");
					datosProceso.MapeosCargados = CargadorMapeos.Cargar(config.ArchivoMapeos);
					LogMessage("Terminado");
					UpdateAllViews();
				} catch (Exception ex) {
					LogMessage("Error cargando:" + ex.Message, ex);
				}
			});
			task.Start();
		}

		private void UpdateAllViews() {
			BeginInvoke(new MethodInvoker(() => {
				UpdateListView(listViewFichas, datosProceso.Fichas.Select(x => x.Nombre));
				UpdateListView(listViewCatalogos, datosProceso.CatalogosFuentes.Keys);
				UpdateListView(listViewMapeos, datosProceso.MapeosCargados.Keys);
			}));
		}

		private void UpdateListView(ListView list, IEnumerable<string> nombres) {
			list.BeginUpdate();
			list.Items.Clear();
			foreach (var nombre in nombres) {
				list.Items.Add(nombre);
			}
			list.EndUpdate();
		}

		public string GetSelected(ListView list) {
			if (list.SelectedItems.Count == 0)
				return "";
			return list.SelectedItems[0].Text;
		}

		private void listViewFichas_MouseDoubleClick(object sender, MouseEventArgs e) {
			var text = GetSelected(listViewFichas);
			if (text == "") return;
			var ficha = datosProceso.Fichas.FirstOrDefault(x => x.Nombre == text);
			if (ficha == null) return;
			var frm = new FrmDebug();
			frm.SetData("Ficha de indicadores " + text, ficha, true);
			frm.ShowDialog();
		}

		private void listViewCatalogos_MouseDoubleClick(object sender, MouseEventArgs e) {
			var text = GetSelected(listViewCatalogos);
			if (text == "") return;
			var ficha = datosProceso.CatalogosFuentes[text];
			if (ficha == null) return;
			var frm = new FrmDebug();
			frm.SetData("Catalogo " + text, ficha, true);
			frm.ShowDialog();
		}

		private void listViewMapeos_MouseDoubleClick(object sender, MouseEventArgs e) {
			var text = GetSelected(listViewMapeos);
			if (text == "") return;
			var ficha = datosProceso.MapeosCargados[text];
			if (ficha == null) return;
			var frm = new FrmDebug();
			frm.SetData("Mapeo " + text, ficha, true);
			frm.ShowDialog();
		}

		public void FillScripts() {
			var files = Directory.GetFiles(config.CarpetaTrabajo, "*.py");
			comboScript.Items.Clear();
			if (files.Length == 0)
				return;
			foreach (var file in files) {
				var f = Path.GetFileName(file);
				comboScript.Items.Add(f ?? "");
			}
			comboScript.SelectedIndex = 0;
		}

		private void BtnProceso_Click(object sender, EventArgs e) {
			var proc = new Procesador();
			var error = PrepararProceso(proc);
			if (!string.IsNullOrEmpty(error)) {
				LogMessage(error);
				MessageBox.Show(error);
				return;
			}

			dynamic opciones = new ExpandoObject();
			opciones.limite = 0;

			var task = new Task(() => {
				proc.OnMessage += LogMessageEvent;
				proc.OnProgress += ReportProgreso;
				var reloj = new Stopwatch();
				reloj.Start();
				LogMessage("Empezando con " + proc.City.Nombre);
				var conf = new ConfigProceso {
					CarpetaTrabajo = config.CarpetaTrabajo,
					ScriptFile = Path.Combine(config.CarpetaTrabajo, config.ScriptProceso)
				};
				try {
					proc.Calcular(conf);
					LogMessage("Calculo terminado para " + proc.City.Nombre);
				} catch (Exception ex) {
					LogMessage("Error " + ex.Message, ex);
				} finally {
					reloj.Stop();
					proc.OnMessage -= LogMessageEvent;
					proc.OnProgress -= ReportProgreso;
					LogMessage("Terminado en " + reloj.Elapsed.TotalSeconds + "segundos");
				}
			});
			task.Start();
		}

		public void ReportProgreso(int numero, string tipo) {
			Invoke(new MethodInvoker(() => {
				switch (tipo) {
					case "inicio":
						progressBar1.Minimum = 0;
						progressBar1.Maximum = numero;
						progressBar1.Value = 0;
						break;
					case "fin":
						var maxnum = Math.Min(numero, progressBar1.Maximum);
						progressBar1.Value = maxnum;
						break;
					default:
						progressBar1.Increment(numero);
						break;
				}
			}
			));
		}

		public string PrepararProceso(Procesador proc) {
			var error = "";
			var aux = GetSelected(listViewFuentes);
			if (string.IsNullOrEmpty(aux))
				error = "Seleccione Fuente";
			else
				proc.Fuente = fuentesFactory.Fuentes.FirstOrDefault(x => x.Name == aux);
			aux = GetSelected(listViewFichas);
			if (string.IsNullOrEmpty(aux))
				error = "Seleccione Ficha";
			else
				proc.Ficha = datosProceso.Fichas.FirstOrDefault(x => x.Nombre == aux);
			aux = GetSelected(listViewCatalogos);
			if (!string.IsNullOrEmpty(aux))
				proc.Catalogo = datosProceso.CatalogosFuentes[aux];
			aux = GetSelected(listViewMapeos);
			if (!string.IsNullOrEmpty(aux))
				proc.Mapeos = datosProceso.MapeosCargados[aux];
			if (string.IsNullOrEmpty(config.ScriptProceso))
				error = "Falta el script";
			proc.City = ComboCiudad.ComboBox.SelectedItem as Ciudad;
			return error;
		}

		private void BtnScript_Click(object sender, EventArgs e) {
			if (openFileDialog1.ShowDialog() != DialogResult.OK)
				return;
			TxtScript.Text = Path.GetFileName(openFileDialog1.FileName);
		}

		private void BtnClear_Click(object sender, EventArgs e) {
			txtLog.Clear();
		}

		private void BtnTestFuente_Click(object sender, EventArgs e) {
			var aux = GetSelected(listViewFuentes);
			if (string.IsNullOrEmpty(aux)) {
				MessageBox.Show("Seleccione Fuente");
				return;
			}
			try {
				var fuente = fuentesFactory.Fuentes.FirstOrDefault(x => x.Name == aux);
				var city = ComboCiudad.ComboBox.SelectedItem as Ciudad;
				var total = fuente.Total(city);
				var lista = fuente.Valores(city, 10);
				var msg = string.Format("Probando canton {0}, fuente{1}", city.Nombre, fuente.Name);
				LogMessage(msg);
				if (lista == null || lista.Count == 0) {
					MessageBox.Show("No vinieron datos");
					return;
				}
				LogMessage("Total: " + total);
				var sb = new StringBuilder("Primero: ").AppendLine();
				var uno = lista.First();
				var json1 = JsonConvert.SerializeObject(uno, Formatting.Indented);
				sb.AppendLine(json1);
				if (lista.Count > 1) {
					var rand = new Random().Next(0, lista.Count - 1);
					var obj = lista[rand];
					var json2 = JsonConvert.SerializeObject(obj, Formatting.Indented);
					sb.AppendLine("Al azar:" + rand).AppendLine(json2);
				}
				sb.AppendFormat("Total Registros: {0}", total).AppendLine();
				var frm = new FrmDebug();
				frm.SetData(msg, sb.ToString());
				frm.Show();
			} catch (Exception ex) {
				LogMessage("Error en prueba fuente:" + ex.Message, ex);
			}
		}

		public static bool corriendo = false;

		private void BtnTestScript_Click(object sender, EventArgs e) {
			TestScript();
		}

		public void TestScript() {
			if (corriendo) {
				LogMessage("Ya esta corriendo un script");
				return;
			}
			if (BtnChecked.Checked)
				txtLog.Clear();
			var procAux = new Procesador();
			PrepararProceso(procAux);
			LogMessage("probando script");
			Task.Factory.StartNew(() => {
				var proc = new CorrerScript();
				proc.Data["proc"] = procAux;
				proc.OnMessage += LogMessageEvent;
				var reloj = new Stopwatch();
				reloj.Start();
				try {
					lock (this)
						corriendo = true;
					proc.Correr(editorScript.Text, config.CarpetaTrabajo);
				} catch (Exception ex) {
					LogMessage("Error " + ex.Message, ex);
				} finally {
					lock (this)
						corriendo = false;
					reloj.Stop();
					proc.OnMessage -= LogMessageEvent;
					LogMessage("Terminado en " + reloj.Elapsed.TotalSeconds);
				}
			}).ContinueWith(a => {
				if (a.IsFaulted && a.Exception != null) {
					var ex = a.Exception.InnerException;
					LogMessage("Error " + ex.Message, ex);
				}
			});
		}

		private void comboScript_SelectedIndexChanged(object sender, EventArgs e) {
			var file = Path.Combine(config.CarpetaTrabajo, comboScript.Text);
			editorScript.ActiveTextAreaControl.ResetText();
			var txt = File.ReadAllText(file);
			editorScript.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			editorScript.Text = txt;
			editorScript.Document.CommitUpdate();
		}

		private void BtnSaveScript_Click(object sender, EventArgs e) {
			var file = Path.Combine(config.CarpetaTrabajo, comboScript.Text);
			var txt = editorScript.Text;
			File.WriteAllText(file, txt);
			LogMessage("Guardado script");
		}

		private void BtnReloadScript_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(comboScript.Text))
				return;
			var file = Path.Combine(config.CarpetaTrabajo, comboScript.Text);
			var txt = File.ReadAllText(file);
			editorScript.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			editorScript.Text = txt;
			editorScript.Document.CommitUpdate();
			LogMessage("script cargado");
		}

		private void BtnProcEspecial_Click(object sender, EventArgs e) {
			var txt = listBoxProcesos.Text;
			if (string.IsNullOrEmpty(txt)) {
				MessageBox.Show("Seleccione un proceso");
				return;
			}
			var ciudad = ComboCiudad.ComboBox.SelectedItem as Ciudad;
			var proc = procesosFactory.GetProceso(txt);
			var task = new Task(() => {
				proc.Mensaje += LogMessageEvent;
				proc.Procesar(ciudad);
				proc.Mensaje -= LogMessageEvent;
			});
			task.Start();
		}

		private void BtnReporte_Click(object sender, EventArgs e) {
			var txt = Exportar();
			if (!string.IsNullOrEmpty(txt))
				MessageBox.Show(txt);
		}

		public string Exportar() {
			var ciudad = ComboCiudad.ComboBox.SelectedItem as Ciudad;
			if (ciudad == null) {
				return "No hay ciudad";
			}
			var ficha = datosProceso.Fichas.FirstOrDefault(x => x.Nombre == "EDIFICACIONES");
			if (ficha == null) return "La ficha edificaciones no existe";
			LogMessage("EXPORTACION: Ciudad" + ciudad.Nombre + " Ficha:" + ficha.Nombre);
			Task.Factory.StartNew(() => {
				LogMessage("Cargando Datos");
				var filename = string.Format("{0}_{1}.xlsx", ciudad.Nombre, ficha.Nombre);
				var path = Path.Combine(config.CarpetaSalida, filename);
				if (File.Exists(path))
					File.Delete(path);
				var datos = new Dictionary<string, IList<Resultado>>();
				datos["predios"] =
					RepoGeneral.Resultados(new ResultadoForm { Ciudad = ciudad.Nombre, Tipo = "predio" });
				datos["manzanas"] =
					RepoGeneral.Resultados(new ResultadoForm { Ciudad = ciudad.Nombre, Tipo = "manzana" });
				var exporter = new ExportadorExcel();
				exporter.Ficha = ficha;
				exporter.Guardar(datos, path);
				LogMessage("Exportado a:" + path);
			}).ContinueWith(a => {
				if (a.IsFaulted) {
					var ex = a.Exception.InnerException;
					if (ex != null) {
						LogMessage("Error " + ex.Message, ex);
					}
				}
			});
			return "";
		}

		public float? ResolverTolerancia() {
			float aux;
			if (float.TryParse(TxtTolerancia.Text, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out aux)) {
				if (aux > 1) return null;
				return aux;
			}
			return null;
		}

		private void BtnHistograma_Click(object sender, EventArgs e) {
			var city = ComboCiudad.ComboBox.SelectedItem as Ciudad;
			var ficha = datosProceso.Fichas.FirstOrDefault(x => x.Nombre == "EDIFICACIONES");
			if (ficha == null || city == null) {
				MessageBox.Show("Seleccione ciudad y asegurese de cargar las fichas");
				return;
			}
			var tol = ResolverTolerancia();
			if (tol == null) {
				MessageBox.Show("La tolerancia debe ser un número menor o igual a 1");
				return;
			}
			var hist = new HistogramaIndicadores();
			hist.City = city;
			hist.Ficha = ficha;
			hist.Tolerancia = tol.Value;
			var tpl = Path.Combine(config.CarpetaTrabajo, "plantilla_histograma.xlsx");
			var salida = Path.Combine(config.CarpetaSalida, hist.City.Nombre + "_histograma.xlsx");
			hist.Construir(tpl, salida);
		}

		private void BtnOcurrencias_Click(object sender, EventArgs e) {
			var city = ComboCiudad.ComboBox.SelectedItem as Ciudad;
			var ficha = datosProceso.Fichas.FirstOrDefault(x => x.Nombre == "EDIFICACIONES");
			if (ficha == null || city == null) {
				MessageBox.Show("Seleccione ciudad y asegurese de cargar las fichas");
				return;
			}
			var tol = ResolverTolerancia();
			if (tol == null) {
				MessageBox.Show("La tolerancia debe ser un número menor o igual a 1");
				return;
			}
			var tabla = new TablaOcurrencias();
			tabla.City = city;
			tabla.Ficha = ficha;
			tabla.Tolerancia = tol.Value;
			var salida = Path.Combine(config.CarpetaSalida, city.Nombre + "_ocurrencias.xlsx");
			tabla.Generar(salida);
		}

		private void Redes_Click(object sender, EventArgs e) {
			var city = ComboCiudad.ComboBox.SelectedItem as Ciudad;
			if (city == null || datosProceso.Fichas.Count == 0) {
				MessageBox.Show("Seleccione ciudad y asegurese de cargar las fichas");
				return;
			}
			var fuente = new FuenteRedes(datosProceso.Fichas, city);
			var proc = new ProcesadorRedes();
			var conf = new ConfigProceso();
			conf.CarpetaTrabajo = Path.Combine(config.CarpetaTrabajo, "Redes");
			conf.ScriptFile = Path.Combine(config.CarpetaTrabajo, "calculo_redes.py");
			proc.Calcular(conf, fuente);
		}

		private void BtnReporteRedes_Click(object sender, EventArgs e) {
			var ciudad = ComboCiudad.ComboBox.SelectedItem as Ciudad;
			if (ciudad == null) {
				MessageBox.Show("No hay ciudad");
				return;
			}
			LogMessage("EXPORTACION REDES: Ciudad" + ciudad.Nombre);
			var task = new Task(() => {
				LogMessage("Cargando Datos");
				var filename = string.Format("{0}_REDES.xlsx", ciudad.Nombre);
				var path = Path.Combine(config.CarpetaSalida, filename);
				if (File.Exists(path))
					File.Delete(path);
				var datos = RepoGeneral.Resultados(new ResultadoForm { Ciudad = ciudad.Nombre, Proceso = "REDES" });
				var exporter = new ExportadorExcel();
				exporter.GuardarRedes(datos, path, datosProceso.Fichas);
				LogMessage("Exportado redes a:" + path);
			});
			task.Start();
		}

		private void crearCiudadToolStripMenuItem_Click(object sender, EventArgs e) {
			var frm = new FrmCiudad();
			if (frm.ShowDialog() == DialogResult.OK) {
				datosProceso.Ciudades = RepoGeneral.Lista();
				ComboCiudad.ComboBox.DataSource = datosProceso.Ciudades;
			}
		}
	}
}
