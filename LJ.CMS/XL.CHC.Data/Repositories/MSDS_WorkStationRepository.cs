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
    public class MSDS_WorkStationRepository: IMSDS_WorkStationRepository
    {
        private readonly CHCContext _context;
        public MSDS_WorkStationRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_WorkStation entity)
        {
            if (_context.MSDS_WorkStation.Any(x=>x.WorkStation_Name == entity.WorkStation_Name))
            {
                throw new Exception(string.Format("工位名称：{0} 已经存在",entity.WorkStation_Name));
            }
            _context.MSDS_WorkStation.Add(entity);
        }

        public void Delete(MSDS_WorkStation entity)
        {
            _context.MSDS_WorkStation.Remove(entity);
        }

        public IList<MSDS_WorkStation> GetAll(Guid workshop_Id)
        {
            return _context.MSDS_WorkStation.Where(x => x.WorkShop.Id == workshop_Id).ToList<MSDS_WorkStation>();
        }

        public MSDS_WorkStation Single(Guid id)
        {
            return _context.MSDS_WorkStation.SingleOrDefault<MSDS_WorkStation>(x => x.Id == id);
        }

        public IPagedList<MSDS_WorkStation> Search(WorkStationSearchModel searchModel)
        {
            var query = _context.MSDS_WorkStation.Where(x => (string.IsNullOrEmpty(searchModel.KeyWord)
            || x.WorkStation_Name.ToLower().Contains(searchModel.KeyWord.ToLower()))
            && (searchModel.WorkShop_Id == new Guid("00000000-0000-0000-0000-000000000000") || x.WorkShop.Id == searchModel.WorkShop_Id)
            )
            .OrderBy(x => x.WorkStation_Name);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<MSDS_WorkStation>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }
    }
}
