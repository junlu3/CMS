using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class EmailService:IEmailService
   {
       private readonly IEmailRepository _emailRepository;
       public EmailService(IEmailRepository emailRepository)
       {
           _emailRepository = emailRepository;
       }

        public void Add(Email item)
        {
            _emailRepository.Add(item);
        }

        //public void Add(Email item)
        //{
        //    try
        //    {
        //        SqlParameter[] paramters = new SqlParameter[6];
        //        paramters[0] = new SqlParameter("@To", item.To );
        //        paramters[1] = new SqlParameter("@ToName", item.ToName);
        //        paramters[2] = new SqlParameter("@Body", item.Body);
        //        paramters[3] = new SqlParameter("@Bcc", item.Bcc);
        //        paramters[4] = new SqlParameter("@CreatedDate", item.CreatedDate);
        //        paramters[5] = new SqlParameter("@CreatedBy", item.CreatedBy);

        //        string strConn = ConfigurationManager.ConnectionStrings["UtilityContext"].ConnectionString;
        //        using (SqlConnection conn = new SqlConnection(strConn))
        //        {
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"INSERT INTO [Email]
        //                                       ([To],[ToName],[Body],[Bcc],[CreatedDate],[CreatedBy])
        //                                 VALUES
        //                                       (@To,@ToName,@Body,@Bcc,@CreatedDate,@CreatedBy)";
        //                cmd.Parameters.AddRange(paramters);
        //                if (conn.State != System.Data.ConnectionState.Open)
        //                {
        //                    conn.Open();
        //                }
        //                cmd.ExecuteNonQuery();

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
