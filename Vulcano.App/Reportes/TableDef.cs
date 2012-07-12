using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Vulcano.App.Reportes{
	public class TableDef {
		public string Nombre { get; set; }
		public string TituloX { get; set; }
		public List<string> EjeX = new List<string>();
		public List<string> EjeY = new List<string>();

		public Tuple<DataTable, Dictionary<string, DataRow>> CreateTable(Type tipo) {
			var dt = new DataTable(Nombre ?? "");
			dt.Columns.Add(TituloX);
			foreach (var nombre in EjeX) {
				dt.Columns.Add(nombre, tipo);
			}
			dt.Columns.Add("TOTAL");
			var rowMap = new Dictionary<string, DataRow>();
			foreach (var nombre in EjeY) {
				var row = dt.NewRow();
				row[0] = nombre;
				dt.Rows.Add(row);
				rowMap[nombre] = row;
			}
			return Tuple.Create(dt, rowMap);
		}

		public DataTable Porcentajes(DataTable dt) {
			var tuple = CreateTable(typeof(float));
			foreach (DataRow row in dt.Rows) {
				float total = Totales(row);
				var n = row[0].ToString();
				var newrow = tuple.Item2[n];
				newrow[0] = row[0];
				foreach (var nombre in EjeX) {
					var valor = row[nombre];
					if (!Esnumero(valor)) continue;
					var por = Convert.ToSingle(valor) / total;
					newrow[nombre] = por;
				}
			}
			return tuple.Item1;
		}

		public float Totales(DataRow row) {
			return EjeX.Select(nombre => row[(string) nombre]).Where(Esnumero).Sum(num => Convert.ToSingle(num));
		}

		public bool Esnumero(object valor) {
			return valor is int || valor is float || valor is double;
		}

	}
}