using ICMS.App_Start;
using ICMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICMS.Controllers
{
    [IsAuthorized]
    public class InspectorController : Controller
    {
        // GET: Inspector
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
                    SqlCommand com = new SqlCommand("usp_getAllInspectorStageRequest", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    com.Parameters.AddWithValue("@RqID", Session["EmpPsNo"].ToString());
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
                            bool containsItem = objRequestPlannerDetailsModel.objInspReqModel.Any(item => item.RqNo == Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]));
                            objInspReqModel.IsUpdateExternalRequest = "0";
                            //if (containsItem == true)
                            //{
                            //    if (Convert.ToString(ds.Tables[0].Rows[i]["RqType"]) == "External")
                            //    {
                            //        objInspReqModel.IsUpdateExternalRequest = "1";
                            //    }
                            //}
                            objInspReqModel.IsUpdateExternalRequest = Convert.ToString(ds.Tables[0].Rows[i]["IsClose"]);
                            objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqID"]);
                            objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                            objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                            objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                            objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.RqTypeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqType"]);
                            objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["OfrDttm"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["InsPsNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["InsPsNo"]);
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RequestDept"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RequestDept"]);
                            objInspReqModel.PlanEndDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
                            objInspReqModel.EmployeeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            objInspReqModel.PlanStartDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                            objInspReqModel.PlanID = Convert.IsDBNull(ds.Tables[0].Rows[i]["PlnID"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PlnID"]);
                            objInspReqModel.EmployeeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            objRequestPlannerDetailsModel.objInspReqModel.Add(objInspReqModel);
                            lstRequestPlannerDetailsModel.Add(objRequestPlannerDetailsModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objRequestPlannerDetailsViewModel.ListRequestPlannerDetails = lstRequestPlannerDetailsModel;
                        objRequestPlannerDetailsViewModel.pager = pager;
                    }
                }
                ViewBag.IsSearch = "false";
                ViewBag.Status = new SelectList(GetAllStatus().ToList(), "StID", "StDesc");
                return View(objRequestPlannerDetailsViewModel);
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return View();
            }

        }

        [HttpGet]
        public ActionResult Search(string StID, string searchText, string searchInspectorWiseReq, int page = 1)
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
                    ViewBag.IsSearch = "false";
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_SearchInspectorRequest", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    com.Parameters.AddWithValue("@StatusID", StID);
                    //com.Parameters.AddWithValue("@searchInspectorWiseReq", searchInspectorWiseReq);
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
                            bool containsItem = objRequestPlannerDetailsModel.objInspReqModel.Any(item => item.RqNo == Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]));
                            objInspReqModel.IsUpdateExternalRequest = "0";
                            //if (containsItem == true)
                            //{
                            //    if (Convert.ToString(ds.Tables[0].Rows[i]["RqType"]) == "External")
                            //    {
                            //        objInspReqModel.IsUpdateExternalRequest = "1";
                            //    }
                            //}
                            objInspReqModel.IsUpdateExternalRequest = Convert.ToString(ds.Tables[0].Rows[i]["IsClose"]);
                            objInspReqModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqID"]);
                            objInspReqModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                            objInspReqModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqRevNo"]);
                            objInspReqModel.PrjNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                            objInspReqModel.FrgNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.RqTypeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqType"]);
                            objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["OfrDttm"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                           // objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["InsPsNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["InsPsNo"]);
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RequestDept"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RequestDept"]);
                            objInspReqModel.PlanEndDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
                            objInspReqModel.PlanStartDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                            objInspReqModel.PlanID = Convert.IsDBNull(ds.Tables[0].Rows[i]["PlnID"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PlnID"]);
                            objInspReqModel.EmployeeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            //objInspReqModel.EmployeeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            objRequestPlannerDetailsModel.objInspReqModel.Add(objInspReqModel);
                            lstRequestPlannerDetailsModel.Add(objRequestPlannerDetailsModel);

                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objRequestPlannerDetailsViewModel.ListRequestPlannerDetails = lstRequestPlannerDetailsModel;
                        objRequestPlannerDetailsViewModel.pager = pager;
                    }
                }
                ViewBag.Status = new SelectList(GetAllStatus().ToList(), "StID", "StDesc");
                ViewBag.SearchData = searchText;

                if (objRequestPlannerDetailsViewModel.ListRequestPlannerDetails.Count == 0)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View("Index", objRequestPlannerDetailsViewModel);
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return View("Index");
            }

        }
        private List<StatusModel> GetAllStatus()
        {
            List<StatusModel> lstStatusModel = new List<StatusModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select StID,StDesc from vwStatusList", con);//where Status=1
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
                    else
                    {
                        StatusModel objStatusModel = new StatusModel();
                        objStatusModel.StID = 0;
                        objStatusModel.StDesc = "";
                        lstStatusModel.Add(objStatusModel);
                        return lstStatusModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                StatusModel objStatusModel = new StatusModel();
                objStatusModel.StID = 0;
                objStatusModel.StDesc = "";
                lstStatusModel.Add(objStatusModel);
                return lstStatusModel;
            }

        }
        [HttpGet]
        public ActionResult Create(string id, string data)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                Boolean isExistingRecords = false;
                RequestPlannerDetailsModel objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                DocumentModel objDocumentModel = new DocumentModel();
                InspReqModel objInspReqModel = new InspReqModel();
                AssignInspectorModel objAssignInspectorModel = new AssignInspectorModel();
                objRequestPlannerDetailsModel.lstDocument = new List<DocumentModel>();
                objRequestPlannerDetailsModel.objInspReqModel = new List<InspReqModel>();
                objRequestPlannerDetailsModel.listAssignInspector = new List<AssignInspectorModel>();
                List<string> EmpList = new List<string>();
                string AgencyID = "0";
                string ReasonID = "0";
                string RequestType = string.Empty;
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneInspectorDetails";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Plan", id);
                                cmd.Parameters.AddWithValue("@RqID", data);
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
                                        objInspReqModel = new InspReqModel();
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
                                        //objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        objInspReqModel.StageName = Convert.ToString(sdr["StgDesc"]);
                                        objInspReqModel.RqDpcd = Convert.ToString(sdr["EmpDpcd"]);
                                        objInspReqModel.RqRegBy = Convert.ToString(sdr["InspectorName"]);
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        RequestType = Convert.ToString(sdr["RqTypeName"]);
                                        objInspReqModel.LcID = Convert.ToString(sdr["RqLoc"]);
                                        // i++;
                                        // }
                                        if (sdr["DocID"] != null && sdr["DocID"] != DBNull.Value)
                                        {
                                            bool containsItem = objRequestPlannerDetailsModel.lstDocument.Any(item => item.DocID == Convert.ToInt32(sdr["DocID"]));
                                            if (containsItem == false)
                                            {
                                                objDocumentModel.DocID = Convert.ToInt32(sdr["DocID"]);
                                                objDocumentModel.DocName = Convert.ToString(sdr["DocName"]);
                                                objDocumentModel.DocAddr = Convert.ToString(sdr["DocAddr"]);
                                                objDocumentModel.UpldBy = Convert.ToString(sdr["DocUploadBy"]);
                                                objDocumentModel.UpldDttm = Convert.ToDateTime(sdr["DocUploadDate"]);
                                                objDocumentModel.DocDadp = Convert.ToString(sdr["DocUploadDept"]);
                                                objRequestPlannerDetailsModel.lstDocument.Add(objDocumentModel);
                                            }
                                        }
                                        bool hasMyColumn = (sdr.GetSchemaTable().Select("ColumnName = 'PlnID'").Count() == 1);
                                        if (hasMyColumn == true)
                                        {
                                            objRequestPlannerDetailsModel.PlnID = Convert.ToInt32(sdr["PlnID"]);
                                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                            objRequestPlannerDetailsModel.PlnRemark = Convert.ToString(sdr["PlnRemark"]);
                                            objRequestPlannerDetailsModel.PlnBy = Convert.ToString(sdr["PlannerName"]);
                                            objRequestPlannerDetailsModel.PlnDttm = Convert.ToDateTime(sdr["PlnDttm"]);
                                            objRequestPlannerDetailsModel.PlnDpcd = Convert.ToString(sdr["PlnDpcd"]);

                                        }
                                        else
                                        {
                                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(DateTime.Today);
                                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(DateTime.Today);
                                            objRequestPlannerDetailsModel.PlnID = 0;
                                        }
                                        if (hasMyColumn == true)
                                        {
                                            if (sdr["InsPsNo"] != null && sdr["InsPsNo"] != DBNull.Value)
                                            {
                                                bool containsItem = objRequestPlannerDetailsModel.listAssignInspector.Any(item => item.InsID == Convert.ToInt32(sdr["InsID"]));
                                                if (containsItem == false)
                                                {
                                                    objAssignInspectorModel.InsID = Convert.ToInt32(sdr["InsID"]);
                                                    objAssignInspectorModel.InsPsNo = Convert.ToInt32(sdr["InsPsNo"]);
                                                    objAssignInspectorModel.PlnID = Convert.ToInt32(sdr["PlnID"]);
                                                    objAssignInspectorModel.InsDttm = Convert.ToDateTime(sdr["AssignInsDateTime"]);
                                                    objAssignInspectorModel.InsBy = Convert.ToString(sdr["AssignInsUsername"]);

                                                    objRequestPlannerDetailsModel.listAssignInspector.Add(objAssignInspectorModel);
                                                }
                                            }
                                        }
                                        if (sdr["ActulStart"] != null && sdr["ActulStart"] != DBNull.Value)
                                        {
                                            objRequestPlannerDetailsModel.ActulStart = Convert.ToDateTime(sdr["ActulStart"]);
                                            objRequestPlannerDetailsModel.ActulEnd = Convert.ToDateTime(sdr["ActulEnd"]);
                                            objRequestPlannerDetailsModel.InspRemark = Convert.ToString(sdr["InspRemark"]);
                                            objRequestPlannerDetailsModel.InsBy = Convert.ToString(sdr["InsBy"]);
                                            objRequestPlannerDetailsModel.ResnID = Convert.ToInt32(sdr["ResnID"]);
                                            ReasonID = Convert.ToString(sdr["ResnID"]);
                                            isExistingRecords = true;
                                            objRequestPlannerDetailsModel.ReasonName = Convert.ToString(sdr["RsnDesc"]);
                                            if (sdr["AgcyID"] != null && sdr["AgcyID"] != DBNull.Value)
                                            {
                                                AgencyID = Convert.ToString(sdr["AgcyID"]);
                                                objRequestPlannerDetailsModel.AgencyName = Convert.ToString(sdr["AgName"]);
                                                objRequestPlannerDetailsModel.AgcyID = Convert.ToInt32(sdr["AgcyID"]);
                                            }
                                        }
                                        else
                                        {
                                            objRequestPlannerDetailsModel.ActulStart = Convert.ToDateTime(sdr["PlnStart"]);
                                            objRequestPlannerDetailsModel.ActulEnd = Convert.ToDateTime(sdr["PlnStart"]);
                                            if (sdr["AgcyID"] != null && sdr["AgcyID"] != DBNull.Value)
                                            {
                                                AgencyID = Convert.ToString(sdr["AgcyID"]);
                                                objRequestPlannerDetailsModel.AgencyName = Convert.ToString(sdr["AgName"]);
                                                objRequestPlannerDetailsModel.AgcyID = Convert.ToInt32(sdr["AgcyID"]);
                                            }
                                        }
                                        if (sdr["InteranalPlanID"] != null && sdr["InteranalPlanID"] != DBNull.Value)
                                        {
                                            objRequestPlannerDetailsModel.ID = Convert.ToInt32(sdr["InteranalPlanID"]);
                                        }
                                        objRequestPlannerDetailsModel.objInspReqModel.Add(objInspReqModel);
                                    }
                                }
                                con.Close();
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
                if (objRequestPlannerDetailsModel.PlnID == 0)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View();
                }
                if (isExistingRecords == true)
                {

                    return View("Details", objRequestPlannerDetailsModel);
                }
                if (objInspReqModel.RqTypeName.ToString() == "External" && objRequestPlannerDetailsModel.ID.ToString() == "0" && objInspReqModel.RqStatus!="Return")
                {
                    return View("Details", objRequestPlannerDetailsModel);
                }
                if (RequestType == "External")
                {
                    ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName", AgencyID);
                    ViewBag.AgencyEmployee = new SelectList(GetAgencyEmployee(AgencyID).ToList(), "TpID", "TpName");
                }
                //ViewBag.Reason = new SelectList(GetReasonMasterModels().ToList(), "RsnID", "RsnDesc", ReasonID);
                ViewBag.ReasonWithFileUpload = new SelectList(GetReasonMasterModelsWithFileUpload().ToList(), "RsnID", "IsReqiredFileUpload", ReasonID);

                return View(objRequestPlannerDetailsModel);
            }
            catch (Exception ex)
            {
                ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
                ViewBag.Reason = new SelectList(GetReasonMasterModels().ToList(), "RsnID", "RsnDesc");
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }
        private List<AgencyMstModel> GetAgencyMstModels()
        {
            List<AgencyMstModel> lstAgencyMstModel = new List<AgencyMstModel>();
            try
            {
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
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            AgencyMstModel objAgencyMstModel = new AgencyMstModel();
                            objAgencyMstModel.AgID = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["AgID"]);
                            objAgencyMstModel.AgName = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgName"]);
                            lstAgencyMstModel.Add(objAgencyMstModel);
                        }
                    }
                    if (lstAgencyMstModel.Count > 0)
                    {
                        return lstAgencyMstModel;
                    }
                    else
                    {
                        AgencyMstModel objAgencyMstModel = new AgencyMstModel();
                        objAgencyMstModel.AgID = 0;
                        objAgencyMstModel.AgName = "";
                        lstAgencyMstModel.Add(objAgencyMstModel);
                        return lstAgencyMstModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                AgencyMstModel objAgencyMstModel = new AgencyMstModel();
                objAgencyMstModel.AgID = 0;
                objAgencyMstModel.AgName = "";
                lstAgencyMstModel.Add(objAgencyMstModel);
                return lstAgencyMstModel;
            }

        }

        private List<TpiMstModel> GetAgencyEmployee(string AgID)
        {
            List<TpiMstModel> lstTpiMstModel = new List<TpiMstModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select TpID,TpName from vwAgencyEmployeelist where AgID='" + AgID + "'", con);//where Status=1
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
                            TpiMstModel objTpiMstModel = new TpiMstModel();
                            objTpiMstModel.TpID = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["TpID"]);
                            objTpiMstModel.TpName = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["TpName"]);
                            lstTpiMstModel.Add(objTpiMstModel);
                        }
                    }
                    if (lstTpiMstModel.Count > 0)
                    {
                        return lstTpiMstModel;
                    }
                    else
                    {
                        TpiMstModel objTpiMstModel = new TpiMstModel();
                        objTpiMstModel.TpID = 0;
                        objTpiMstModel.TpName = "";
                        lstTpiMstModel.Add(objTpiMstModel);
                        return lstTpiMstModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                TpiMstModel objTpiMstModel = new TpiMstModel();
                objTpiMstModel.TpID = 0;
                objTpiMstModel.TpName = "";
                lstTpiMstModel.Add(objTpiMstModel);
                return lstTpiMstModel;
            }

        }

        private List<ReasonMstModel> GetReasonMasterModels()
        {
            List<ReasonMstModel> lstReasonMstModel = new List<ReasonMstModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select RsnID,RsnDesc,IsReqiredFileUpload from vwReasonList", con);//where Status=1
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
                            objReasonMstModel.RsnID = Convert.ToInt32(ds.Tables[0].Rows[i]["RsnID"]);
                            objReasonMstModel.RsnDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RsnDesc"]);
                            objReasonMstModel.IsReqiredFileUpload = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsReqiredFileUpload"]) ? false : Convert.ToBoolean(ds.Tables[0].Rows[i]["IsReqiredFileUpload"]);
                            lstReasonMstModel.Add(objReasonMstModel);
                        }
                    }
                    if (lstReasonMstModel.Count > 0)
                    {
                        return lstReasonMstModel;
                    }
                    else
                    {
                        ReasonMstModel objReasonMstModel = new ReasonMstModel();
                        objReasonMstModel.RsnID = 0;
                        objReasonMstModel.RsnDesc = "";
                        objReasonMstModel.IsReqiredFileUpload = false;
                        lstReasonMstModel.Add(objReasonMstModel);
                        return lstReasonMstModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                ReasonMstModel objReasonMstModel = new ReasonMstModel();
                objReasonMstModel.RsnID = 0;
                objReasonMstModel.RsnDesc = "";
                objReasonMstModel.IsReqiredFileUpload = false;
                lstReasonMstModel.Add(objReasonMstModel);
                return lstReasonMstModel;
            }

        }

        public ActionResult GetReasonList(string id)
        {
            try
            {
                var result = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select RsnID,RsnDesc,IsReqiredFileUpload from vwReasonList where IsReturn='" + id + "'", con);//where Status=1
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
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnID"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["RsnID"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RsnDesc"])
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

        private List<ReasonMstModel> GetReasonMasterModelsWithFileUpload()
        {
            List<ReasonMstModel> lstReasonMstModel = new List<ReasonMstModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select RsnID,RsnDesc,IsReqiredFileUpload from vwReasonList", con);//where Status=1
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
                            objReasonMstModel.RsnID = Convert.ToInt32(ds.Tables[0].Rows[i]["RsnID"]);
                            objReasonMstModel.IsReqiredFileUpload = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsReqiredFileUpload"]) ? false : Convert.ToBoolean(ds.Tables[0].Rows[i]["IsReqiredFileUpload"]);
                            lstReasonMstModel.Add(objReasonMstModel);
                        }
                    }
                    if (lstReasonMstModel.Count > 0)
                    {
                        return lstReasonMstModel;
                    }
                    else
                    {
                        ReasonMstModel objReasonMstModel = new ReasonMstModel();
                        objReasonMstModel.RsnID = 0;
                        objReasonMstModel.IsReqiredFileUpload = false;
                        lstReasonMstModel.Add(objReasonMstModel);
                        return lstReasonMstModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                ReasonMstModel objReasonMstModel = new ReasonMstModel();
                objReasonMstModel.RsnID = 0;
                objReasonMstModel.RsnDesc = "";
                objReasonMstModel.IsReqiredFileUpload = false;
                lstReasonMstModel.Add(objReasonMstModel);
                return lstReasonMstModel;
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
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            TempData["RecordException"] = ex.Message.ToString();
                            return RedirectToAction("Create", new { id = data });
                        }
                    }
                }
                string insertQuery = "insert into tblLogs(LgRqstID,LgDttm,LgBy,LgDpcd,LgTxt) values('" + data.ToString() + "','" + DateTime.Today + "','" + Session["EmpPsNo"].ToString() + "','" + Session["EmployeeDeparment"].ToString() + "','Document is deleted and ID is : " + id.ToString() + "')";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(insertQuery))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                            ViewBag.RecordException = ex.Message.ToString();
                            return View();
                        }
                    }
                }
                return RedirectToAction("Create", new { id = data });
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.Message.ToString();
                return RedirectToAction("Create", new { id = data });
            }

        }

        [HttpPost]
        public ActionResult Create(RequestPlannerDetailsModel requestPlannerDetailsModel)
        {
            Random randomNumber = new Random();
            try
            {
                string query = "usp_DMLInspectorRequest";
                var OutputID = string.Empty;
                string ReasonName = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string ExternalInspector = string.Empty;
                List<string> listExternalInspector = null;
                if (requestPlannerDetailsModel.ExternaltempValue != null)
                {
                    if (requestPlannerDetailsModel.ExternaltempValue.Contains(","))
                    {
                        ExternalInspector = requestPlannerDetailsModel.ExternaltempValue.Substring(0, requestPlannerDetailsModel.ExternaltempValue.Length - 1);
                        listExternalInspector = new List<string>();
                        listExternalInspector = ExternalInspector.Split(',').ToList();
                    }
                }



                using (SqlConnection con = new SqlConnection(constr))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PlnID", requestPlannerDetailsModel.PlnID);
                            cmd.Parameters.AddWithValue("@RqID", requestPlannerDetailsModel.objInspReqModel[0].RqID);
                            if (requestPlannerDetailsModel.objInspReqModel[0].RqTypeName == "External")
                            {
                                cmd.Parameters.AddWithValue("@RqstType", "2");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@RqstType", "1");
                            }
                            if (requestPlannerDetailsModel.IsReturn)
                            {
                                cmd.Parameters.AddWithValue("@IsClosed", 1);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@IsClosed", 0);
                            }
                            cmd.Parameters.AddWithValue("@ActulEnd", requestPlannerDetailsModel.ActulEnd);
                            cmd.Parameters.AddWithValue("@ActulStart", requestPlannerDetailsModel.ActulStart);
                            cmd.Parameters.AddWithValue("@AgcyID", requestPlannerDetailsModel.AgcyID);
                            cmd.Parameters.AddWithValue("@ResnID", requestPlannerDetailsModel.ResnID);
                            cmd.Parameters.AddWithValue("@InsBy", Session["EmpPsNo"].ToString());
                            cmd.Parameters.AddWithValue("@InspDpcd", Session["EmployeeDeparment"].ToString());
                            cmd.Parameters.AddWithValue("@InspRemark", requestPlannerDetailsModel.InspRemark);
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
                        ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                        TempData["RecordException"] = ex.Message.ToString();
                        return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID });
                    }

                }
                foreach (HttpPostedFileBase file in requestPlannerDetailsModel.UploadedFile)
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
                                    cmd.Parameters.AddWithValue("@RqstID", requestPlannerDetailsModel.objInspReqModel[0].RqID);
                                    cmd.Parameters.AddWithValue("@RqRevNo", requestPlannerDetailsModel.objInspReqModel[0].RqRevNo);
                                    cmd.Parameters.AddWithValue("@DocName", InputFileName);
                                    cmd.Parameters.AddWithValue("@DocAddr", ServerSavePath);
                                    cmd.Parameters.AddWithValue("@Replan", "1");
                                    cmd.Parameters.AddWithValue("@PlanID", requestPlannerDetailsModel.PlnID);
                                    cmd.Parameters.AddWithValue("@UpldBy", Session["EmpPsNo"].ToString());
                                    cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                                    cmd.Connection = conection;
                                    conection.Open();
                                    cmd.ExecuteNonQuery();
                                    conection.Close();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                    TempData["RecordException"] = ex.Message.ToString();
                                    return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID });
                                }

                            }
                        }

                    }
                }
                if (requestPlannerDetailsModel.EmpPsNo != null)
                {
                    foreach (var item in requestPlannerDetailsModel.EmpPsNo)
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
                                    ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                    TempData["RecordException"] = ex.Message.ToString();
                                    return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID });
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
                            using (SqlCommand cmd = new SqlCommand("usp_DMLAssignAgncyInspct"))
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
                                    ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                    TempData["RecordException"] = ex.Message.ToString();
                                    return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID });
                                }

                            }
                        }
                    }
                }
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select RsnID,RsnDesc,IsReqiredFileUpload from vwReasonList where RsnID='" + requestPlannerDetailsModel.ResnID + "'", con);//where Status=1
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
                            ReasonName += ds.Tables[0].Rows[i]["RsnDesc"].ToString();
                        }
                    }
                }
                string body = "Inspection request " + requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString() + " has been attended with Reason '" + ReasonName + "' and remark " + requestPlannerDetailsModel.InspRemark;
                if (!EmailNotification.sendEmail(Session["EmployeeEmail"].ToString(), "Closed", "Request Closed " + requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString(), body, OutputID, requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString()))
                {
                    TempData["RecordException"] = "Email Notification is not working";
                }
                TempData["TransactionStatus"] = "Request Closed Successfully :" + requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.Message.ToString();
                return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID });
            }
        }

    }
}