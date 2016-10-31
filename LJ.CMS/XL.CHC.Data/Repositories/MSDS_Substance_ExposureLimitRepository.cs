using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class MSDS_Substance_ExposureLimitRepository: IMSDS_Substance_ExposureLimitRepository
    {
        private readonly CHCContext _context;
        public MSDS_Substance_ExposureLimitRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_Substance_ExposureLimit entity)
        {
            if (_context.MSDS_Substance_ExposureLimit.Any(x=>x.CASCode == entity.CASCode))
            {
                throw new Exception("该CAS号已经存在");
            }
            _context.MSDS_Substance_ExposureLimit.Add(entity);
        }

        public void Add(IList<MSDS_Substance_ExposureLimit> entities)
        {
            _context.MSDS_Substance_ExposureLimit.AddRange(entities);
        }

        public MSDS_Substance_ExposureLimit Single(string CASCode)
        {
            return _context.MSDS_Substance_ExposureLimit.SingleOrDefault(x=>x.CASCode == CASCode);
        }

        public IList<MSDS_Substance_ExposureLimit> GetAll()
        {
            return _context.MSDS_Substance_ExposureLimit.Select(x => x).ToList();
        }

        public void DeleteAll(IList<MSDS_Substance_ExposureLimit> entities)
        {
            _context.MSDS_Substance_ExposureLimit.RemoveRange(entities);
        }

        public IPagedList<MSDS_Substance_ExposureLimit> Search(Substance_ExposureLimitSearchModel searchModel)
        {
            var query = _context.MSDS_Substance_ExposureLimit.Where(x => 
            (string.IsNullOrEmpty(searchModel.CASCode) || x.CASCode.ToLower().Contains(searchModel.CASCode.ToLower()))
            && (string.IsNullOrEmpty(searchModel.Substance_Name ) || x.Substance_Name.ToLower().Contains(searchModel.Substance_Name.ToLower()))
            && (string.IsNullOrEmpty(searchModel.Substance_CN_Name) || x.Substance_CN_Name.Contains(searchModel.Substance_CN_Name))
            )
            .OrderBy(x => x.Substance_Name);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<MSDS_Substance_ExposureLimit>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }
    }
}
