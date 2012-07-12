using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using SqlDataClasses;
using Vulcano.Engine.Interfaces;

namespace Vulcano.App.Implementaciones {
	public class FuenteBasePredios : IFuenteDatos {
		public string Name { get { return "Predios base(Datareader)"; } }

		public void CompletarDatos(Resultado res, ValoresFuente fuente) {
			res.Nombre = Utils.PruebeValor(fuente, new[] { "nombres", "direccion", "calle" });
			res.Codigo3 = CodManzana(fuente);
			if (fuente.ContainsKey("num_construccion")) {
				var temp = fuente.GetString("num_construccion");
				foreach (var detalle in res.Detalles) {
					detalle.Id_componente = temp;
				}
			}
			if (fuente.ContainsKey("Area Bloq"))
				res.Area = Utils.GetNumero(fuente["Area Bloq"]);
		}

		public string CodManzana(ValoresFuente fuente) {
			return string.Format("{0}{1}{2}",
				fuente.GetString("c_zona"), fuente.GetString("c_sector"), fuente.GetString("c_manzana")
			);
			//return fuente.Codigo1.Substring(0, 6);
		}

		public int Total(Ciudad city, dynamic options = null) {
			const string sql = "select count(*) from v_predio where id_ciudad = @id ";
			var str = ConfigurationManager.ConnectionStrings["riesgos"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var total = new SqlHelper(conn).UniqueResult<int>(sql, new { id = city.Id });
			conn.Close();
			return total;
		}

		public IList<ValoresFuente> Valores(Ciudad city, int limite, dynamic options = null) {
			var sql = string.Format("select {0} * ", limite == 0 ? "" : "top " + limite);
			sql += " from v_predio where id_ciudad = @id order by clave, num_construccion";
			var str = ConfigurationManager.ConnectionStrings["riesgos"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var mapa = new Dictionary<string, ValoresFuente>();
			var reader = new SqlHelper(conn).ExecuteReader(sql, new { id = city.Id });
			while (reader.Read()) {
				var temp = new ValoresFuente();
				temp.Tipo = "construccion";
				Utils.FillDict(reader, temp);
				var cod = temp.GetString("clave");
				var num = temp.GetString("num_construccion") ?? "0";
				var key = Utils.MakeKey(cod, num);
				if (!mapa.ContainsKey(key)) {
					temp.Codigo1 = cod;
					temp.Codigo2 = num;
					temp.IdItem = temp["id"];
					mapa[key] = temp;
				}
				var cat = temp.GetString("clave_dato");
				if (!string.IsNullOrEmpty(cat))
					mapa[key][cat] = temp.GetString("valor_texto");
				temp.Remove("clave_dato");
				temp.Remove("valor_texto");
			}
			reader.Close();
			conn.Close();
			return mapa.Values.ToList();
		}
	}
}