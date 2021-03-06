﻿using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_WorkerService
    {
        void Add(MSDS_Worker entity);
        IList<MSDS_Worker> GetAll(Guid company_Id);
        void Delete(MSDS_Worker entity);
        MSDS_Worker Single(Guid id);
        IPagedList<MSDS_Worker> Search(WorkerSearchModel searchModel);
    }
}
