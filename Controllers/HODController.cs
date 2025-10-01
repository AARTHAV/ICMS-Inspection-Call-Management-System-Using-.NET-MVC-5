using ICMS.App_Start;
using ICMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICMS.Controllers
{
    [IsAuthorized]
    public class HODController : Controller
    {
        // GET: HOD
        public ActionResult Index(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 50;
                //Creating the ViewModel's Object
                DataSet ds = new DataSet();
                //List of the Person
                GroupHeaderViewModel objGroupHeaderViewModel = new GroupHeaderViewModel();

                List<GroupHeaderModel> lstGroupHeaderModel = new List<GroupHeaderModel>();

                GroupHeaderModel objGroupHeaderModel = new GroupHeaderModel();
                objGroupHeaderModel.listInspReqModel = new List<InspReqModel>();
                objGroupHeaderModel.listRequestPlannerDetailsModels = new List<RequestPlannerDetailsModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllHODExternalRequest", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    com.Parameters.AddWithValue("@OffsetValue", (page - 1) * PageSize);
                    com.Parameters.AddWithValue("@PagingSize", PageSize);
                    SqlDataAdapter adapt = new SqlDataAdapter(com);
                    //Fill the Dataset and Close the connection
                    adapt.Fill(ds);
                    con.Close();
                    //Bind the data in List of type Person
                    //We are returning Dataset with two Datatable, one contains the Person Data and Other contains the total records count
                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            InspReqModel objInspReqModel = new InspReqModel();
                            RequestPlannerDetailsModel objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                            bool containsItem = lstGroupHeaderModel.Any(item => item.GroupID == Convert.ToInt32(ds.Tables[0].Rows[i]["GroupID"]));
                            if (containsItem == false)
                            {
                                objGroupHeaderModel = new GroupHeaderModel();
                                objGroupHeaderModel.listInspReqModel = new List<InspReqModel>();
                                objGroupHeaderModel.listRequestPlannerDetailsModels = new List<RequestPlannerDetailsModel>();
                                objGroupHeaderModel.GroupID = Convert.IsDBNull(ds.Tables[0].Rows[i]["GroupID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["GroupID"]);
                                objGroupHeaderModel.GroupDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["GroupDttm"]);
                                objGroupHeaderModel.GroupDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["Groupdadp"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["Groupdadp"]);
                                objGroupHeaderModel.GroupBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["GroupBy"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["GroupBy"]);
                            }
                            objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqID"]);
                            objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                            objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                            objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                            objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                            objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["OfrDttm"]);
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqDpcd"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqRegBy"]);
                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
                            objRequestPlannerDetailsModel.PlnDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnDttm"]);
                            objRequestPlannerDetailsModel.PlnBy = Convert.ToString(ds.Tables[0].Rows[i]["PlannedBy"]);
                            objGroupHeaderModel.listInspReqModel.Add(objInspReqModel);
                            objGroupHeaderModel.listRequestPlannerDetailsModels.Add(objRequestPlannerDetailsModel);
                            if (containsItem == false)
                            {
                                lstGroupHeaderModel.Add(objGroupHeaderModel);
                            }
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objGroupHeaderViewModel.ListGroup = lstGroupHeaderModel;
                        objGroupHeaderViewModel.pager = pager;
                    }
                }
                ViewBag.Reason = new SelectList(GetReasonMasterModels().ToList(), "RsnID", "RsnDesc");
                return View("Index", objGroupHeaderViewModel);
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                ViewBag.RecordException = ex.ToString();
                return View();
            }

        }
        private List<ReasonMstModel> GetReasonMasterModels()
        {
            try
            {
                List<ReasonMstModel> lstReasonMstModel = new List<ReasonMstModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select RsnID,RsnDesc from vwReasonList", con);//where Status=1
                    com.CommandType = CommandType.Text;
                    //Passing the Offset value in the procedure
                    SqlDataAdapter adapt = new SqlDataAdapter(com);
                    //Fill the Dataset and Close the connection
                    adapt.Fill(ds);
                    con.Close();
                    //Bind the data in List of type Person
                    //We are returning Dataset with two Datatable, one contains the Person Data and Other contains the total records count
                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ReasonMstModel objReasonMstModel = new ReasonMstModel();
                            objReasonMstModel.RsnID =Convert.ToInt32(ds.Tables[0].Rows[i]["RsnID"]);
                            objReasonMstModel.RsnDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RsnDesc"]);
                            lstReasonMstModel.Add(objReasonMstModel);
                        }
                    }
                    if (lstReasonMstModel.Count > 0)
                    {
                        return lstReasonMstModel;
                    }
                    else
                    {
                        TempData["RecordException"] = "Please check Reason table because there is no data in this table";
                        ReasonMstModel objReasonMstModel = new ReasonMstModel();
                        objReasonMstModel.RsnID = 0;
                        objReasonMstModel.RsnDesc = "";
                        lstReasonMstModel.Add(objReasonMstModel);
                        return lstReasonMstModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return null;
            }

        }
        [HttpGet]
        public ActionResult ApproveRequest(int? id)
        {
            try
            {
                var OutputID = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_DMLHODRequest";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IsApproved", 1);
                            cmd.Parameters.AddWithValue("@GroupID", id);
                            cmd.Parameters.AddWithValue("@ReasonID", DBNull.Value);
                            cmd.Parameters.AddWithValue("@GroupBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@GroupDpcd", Session["EmployeeDeparment"].ToString());
                            var returnParameter = cmd.Parameters.Add("@OutputID", SqlDbType.NVarChar, 50);
                            returnParameter.Direction = ParameterDirection.Output;
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            OutputID = cmd.Parameters["@OutputID"].Value.ToString();
                            TempData["TransactionStatus"] = "Request Approved Successfully :" + OutputID;
                        }
                        catch (Exception ex)
                        {
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            TempData["RecordException"] = ex.ToString();
                            return RedirectToAction("Index");
                        }
                    }
                    string ReqNumberListQuery = "SELECT ReqNum = STUFF((";
                    ReqNumberListQuery += " SELECT distinct ','''+cast(T.RqNo as varchar)+'-'+cast(T.RqRevNo as varchar)+''''  ";
                    ReqNumberListQuery += " FROM tblRqstPlnDtl T ";
                    ReqNumberListQuery += " JOIN GroupTransDetails GD on GD.RqstID=T.RqID and GD.Status='1'";
                    ReqNumberListQuery += " where GD.GroupID in (" + id + ") and T.RqstType='2' and T.RqStatus!='8' ";
                    ReqNumberListQuery += " FOR XML PATH('') ";
                    ReqNumberListQuery += " ), 1, 1, '') ";
                    string ReqNum = string.Empty;
                    using (SqlCommand cmd = new SqlCommand(ReqNumberListQuery))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = ReqNumberListQuery;
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ReqNum = ReqNum + Convert.ToString(sdr["ReqNum"]) + ",";
                            }
                        }
                    }
                    string body = "Request is Approved and RequestNo is " + ReqNum.Substring(0, ReqNum.Length - 1);
                    if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "HOD", "Request is Grouped ", body, OutputID, ReqNum))
                    {
                        TempData["RecordException"] = "Email Notification is not working";
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult RejectRequest(GroupHeaderViewModel objGroupHeaderModel)
        {
            try
            {
                var OutputID = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_DMLHODRequest";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IsApproved", 0);
                            cmd.Parameters.AddWithValue("@GroupID", objGroupHeaderModel.ListGroup[0].ID);
                            cmd.Parameters.AddWithValue("@Remark", objGroupHeaderModel.ListGroup[0].Remark);
                            cmd.Parameters.AddWithValue("@ReasonID", objGroupHeaderModel.ListGroup[0].Reason);
                            cmd.Parameters.AddWithValue("@GroupBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@GroupDpcd", Session["EmployeeDeparment"].ToString());
                            var returnParameter = cmd.Parameters.Add("@OutputID", SqlDbType.NVarChar, 50);
                            returnParameter.Direction = ParameterDirection.Output;
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            OutputID = cmd.Parameters["@OutputID"].Value.ToString();
                            TempData["TransactionStatus"] = "Request Rejected Successfully :" + OutputID;
                        }
                        catch (Exception ex)
                        {
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            TempData["RecordException"] = ex.ToString();
                            return RedirectToAction("Index");
                        }
                    }
                
                    string ReqNumberListQuery = "SELECT ReqNum = STUFF((";
                    ReqNumberListQuery += " SELECT distinct ','''+cast(T.RqNo as varchar)+'-'+cast(T.RqRevNo as varchar)+''''  ";
                    ReqNumberListQuery += " FROM tblRqstPlnDtl T ";
                    ReqNumberListQuery += " JOIN GroupTransDetails GD on GD.RqstID=T.RqID and GD.Status='1'";
                    ReqNumberListQuery += " where GD.GroupID in (" + objGroupHeaderModel.ListGroup[0].ID + ") and T.RqstType='2' and T.RqStatus!='8' ";
                    ReqNumberListQuery += " FOR XML PATH('') ";
                    ReqNumberListQuery += " ), 1, 1, '') ";
                    string ReqNum = string.Empty;
                    using (SqlCommand cmd = new SqlCommand(ReqNumberListQuery))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = ReqNumberListQuery;
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ReqNum = ReqNum + Convert.ToString(sdr["ReqNum"]) + ",";
                            }
                        }
                    }
                    string body = "Request is Rejected and RequestNo is " + ReqNum.Substring(0, ReqNum.Length - 1);
                    if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "HOD", "Request is Grouped ", body, OutputID, ReqNum))
                    {
                        TempData["RecordException"] = "Email Notification is not working";
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}