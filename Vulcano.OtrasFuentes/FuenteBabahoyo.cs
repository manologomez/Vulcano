using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using SqlDataClasses;
using Vulcano.Engine.Interfaces;

namespace Vulcano.OtrasFuentes {
	public class FuenteBabahoyo : IFuenteDatos {

		public string Name { get { return "Babahoyo"; } }
		string ClaveFormat = "{0:d2}{1:d2}{2:d3}{3:d2}00000000";
		string ClaveManzana = "{0:d2}{1:d2}{2:d3}";

		public void CompletarDatos(Resultado res, ValoresFuente fuente) {
			res.Nombre = Utils.PruebeValor(fuente, new[] { "nombres", "direccion" });
			res.Codigo3 = CodManzana(res);
			if (fuente.ContainsKey("numero_edificacion")) {
				var temp = fuente.GetString("numero_edificacion");
				foreach (var detalle in res.Detalles) {
					detalle.Id_componente = temp;
				}
			}
			res.Area = Utils.GetNumero(fuente["area_construccion"]);
		}

		public string CodManzana(Resultado res) {
			return string.IsNullOrEmpty(res.Codigo) || res.Codigo.Length < 10
				? "" : res.Codigo.Substring(0, 7);
			//return string.Format(ClaveManzana,fuente["zona"], fuente["sector"], fuente["manzana"]);
		}

		public string ClaveCatastral(object zona, object sector, object manzana, object solar) {
			return string.Format(ClaveFormat, zona, sector, manzana, solar);
		}

		public int Total(Ciudad city, dynamic options = null) {
			const string sql = "select count(*) from v_reporte where catalogo is not null ";
			var str = ConfigurationManager.ConnectionStrings["babahoyo"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var total = new SqlHelper(conn).UniqueResult<int>(sql, new { id = city.Id });
			conn.Close();
			return total;
		}

		public IList<ValoresFuente> Valores(Ciudad city, int limite, dynamic options = null) {
			var top = limite > 0 ? "top " + limite : "";
			var sql = "select " + top + @" * from v_reporte 
			where catalogo is not null 
			order by zona,sector,manzana,solar, numero_edificacion, anio, area_construccion desc";

			var str = ConfigurationManager.ConnectionStrings["babahoyo"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var mapa = new Dictionary<string, ValoresFuente>();
			var reader = new SqlHelper(conn).ExecuteReader(sql);
			while (reader.Read()) {
				var temp = new ValoresFuente();
				temp.Tipo = "construccion";
				Utils.FillDict(reader, temp);
				var cod = temp.GetString("codigo_predio");
				var num = temp.GetString("numero_edificacion");
				var key = Utils.MakeKey(cod, num);
				if (!mapa.ContainsKey(key)) {
					var clave = ClaveCatastral(
						temp["zona"], temp["sector"], temp["manzana"], temp["solar"]
					);
					temp["clave"] = clave;
					temp.Codigo1 = clave;
					temp.Codigo2 = temp.GetString("numero_edificacion");
					temp.IdItem = temp["codigo_predio"];
					mapa[key] = temp;
				}
				var cat = temp.GetString("catalogo");
				if (!string.IsNullOrEmpty(cat))
					mapa[key][cat] = temp.GetString("valor_catalogo");
				temp.Remove("catalogo");
				temp.Remove("valor_catalogo");
			}
			reader.Close();
			conn.Close();
			return mapa.Values.ToList();
		}

	}
}
