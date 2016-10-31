using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;

namespace XL.Utilities
{
    public static class DbQueryExtension
    {
        public static ObjectQuery<T> ToObjectQuery<T>(this DbQuery<T> query)
        {
            var internalQuery = query.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_internalQuery")
                .Select(field => field.GetValue(query))
                .First();

            var objectQuery = internalQuery.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_objectQuery")
                .Select(field => field.GetValue(internalQuery))
                .Cast<ObjectQuery<T>>()
                .First();

            return objectQuery;
        }

        public static SqlCommand ToSqlCommand<T>(this DbQuery<T> query)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = query.ToString();

            var objectQuery = query.ToObjectQuery();

            foreach (var param in objectQuery.Parameters)
            {
                command.Parameters.AddWithValue(param.Name, param.Value);
            }

            return command;
        }

        public static string ToTraceString<T>(this DbQuery<T> query)
        {
            var objectQuery = query.ToObjectQuery();

            return objectQuery.ToTraceStringWithParameters();
        }

        public static string ToTraceStringWithParameters<T>(this ObjectQuery<T> query)
        {
            string traceString = query.ToTraceString() + "\n";

            foreach (var parameter in query.Parameters)
            {
                traceString += parameter.Name + " [" + parameter.ParameterType.FullName + "] = " + parameter.Value + "\n";
            }

            return traceString;
        }
    }

    public class EntityCache<TEntity, TDbContext>
        : IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class
    {
        private DbContext _context;
        private Expression<Func<TEntity, bool>> _query;

        private string _cacheKey = Guid.NewGuid().ToString();

        public EntityCache(Expression<Func<TEntity, bool>> query)
        {
            _context = new TDbContext();
            _query = query;
        }

        private IEnumerable<TEntity> GetCurrent()
        {
            var query = _context.Set<TEntity>().Where(_query);

            return query;
        }

        private IEnumerable<TEntity> GetResults()
        {
            List<TEntity> value = MemoryCache.Default[_cacheKey] as List<TEntity>;

            if (value == null)
            {
                value = GetCurrent().ToList();

                var changeMonitor = new EntityChangeMonitor<TEntity, TDbContext>(_query);

                CacheItemPolicy policy = new CacheItemPolicy();

                policy.ChangeMonitors.Add(changeMonitor);

                MemoryCache.Default.Add(_cacheKey, value, policy);

                Console.WriteLine("From Database...");
            }
            else
            {
                Console.WriteLine("From Cache...");
            }

            return value;
        }

        public IEnumerable<TEntity> Results
        {
            get
            {
                return GetResults();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class EntityChangeEventArgs<T> : EventArgs
    {
        public IEnumerable<T> Results { get; set; }
        public bool ContinueListening { get; set; }
    }

    public class EntityChangeMonitor<TEntity, TDbContext> : ChangeMonitor,
        IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class
    {
        private DbContext _context;
        private Expression<Func<TEntity, bool>> _query;
        private EntityChangeNotifier<TEntity, TDbContext> _changeNotifier;

        private string _uniqueId;

        public EntityChangeMonitor(Expression<Func<TEntity, bool>> query)
        {
            _context = new TDbContext();
            _query = query;
            _uniqueId = Guid.NewGuid().ToString();
            _changeNotifier = new EntityChangeNotifier<TEntity, TDbContext>(_query);

            _changeNotifier.Error += new EventHandler<NotifierErrorEventArgs>(_changeNotifier_Error);
            _changeNotifier.Changed += new EventHandler<EntityChangeEventArgs<TEntity>>(_changeNotifier_Changed);

            InitializationComplete();
        }

        void _changeNotifier_Error(object sender, NotifierErrorEventArgs e)
        {
            base.OnChanged(null);
        }

        void _changeNotifier_Changed(object sender, EntityChangeEventArgs<TEntity> e)
        {
            base.OnChanged(e.Results);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_changeNotifier != null)
                {
                    _changeNotifier.Dispose();
                    _changeNotifier = null;
                }
            }
        }

        public override string UniqueId
        {
            get { return _uniqueId; }
        }
    }

    public class EntityChangeNotifier<TEntity, TDbContext>
        : IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class
    {
        private DbContext _context;
        private Expression<Func<TEntity, bool>> _query;
        private string _connectionString;

        public event EventHandler<EntityChangeEventArgs<TEntity>> Changed;
        public event EventHandler<NotifierErrorEventArgs> Error;

        public EntityChangeNotifier(Expression<Func<TEntity, bool>> query)
        {
            _context = new TDbContext();
            _query = query;
            _connectionString = _context.Database.Connection.ConnectionString;

            EntityChangeNotifier.AddConnectionString(_connectionString);

            RegisterNotification();
        }

        private void RegisterNotification()
        {
            _context = new TDbContext();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = GetCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    var sqlDependency = new SqlDependency(command);
                    sqlDependency.OnChange += new OnChangeEventHandler(_sqlDependency_OnChange);

                    // NOTE: You have to execute the command, or the notification will never fire.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }

        private string GetSql()
        {
            var q = GetCurrent();

            return q.ToTraceString();
        }

        private SqlCommand GetCommand()
        {
            var q = GetCurrent();

            return q.ToSqlCommand();
        }

        private DbQuery<TEntity> GetCurrent()
        {
            var query = _context.Set<TEntity>().Where(_query) as DbQuery<TEntity>;

            return query;
        }

        private void _sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (_context == null)
                return;

            if (e.Type == SqlNotificationType.Subscribe || e.Info == SqlNotificationInfo.Error)
            {
                var args = new NotifierErrorEventArgs
                {
                    Reason = e,
                    Sql = GetCurrent().ToString()
                };

                OnError(args);
            }
            else
            {
                var args = new EntityChangeEventArgs<TEntity>
                {
                    Results = GetCurrent(),
                    ContinueListening = true
                };

                OnChanged(args);

                if (args.ContinueListening)
                {
                    RegisterNotification();
                }
            }
        }

        protected virtual void OnChanged(EntityChangeEventArgs<TEntity> e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected virtual void OnError(NotifierErrorEventArgs e)
        {
            if (Error != null)
            {
                Error(this, e);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public static class EntityChangeNotifier
    {
        private static List<string> _connectionStrings;
        private static object _lockObj = new object();

        static EntityChangeNotifier()
        {
            _connectionStrings = new List<string>();

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                foreach (var cs in _connectionStrings)
                    SqlDependency.Stop(cs);
            };
        }

        internal static void AddConnectionString(string cs)
        {
            if (!_connectionStrings.Contains(cs))
            {
                lock (_lockObj)
                {
                    if (!_connectionStrings.Contains(cs))
                    {
                        SqlDependency.Start(cs);
                        _connectionStrings.Add(cs);
                    }
                }
            }
        }
    }

    public class NotifierErrorEventArgs : EventArgs
    {
        public string Sql { get; set; }
        public SqlNotificationEventArgs Reason { get; set; }
    }
}
