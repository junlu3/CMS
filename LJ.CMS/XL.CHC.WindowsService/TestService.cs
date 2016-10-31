using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.Utilities;

namespace XL.CHC.WindowsService
{
    public partial class TestService : ServiceBase
    {
        private string logFilePath = System.Windows.Forms.Application.StartupPath + @"\Log";
        private LogHelper logHelper = new LogHelper();
        private Thread thread1;
        private Thread thread2;

        public TestService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            thread1 = new Thread(new ThreadStart(Test1));
            thread2 = new Thread(new ThreadStart(Test2));
            thread1.Start();
            thread2.Start();
        }

        protected override void OnStop()
        {
            thread1.Abort();
            thread2.Abort();
        }

        public void Test1()
        {
            try
            {
                using (var cache = new EntityCache<CompanyOrder, CHCContext>(p => p.Locked == true))
                {
                    logHelper.WriteLogFile(logFilePath, string.Format("{0}:{1}", DateTime.Now.ToString(), "Start---------------------------------------------------------------------------------------------"));

                    while (true)
                    {
                        logHelper.WriteLogFile(logFilePath, string.Format("{0}:{1}", DateTime.Now.ToString(), cache.Results.Count().ToString()));
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.WriteLogFile(logFilePath, string.Format("{0}:{1}", DateTime.Now.ToString(), ex.ToString()));
            }
        }

        public void Test2()
        {
            using (var cache = new EntityCache<CompanyEmployee, CHCContext>(p => p.WorkNumber == "0001"))
            {
                logHelper.WriteLogFile(logFilePath, string.Format("{0}:{1}", DateTime.Now.ToString(), "Start---------------------------------------------------------------------------------------------"));

                while (true)
                {
                    logHelper.WriteLogFile(logFilePath, string.Format("{0}:{1}", DateTime.Now.ToString(), cache.Results.Count().ToString()));
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
