using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingNote.DBSource
{
    public class UserInfoManager
    {
        public static DataRow GetUserInfoByAccount(string account)
        {
            string connectionString = GetConnectionString();
            string dbCommandString =
                @" SELECT [ID], [Account], [PWD], [Name], [Email]
                   FROM UserInfo
                   WHERE [Account] = @account
                ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(dbCommandString, connection))
                {
                    command.Parameters.AddWithValue("@account", account);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        reader.Close();

                        if (dt.Rows.Count == 0)
                            return null;

                        DataRow dr = dt.Rows[0];
                        return dr;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(ex);
                        return null;
                    }
                }
            }
        }

        public static string GetConnectionString()
        {
            //string val =
            //    ConfigurationManager.AppSettings["ConnectionString"];
            string val =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return val;
        }

    }
}
