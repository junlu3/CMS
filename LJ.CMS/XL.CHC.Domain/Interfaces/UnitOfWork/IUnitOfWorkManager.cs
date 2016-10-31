using System;
 

namespace XL.CHC.Domain.Interfaces.UnitOfWork
{
    public partial interface IUnitOfWorkManager : IDisposable
    {
        IUnitOfWork NewUnitOfWork();
    }
}
