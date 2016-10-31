using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Web.Infrastructure
{
    public class AJAXReturnObject
    {
        /// <summary>
        /// default 0, success, -1 failed
        /// </summary>
        public AJAXReturnResult Status { get; set; } = 0;
        public string Message { get; set; } = string.Empty;
        public string DataJson { get; set; }
    }

    public enum AJAXReturnResult
    {
        Success = 0,
        Failed = -1
    }
}
