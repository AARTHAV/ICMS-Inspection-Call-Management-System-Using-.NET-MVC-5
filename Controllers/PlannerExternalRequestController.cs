using ICMS.App_Start;
using ICMS.Models;
using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ICMS.Controllers
{
    [IsAuthorized]
    public class PlannerExternalRequestController : Controller
    {
        // GET: PlannerExternalRequest
        public ActionResult Index(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 50;
                //Creating the ViewModel's Object
                DataSet ds = new DataSet();
                //List of the Person
                RequestPlannerDetailsViewModel objRequestPlannerDetailsViewModel = new RequestPlannerDetailsViewModel();

                List<RequestPlannerDetailsModel> lstRequestPlannerDetailsModel = new List<RequestPlannerDetailsModel>();

                RequestPlannerDetailsModel objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                objRequestPlannerDetailsModel.objInspReqModel = new List<InspReqModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllNeedToPlanExternalRequest", con);
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
                            objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqID"]);
                            objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                            objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                            objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                            objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                            objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["OfrDttm"]);
                            objInspReqModel.PlanStartDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                            objInspReqModel.PlanEndDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqDpcd"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqRegBy"]);
                            objRequestPlannerDetailsModel.tempValue = "";
                            objRequestPlannerDetailsModel.objInspReqModel.Add(objInspReqModel);
                            lstRequestPlannerDetailsModel.Add(objRequestPlannerDetailsModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objRequestPlannerDetailsViewModel.ListRequestPlannerDetails = lstRequestPlannerDetailsModel;
                        objRequestPlannerDetailsViewModel.pager = pager;
                    }
                }

                return View(objRequestPlannerDetailsViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View();
            }

        }

        [HttpPost]
        public ActionResult Create(RequestPlannerDetailsModel requestPlannerDetailsModel)
        {
            try
            {
                string query = "usp_DMLPlannerGroupRequest";
                string Inspector = requestPlannerDetailsModel.tempValue.Substring(0, requestPlannerDetailsModel.tempValue.Length - 1);
                var OutputID = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@GroupID", "0");
                            cmd.Parameters.AddWithValue("@RqNo", requestPlannerDetailsModel.tempValue);
                            cmd.Parameters.AddWithValue("@GroupBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@GroupDpcd", Session["EmployeeDeparment"].ToString());
                            cmd.Parameters.AddWithValue("@Status", "1");
                            var returnParameter = cmd.Parameters.Add("@OutputID", SqlDbType.NVarChar, 50);
                            returnParameter.Direction = ParameterDirection.Output;
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            OutputID = cmd.Parameters["@OutputID"].Value.ToString();
                            if (OutputID.StartsWith("Request"))
                            {
                                TempData["RecordException"] = "Request Already Grouped";
                                return View();
                            }
                            else
                            {
                                TempData["RecordException"] = "";
                            }
                            con.Close();
                            TempData["TransactionStatus"] = "Request Group Successfully :" + OutputID;
                        }
                        string ReqNumberListQuery = "SELECT ReqNum = STUFF((";
                        ReqNumberListQuery += " SELECT distinct ','+cast(T.RqNo as varchar)+'-'+cast(T.RqRevNo as varchar) ";
                        ReqNumberListQuery += " FROM tblRqstPlnDtl T ";
                        ReqNumberListQuery += " where T.RqID in (" + requestPlannerDetailsModel.tempValue.Substring(0, requestPlannerDetailsModel.tempValue.Length - 1) + ") ";
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
                        string body = "Request is Grouped and RequestNo is " + ReqNum.Substring(0, ReqNum.Length - 1);
                        if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "Group", "Request is Grouped ", body, OutputID, requestPlannerDetailsModel.tempValue))
                        {
                            TempData["RecordException"] = "Email Notification is not working";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["RecordException"] = ex.Message.ToString();
                        ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                        return RedirectToAction("Index");
                    }

                }
                //if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "Assigned", "Request Assigned", "Hello, Request is asgigned and request number is " + requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString(), OutputID, requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString()))
                //{
                //    TempData["RecordException"] = "Email Notification is not working";
                //}
                TempData["TransactionStatus"] = "Request Group Successfully :" + Inspector;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.Message.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Index");
            }
        }

        public ActionResult Rejected(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 50;
                //Creating the ViewModel's Object
                DataSet ds = new DataSet();
                //List of the Person
                string Inspector = string.Empty;
                GroupHeaderViewModel objGroupHeaderViewModel = new GroupHeaderViewModel();

                List<GroupHeaderModel> lstGroupHeaderModel = new List<GroupHeaderModel>();

                GroupHeaderModel objGroupHeaderModel = new GroupHeaderModel();
                objGroupHeaderModel.listInspReqModel = new List<InspReqModel>();
                objGroupHeaderModel.listRequestPlannerDetailsModels = new List<RequestPlannerDetailsModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllPlannerRejectedExternalRequest", con);
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
                                objGroupHeaderModel.Remark = Convert.IsDBNull(ds.Tables[0].Rows[i]["HODRemark"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["HODRemark"]);
                            }
                            objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqID"]);
                            objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                            objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                            objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                            objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                            if (!Convert.IsDBNull(ds.Tables[0].Rows[i]["InternalPlanEndDate"]))
                            {
                                objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["InternalPlanEndDate"]);
                            }
                            else
                            {
                                objInspReqModel.OfrDttm = DateTime.Today;
                            }
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqDpcd"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqRegBy"]);
                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
                            objRequestPlannerDetailsModel.PlnDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnDttm"]);
                            objRequestPlannerDetailsModel.PlnBy = Convert.ToString(ds.Tables[0].Rows[i]["PlannedBy"]);
                            objRequestPlannerDetailsModel.PlnRemark = Convert.ToString(ds.Tables[0].Rows[i]["PlnRemark"]);
                            objRequestPlannerDetailsModel.AgcyID = Convert.ToInt32(ds.Tables[0].Rows[i]["AgcyID"]);
                            objRequestPlannerDetailsModel.InsBy = Convert.ToString(ds.Tables[0].Rows[i]["InsPsNo"]);
                            objRequestPlannerDetailsModel.PlnID = Convert.ToInt32(ds.Tables[0].Rows[i]["PlnID"]);
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
                ViewBag.Employee = new SelectList(GetAllEmployee(0).ToList(), "EmpPsNo", "EmpName");
                return View(objGroupHeaderViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View();
            }

        }

        public ActionResult Print(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 50;
                //Creating the ViewModel's Object
                DataSet ds = new DataSet();
                //List of the Person
                string Inspector = string.Empty;
                GroupHeaderViewModel objGroupHeaderViewModel = new GroupHeaderViewModel();

                List<GroupHeaderModel> lstGroupHeaderModel = new List<GroupHeaderModel>();

                GroupHeaderModel objGroupHeaderModel = new GroupHeaderModel();
                objGroupHeaderModel.listInspReqModel = new List<InspReqModel>();
                objGroupHeaderModel.listRequestPlannerDetailsModels = new List<RequestPlannerDetailsModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllPrintDataByPlanner", con);
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
                            objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqstID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqstID"]);
                            objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                            objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                            objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                            objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
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
                return View("Print", objGroupHeaderViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View();
            }

        }

        private List<EmployeeModel> GetAllEmployee(int PlnID)
        {
            try
            {
                List<EmployeeModel> lstEmployeeModel = new List<EmployeeModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com;
                    if (PlnID == 0)
                    {
                        com = new SqlCommand("Select * from vwInspectorList", con);//where Status=1
                    }
                    else
                    {
                        com = new SqlCommand("Select EmpPsNo,EmpPsNo+'-'+EmpName 'EmpName' from vwInspectorList where EmpPsNo not in (select InsPsNo from tblAssignInspct where PlnID='" + PlnID + "')", con);//where Status=1
                    }
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
                            EmployeeModel objEmployeeModel = new EmployeeModel();
                            objEmployeeModel.EmpPsNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpPsNo"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["EmpPsNo"]);
                            objEmployeeModel.EmpName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            lstEmployeeModel.Add(objEmployeeModel);
                        }
                    }
                    if (lstEmployeeModel.Count > 0)
                    {
                        return lstEmployeeModel;
                    }
                    ViewBag.RecordException = "Please check Employee Master table because there is no data";
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                ViewBag.RecordException = ex.ToString();
                return null;
            }

        }

        public ActionResult GetAgencyInfromation(string RequestType)
        {
            try
            {
                var result = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select AgID,AgName from vwAgencyList", con);//where Status=1
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
                        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            result.Add(new SelectListItem
                            {
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgID"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["AgID"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgName"])
                            });
                        }
                    }
                    if (result.Count == 0)
                    {
                        ViewBag.RecordException = "Please check Agency Master because there is no data";
                        return RedirectToAction("Reject");
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Reject");
            }
        }

        [HttpPost]
        public ActionResult ReschduleRequest(GroupHeaderViewModel groupHeaderViewModel)
        {
            try
            {
                string OldEmployee = string.Empty;
                Random randomNumber = new Random();

                if (groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ID != 0)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                    {
                        con.Open();
                        DataSet ds = new DataSet();
                        SqlCommand com = new SqlCommand("select T1.EmpMail from tblAssignInspct T JOIN tblEmpMst T1 on T.InsPsNo=T1.EmpPsNo where T.PlnID='" + groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ID + "'", con);//where Status=1
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
                                OldEmployee += ds.Tables[0].Rows[i][0].ToString() + ",";
                            }
                        }
                    }
                }
                string query = "usp_DMLPlannerRequest";
                string body = string.Empty;

                string ExternalInspector = groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ExternaltempValue.Substring(0, groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ExternaltempValue.Length - 1);
                List<string> listExternalInspector = ExternalInspector.Split(',').ToList();


                var OutputID = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PlnID", groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ID);
                            cmd.Parameters.AddWithValue("@PlnStart", groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ExtenalRquestPlnStart);
                            cmd.Parameters.AddWithValue("@PlnEnd", groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ExtenalRquestPlnEnd);
                            cmd.Parameters.AddWithValue("@RqstType", "2");
                            cmd.Parameters.AddWithValue("@RqID", "0");
                            cmd.Parameters.AddWithValue("@RqNo", "0");
                            cmd.Parameters.AddWithValue("@RqRevNo", "0");
                            cmd.Parameters.AddWithValue("@IsReplan", "1");
                            cmd.Parameters.AddWithValue("@AgencyID", groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].AgcyID);
                            cmd.Parameters.AddWithValue("@PlnBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@PlnDpcd", Session["EmployeeDeparment"].ToString());
                            cmd.Parameters.AddWithValue("@PlnRemark", groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ExternalPlnRemark);
                            var returnParameter = cmd.Parameters.Add("@OutputID", SqlDbType.NVarChar, 50);
                            returnParameter.Direction = ParameterDirection.Output;
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            OutputID = cmd.Parameters["@OutputID"].Value.ToString();
                            con.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["RecordException"] = ex.Message.ToString();
                        ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                        //return View("Rejected", new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString()));
                        return RedirectToAction("Rejected");
                    }

                }

                foreach (HttpPostedFileBase file in groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].UploadedExternalFile)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var InputFileName = Path.GetFileName(file.FileName);
                        InputFileName = randomNumber.Next().ToString() + "-" + InputFileName;
                        var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                        file.SaveAs(ServerSavePath);

                        using (SqlConnection conection = new SqlConnection(constr))
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_DMLDocument"))
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@DocID", 0);
                                    cmd.Parameters.AddWithValue("@RqstID", groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ID);
                                    cmd.Parameters.AddWithValue("@RqRevNo", groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].RqRevNo);
                                    cmd.Parameters.AddWithValue("@DocName", InputFileName);
                                    cmd.Parameters.AddWithValue("@DocAddr", ServerSavePath);
                                    cmd.Parameters.AddWithValue("@Replan", "0");
                                    cmd.Parameters.AddWithValue("@PlanID", OutputID);
                                    cmd.Parameters.AddWithValue("@UpldBy", Session["EmpPsNo"].ToString());
                                    cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                                    cmd.Connection = conection;
                                    conection.Open();
                                    cmd.ExecuteNonQuery();
                                    conection.Close();
                                }
                                catch (Exception ex)
                                {
                                    TempData["RecordException"] = ex.Message.ToString();
                                    ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                    //return View("Rejected", new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString()));
                                    return RedirectToAction("Rejected");
                                }

                            }
                        }

                    }
                }

                if (listExternalInspector != null)
                {
                    foreach (var item in listExternalInspector)
                    {
                        using (SqlConnection conection = new SqlConnection(constr))
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_DMLAssignInspct"))
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@InsID", 0);
                                    cmd.Parameters.AddWithValue("@InsPsNo", item);
                                    cmd.Parameters.AddWithValue("@PlnID", OutputID);
                                    cmd.Parameters.AddWithValue("@InsBy", Session["EmpPsNo"].ToString());
                                    cmd.Parameters.AddWithValue("@InspDpcd", Session["EmployeeDeparment"].ToString());
                                    cmd.Connection = conection;
                                    conection.Open();
                                    cmd.ExecuteNonQuery();
                                    conection.Close();
                                }
                                catch (Exception ex)
                                {
                                    TempData["RecordException"] = ex.Message.ToString();
                                    ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                    //return View("Rejected", new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString()));
                                    return RedirectToAction("Rejected");
                                }

                            }
                        }
                    }
                }


                //body = "Inspection external request has been assigned to you for Proejct" + groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].objInspReqModel[0].PrjNo.ToString() + "/" + groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].objInspReqModel[0].FrgNo.ToString() + " for stage '" +  groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0]. groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0][0].StageName.ToString() + "' of planned on '" + groupHeaderViewModel.PlnStart + "'";
                //if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "Assigned", "Request Assigned " + groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].objInspReqModel[0].RqNo + "-" + groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].objInspReqModel[0].RqRevNo.ToString(), body, OutputID, groupHeaderViewModel.objInspReqModel[0].RqNo + "-" + groupHeaderViewModel.objInspReqModel[0].RqRevNo.ToString()))
                //{
                //    TempData["RecordException"] = "Email Notification is not working";
                //}
                TempData["TransactionStatus"] = "Request Replanned Successfully :" + groupHeaderViewModel.ListGroup[0].listRequestPlannerDetailsModels[0].ID;
                return RedirectToAction("Rejected");
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.Message.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Rejected");
            }
        }

        [HttpGet]
        public ActionResult ResubmitRequest(int? id)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_DMLPlannerGroupRequest"))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@GroupID", id);
                            cmd.Parameters.AddWithValue("@RqNo", 0);
                            cmd.Parameters.AddWithValue("@Status", 0);
                            cmd.Parameters.AddWithValue("@GroupBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@GroupDpcd", Session["EmployeeDeparment"].ToString());
                            var returnParameter = cmd.Parameters.Add("@OutputID", SqlDbType.NVarChar, 50);
                            returnParameter.Direction = ParameterDirection.Output;
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            TempData["RecordException"] = ex.Message.ToString();
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            return RedirectToAction("Rejected");
                        }

                    }
                }
                TempData["TransactionStatus"] = "Request Resubmitted Successfully";
                return RedirectToAction("Rejected");
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.Message.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Rejected");
            }
        }

        [HttpGet]
        public ActionResult RemoveGroupRequest(int? id, string data)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_DMLPlannerRequestRemoveFromGroup"))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@RqNo", id);
                            cmd.Parameters.AddWithValue("@GroupID", data);
                            cmd.Parameters.AddWithValue("@PlnnerBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@PlnnerDpcd", Session["EmployeeDeparment"].ToString());
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            TempData["RecordException"] = ex.Message.ToString();
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            return RedirectToAction("Rejected");
                        }

                    }
                }
                TempData["TransactionStatus"] = "Request Removed from group Successfully :" + id;
                return RedirectToAction("Rejected");
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.Message.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Rejected");
            }
        }

        public ActionResult DownloadExcelFile(string id)
        {
            try
            {
                //Defining the PageSize
                //Creating the ViewModel's Object
                DataTable ds = new DataTable();
                //List of the Person
                string Inspector = string.Empty;
                List<InspReqModel> lstInspReqModel = new List<InspReqModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getOnePrintDataByPlanner", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    //com.Parameters.AddWithValue("@GroupID", id);
                    SqlDataAdapter adapt = new SqlDataAdapter(com);
                    //Fill the Dataset and Close the connection
                    adapt.Fill(ds);
                    con.Close();
                    //Bind the data in List of type Person
                    //We are returning Dataset with two Datatable, one contains the Person Data and Other contains the total records count
                    if (ds != null)
                    {
                        //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        //{
                        //    InspReqModel objInspReqModel = new InspReqModel();
                        //    objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqstID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqstID"]);
                        //    objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                        //    objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                        //    objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                        //    objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                        //    objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                        //    objInspReqModel.PlanEndDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
                        //    objInspReqModel.PlanStartDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                        //    objInspReqModel.Remark = Convert.ToString(ds.Tables[0].Rows[i]["Remarks"]);
                        //    lstInspReqModel.Add(objInspReqModel);
                        //}
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        //var gv = new GridView();
                        //gv.DataSource = lstInspReqModel.Select(i => new { i.RqNo, i.RqRevNo, i.PrjNo, i.FrgNo, i.StageName, i.PlanStartDateTime, i.PlanEndDateTime, i.Remark }).ToList();
                        //gv.DataBind();
                        //Response.ClearContent();
                        //Response.Buffer = true;
                        //Response.AddHeader("content-disposition", "attachment; filename=ExportFile.xls");
                        //Response.ContentType = "application/ms-excel";
                        //Response.Charset = "";
                        //StringWriter objStringWriter = new StringWriter();
                        //HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                        //gv.RenderControl(objHtmlTextWriter);
                        //Response.Output.Write(objStringWriter.ToString());
                        //Response.Flush();
                        //Response.End();
                        ReportViewer reportViewer = new ReportViewer();
                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.LocalReport.ReportPath = Server.MapPath(@"\Reports\Inspection.rdl");
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(900);
                        reportViewer.Height = Unit.Percentage(900);
                        //ReportParameter parameter = new ReportParameter("GroupID", id);
                        //ReportParameter[] parameter = new ReportParameter[1];
                        //parameter[0] = new ReportParameter("GroupID", id);
                        //reportViewer.LocalReport.SetParameters(parameter);
                        //DataSet converted = ConvertToDataSet(lstInspReqModel, "DataSet1");
                        DataTable filtered = ds.Select("GroupID='" + id + "'").CopyToDataTable();// .AsEnumerable().Where(r => r.Field<Int32>("GroupID").Equals(id));
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", filtered));
                        reportViewer.LocalReport.Refresh();
                        ViewBag.ReportViewer = reportViewer;
                    }

                }
                return View("DownloadExcelFile");
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Print");
            }
        }
        //public ActionResult DownloadExcelFile(string id)
        //{
        //    try
        //    {
        //        //Defining the PageSize
        //        //Creating the ViewModel's Object
        //        DataSet ds = new DataSet();
        //        //List of the Person
        //        string Inspector = string.Empty;
        //        List<InspReqModel> lstInspReqModel = new List<InspReqModel>();
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
        //        {
        //            con.Open();
        //            SqlCommand com = new SqlCommand("usp_getOnePrintDataByPlanner", con);
        //            com.CommandType = CommandType.StoredProcedure;
        //            //Passing the Offset value in the procedure
        //            com.Parameters.AddWithValue("@GroupID", id);
        //            SqlDataAdapter adapt = new SqlDataAdapter(com);
        //            //Fill the Dataset and Close the connection
        //            adapt.Fill(ds);
        //            con.Close();
        //            //Bind the data in List of type Person
        //            //We are returning Dataset with two Datatable, one contains the Person Data and Other contains the total records count
        //            if (ds != null)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    InspReqModel objInspReqModel = new InspReqModel();
        //                    objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqstID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqstID"]);
        //                    objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
        //                    objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
        //                    objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
        //                    objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
        //                    objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
        //                    objInspReqModel.PlanEndDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
        //                    objInspReqModel.PlanStartDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
        //                    objInspReqModel.Remark = Convert.ToString(ds.Tables[0].Rows[i]["Remarks"]);
        //                    lstInspReqModel.Add(objInspReqModel);
        //                }
        //                //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
        //                var gv = new GridView();
        //                gv.DataSource = lstInspReqModel.Select(i => new { i.RqNo, i.RqRevNo, i.PrjNo, i.FrgNo, i.StageName, i.PlanStartDateTime, i.PlanEndDateTime, i.Remark }).ToList();
        //                gv.DataBind();
        //                Response.ClearContent();
        //                Response.Buffer = true;
        //                Response.AddHeader("content-disposition", "attachment; filename=ExportFile.xls");
        //                Response.ContentType = "application/ms-excel";
        //                Response.Charset = "";
        //                StringWriter objStringWriter = new StringWriter();
        //                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
        //                gv.RenderControl(objHtmlTextWriter);
        //                Response.Output.Write(objStringWriter.ToString());
        //                Response.Flush();
        //                Response.End();
        //            }

        //        }
        //        return RedirectToAction("Print");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.RecordException = ex.ToString();
        //        ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
        //        return RedirectToAction("Print");
        //    }
        //}
    }
}