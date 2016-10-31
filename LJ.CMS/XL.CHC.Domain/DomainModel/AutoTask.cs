using System;

namespace XL.CHC.Domain.DomainModel
{
    public class AutoTask
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public virtual Category AutoTaskStatus { get; set; }

        public virtual Category AutoTaskType { get; set; }

        public DateTime? LastRunStartTime { get; set; }

        public string Comment { get; set; }
    }
}
