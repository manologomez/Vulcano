using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using SqlDataClasses;
using Vulcano.Engine.Interfaces;

namespace Vulcano.OtrasFuentes {
	public class FuenteLatacunga : IFuenteDatos {

		public string Name { get { return "Latacunga"; } }

		public void CompletarDatos(Resultado res, ValoresFuente fuente) {
			res.Nombre = Utils.PruebeValor(fuente, new[] { "propietario", "calle" });
			res.Codigo3 = CodManzana(res);
			if (fuente.ContainsKey("numero_edificacion")) {
				var temp = fuente.GetString("numero_edificacion");
				foreach (var detalle in res.Detalles) {
					detalle.Id_componente = temp;
				}
			}
			res.Area = Utils.GetNumero(fuente["area_piso"]);
		}

		public string CodManzana(Resultado res) {
			return string.IsNullOrEmpty(res.Codigo) || res.Codigo.Length < 10
				? "S/N" : res.Codigo.Substring(0, 7);
		}

		public int Total(Ciudad city, dynamic options = null) {
			const string sql = "select count(*) from v_reporte";
			var str = ConfigurationManager.ConnectionStrings["latacunga"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var total = new SqlHelper(conn).UniqueResult<int>(sql, new { id = city.Id });
			conn.Close();
			return total;
		}

		public IList<ValoresFuente> Valores(Ciudad city, int limite, dynamic options = null) {
			var top = limite > 0 ? "top " + limite : "";
			var sql = "select " + top + " * from v_reporte order by clave, bloque, piso, numero_pisos";
			var str = ConfigurationManager.ConnectionStrings["latacunga"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var lista = new List<ValoresFuente>();
			var reader = new SqlHelper(conn).ExecuteReader(sql);
			while (reader.Read()) {
				var temp = new ValoresFuente();
				Utils.FillDict(reader, temp);
				temp.Tipo = "construccion";
				temp.Codigo1 = temp.GetString("clave");
				var key = string.Format("{0}.{1}", temp["bloque"], temp["piso"]);
				temp.Codigo2 = key;
				temp.IdItem = temp["id"];
				lista.Add(temp);
			}
			reader.Close();
			conn.Close();
			return lista;
		}

	}
}