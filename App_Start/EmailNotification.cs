using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ICMS.App_Start
{
    public class EmailNotification
    {
        public static bool sendEmail(string CcEmailAddress, string RequestStage, string subject, string body, string RqID, string RqNum)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string ToEmailID = string.Empty;
                string RqNumber = string.Empty;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = string.Empty;

                    if (RequestStage == "Planner")
                    {
                        //body = "Request has been raised for @project of @Foregin for @inspectionStage of offered on this @offerdatetime";
                        //TO all Planner and initiator
                        query = "select tblEmpMst.EmpMail from tblEmpMst JOIN tblUsrRoleMst on tblEmpMst.EmpID=tblUsrRoleMst.EmpNo JOIN tblRoleMst on tblRoleMst.RoID=tblUsrRoleMst.RoleID where tblRoleMst.RoDesc='QCP'  and ISNULL(tblEmpMst.EmpMail,'')!=''";
                    }
                    if (RequestStage == "Assigned")
                    {
                        // body = "Planner has assigned you to this request has been raised for @project of @Foregin for @inspectionStage of offered on this @offerdatetime";
                        //to selected inspector and cc planner and inititator
                        //query = "select tblEmpMst.EmpMail from tblEmpMst JOIN tblUsrRoleMst on tblEmpMst.EmpID=tblUsrRoleMst.EmpNo JOIN tblRoleMst on tblRoleMst.RoID=tblUsrRoleMst.RoleID where tblRoleMst.RoDesc='QCP'  and ISNULL(tblEmpMst.EmpMail,'')!=''  and tblEmpMst.EmpPsNo in (" + ToEmailAddress.Substring(0, ToEmailAddress.Length - 1) + ")";
                        query = @"select InititorEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl
                            JOIN tblInspReq on tblInspReq.RqID = tblRqstPlnDtl.RqID
                            JOIN tblEmpMst InititorEmp on InititorEmp.EmpPsNo = tblInspReq.RqRegBy
                            where tblRqstPlnDtl.PlnID = '" + RqID + "' ";
                        query += @"    UNION
                            select AssignEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl
                            JOIN tblAssignInspct on tblAssignInspct.PlnID = tblRqstPlnDtl.PlnID
                            JOIN tblEmpMst AssignEmp on AssignEmp.EmpPsNo = tblAssignInspct.InsPsNo
                            where tblRqstPlnDtl.PlnID = '" + RqID + "'";
                    }
                    if (RequestStage == "Closed")
                    {
                        //body = "Request has been closed with @reason and @remark for @project of @Foregin for @inspectionStage of offered on this @offerdatetime";
                        //To initiator planner and cc selected inspector
                        query = @"select InititorEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl
                            JOIN tblInspReq on tblInspReq.RqID = tblRqstPlnDtl.RqID
                            JOIN tblEmpMst InititorEmp on InititorEmp.EmpPsNo = tblInspReq.RqRegBy
                            where tblRqstPlnDtl.PlnID = '" + RqID + "' ";
                        query += @" UNION
                            select AssignEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl
                            JOIN tblEmpMst AssignEmp on AssignEmp.EmpPsNo = tblRqstPlnDtl.PlnBy
                                where tblRqstPlnDtl.PlnID = '" + RqID + "'";
                        query += @"    UNION
                            select AssignEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl
                            JOIN tblAssignInspct on tblAssignInspct.PlnID = tblRqstPlnDtl.PlnID
                            JOIN tblEmpMst AssignEmp on AssignEmp.EmpPsNo = tblAssignInspct.InsPsNo
                            where tblRqstPlnDtl.PlnID = '" + RqID + "'";
                    }
                    if (RequestStage == "Group")
                    {
                        query = @"select distinct AssignEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl T
                            JOIN tblEmpMst AssignEmp on AssignEmp.EmpPsNo = T.PlnBy
                            JOIN tblInspReq on tblInspReq.RqID = T.RqID
                            where T.RqID in (" + RqNum.Substring(0, RqNum.Length - 1) + ")";
                        query += @"    UNION
                            select distinct T.EmpMail as EmpMail
                            from tblEmpMst T
                            JOIN tblUsrRoleMst T1 on T.EmpID=T1.EmpNo
                            JOIN tblRoleMst T2 on T2.RoID=T1.RoleID
                            where T2.RoID='2002'";
                    }
                    if (RequestStage == "HOD")
                    {
                        query = @"select distinct AssignEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl T
                            JOIN tblEmpMst AssignEmp on AssignEmp.EmpPsNo = T.PlnBy
                            JOIN tblInspReq on tblInspReq.RqID = T.RqID
                            where cast(T.RqNo as varchar)+'-'+cast(T.RqRevNo as varchar) in (" + RqNum.Substring(0, RqNum.Length - 1) + ")";
                        query += @" UNION
                            select AssignEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl
                            JOIN tblEmpMst AssignEmp on AssignEmp.EmpPsNo = tblRqstPlnDtl.PlnBy
                            where cast(RqNo as varchar)+'-'+cast(RqRevNo as varchar) in (" + RqNum.Substring(0, RqNum.Length - 1) + ")";
                        query += @"    UNION
                            select AssignEmp.EmpMail as EmpMail
                            from tblRqstPlnDtl
                            JOIN tblAssignInspct on tblAssignInspct.PlnID = tblRqstPlnDtl.PlnID
                            JOIN tblEmpMst AssignEmp on AssignEmp.EmpPsNo = tblAssignInspct.InsPsNo
                            where cast(RqNo as varchar)+'-'+cast(RqRevNo as varchar) in (" + RqNum.Substring(0, RqNum.Length - 1) + ")";
                    }
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ToEmailID = ToEmailID + Convert.ToString(sdr["EmpMail"]) + ",";
                            }
                        }
                        con.Close();
                    }

                }
                if (CcEmailAddress.EndsWith(",") == true)
                {
                    CcEmailAddress = CcEmailAddress.Substring(0, CcEmailAddress.Length - 1);
                }
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["From"].ToString());
                    mail.To.Add(ToEmailID.Substring(0, ToEmailID.Length - 1));
                    mail.CC.Add(CcEmailAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                    using (SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpClient"].ToString(), 25))
                    {
                        // smtp.Credentials = new NetworkCredential("LTSSHF.WebAppTest@larsentoubro.com", "dddcjlwmlqqzseya");
                        smtp.EnableSsl = false;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}