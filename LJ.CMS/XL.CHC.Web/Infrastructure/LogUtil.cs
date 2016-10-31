using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace XL.CHC.Web.Infrastructure
{
    public class LogUtil
    {
        public static ILog CHCLog = LogManager.GetLogger("CHCLogger");
    }
}
