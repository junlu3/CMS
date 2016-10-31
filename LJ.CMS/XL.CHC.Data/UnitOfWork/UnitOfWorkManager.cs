using System.Data.Entity;
using XL.CHC.Data.Context;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.UnitOfWork;
namespace XL.CHC.Data.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private bool _isDisposed;
        private readonly CHCContext _context;

        public UnitOfWorkManager(ICHCContext context)
        {
            // http://stackoverflow.com/questions/3552000/entity-framework-code-only-error-the-model-backing-the-context-has-changed-sinc
            Database.SetInitializer<CHCContext>(null);

            _context = context as CHCContext;
        }

        /// <summary>
        /// Provides an instance of a unit of work. This wrapping in the manager
        /// class helps keep concerns separated
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(_context);
        }

        /// <summary>
        /// Make sure there are no open sessions.
        /// In the web app this will be called when the injected UnitOfWork manager
        /// is disposed at the end of a request.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _context.Dispose();
                _isDisposed = true;
            }
        }


    }
}
