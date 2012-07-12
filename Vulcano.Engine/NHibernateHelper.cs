using System;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;

namespace Vulcano.Engine {
	/// <summary>
	/// fuente: http://stackoverflow.com/questions/2380175/architectural-question-asp-net-mvc-nhibernate-castle
	/// </summary>
	public class NHibernateHelper { //: INHibernateHelper
		private static ISessionFactory sessionFactory;
		private const string ConfigFile = "nhibernate.config";
		public static Configuration Configuration { get; private set; }
		public static Action<Configuration> PostConfig { get; set; }

		/// <summary>
		/// SessionFactory is static because it is expensive to create and is therefore at application scope.
		/// The property exists to provide 'instantiate on first use' behaviour.
		/// </summary>
		public static ISessionFactory SessionFactory {
			get {
				if (sessionFactory == null) {
					try {
						//sessionFactory = new Configuration().Configure().AddAssembly("Bla").BuildSessionFactory();
						CreateSessionFactory();
					} catch (Exception e) {
						throw new Exception("NHibernate initialization failed.", e);
					}
				}
				return sessionFactory;
			}
		}

		private static void CreateSessionFactory() {
			Configuration = new Configuration();
			if (File.Exists(ConfigFile))
				Configuration.Configure(ConfigFile);
			else
				Configuration.Configure();
			if (PostConfig != null)
				PostConfig(Configuration);
			sessionFactory = Configuration.BuildSessionFactory();
		}

		public static ISession GetCurrentSession() {
			if (!CurrentSessionContext.HasBind(SessionFactory)) {
				CurrentSessionContext.Bind(SessionFactory.OpenSession());
			}
			return SessionFactory.GetCurrentSession();
		}

		public static void DisposeSession() {
			var session = GetCurrentSession();
			session.Close();
			session.Dispose();
		}

		public static void BeginTransaction() {
			GetCurrentSession().BeginTransaction();
		}

		public static void CommitTransaction() {
			var session = GetCurrentSession();
			if (session.Transaction.IsActive)
				session.Transaction.Commit();
		}

		public static void RollbackTransaction() {
			var session = GetCurrentSession();
			if (session.Transaction.IsActive)
				session.Transaction.Rollback();
		}

		public static string TableName<T>() {
			Type t = typeof(T);
			var map = Configuration.GetClassMapping(t);
			return map.Table.Name;
		}

	}

}
