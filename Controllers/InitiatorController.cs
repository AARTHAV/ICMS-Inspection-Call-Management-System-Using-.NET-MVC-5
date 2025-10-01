using ICMS.App_Start;
using ICMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace ICMS.Controllers
{
    //[HandleError]
    //[HandleError(ExceptionType = typeof(Exception), View = "~/Views/Error/Error.cshtml")]
    [IsAuthorized]
    public class InitiatorController : Controller
    {
        // GET: Initiator
        public ActionResult Index(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 50;
                //Creating the ViewModel's Object
                InspReqViewModel objInspReqViewModel = new InspReqViewModel();
                DataSet ds = new DataSet();
                //List of the Person
                List<InspReqModel> lstInspReqModel = new List<InspReqModel>();


                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllInitiatorRequest", con);
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
                            objInspReqModel.RqRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["RqRegDttm"], CultureInfo.CurrentCulture);
                            objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["OfrDttm"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqDpcd"]);
                            objInspReqModel.RqRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpPsNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpPsNo"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                           // objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpPsNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpPsNo"]);
                            objInspReqModel.EmployeeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            objInspReqModel.PlanID = Convert.IsDBNull(ds.Tables[0].Rows[i]["PlnID"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["PlnID"]);
                            if (Convert.ToString(ds.Tables[0].Rows[i]["RqType"]) == "1")
                            {
                                objInspReqModel.RqTypeName = "Internal";
                            }
                            else
                            {
                                objInspReqModel.RqTypeName = "External";
                            }
                            lstInspReqModel.Add(objInspReqModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objInspReqViewModel.ListInspReq = lstInspReqModel;
                        objInspReqViewModel.pager = pager;
                    }
                }
                ViewBag.IsSearch = "false";
                ViewBag.Status = new SelectList(GetAllStatus().ToList(), "StID", "StDesc");
                return View(objInspReqViewModel);
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }

        [HttpGet]
        public ActionResult Search(string StID, string searchText, int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 50;
                //Creating the ViewModel's Object
                InspReqViewModel objInspReqViewModel = new InspReqViewModel();
                DataSet ds = new DataSet();
                //List of the Person
                List<InspReqModel> lstInspReqModel = new List<InspReqModel>();


                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    ViewBag.IsSearch = "false";
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_SearchInitiatorRequest", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    com.Parameters.AddWithValue("@StatusID", StID);
                    if (searchText != null)
                    {
                        com.Parameters.AddWithValue("@text", searchText);
                    }
                    else
                    {
                        com.Parameters.AddWithValue("@text", "");
                    }
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
                            if (i == 0)
                            {
                                ViewBag.IsSearch = "true";
                                ViewBag.SearchStatusID = StID;
                                ViewBag.SearchText = searchText;
                            }
                            InspReqModel objInspReqModel = new InspReqModel();
                            objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqID"]);
                            objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                            objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                            objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                            objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                            objInspReqModel.RqRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["RqRegDttm"], CultureInfo.CurrentCulture);
                            objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["OfrDttm"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqDpcd"]);
                            objInspReqModel.RqRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpPsNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpPsNo"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            objInspReqModel.EmployeeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            if (Convert.ToString(ds.Tables[0].Rows[i]["RqType"]) == "1")
                            {
                                objInspReqModel.RqTypeName = "Internal";
                            }
                            else
                            {
                                objInspReqModel.RqTypeName = "External";
                            }
                            lstInspReqModel.Add(objInspReqModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objInspReqViewModel.ListInspReq = lstInspReqModel;
                        objInspReqViewModel.pager = pager;
                    }
                }
                ViewBag.Status = new SelectList(GetAllStatus().ToList(), "StID", "StDesc");
                ViewBag.SearchData = searchText;
                if (objInspReqViewModel.ListInspReq.Count == 0)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View("Index", objInspReqViewModel);
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                ViewBag.RecordException = ex.ToString();
                return View("Index");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            //TempData["RecordException"] = "";
            //TempData["ExistingReqNo"] = "";
            try
            {
                var model = new InspReqModel();
                model.listProject = GetProjectList();
                ViewBag.stage = new SelectList(GetAllStage().ToList(), "StgID", "StgDesc");
                ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName");
                ViewBag.Forging = new SelectList(GetForging().ToList(), "FrgNo", "FrgNo");
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Index");
            }

        }

        private bool IsOpenAnyRequest(string ProjectNo, string FrgNo, string StageID)
        {
            try
            {
                string countOfWithClose = "";
                string countTotalRecord = "";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    SqlCommand com = new SqlCommand("select TOP 1 (select count(*) from tblRqstPlnDtl T1 where T1.RqID=T.RqID and T1.RqstType='2' and T1.RqStatus in ('4003','7','2002','3003','2003','5')) cnt,T.RqNo,T.RqRevNo from tblInspReq T where T.PrjNo='" + ProjectNo + "' and T.FrgNo like '%" + FrgNo + "%' and T.StgID='" + StageID + "'  order by T.RqRegDttm desc", con);//where Status=1
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
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            countTotalRecord = ds.Tables[0].Rows[0][0].ToString();
                            if (countTotalRecord.ToString() != "0")
                            {
                                TempData["ExistingReqNo"] = "Please Check :" + ds.Tables[0].Rows[0][1].ToString() + "-" + ds.Tables[0].Rows[0][2].ToString() + " Request Number";
                                return false;
                            }
                        }
                        //else
                        //{
                        //    return true;
                        //}
                    }
                    else
                    {
                        return true;
                    }
                }
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    SqlCommand com = new SqlCommand("select count(*) from tblInspReq where PrjNo='" + ProjectNo + "' and StgID='" + StageID + "' and FrgNo like '%" + FrgNo + "%' ", con);//where Status=1
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
                        if (ds.Tables.Count > 0)
                        {
                            countOfWithClose = ds.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    SqlCommand com = new SqlCommand("select TOP 1 count(*),max(RqNo) RqNo,max(RqRevNo) RqNo from tblInspReq where PrjNo='" + ProjectNo + "' and StgID='" + StageID + "' and FrgNo like '%" + FrgNo + "%' ", con);//where Status=1
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
                        if (ds.Tables.Count > 0)
                        {
                            countTotalRecord = ds.Tables[0].Rows[0][0].ToString();
                            if (countTotalRecord.ToString() != "0")
                            {
                                TempData["ExistingReqNo"] = "Please Check :" + ds.Tables[0].Rows[0][1].ToString() + "-" + ds.Tables[0].Rows[0][2].ToString() + " Request Number";

                            }
                        }
                    }
                }
                if (countOfWithClose != countTotalRecord)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return true;
            }
        }
        private List<SelectListItem> GetProjectList()
        {
            try
            {
                var result = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    SqlCommand com = new SqlCommand("Select distinct ProjectNo ProjectInfo from vwProejctlst order by ProjectNo", con);//where Status=1
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
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectInfo"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectInfo"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectInfo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectInfo"])
                            });
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return null;
            }
        }

        private JsonResult GetProjectAutoComplete(string term)
        {
            try
            {
                var result = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    SqlCommand com = new SqlCommand("Select  distinct TOP 10 ProjectNo ProjectInfo from vwProejctlst where ProjectNo like '" + term + "%'", con);//where Status=1
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
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectInfo"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectInfo"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectInfo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectInfo"])
                            });
                        }
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return null;
            }
        }

        private List<LocMstModel> GetAllLocation()
        {
            try
            {
                List<LocMstModel> lstLocMstModel = new List<LocMstModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select * from vwLocationMaster order by LcName", con);//where Status=1
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
                            LocMstModel objLocMstModel = new LocMstModel();
                            objLocMstModel.LcID = Convert.IsDBNull(ds.Tables[0].Rows[i]["LcID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["LcID"]);
                            objLocMstModel.LcName = Convert.IsDBNull(ds.Tables[0].Rows[i]["LcName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["LcName"]);
                            lstLocMstModel.Add(objLocMstModel);
                        }
                    }
                    if (lstLocMstModel.Count > 0)
                    {
                        return lstLocMstModel;
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

        private List<StageModel> GetAllStage()
        {
            try
            {
                List<StageModel> lstStageModel = new List<StageModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select * from vwStageList order by StgDesc", con);//where Status=1
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
                            StageModel objStageModel = new StageModel();
                            objStageModel.StgID = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["StgID"]);
                            objStageModel.StgDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            lstStageModel.Add(objStageModel);
                        }
                    }
                    if (lstStageModel.Count > 0)
                    {
                        return lstStageModel;
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

        private List<StatusModel> GetAllStatus()
        {
            try
            {
                List<StatusModel> lstStatusModel = new List<StatusModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select * from vwStatusList", con);//where Status=1
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
                            StatusModel objStatusModel = new StatusModel();
                            objStatusModel.StID = Convert.IsDBNull(ds.Tables[0].Rows[i]["StID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["StID"]);
                            objStatusModel.StDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            lstStatusModel.Add(objStatusModel);
                        }
                    }
                    if (lstStatusModel.Count > 0)
                    {
                        return lstStatusModel;
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

        public ActionResult GetForgingList(string ProjectNo)
        {
            try
            {
                var result = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select ForgingID,ForgingNo ForgingInfo from vwProejctlst where ProjectNo='" + ProjectNo + "' order by ForgingNo ", con);//where Status=1
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
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["ForgingInfo"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ForgingInfo"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["ForgingInfo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ForgingInfo"])
                            });
                        }
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return null;
            }
        }

        public List<ForgingModel> GetForging()
        {
            try
            {
                List<ForgingModel> lstForgingModel = new List<ForgingModel>();
                ForgingModel objForgingModel;
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select ForgingID,ForgingNo from vwProejctlst order by ForgingNo", con);//where Status=1
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
                            objForgingModel = new ForgingModel();
                            objForgingModel.FrgID = Convert.IsDBNull(ds.Tables[0].Rows[i]["ForgingNo"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ForgingNo"]);
                            objForgingModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["ForgingNo"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ForgingNo"]);
                            lstForgingModel.Add(objForgingModel);
                        }
                    }
                    return lstForgingModel;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Create(InspReqModel inspReqModel, string Submitted, string Draft)
        {
            try
            {
                List<string> listExternalInspector = null;
                Random randomNumber;
                if (inspReqModel.RqOts == false)
                {
                    var model = new InspReqModel();
                    model.listProject = GetProjectList();
                    ViewBag.selectedForgNo = inspReqModel.FrgNo;
                    ViewBag.stage = new SelectList(GetAllStage().ToList(), "StgID", "StgDesc", inspReqModel.StgID);
                    ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName", inspReqModel.LcID);
                    ViewBag.RecordException = "Please confirm OTS previous stages closed in system.";
                    GetForgingList(inspReqModel.ProjectName);
                    return View(model);
                }
                listExternalInspector = new List<string>();
                listExternalInspector = inspReqModel.hiddenfieldSelectedFrgNo.Split(',').ToList();
                foreach (var item in listExternalInspector)
                {
                    if (item != "")
                    {
                        if (IsOpenAnyRequest(inspReqModel.ProjectName, item, inspReqModel.StgID.ToString()) == false)
                        {
                            TempData["RecordException"] = "Request is already open for " + inspReqModel.ProjectName + "/" + item + " For this Stage " + inspReqModel.StageName + " as ";
                            var model = new InspReqModel();
                            model.listProject = GetProjectList();
                            ViewBag.selectedForgNo = inspReqModel.hiddenfieldSelectedFrgNo.Substring(0, inspReqModel.hiddenfieldSelectedFrgNo.Length - 1);
                            ViewBag.stage = new SelectList(GetAllStage().ToList(), "StgID", "StgDesc", inspReqModel.StgID);
                            ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName", inspReqModel.LcID);
                            GetForgingList(inspReqModel.ProjectName);
                            //return View(model);
                        }
                    }

                }

                string query = "usp_DMLInitiator";
                var OutputID = string.Empty;
                var OutPutRqRevNo = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@RqID", 0);
                            cmd.Parameters.AddWithValue("@RqNo", "");
                            cmd.Parameters.AddWithValue("@RqRevNo", 1);
                            cmd.Parameters.AddWithValue("@ReasonID", 0);
                            cmd.Parameters.AddWithValue("@PrjNo", inspReqModel.ProjectName);
                            if (inspReqModel.hiddenfieldSelectedFrgNo.EndsWith(","))
                            {
                                cmd.Parameters.AddWithValue("@FrgNo", inspReqModel.hiddenfieldSelectedFrgNo.Substring(0, inspReqModel.hiddenfieldSelectedFrgNo.Length - 1));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@FrgNo", inspReqModel.hiddenfieldSelectedFrgNo);
                            }
                            
                            cmd.Parameters.AddWithValue("@StgID", inspReqModel.StgID);
                            cmd.Parameters.AddWithValue("@OfrDttm", inspReqModel.OfrDttm);
                            cmd.Parameters.AddWithValue("@Remark", inspReqModel.Remark);
                            if (!string.IsNullOrEmpty(Submitted))
                            {
                                cmd.Parameters.AddWithValue("@RqStatus", "Submitted");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@RqStatus", "Draft");
                            }
                            cmd.Parameters.AddWithValue("@RqType", inspReqModel.RqType);
                            cmd.Parameters.AddWithValue("@RqLoc", inspReqModel.LcID);
                            cmd.Parameters.AddWithValue("@RqRegBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                            cmd.Parameters.AddWithValue("@RqOts", inspReqModel.RqOts);
                            var returnParameter = cmd.Parameters.Add("@OutputID", SqlDbType.NVarChar, 50);
                            returnParameter.Direction = ParameterDirection.Output;
                            var returnOutPutRqRevNo = cmd.Parameters.Add("@OutPutRqRevNo", SqlDbType.NVarChar, 50);
                            returnOutPutRqRevNo.Direction = ParameterDirection.Output;

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            OutputID = cmd.Parameters["@OutputID"].Value.ToString();
                            OutPutRqRevNo = cmd.Parameters["@OutPutRqRevNo"].Value.ToString();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            var model = new InspReqModel();
                            model.listProject = GetProjectList();
                            ViewBag.stage = new SelectList(GetAllStage().ToList(), "StgID", "StgDesc");
                            ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName");
                            ViewBag.RecordException = ex.Message.ToString();
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            return View(model);
                        }
                    }
                }
                foreach (HttpPostedFileBase file in inspReqModel.UploadedFile)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var InputFileName = Path.GetFileName(file.FileName);
                        randomNumber = new Random();
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
                                    cmd.Parameters.AddWithValue("@RqstID", OutputID);
                                    int RevNo = Convert.ToInt32(OutPutRqRevNo.Substring(OutPutRqRevNo.IndexOf("-")));
                                    cmd.Parameters.AddWithValue("@RqRevNo", RevNo);
                                    cmd.Parameters.AddWithValue("@DocName", InputFileName);
                                    cmd.Parameters.AddWithValue("@DocAddr", ServerSavePath);
                                    cmd.Parameters.AddWithValue("@Replan", "1");
                                    cmd.Parameters.AddWithValue("@PlanID", "0");
                                    cmd.Parameters.AddWithValue("@UpldBy", Session["EmpPsNo"].ToString());
                                    cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                                    cmd.Connection = conection;
                                    conection.Open();
                                    cmd.ExecuteNonQuery();
                                    conection.Close();
                                }
                                catch (Exception ex)
                                {
                                    var model = new InspReqModel();
                                    model.listProject = GetProjectList();
                                    ViewBag.stage = new SelectList(GetAllStage().ToList(), "StgID", "StgDesc");
                                    ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName");
                                    ViewBag.RecordException = ex.Message.ToString();
                                    ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                    return View(model);
                                }

                            }
                        }

                    }
                }
                string body = "Inspection request has been raised for Project :" + inspReqModel.ProjectName + " Forgings: " + inspReqModel.hiddenfieldSelectedFrgNo.Substring(0, inspReqModel.hiddenfieldSelectedFrgNo.Length - 1) + " for stage '" + inspReqModel.StageName + "' offered on Date " + inspReqModel.OfrDttm + "";
                if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "Planner", "Inspection Request Raised for " + OutPutRqRevNo, body, OutputID, OutPutRqRevNo))
                {
                    TempData["RecordException"] = "Email Notification is not working";
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + OutPutRqRevNo;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var model = new InspReqModel();
                model.listProject = GetProjectList();
                ViewBag.stage = new SelectList(GetAllStage().ToList(), "StgID", "StgDesc");
                ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName");
                ViewBag.RecordException = ex.Message.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View(model);
            }
            //return View();
        }


        [HttpGet]
        public ActionResult Details(int? id, string data)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                InspReqModel objInspReqModel = new InspReqModel();
                DocumentModel objDocumentModel = new DocumentModel();
                RequestPlannerDetailsModel objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                AssignInspectorModel objAssignInspectorModel = new AssignInspectorModel();
                objInspReqModel.lstDocument = new List<DocumentModel>();
                objInspReqModel.lstRequest = new List<RequestPlannerDetailsModel>();
                objInspReqModel.lstAssignIns = new List<AssignInspectorModel>();
                TpiMstModel objTpiMstModel = new TpiMstModel();
                objInspReqModel.listTpiMstModel = new List<TpiMstModel>();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneInitiatorRequest";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@RqID", id);
                                if (data == "0" || data == "")
                                {
                                    cmd.Parameters.AddWithValue("@PlanID", 0);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@PlanID", data);
                                }
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        // if (i == 0)
                                        //{
                                        objTpiMstModel = new TpiMstModel();
                                        objDocumentModel = new DocumentModel();
                                        objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                                        objAssignInspectorModel = new AssignInspectorModel();
                                        objInspReqModel.FilePath = Convert.ToString(sdr["DocName"]);
                                        objInspReqModel.FrgNo = Convert.ToString(sdr["FrgNo"]);
                                        objInspReqModel.LcName = Convert.ToString(sdr["LcName"]);
                                        objInspReqModel.OfrDttm = Convert.ToDateTime(sdr["OfrDttm"]);
                                        objInspReqModel.PrjNo = Convert.ToString(sdr["PrjNo"]);
                                        objInspReqModel.Remark = Convert.ToString(sdr["Remark"]);
                                        objInspReqModel.RqDpcd = Convert.ToString(sdr["RqDpcd"]);
                                        objInspReqModel.RqID = Convert.ToInt32(sdr["RqID"]);
                                        objInspReqModel.RqNo = Convert.ToString(sdr["RqNo"]);
                                        objInspReqModel.RqOts = Convert.ToBoolean(sdr["RqOts"]);
                                        objInspReqModel.RqRegDttm = Convert.ToDateTime(sdr["RqRegDttm"]);
                                        objInspReqModel.RqRevNo = Convert.ToInt32(sdr["RqRevNo"]);
                                        objInspReqModel.RqStatus = Convert.ToString(sdr["StDesc"]);
                                        objInspReqModel.RqType = Convert.ToInt32(sdr["RqType"]);
                                        objInspReqModel.StageName = Convert.ToString(sdr["StgDesc"]);
                                        objInspReqModel.RqDpcd = Convert.ToString(sdr["RqDpcd"]);
                                        objInspReqModel.RqRegBy = Convert.ToString(sdr["EmpName"]);
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        objInspReqModel.LcID = Convert.ToString(sdr["RqLoc"]);
                                        objInspReqModel.EmployeeID = Convert.ToString(sdr["EmployeeID"]);


                                        // i++;
                                        // }
                                        if (sdr["DocID"] != null && sdr["DocID"] != DBNull.Value)
                                        {
                                            bool containsItem = objInspReqModel.lstDocument.Any(item => item.DocID == Convert.ToInt32(sdr["DocID"]) && item.UpldBy == Convert.ToString(sdr["DocUploadBy"]));
                                            if (containsItem == false)
                                            {
                                                objDocumentModel.DocID = Convert.ToInt32(sdr["DocID"]);
                                                objDocumentModel.DocName = Convert.ToString(sdr["DocName"]);
                                                objDocumentModel.DocAddr = Convert.ToString(sdr["DocAddr"]);
                                                objDocumentModel.UpldBy = Convert.ToString(sdr["DocUploadBy"]);
                                                objDocumentModel.UpldDttm = Convert.ToDateTime(sdr["DocUploadDate"]);
                                                objInspReqModel.lstDocument.Add(objDocumentModel);
                                            }

                                        }
                                        bool hasMyColumn = (sdr.GetSchemaTable().Select("ColumnName = 'PlnEnd'").Count() == 1);
                                        if (hasMyColumn == true)
                                        {
                                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                            objRequestPlannerDetailsModel.PlnBy = Convert.ToString(sdr["PlannerName"]);
                                            objRequestPlannerDetailsModel.PlnDpcd = Convert.ToString(sdr["PlnDpcd"]);
                                            objRequestPlannerDetailsModel.PlnRemark = Convert.ToString(sdr["PlnRemark"]);
                                            objRequestPlannerDetailsModel.PlnDttm = Convert.ToDateTime(sdr["PlnDttm"]);
                                            if (sdr["AgName"] != null && sdr["AgName"] != DBNull.Value)
                                            {
                                                objRequestPlannerDetailsModel.AgencyName = Convert.ToString(sdr["AgName"]);
                                            }
                                            if (sdr["ActulStart"] != null && sdr["ActulStart"] != DBNull.Value)
                                            {
                                                objRequestPlannerDetailsModel.ActulStart = Convert.ToDateTime(sdr["ActulStart"]);
                                                objRequestPlannerDetailsModel.ActulEnd = Convert.ToDateTime(sdr["ActulEnd"]);
                                                objRequestPlannerDetailsModel.InsBy = Convert.ToString(sdr["InspectorBy"]);
                                                objRequestPlannerDetailsModel.InsDttm = Convert.ToDateTime(sdr["InspectorDttm"]);
                                                objRequestPlannerDetailsModel.InspDpcd = Convert.ToString(sdr["InspectorDpcd"]);
                                                objRequestPlannerDetailsModel.InspRemark = Convert.ToString(sdr["InspRemark"]);
                                                objRequestPlannerDetailsModel.ReasonName = Convert.ToString(sdr["RsnDesc"]);

                                            }
                                            objInspReqModel.lstRequest.Add(objRequestPlannerDetailsModel);
                                            bool containsItem = objInspReqModel.lstAssignIns.Any(item => item.InsPsNo == Convert.ToInt32(sdr["InsPsNo"]));
                                            if (containsItem == false)
                                            {
                                                if (sdr["InsPsNo"] != null && sdr["InsPsNo"] != DBNull.Value)
                                                {
                                                    objAssignInspectorModel.InsPsNo = Convert.ToInt32(sdr["InsPsNo"]);
                                                    objAssignInspectorModel.InsPsName = Convert.ToString(sdr["AssignInsUsername"]);
                                                    objAssignInspectorModel.InsDttm = Convert.ToDateTime(sdr["InsDttm"]);
                                                    objAssignInspectorModel.InspDpcd = Convert.ToString(sdr["InspDpcd"]);
                                                    objInspReqModel.lstAssignIns.Add(objAssignInspectorModel);
                                                }

                                            }
                                            bool hasTpiColumn = (sdr.GetSchemaTable().Select("ColumnName = 'TpName'").Count() == 1);
                                            if (hasTpiColumn == true)
                                            {
                                                if (sdr["TpName"] != null && sdr["TpName"] != DBNull.Value)
                                                {
                                                    containsItem = objInspReqModel.listTpiMstModel.Any(item => item.TpID == Convert.ToInt32(sdr["TpID"]));
                                                    if (containsItem == false)
                                                    {
                                                        objTpiMstModel.TpID = Convert.ToInt32(sdr["TpID"]);
                                                        objTpiMstModel.TpName = Convert.ToString(sdr["TpName"]);
                                                        objTpiMstModel.TpMail = Convert.ToString(sdr["TpMail"]);
                                                        objTpiMstModel.TpMob = Convert.ToString(sdr["TpMob"]);
                                                        objInspReqModel.listTpiMstModel.Add(objTpiMstModel);
                                                    }
                                                }
                                            }
                                        }


                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                TempData["RecordException"] = ex.Message.ToString();
                                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                return RedirectToAction("Index");
                            }

                        }
                    }
                }
                if (objInspReqModel.RqNo == null)
                {
                    ViewBag.RecordNotExist = "Sorry...Record dosen't exists ! ";
                }
                return View(objInspReqModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View();
            }
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                InspReqModel objInspReqModel = new InspReqModel();
                DocumentModel objDocumentModel = new DocumentModel();
                objInspReqModel.lstDocument = new List<DocumentModel>();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneInitiatorRequest";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@RqID", id);
                                cmd.Parameters.AddWithValue("@PlanID", DBNull.Value);
                                cmd.Connection = con;
                                con.Open();
                                int i = 0;
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        // if (i == 0)
                                        //{
                                        objDocumentModel = new DocumentModel();
                                        objInspReqModel.FilePath = Convert.ToString(sdr["DocName"]);
                                        objInspReqModel.FrgNo = Convert.ToString(sdr["FrgNo"]);
                                        objInspReqModel.LcName = Convert.ToString(sdr["LcName"]);
                                        objInspReqModel.OfrDttm = Convert.ToDateTime(sdr["OfrDttm"]);
                                        objInspReqModel.PrjNo = Convert.ToString(sdr["PrjNo"]);
                                        objInspReqModel.Remark = Convert.ToString(sdr["Remark"]);
                                        objInspReqModel.RqDpcd = Convert.ToString(sdr["RqDpcd"]);
                                        objInspReqModel.RqID = Convert.ToInt32(sdr["RqID"]);
                                        objInspReqModel.RqNo = Convert.ToString(sdr["RqNo"]);
                                        objInspReqModel.RqOts = Convert.ToBoolean(sdr["RqOts"]);
                                        objInspReqModel.RqRegDttm = Convert.ToDateTime(sdr["RqRegDttm"]);
                                        objInspReqModel.RqRevNo = Convert.ToInt32(sdr["RqRevNo"]);
                                        objInspReqModel.RqStatus = Convert.ToString(sdr["StDesc"]);
                                        objInspReqModel.RqType = Convert.ToInt32(sdr["RqType"]);
                                        objInspReqModel.StageName = Convert.ToString(sdr["StgDesc"]);
                                        objInspReqModel.RqDpcd = Convert.ToString(sdr["EmpDpcd"]);
                                        objInspReqModel.RqRegBy = Convert.ToString(sdr["EmpName"]);
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        objInspReqModel.LcID = Convert.ToString(sdr["RqLoc"]);
                                        objInspReqModel.EmployeeID = Convert.ToString(sdr["EmployeeID"]);
                                        // i++;
                                        // }
                                        if (sdr["DocID"] != null && sdr["DocID"] != DBNull.Value)
                                        {
                                            objDocumentModel.DocID = Convert.ToInt32(sdr["DocID"]);
                                            objDocumentModel.DocName = Convert.ToString(sdr["DocName"]);
                                            objDocumentModel.DocAddr = Convert.ToString(sdr["DocAddr"]);
                                            objDocumentModel.UpldBy = Convert.ToString(sdr["DocUploadBy"]);
                                            objDocumentModel.UpldDttm = Convert.ToDateTime(sdr["DocUploadDate"]);
                                            objInspReqModel.lstDocument.Add(objDocumentModel);
                                        }

                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                TempData["RecordException"] = ex.Message.ToString();
                                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                return RedirectToAction("Index");
                            }

                        }
                    }
                }
                if (objInspReqModel.RqNo == null)
                {
                    ViewBag.RecordNotExist = "Sorry ...Record dosen't Exists !";
                }
                ViewBag.Reason = new SelectList(GetReasonMasterModels().ToList(), "RsnID", "RsnDesc");
                ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName", objInspReqModel.LcID);
                return View(objInspReqModel);
            }
            catch (Exception ex)
            {
                ViewBag.location = new SelectList(GetAllLocation().ToList(), "LcID", "LcName");
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult DeleteDoc(int? id, string data)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "DELETE from tblDocList where DocID='" + id + "'";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            ViewBag.RecordException = ex.Message.ToString();
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            return View();
                        }
                    }
                }
                return RedirectToAction("Edit", new { id = data });
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.Message.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View();
            }

        }

        [HttpPost]
        public ActionResult Edit(InspReqModel inspReqModel, string Submitted, string Draft)
        {
            try
            {
                string query = "usp_DMLInitiator";
                var OutputID = string.Empty;
                var OutPutRqRevNo = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@RqID", inspReqModel.RqID);
                            cmd.Parameters.AddWithValue("@ReasonID", inspReqModel.ResnID);
                            cmd.Parameters.AddWithValue("@RqNo", "");
                            cmd.Parameters.AddWithValue("@RqRevNo", 0);
                            cmd.Parameters.AddWithValue("@PrjNo", "");
                            cmd.Parameters.AddWithValue("@FrgNo", "");
                            cmd.Parameters.AddWithValue("@StgID", 0);
                            cmd.Parameters.AddWithValue("@OfrDttm", inspReqModel.OfrDttm);
                            cmd.Parameters.AddWithValue("@Remark", inspReqModel.Remark);
                            if (!string.IsNullOrEmpty(Submitted))
                            {
                                cmd.Parameters.AddWithValue("@RqStatus", "Submitted");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@RqStatus", "Draft");
                            }
                            cmd.Parameters.AddWithValue("@RqType", 0);
                            cmd.Parameters.AddWithValue("@RqLoc", inspReqModel.LcID);
                            cmd.Parameters.AddWithValue("@RqRegBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                            cmd.Parameters.AddWithValue("@RqOts", 0);
                            var returnParameter = cmd.Parameters.Add("@OutputID", SqlDbType.NVarChar, 50);
                            returnParameter.Direction = ParameterDirection.Output;
                            var returnOutPutRqRevNo = cmd.Parameters.Add("@OutPutRqRevNo", SqlDbType.NVarChar, 50);
                            returnOutPutRqRevNo.Direction = ParameterDirection.Output;



                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            OutputID = cmd.Parameters["@OutputID"].Value.ToString();
                            OutPutRqRevNo = cmd.Parameters["@OutPutRqRevNo"].Value.ToString();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            TempData["RecordException"] = ex.Message.ToString();
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            return RedirectToAction("Edit", new { id = inspReqModel.RqID });
                        }
                    }
                    foreach (HttpPostedFileBase file in inspReqModel.UploadedFile)
                    {
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            var InputFileName = Path.GetFileName(file.FileName);
                            InputFileName = Guid.NewGuid() + "-" + InputFileName;
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
                                        cmd.Parameters.AddWithValue("@RqstID", @OutputID);
                                        cmd.Parameters.AddWithValue("@RqRevNo", OutPutRqRevNo.Substring(OutPutRqRevNo.IndexOf("-") + 1));
                                        cmd.Parameters.AddWithValue("@DocName", InputFileName);
                                        cmd.Parameters.AddWithValue("@DocAddr", ServerSavePath);
                                        cmd.Parameters.AddWithValue("@Replan", "1");
                                        cmd.Parameters.AddWithValue("@PlanID", "0");
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
                                        return RedirectToAction("Edit", new { id = inspReqModel.RqID });
                                    }

                                }
                            }

                        }
                    }
                }
                //string body = "Request has been raised for '" + inspReqModel.ProjectName + "' of '" + inspReqModel.FrgNo + "' for '" + inspReqModel.StgID + "' of offered on this '" + inspReqModel.OfrDttm + "'";
                //if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "Planner", "Request Update " + OutPutRqRevNo, body, inspReqModel.RqID.ToString(), OutPutRqRevNo))
                //{
                //    TempData["RecordException"] = "Email Notification is not working";
                //}
                TempData["TransactionStatus"] = "Request Update Successfully :" + OutPutRqRevNo;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.Message.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Edit", new { id = inspReqModel.RqID });
            }
            //return View();
        }

        [HttpGet]
        public ActionResult Cancel(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                InspReqModel objInspReqModel = new InspReqModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneInitiatorRequest";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@RqID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objInspReqModel.FilePath = Convert.ToString(sdr["DocName"]);
                                        objInspReqModel.FrgNo = Convert.ToString(sdr["FrgNo"]);
                                        objInspReqModel.LcName = Convert.ToString(sdr["LcName"]);
                                        objInspReqModel.OfrDttm = Convert.ToDateTime(sdr["OfrDttm"]);
                                        objInspReqModel.PrjNo = Convert.ToString(sdr["PrjNo"]);
                                        objInspReqModel.Remark = Convert.ToString(sdr["Remark"]);
                                        objInspReqModel.RqDpcd = Convert.ToString(sdr["RqDpcd"]);
                                        objInspReqModel.RqID = Convert.ToInt32(sdr["RqID"]);
                                        objInspReqModel.RqNo = Convert.ToString(sdr["RqNo"]);
                                        objInspReqModel.RqOts = Convert.ToBoolean(sdr["RqOts"]);
                                        objInspReqModel.RqRegDttm = Convert.ToDateTime(sdr["RqRegDttm"]);
                                        objInspReqModel.RqRevNo = Convert.ToInt32(sdr["RqRevNo"]);
                                        objInspReqModel.RqStatus = Convert.ToString(sdr["StDesc"]);
                                        objInspReqModel.RqType = Convert.ToInt32(sdr["RqType"]);
                                        objInspReqModel.StageName = Convert.ToString(sdr["StgDesc"]);
                                        objInspReqModel.RqDpcd = Convert.ToString(sdr["EmpDpcd"]);
                                        objInspReqModel.RqRegBy = Convert.ToString(sdr["EmpName"]);
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        objInspReqModel.EmployeeID = Convert.ToString(sdr["EmployeeID"]);
                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                TempData["RecordException"] = ex.Message.ToString();
                                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                return RedirectToAction("Index");
                            }

                        }
                    }
                }
                if (objInspReqModel.RqNo == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objInspReqModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View();
            }
        }

        [HttpPost]
        public ActionResult Cancel(InspReqModel inspReqModel)
        {
            try
            {
                string query = "usp_DMCCancelInitiatorRequest";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ReqId", inspReqModel.RqID);
                        cmd.Parameters.AddWithValue("@Remark", inspReqModel.Remark);
                        cmd.Parameters.AddWithValue("@LcRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@LcRegByDept", Session["EmployeeDeparment"].ToString());
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                string body = "Inspection request has been cancelled for Project : " + inspReqModel.ProjectName + " Forging :" + inspReqModel.FrgNo + " for stage '" + inspReqModel.StageName + "' offered on Date " + inspReqModel.OfrDttm + "";
                //if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "Planner", "Request Cancel " + inspReqModel.RqNo + "-" + inspReqModel.RqRevNo.ToString(), body, inspReqModel.RqID.ToString(), inspReqModel.RqNo + "-" + inspReqModel.RqRevNo.ToString()))
                //{
                //    TempData["RecordException"] = "Email Notification is not working";
                //}
                TempData["TransactionStatus"] = "Request Cancel Successfully :" + inspReqModel.RqNo + "-" + inspReqModel.RqRevNo.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordNotExist = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return View();
            }
        }

        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            try
            {
                var result = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    //SqlCommand com = new SqlCommand("Select distinct TOP 10 ProjectNo ProjectInfo from vwProejctlst where ProjectNo like '%" + prefix + "%'", con);//where Status=1
                    SqlCommand com = new SqlCommand("Select PrjID ,PrjNo ProjectInfo from tblProject where PrjNo like '%" + prefix + "%' and PrjStatus='True'", con);//where Status=1
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
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectInfo"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectInfo"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectInfo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectInfo"])
                            });
                        }
                    }
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return Json(ex.ToString());
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
                            objReasonMstModel.RsnID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RsnID"]);
                            objReasonMstModel.RsnDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RsnDesc"]);
                            lstReasonMstModel.Add(objReasonMstModel);
                        }
                    }
                    if (lstReasonMstModel.Count > 0)
                    {
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
    }
}