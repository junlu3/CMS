using System;

namespace XL.CHC.Domain.DomainModel
{
    public class Law
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime? ImplementationDate { get; set; }

        public string FilePath { get; set; }

        public bool Deleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class LawSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
    }
}
