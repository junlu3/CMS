using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Web.Models
{
    public class HospitalCalendarViewModel
    {
        public Guid? Id { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public string StartDateString { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string EndDateString { get; set; }
        public bool Enabled { get; set; } = true;
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 预定的状态 0 未被预定 1 预定未确认 2 确认
        /// </summary>
        public int OrderStatus { get; set; } = 0;
    }
}
