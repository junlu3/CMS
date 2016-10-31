using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.WindowsService.AutoTaskHandlers
{
    public abstract class BaseAutoTaskHandler
    {
        public abstract void HandleAutoTask();
    }
}
