using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.Utilities;

namespace XL.CHC.WindowsService
{
    public class LogManager
    {
        public LogHelper logHelper { get; set; } = new LogHelper();

        public string logFilePath = System.Windows.Forms.Application.StartupPath + @"\Log";

        private LogManager() { }

        public static readonly LogManager Instance = new LogManager();

        public void Write(string content)
        {
            logHelper.WriteLogFile(logFilePath, string.Format("{0}:{1}", DateTime.Now.ToString(), content));
        }
    }
}
