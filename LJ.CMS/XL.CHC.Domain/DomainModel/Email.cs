using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class Email
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string To { get; set; }

        public string ToName { get; set; }

        public string ReplyTo { get; set; }

        public string ReplyToName { get; set; }

        public string CC { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; } = "上海职业健康体检系统通知";

        public string AttachmentFilePath { get; set; }

        public string AttachmentFileName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = string.Empty;

        public int SentTries { get; set; } = 0;

        public DateTime? SentDate { get; set; }
    }

    public static class EmailBodyFormatter
    {
        public static string EnterpriseBody { get; set; } = "<div >您好：<div><br></div><div>&nbsp; &nbsp; &nbsp; 您有一个待审批的 SDS —— ({1})，请点击 <a href='{0}' target='_blank'>连接</a> 获取更多信息。 </div><div><br></div><div>全面化学品管理系统 (MSDS)</div><div><div><br></div></div> </div>";
    }
}
