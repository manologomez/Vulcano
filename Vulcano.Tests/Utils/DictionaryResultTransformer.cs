using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Transform;

namespace Vulcano.Tests.Utils {
	[Serializable]
	public class DictionaryResultTransformer : IResultTransformer {

		public DictionaryResultTransformer() {
		}

		public IList TransformList(IList collection) {
			return collection;
		}

		public object TransformTuple(object[] tuple, string[] aliases) {
			var result = new Dictionary<string, object>();
			for (var i = 0; i < aliases.Length; i++) {
				result[aliases[i]] = tuple[i];
			}
			return result;
		}

	}

}
