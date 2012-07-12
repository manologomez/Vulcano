using System;
using System.Collections.Generic;
using System.Data;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Vulcano.Tests.Utils{
	/// <summary>
	/// fuente http://stackoverflow.com/questions/1660887/ironruby-performance-issue-while-using-variables
	/// Tambien ver http://stackoverflow.com/questions/1967079/is-ironruby-scriptsource-execute-thread-safe
	/// </summary>
	public class BasicExpressionEvaluator {
		ScriptEngine engine;
		ScriptScope scope;
		public Exception LastException {
			get;
			set;
		}
		private static readonly Dictionary<string, ScriptSource> parserCache = new Dictionary<string, ScriptSource>();
		public BasicExpressionEvaluator() {
			engine = Python.CreateEngine();
			scope = engine.CreateScope();

		}

		public object Evaluate(string expression, DataRow context) {
			ScriptSource source;
			parserCache.TryGetValue(expression, out source);
			if (source == null) {
				source = engine.CreateScriptSourceFromString(expression, SourceCodeKind.SingleStatement);
				parserCache.Add(expression, source);
			}

			var result = source.Execute(scope);
			return result;
		}
		public void SetVariable(string variableName, object value) {
			scope.SetVariable(variableName, value);
		}
	}

	

}