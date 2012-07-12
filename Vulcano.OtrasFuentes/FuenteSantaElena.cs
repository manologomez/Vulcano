using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using SqlDataClasses;
using Vulcano.Engine.Interfaces;

namespace Vulcano.OtrasFuentes {
	public class FuenteSantaElena : IFuenteDatos {

		public string Name { get { return "SantaElena"; } }

		public void CompletarDatos(Resultado res, ValoresFuente fuente) {
			res.Nombre = Utils.PruebeValor(fuente, new[] { "contribuyente", "direccion" });
			res.Codigo3 = fuente.GetString("manzana");
			foreach (var detalle in res.Detalles) {
				detalle.Id_componente = "1";
			}
			res.Area = Utils.GetNumero(fuente["area_construccion"]);
		}

		public string GetManzana(string clave) {
			return string.IsNullOrEmpty(clave) || clave.Length < 11 ? "S/N" : clave.Substring(0, 7);
		}

		public int Total(Ciudad city, dynamic options = null) {
			const string sql = "select count(*) from sta_elena";
			var str = ConfigurationManager.ConnectionStrings["riesgos"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var total = new SqlHelper(conn).UniqueResult<int>(sql);
			conn.Close();
			return total;
		}

		public IList<ValoresFuente> Valores(Ciudad city, int limite, dynamic options = null) {
			var top = limite > 0 ? "top " + limite : "";
			var sql = "select " + top + " * from sta_elena order by codigo_catastral";
			var str = ConfigurationManager.ConnectionStrings["riesgos"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var reader = new SqlHelper(conn).ExecuteReader(sql);
			var mapa = new Dictionary<string, ValoresFuente>();
			var i = 0;
			while (reader.Read()) {
				i++;
				var temp = new ValoresFuente();
				Utils.FillDict(reader, temp);
				var clave = temp.GetString("codigo_catastral");
				if (mapa.ContainsKey(clave))
					continue;
				var manzana = GetManzana(clave);
				temp.Tipo = "construccion";
				temp.Codigo1 = clave;
				temp.Codigo2 = manzana;
				temp.IdItem = i;
				temp["clave"] = clave;
				temp["manzana"] = manzana;
				mapa[clave] = temp;
			}
			reader.Close();
			conn.Close();
			return mapa.Values.ToList();
		}
	}
}