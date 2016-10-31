using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.Utilities;

namespace XL.CHC.WindowsService
{
    partial class AutoTaskService : ServiceBase
    {
        private Thread mainThread;

        public AutoTaskService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            mainThread = new Thread(new ThreadStart(HandleTasks));
            mainThread.Start();
        }

        protected override void OnStop()
        {
            AutoTaskHandlerManager.Instance.StopAllAutoTasks();
            mainThread.Abort();
        }

        private void HandleTasks()
        {
            //for test
            //Thread.Sleep(10000);

            try
            {
                using (var cache = new EntityCache<AutoTask, CHCContext>(p => p.AutoTaskStatus.Name == "开始" || p.AutoTaskStatus.Name == "停止"))
                {
                    LogManager.Instance.Write("Start ---------------------------------------------------------------------------------------------");

                    while (true)
                    {
                        var count = cache.Results.Count();
                        LogManager.Instance.Write(count.ToString());

                        var startList = cache.Results.Where(e => e.AutoTaskStatus.Name == "开始").ToList();
                        if (startList.Count > 0)
                        {
                            foreach (var task in startList)
                            {
                                AutoTaskHandlerManager.Instance.StartAutoTask(task);
                            }
                        }

                        var stopList = cache.Results.Where(e => e.AutoTaskStatus.Name == "停止").ToList();
                        if (stopList.Count > 0)
                        {
                            foreach (var task in stopList)
                            {
                                AutoTaskHandlerManager.Instance.StartAutoTask(task);
                            }
                        }

                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Write(ex.ToString());
            }
        }
    }
}
