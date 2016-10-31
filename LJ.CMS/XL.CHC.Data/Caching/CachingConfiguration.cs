using EFCache;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.SqlServer;

namespace XL.CHC.Data.Caching
{
    public class CachingConfiguration : DbConfiguration
    {
        public CachingConfiguration()
        {
            //SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            var transactionHandler = new CacheTransactionHandler(new InMemoryCache());

            AddInterceptor(transactionHandler);

            var cachingPolicy = new CachingPolicy();

            Loaded +=
              (sender, args) => args.ReplaceService<DbProviderServices>(
                (s, _) => new CachingProviderServices(s, transactionHandler,
                  cachingPolicy));
        }
    }
}
