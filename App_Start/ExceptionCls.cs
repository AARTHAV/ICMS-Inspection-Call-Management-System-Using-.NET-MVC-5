using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ICMS.App_Start
{
    public class ExceptionCls
    {
        public static void SaveException(Exception exception,string methodName,string controllerName)
        {
            try
            {
                string query = "usp_DMLAppExceptionLog";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ControllerName", controllerName);
                        cmd.Parameters.AddWithValue("@ExceptionInformation", exception.ToString());
                        cmd.Parameters.AddWithValue("@MethodName", methodName.ToString());
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteScalar();
                        con.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}