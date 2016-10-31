using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_GHS
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GHS_Category { get; set; }
        public string GHS_Warning { get; set; }
        public string GHS_HazardouDes_Values { get; set; }
        public string GHS_HazardouDes_Append { get; set; }
        public string GHS_DefenceDes_Values { get; set; }
        public string GHS_DefenceDes_Append { get; set; }
        public string GHS_DealDES_Values { get; set; }
        public string GHS_DealDES_Append { get; set; }
        public string GHS_StoreDes_Values { get; set; }
        public string GHS_StoreDes_Append { get; set; }
        public bool IsExplosive { get; set; }
        public bool IsFlammable { get; set; }
        public bool IsCorrosive { get; set; }
        public bool IsHealthHazard { get; set; }
        public bool IsToxic { get; set; }
        public bool IsOxidizing { get; set; }
        public bool IsGasUnderPressure { get; set; }
        public bool IsIrritant { get; set; }
        public bool IsDangerousToEnvironment { get; set; }
    }
}
