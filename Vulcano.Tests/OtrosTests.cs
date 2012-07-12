using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Vulcano.Tests {
	[TestFixture]
	public class OtrosTests {

		[Test]
		public void TestCosas() {
			// clave de Babahoyo
			int zona = 4;
			int sector = 1;
			int manzana = 236; //99
			int solar = 21;
			int div1 = 0;
			int div2 = 0;

			var format = "{0:d2}{1:d2}{2:d3}{3:d2}00000000";
			Console.WriteLine(string.Format(format, zona, sector, manzana, solar));
		}

		[Test]
		public void TestString() {
			var clave = "0206190035000000";
			Console.WriteLine(clave.Substring(0, 7));

			clave = "601-396-008-00-00-00";
			Console.WriteLine(clave.Substring(0, 11));

			var que = new decimal[] { 1, 0.0m, 5, 10 };
			foreach (var num in que) {
				Console.WriteLine(num.ToString("##"));
			}

		}

	}
}
