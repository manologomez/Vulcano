using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using ISpreadsheet;
using IronPython;
using Microsoft.Scripting.Hosting;
using Vulcano.Engine.Dominio;
using ScriptingUtils;

namespace Vulcano.Engine {
	/// <summary>
	/// Utilitarios generales del sistema
	/// </summary>
	public static class Utils {
		private static readonly CultureInfo cul = CultureInfo.GetCultureInfo("en-US");

		public static void FillDict(IDataReader reader, IDictionary<string, object> dict) {
			for (int i = 0; i < reader.FieldCount; i++) {
				var name = reader.GetName(i);
				var value = reader.GetValue(i);
				dict[name] = value == DBNull.Value ? null : value;
			}
		}

		public static string PruebeValor(IDictionary<string, object> ctx, IEnumerable<string> keys) {
			return (from key in keys where ctx.ContainsKey(key) select SafeString(ctx[key]))
				.FirstOrDefault(temp => !string.IsNullOrEmpty(temp));
		}

		public static string SafeString(object obj, string def = null) {
			if (IsSafeNull(obj))
				return def;
			var t = obj.ToString();
			return string.IsNullOrEmpty(t) ? def : t;
		}

		public static bool IsSafeNull(object obj) {
			return obj == null || obj == DBNull.Value;
		}

		public static string MakeKey(params object[] valores) {
			return string.Join("_", valores.Where(x => x != null).Select(x => x.ToString()));
		}

		public static string MakeKey(IEnumerable<object> valores) {
			return string.Join("_", valores.Where(x => x != null).Select(x => x.ToString()));
		}

		public static float? GetFloat(this IWorksheet sheet, string col, int? row = null) {
			object temp = null;
			temp = !row.HasValue ? sheet.GetCell(col) : sheet.GetCell(col, row.Value);
			return GetNumero(temp);
		}

		public static float? GetFloat(this IWorksheet sheet, int col, int row) {
			var temp = sheet.GetCell(col, row);
			return GetNumero(temp);
		}

		public static float? GetNumero(object valor) {
			if (valor == null)
				return null;
			if (valor is float || valor is int || valor is long || valor is double)
				return Convert.ToSingle(valor);
			var txt = valor.ToString().Replace(",", ".").Trim();
			if (string.IsNullOrEmpty(txt))
				return null;
			float aux;
			if (float.TryParse(txt, NumberStyles.Any, cul, out aux))
				return aux;
			return null;
		}

		public static ScriptEngine GetScriptingEngine(Action<ScriptEngineFactory> configureAction = null) {
			// TODO: Tomar alguna configuración general para poner los paths de libs y esas cosas
			var factory = new ScriptEngineFactory();
			factory.AddReference(typeof(FichaIndicadores))
				.AddReference(typeof(SqlDataClasses.SqlHelper))
				.AddReference(typeof(UtilidadesGenerales.ReflectionUtil))
				//.AddReference(typeof(Dapper.SqlMapper))
				.AddReference(typeof(PetaPoco.Database))
				.AddReference(typeof(IWorkbook));
			factory.AddSearchPath(Environment.CurrentDirectory);
			factory.SetOption("LightweightScopes", true);
			factory.SetOption("DivisionOptions", PythonDivisionOptions.New);
			if (configureAction != null)
				configureAction(factory);
			var engine = factory.CreateEngine();
			engine.Runtime.Globals.SetVariable("app_id", "Riesgos 1.0");
			return engine;
		}


		public static string FormatSyntaxError(Microsoft.Scripting.SyntaxErrorException exs) {
			var sb = new StringBuilder();
			sb.AppendFormat("Message {0}", exs.Message).AppendLine();
			sb.AppendFormat("Column {0}", exs.Column).AppendLine();
			sb.AppendFormat("Code {0}", exs.ErrorCode).AppendLine();
			sb.AppendFormat("CodeLine {0}", exs.GetCodeLine()).AppendLine();
			sb.AppendFormat("Line {0}", exs.Line).AppendLine();
			sb.AppendFormat("SourcePath {0}", exs.SourcePath).AppendLine();
			return sb.ToString();
		}
	}
}