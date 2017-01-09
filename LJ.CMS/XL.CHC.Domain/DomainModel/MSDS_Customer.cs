using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(50,ErrorMessage ="名字不能大于50个字符")]
        public string Name { get; set; }
        [MaxLength(11,ErrorMessage = "手机号码只能为11位")]
        [Required]
        public string Phone { get; set; }
        [MaxLength(200,ErrorMessage = "地址不能超过200个字符")]
        public string Address { get; set; }
        public string HeadPic { get; set; }
        public bool Enabled { get; set; }
        public int Type { get; set; }
        [MaxLength(50,ErrorMessage ="密码不能超过50个字符")]
        public string Password { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public string CreateBy { get; set; }

    }

    public class CustomerSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
        public Guid CustomerId { get; set; }
    }
}
