using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Dapper;
using PetaPoco;
using UtilidadesGenerales;

namespace Vulcano.Tests {
	[TestFixture]
	public class TestPersistencia {
		string ClaveLata = "{0:d2}{1:d2}{2:d3}{3:d3}000000";
		public string ClaveCatastral(object zona, object sector, object manzana, object predio) {
			return string.Format(ClaveLata, zona, sector, manzana, predio);
		}

		/// <summary>
		/// Esto corrige (crea) las claves catastrales de latacunga
		/// </summary>
		[Test]
		public void ClaveLatacunga() {
			var sql = "select id, zona,sector,manzana,predio from construccion";
			var db = new Database("latacunga");
			var lista = db.Query<TempLata>(sql);
			var sb = new StringBuilder();
			var template = "update construccion set clave_catastral = '{0}' where id = {1}";
			foreach (var data in lista) {
				var clave = ClaveCatastral(data.Zona, data.Sector, data.Manzana, data.Predio);
				var update = string.Format(template, clave, data.Id);
				sb.AppendLine(update);
			}
			db.CloseSharedConnection();
			File.WriteAllText("claves_latacunga.sql", sb.ToString());
		}

		public class TempLata {
			public int Id { get; set; }
			public int Zona { get; set; }
			public int Sector { get; set; }
			public int Manzana { get; set; }
			public int Predio { get; set; }
		}


		[Test]
		public void TestDapper() {
			var str = ConfigurationManager.ConnectionStrings["riesgos"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();

			var lista = conn.Query<Ciudad>("select * from ciudad");

			foreach (var ciudad in lista) {
				var d = ReflectionUtil.Dict(ciudad);
				var line = PrintDict(d);
				Console.WriteLine(line);
			}
			conn.Close();
		}

		[Test]
		public void TextPeta() {
			var db = new Database("riesgos");
			var lista = db.Query<Ciudad>("from ciudad order by nombre");
			foreach (var ciudad in lista) {
				var d = ReflectionUtil.Dict(ciudad);
				var line = PrintDict(d);
				Console.WriteLine(line);
			}
			db.CloseSharedConnection();
		}

		public static string PrintDict(IDictionary<string, object> d, string separador = "| ") {
			var sep = separador ?? Environment.NewLine;
			var sb = new StringBuilder();
			foreach (var p in d) {
				sb.AppendFormat("{0}: {1}", p.Key, p.Value).Append(sep);
			}
			return sb.ToString();
		}

		[Test]
		public void TestMultiDapper() {
			var ciudad = new Ciudad { Id = 1 };

			var sql = @"select d.*,p.* from predio p
			left join dato_predio d on p.id = d.id_predio
			where id_ciudad = @id order by p.id"; //where p.id = 40 

			var str = ConfigurationManager.ConnectionStrings["riesgos"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();

			var reloj = new Stopwatch();
			reloj.Start();
			var mapa = new Dictionary<int, Predio>();
			var data = conn.Query<Dato_Predio, Predio, Dato_Predio>(sql,
				(dato, predio) => {
					if (!mapa.ContainsKey(predio.Id))
						mapa[predio.Id] = predio;
					mapa[predio.Id].AddDato(dato);
					return dato;
				}, new { id = 1 }
			);
			var lista = data.ToList();
			reloj.Stop();
			var dat = lista.First();
			Console.WriteLine(reloj.Elapsed.TotalSeconds);
			Console.WriteLine(dat.Clave);
			conn.Close();
		}

		[Test]
		public void TestExtract() {
			var sql = @"select d.*,p.* from predio p
			left join dato_predio d on p.id = d.id_predio
			where p.id = 26 order by p.id"; //where p.id = 40 

			var str = ConfigurationManager.ConnectionStrings["riesgos"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var mapa = new Dictionary<int, Auxcosa>();
			var query = conn.Query<Dato_Predio, Predio, Dato_Predio>(sql, (dato, predio) => {
				if (!mapa.ContainsKey(predio.Id)) {
					mapa[predio.Id] = new Auxcosa {
						Id = predio.Id,
						Template = new MyDick(ReflectionUtil.Dict(predio))
					};
					mapa[predio.Id].Template.Remove("Datos");
				}
				if (dato != null)
					mapa[predio.Id].AddDato(dato);
				//dato.Padre = predio;);
				return dato;
			}).ToList();
			var lista = new List<MyDick>();
			foreach (var auxcosa in mapa.Values) {
				if (auxcosa.Filas.Count == 0) {
					auxcosa.Template["num_construccion"] = 0;
					lista.Add(auxcosa.Template);
					continue;
				}
				lista.AddRange(auxcosa.Filas.Values);
			}
			conn.Close();
			foreach (var que in lista) {
				Console.WriteLine(PrintDict(que));
			}

		}

		public class Auxcosa {
			public int Id;
			public MyDick Template;
			public Dictionary<int, MyDick> Filas = new Dictionary<int, MyDick>();
			public void AddDato(Dato_Predio dato) {
				if (!Filas.ContainsKey(dato.Num_Construccion)) {
					Filas[dato.Num_Construccion] = new MyDick(Template);
					Filas[dato.Num_Construccion]["num_construccion"] = dato.Num_Construccion;
				}
				var fuente = Filas[dato.Num_Construccion];
				dynamic valor = dato.Valor_Numero.HasValue
				? dato.Valor_Numero.Value
				: (object)(string.IsNullOrEmpty(dato.Valor_Texto) ? null : dato.Valor_Texto);
				if (valor == null) return;
				fuente[dato.Clave] = valor;
			}
		}

		public class MyDick : Dictionary<string, object> {
			public MyDick(IDictionary<string, object> dictionary)
				: base(dictionary) {
			}

			public int Id { get; set; }

			public MyDick() { }
		}

	}

	public class Ciudad {
		public virtual int Id { get; set; }
		public virtual string Codigo { get; set; }
		public virtual string Nombre { get; set; }
		public virtual string Contacto { get; set; }
		[PetaPoco.Column("script_load")]
		public virtual string ScriptLoad { get; set; }
		public virtual string Observaciones { get; set; }
	}

	public class Predio {
		public virtual int Id { get; set; }
		public virtual string Clave { get; set; }
		public virtual string c_zona { get; set; }
		public virtual string c_sector { get; set; }
		public virtual string c_manzana { get; set; }
		public virtual string c_predio { get; set; }
		public virtual string c_horizontal { get; set; }
		public virtual string Sitio { get; set; }
		public virtual string Calle { get; set; }
		public virtual string Numero { get; set; }
		public virtual string Ruc_ci { get; set; }
		public virtual string Direccion { get; set; }
		public virtual string Telefonos { get; set; }
		public virtual double Area_total { get; set; }
		public virtual double Area_construccion { get; set; }
		public virtual string Suelo { get; set; }
		public virtual string Topologia { get; set; }
		public virtual string Localizacion { get; set; }
		public virtual string Forma { get; set; }
		public virtual IList<Dato_Predio> Datos { get; set; }

		public Predio() {
			Datos = new List<Dato_Predio>();
		}

		public virtual void AddDato(Dato_Predio dato) {
			Datos.Add(dato);
			dato.Padre = this;
		}

	}

	public class Dato_Predio {
		public virtual int Id { get; set; }
		public virtual int Num_Construccion { get; set; }
		public virtual string Clave { get; set; }
		public virtual string Valor_Texto { get; set; }
		public virtual double? Valor_Numero { get; set; }
		public virtual Predio Padre { get; set; }
	}

}
