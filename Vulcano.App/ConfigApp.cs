using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Vulcano.App {
	public class ConfigApp {
		public string CarpetaTrabajo { get; set; }
		public string CarpetaSalida { get; set; }

		public string ArchivoFichas { get; set; }
		public string ArchivoCatalogos { get; set; }
		public string ArchivoMapeos { get; set; }
		public string ScriptProceso { get; set; }

		public static ConfigApp Cargar(string file) {
			var json = File.ReadAllText(file);
			return JsonConvert.DeserializeObject<ConfigApp>(json);
		}

		public void Save(string file) {
			var json = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(file, json);
		}

	}
}
