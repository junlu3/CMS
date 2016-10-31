using System;
using System.Collections.Generic;
using System.Threading;
using XL.CHC.Domain.DomainModel;
using XL.CHC.WindowsService.AutoTaskHandlers;

namespace XL.CHC.WindowsService
{
    public class AutoTaskHandlerManager
    {
        private Dictionary<string, Thread> handlerByThreadRunningList = new Dictionary<string, Thread>();

        private AutoTaskHandlerManager() { }

        public static readonly AutoTaskHandlerManager Instance = new AutoTaskHandlerManager();

        public void StartAutoTask(AutoTask task)
        {
            try
            {
                switch (task.AutoTaskType.Name)
                {
                    case "多线程":
                        if (!handlerByThreadRunningList.ContainsKey(task.Code))
                        {
                            switch (task.Code)
                            {
                                //case "CompanyOrderHandler":
                                //    handlerByThreadRunningList.Add(task.Code, new Thread(new ThreadStart(new CompanyOrderHandler().HandleAutoTask)));
                                //    handlerByThreadRunningList[task.Code].Start();
                                //    break;
                                case "EmailHandler":
                                    handlerByThreadRunningList.Add(task.Code, new Thread(new ThreadStart(new EmailHandler().HandleAutoTask)));
                                    handlerByThreadRunningList[task.Code].Start();
                                    break;

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void StopAutoTask(AutoTask task)
        {
            try
            {
                switch (task.AutoTaskType.Name)
                {
                    case "多线程":
                        if (handlerByThreadRunningList[task.Code] != null)
                        {
                            handlerByThreadRunningList[task.Code].Abort();
                            handlerByThreadRunningList.Remove(task.Code);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void StopAllAutoTasks()
        {
            try
            {
                foreach (var item in handlerByThreadRunningList)
                {
                    item.Value.Abort();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
