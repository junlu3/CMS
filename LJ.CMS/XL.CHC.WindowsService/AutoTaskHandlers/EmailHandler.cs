using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.Utilities;

namespace XL.CHC.WindowsService.AutoTaskHandlers
{
    public class EmailHandler : BaseAutoTaskHandler
    {
        public override void HandleAutoTask()
        {
            LogManager.Instance.Write("Start Email Handler ******************************************************");

            var emailAccount = new EmailAccount()
            {
                Email = ConfigurationManager.AppSettings["EmailAccount"],
                DisplayName = ConfigurationManager.AppSettings["EmailDisplayName"],
                Host = ConfigurationManager.AppSettings["EmailHost"],
                Port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]),
                Username = ConfigurationManager.AppSettings["EmailUsername"],
                Password = ConfigurationManager.AppSettings["EmailPassword"],
                EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EmailEnableSsl"]),
                UseDefaultCredentials = bool.Parse(ConfigurationManager.AppSettings["EmailUseDefaultCredentials"]),
            };

            using (var cache = new EntityCache<Email, CHCContext>(p => !p.SentDate.HasValue))
            {
                while (true)
                {
                    LogManager.Instance.Write(cache.Results.Count().ToString());

                    if (cache.Results.Count() > 0)
                    {
                        foreach (var email in cache.Results)
                        {
                            var bcc = String.IsNullOrWhiteSpace(email.Bcc)
                                        ? null
                                        : email.Bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            var cc = String.IsNullOrWhiteSpace(email.CC)
                                        ? null
                                        : email.CC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            try
                            {
                                EmailHelper.SendEmail(emailAccount, email.Subject, email.Body, emailAccount.Email, emailAccount.DisplayName, email.To, email.ToName,
                                    email.ReplyTo, email.ReplyToName, bcc, cc, email.AttachmentFilePath, email.AttachmentFileName);

                                using (var context = new CHCContext())
                                {
                                    var entity = context.Email.SingleOrDefault(e => e.Id == email.Id);
                                    entity.SentTries++;
                                    entity.SentDate = DateTime.Now;
                                    context.SaveChanges();
                                }
                            }
                            catch(Exception ex)
                            {
                                LogManager.Instance.Write(ex.ToString());
                                using (var context = new CHCContext())
                                {
                                    var entity = context.Email.SingleOrDefault(e => e.Id == email.Id);
                                    entity.SentTries++;
                                    context.SaveChanges();
                                }
                            }
                        }
                    }

                    Thread.Sleep(5000);
                }
            }
        }
    }
}
