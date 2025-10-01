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
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ICMS.Controllers
{
[IsAuthorized]
    public class RequestPlannerController : Controller
    {
        // GET: RequestPlanner
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
                    SqlCommand com = new SqlCommand("usp_getAllPlannerRequest", con);
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
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqDpcd"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            //objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqRegBy"]);
                             objInspReqModel.EmployeeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            objInspReqModel.AgencyName = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgencyID"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgencyID"]);
                            objInspReqModel.RqTypeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["RequestType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RequestType"]);
                            objInspReqModel.PlanID = Convert.IsDBNull(ds.Tables[0].Rows[i]["PlnID"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["PlnID"]);
                            objRequestPlannerDetailsModel.ReasonName = Convert.IsDBNull(ds.Tables[0].Rows[i]["ReasonID"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ReasonID"]);
                            objInspReqModel.IsUpdateExternalRequest = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsGenerateExternalReq"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["IsGenerateExternalReq"]);
                            objInspReqModel.IsUpdateInternalReturnRequest = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsGenerateInternalReq"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["IsGenerateInternalReq"]);

                            objRequestPlannerDetailsModel.objInspReqModel.Add(objInspReqModel);
                            lstRequestPlannerDetailsModel.Add(objRequestPlannerDetailsModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 1, page, PageSize);
                        objRequestPlannerDetailsViewModel.ListRequestPlannerDetails = lstRequestPlannerDetailsModel;
                        objRequestPlannerDetailsViewModel.pager = pager;
                    }
                }
                try
                {
                    ViewBag.IsSearch = "false";
                    ViewBag.Status = new SelectList(GetAllStatus().ToList(), "StID", "StDesc");

                }
                catch (Exception ex)
                {
                    ViewBag.Status = null;
                    ViewBag.RecordException = ex.ToString();
                    return View();
                }

                return View(objRequestPlannerDetailsViewModel);
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
                    SqlCommand com = new SqlCommand("usp_SearchPlannerRequest", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    com.Parameters.AddWithValue("@StatusID", StID);
                    if (searchText!=null)
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
                            objInspReqModel.OfrDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["OfrDttm"]);
                            objInspReqModel.RqDpcd = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqDpcd"]);
                            objInspReqModel.RqStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objInspReqModel.StageName = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                            objInspReqModel.EmployeeID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqRegBy"]);
                            objInspReqModel.AgencyName = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgencyID"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgencyID"]);
                            objInspReqModel.RqTypeName = Convert.IsDBNull(ds.Tables[0].Rows[i]["RequestType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RequestType"]);
                            objInspReqModel.PlanID = Convert.IsDBNull(ds.Tables[0].Rows[i]["PlnID"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["PlnID"]);
                            objRequestPlannerDetailsModel.ReasonName = Convert.IsDBNull(ds.Tables[0].Rows[i]["ReasonID"]) ? "0" : Convert.ToString(ds.Tables[0].Rows[i]["ReasonID"]);
                            objInspReqModel.IsUpdateExternalRequest = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsGenerateExternalReq"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["IsGenerateExternalReq"]);
                            objInspReqModel.IsUpdateInternalReturnRequest = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsGenerateInternalReq"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["IsGenerateInternalReq"]);
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
                ViewBag.RecordException = ex.ToString();
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
                        TempData["RecordException"] = "Please check Status table because there is no records in this table";
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
                TempData["RecordException"] = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
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
                DateTime? dtPlanStartDate = DateTime.Now;
                DateTime? dtPlanEndDate = DateTime.Now;
                DateTime? dtPlanOn = DateTime.Now;

                DateTime? dtActualStartDate = DateTime.Now;
                DateTime? dtActualEndDate = DateTime.Now;
                DateTime? dtActualOn = DateTime.Now;
                string concateExternalInspector = string.Empty;
                string concateInternalInspector = string.Empty;
                string RequestStatus = string.Empty;
                List<SelectListItem> RequestType = new List<SelectListItem>();

                string externalRemark = string.Empty;
                string ReasonName = string.Empty;
                string InsBy = string.Empty;
                string InsDept = string.Empty;
                string externalActualRemark = string.Empty;
                int IsExternalReplan = 0;
                int IsInternalReplan = 0;
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                RequestPlannerDetailsModel objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                DocumentModel objDocumentModel = new DocumentModel();
                InspReqModel objInspReqModel = new InspReqModel();
                AssignInspectorModel objAssignInspectorModel = new AssignInspectorModel();
                TpiMstModel objTpiMstModel = new TpiMstModel();
                objRequestPlannerDetailsModel.lstDocument = new List<DocumentModel>();
                objRequestPlannerDetailsModel.objInspReqModel = new List<InspReqModel>();
                objRequestPlannerDetailsModel.listAssignInspector = new List<AssignInspectorModel>();
                objRequestPlannerDetailsModel.lstTpiMaster = new List<TpiMstModel>();
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
                                if (data == "" || data == null)
                                {
                                    cmd.Parameters.AddWithValue("@PlanID", 0);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@PlanID", data);
                                }
                                cmd.Connection = con;
                                con.Open();
                                int i = 0;

                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objDocumentModel = new DocumentModel();
                                        objInspReqModel = new InspReqModel();
                                        objAssignInspectorModel = new AssignInspectorModel();
                                        objTpiMstModel = new TpiMstModel();
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
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        RequestStatus = Convert.ToString(sdr["StDesc"]);
                                        objInspReqModel.StageName = Convert.ToString(sdr["StgDesc"]);
                                        objInspReqModel.RqRegBy = Convert.ToString(sdr["EmpName"]);
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        objInspReqModel.LcID = Convert.ToString(sdr["RqLoc"]);
                                        if (sdr["IsGenerateExternalReq"] != null && sdr["IsGenerateExternalReq"] != DBNull.Value)
                                        {
                                            IsExternalReplan = Convert.ToInt32(sdr["IsGenerateExternalReq"]);
                                            objInspReqModel.IsUpdateExternalRequest = IsExternalReplan.ToString();
                                        }
                                        if (sdr["IsGenerateInternalReq"] != null && sdr["IsGenerateInternalReq"] != DBNull.Value)
                                        {
                                            IsInternalReplan = Convert.ToInt32(sdr["IsGenerateInternalReq"]);
                                            objInspReqModel.IsUpdateInternalReturnRequest = IsInternalReplan.ToString();
                                        }

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
                                                objRequestPlannerDetailsModel.lstDocument.Add(objDocumentModel);
                                            }
                                        }
                                        bool hasMyColumn = (sdr.GetSchemaTable().Select("ColumnName = 'PlnID'").Count() == 1);
                                        if (hasMyColumn == true)
                                        {
                                            objRequestPlannerDetailsModel.PlnID = Convert.ToInt32(sdr["PlnID"]);
                                            if (Convert.ToString(sdr["RqTypeName"]).ToString() == "External")
                                            {
                                                if (sdr["InternalPlanDate"] != null && sdr["InternalPlanDate"] != DBNull.Value)
                                                {
                                                    objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["InternalPlanDate"]); ;
                                                    objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["InternalEndDate"]);
                                                    objRequestPlannerDetailsModel.ID = Convert.ToInt32(sdr["InternalPlanID"]);
                                                }
                                                else
                                                {
                                                    objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                    objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                                    objRequestPlannerDetailsModel.ID = 0;
                                                }
                                            }
                                            else
                                            {
                                                objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                                objRequestPlannerDetailsModel.ID = 0;
                                            }

                                            objRequestPlannerDetailsModel.PlnRemark = Convert.ToString(sdr["PlnRemark"]);
                                            objRequestPlannerDetailsModel.PlnBy = Convert.ToString(sdr["PlannerUserName"]);
                                            objRequestPlannerDetailsModel.PlnDpcd = Convert.ToString(sdr["PlnDpcd"]);

                                            if (sdr["AgName"] != null && sdr["AgName"] != DBNull.Value)
                                            {
                                                objRequestPlannerDetailsModel.AgencyName = Convert.ToString(sdr["AgName"]);
                                                objRequestPlannerDetailsModel.ExternalPlnRemark = Convert.ToString(sdr["PlnRemark"]);
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                                objRequestPlannerDetailsModel.ExtenalPlannedOn = Convert.ToDateTime(sdr["PlnDttm"]);
                                            }
                                            else
                                            {
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
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
                                        }
                                        else
                                        {
                                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.ExtenalRquestPlnStart = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.ExtenalRquestPlnEnd = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.PlnID = 0;
                                        }
                                        if (hasMyColumn == true)
                                        {
                                            if (sdr["InsPsNo"] != null && sdr["InsPsNo"] != DBNull.Value)
                                            {
                                                bool containsItem = objRequestPlannerDetailsModel.listAssignInspector.Any(item => item.InsID == Convert.ToInt32(sdr["InsID"]));
                                                if (containsItem == false)
                                                {
                                                    if (sdr["InsID"] != null && sdr["InsID"] != DBNull.Value)
                                                    {
                                                        objAssignInspectorModel.InsID = Convert.ToInt32(sdr["InsID"]);
                                                        objAssignInspectorModel.InsPsNo = Convert.ToInt32(sdr["InsPsNo"]);
                                                        objAssignInspectorModel.PlnID = Convert.ToInt32(sdr["PlnID"]);
                                                        objAssignInspectorModel.InsDttm = Convert.ToDateTime(sdr["InsDttm"]);
                                                        objAssignInspectorModel.InsBy = Convert.ToString(sdr["AssignInsUsername"]);
                                                        if (Convert.ToString(sdr["RqTypeName"]) == "Internal")
                                                        {
                                                            objAssignInspectorModel.TypeOfInspector = "Internal";
                                                            concateInternalInspector += Convert.ToInt32(sdr["InsPsNo"]) + "-" + Convert.ToString(sdr["AssignInsUsername"]) + ",";
                                                        }
                                                        else
                                                        {
                                                            objAssignInspectorModel.TypeOfInspector = "External";
                                                            concateExternalInspector += Convert.ToInt32(sdr["InsPsNo"]) + "-" + Convert.ToString(sdr["AssignInsUsername"]) + ",";
                                                        }
                                                        objRequestPlannerDetailsModel.listAssignInspector.Add(objAssignInspectorModel);
                                                    }

                                                }
                                            }
                                        }
                                        bool hasTpiColumn = (sdr.GetSchemaTable().Select("ColumnName = 'TpName'").Count() == 1);
                                        if (hasTpiColumn == true)
                                        {
                                            if (sdr["TpName"] != null && sdr["TpName"] != DBNull.Value)
                                            {
                                                bool containsItem = objRequestPlannerDetailsModel.lstTpiMaster.Any(item => item.TpID == Convert.ToInt32(sdr["TpID"]));
                                                if (containsItem == false)
                                                {
                                                    objTpiMstModel.TpID = Convert.ToInt32(sdr["TpID"]);
                                                    objTpiMstModel.TpName = Convert.ToString(sdr["TpName"]);
                                                    objTpiMstModel.TpMail = Convert.ToString(sdr["TpMail"]);
                                                    objTpiMstModel.TpMob = Convert.ToString(sdr["TpMob"]);
                                                    objRequestPlannerDetailsModel.lstTpiMaster.Add(objTpiMstModel);
                                                }
                                            }
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
                if (objRequestPlannerDetailsModel.objInspReqModel.Count == 0)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View();
                }
                if (objRequestPlannerDetailsModel.PlnID == 0)
                {
                    ViewBag.Employee = new SelectList(GetAllEmployee(0).ToList(), "EmpPsNo", "EmpName");
                }
                else
                {
                    ViewBag.Employee = new SelectList(GetAllEmployee(objRequestPlannerDetailsModel.PlnID).ToList(), "EmpPsNo", "EmpName");
                    ViewBag.selectedInspector = concateExternalInspector;
                    ViewBag.selectedInteralInspector = concateInternalInspector;
                }

                if (RequestStatus == "Closed" || RequestStatus == "Submited for Approval")
                {
                    return View("Details", objRequestPlannerDetailsModel);
                }
                if (IsInternalReplan == 0 && IsExternalReplan == 0 && objInspReqModel.RqStatus == "Return")
                {
                    return View("Details", objRequestPlannerDetailsModel);
                }
                if (IsInternalReplan == 1 && IsExternalReplan == 0 && objInspReqModel.RqStatus == "Return")
                {
                    return View("Details", objRequestPlannerDetailsModel);
                }
                if ((IsExternalReplan >= 1) && objInspReqModel.RqTypeName == "External" && objInspReqModel.RqStatus == "Return")
                {
                    if (IsInternalReplan == 0)
                    {
                        ViewBag.Reason = new SelectList(GetReasonMasterModels().ToList(), "RsnID", "RsnDesc");
                        RequestType.Add(new SelectListItem() { Text = "Internal", Value = "1", Selected = true });
                        RequestType.Add(new SelectListItem() { Text = "External", Value = "2" });

                        this.ViewBag.RequestType = new SelectList(RequestType, "Value", "Text");
                        return View("Create", objRequestPlannerDetailsModel);

                    }
                    else
                    {
                        return View("Details", objRequestPlannerDetailsModel);
                    }
                }

                if ((IsInternalReplan >= 1 && IsExternalReplan == 0) && objInspReqModel.RqTypeName == "Internal" && objInspReqModel.RqStatus == "Return")
                {
                    return View("Details", objRequestPlannerDetailsModel);
                }
                ViewBag.Reason = new SelectList(GetReasonMasterModels().ToList(), "RsnID", "RsnDesc");
                RequestType.Add(new SelectListItem() { Text = "Internal", Value = "1", Selected = true });
                RequestType.Add(new SelectListItem() { Text = "External", Value = "2" });

                this.ViewBag.RequestType = new SelectList(RequestType, "Value", "Text");
                return View("Create", objRequestPlannerDetailsModel);
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult RecreateExternalRequest(string id, string data)
        {
            try
            {

                string concateExternalInspector = string.Empty;
                string concateInternalInspector = string.Empty;
                string RequestStatus = string.Empty;

                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                RequestPlannerDetailsModel objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                DocumentModel objDocumentModel = new DocumentModel();
                InspReqModel objInspReqModel = new InspReqModel();
                AssignInspectorModel objAssignInspectorModel = new AssignInspectorModel();
                TpiMstModel objTpiMstModel = new TpiMstModel();
                objRequestPlannerDetailsModel.lstDocument = new List<DocumentModel>();
                objRequestPlannerDetailsModel.objInspReqModel = new List<InspReqModel>();
                objRequestPlannerDetailsModel.listAssignInspector = new List<AssignInspectorModel>();
                objRequestPlannerDetailsModel.lstTpiMaster = new List<TpiMstModel>();
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
                                if (data == "")
                                {
                                    cmd.Parameters.AddWithValue("@PlanID", 0);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@PlanID", data);
                                }
                                cmd.Connection = con;
                                con.Open();
                                int i = 0;

                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objDocumentModel = new DocumentModel();
                                        objInspReqModel = new InspReqModel();
                                        objAssignInspectorModel = new AssignInspectorModel();
                                        objTpiMstModel = new TpiMstModel();
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
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        RequestStatus = Convert.ToString(sdr["StDesc"]);
                                        objInspReqModel.StageName = Convert.ToString(sdr["StgDesc"]);
                                        objInspReqModel.RqRegBy = Convert.ToString(sdr["EmpName"]);
                                        objInspReqModel.RqTypeName = Convert.ToString(sdr["RqTypeName"]);
                                        objInspReqModel.LcID = Convert.ToString(sdr["RqLoc"]);

                                        bool hasMyColumn = (sdr.GetSchemaTable().Select("ColumnName = 'PlnID'").Count() == 1);
                                        if (hasMyColumn == true)
                                        {
                                            objRequestPlannerDetailsModel.PlnID = Convert.ToInt32(sdr["PlnID"]);
                                            if (Convert.ToString(sdr["RqTypeName"]).ToString() == "External")
                                            {
                                                if (sdr["InternalPlanDate"] != null && sdr["InternalPlanDate"] != DBNull.Value)
                                                {
                                                    objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["InternalPlanDate"]); ;
                                                    objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["InternalEndDate"]);
                                                    objRequestPlannerDetailsModel.ID = Convert.ToInt32(sdr["InternalPlanID"]);
                                                }
                                                else
                                                {
                                                    objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                    objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                                    objRequestPlannerDetailsModel.ID = 0;
                                                }
                                            }
                                            else
                                            {
                                                objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                                objRequestPlannerDetailsModel.ID = 0;
                                            }

                                            objRequestPlannerDetailsModel.PlnRemark = Convert.ToString(sdr["PlnRemark"]);
                                            objRequestPlannerDetailsModel.PlnBy = Convert.ToString(sdr["PlannerUserName"]);
                                            objRequestPlannerDetailsModel.PlnDpcd = Convert.ToString(sdr["PlnDpcd"]);

                                            if (sdr["AgName"] != null && sdr["AgName"] != DBNull.Value)
                                            {
                                                objRequestPlannerDetailsModel.AgencyName = Convert.ToString(sdr["AgName"]);
                                                objRequestPlannerDetailsModel.ExternalPlnRemark = Convert.ToString(sdr["PlnRemark"]);
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
                                                objRequestPlannerDetailsModel.ExtenalPlannedOn = Convert.ToDateTime(sdr["PlnDttm"]);
                                            }
                                            else
                                            {
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnStart = Convert.ToDateTime(sdr["PlnStart"]);
                                                objRequestPlannerDetailsModel.ExtenalRquestPlnEnd = Convert.ToDateTime(sdr["PlnEnd"]);
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
                                        }
                                        else
                                        {
                                            objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.ExtenalRquestPlnStart = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.ExtenalRquestPlnEnd = Convert.ToDateTime(sdr["OfrDttm"]);
                                            objRequestPlannerDetailsModel.PlnID = 0;
                                        }
                                        if (hasMyColumn == true)
                                        {
                                            if (sdr["InsPsNo"] != null && sdr["InsPsNo"] != DBNull.Value)
                                            {
                                                bool containsItem = objRequestPlannerDetailsModel.listAssignInspector.Any(item => item.InsID == Convert.ToInt32(sdr["InsID"]));
                                                if (containsItem == false)
                                                {
                                                    if (sdr["InsID"] != null && sdr["InsID"] != DBNull.Value)
                                                    {
                                                        objAssignInspectorModel.InsID = Convert.ToInt32(sdr["InsID"]);
                                                        objAssignInspectorModel.InsPsNo = Convert.ToInt32(sdr["InsPsNo"]);
                                                        objAssignInspectorModel.PlnID = Convert.ToInt32(sdr["PlnID"]);
                                                        objAssignInspectorModel.InsDttm = Convert.ToDateTime(sdr["InsDttm"]);
                                                        objAssignInspectorModel.InsBy = Convert.ToString(sdr["AssignInsUsername"]);
                                                        if (Convert.ToString(sdr["RqTypeName"]) == "Internal")
                                                        {
                                                            objAssignInspectorModel.TypeOfInspector = "Internal";
                                                            concateInternalInspector += Convert.ToInt32(sdr["InsPsNo"]) + "-" + Convert.ToString(sdr["AssignInsUsername"]) + ",";
                                                        }
                                                        else
                                                        {
                                                            objAssignInspectorModel.TypeOfInspector = "External";
                                                            concateExternalInspector += Convert.ToInt32(sdr["InsPsNo"]) + "-" + Convert.ToString(sdr["AssignInsUsername"]) + ",";
                                                        }
                                                        objRequestPlannerDetailsModel.listAssignInspector.Add(objAssignInspectorModel);
                                                    }

                                                }
                                            }
                                        }

                                        objRequestPlannerDetailsModel.objInspReqModel.Add(objInspReqModel);
                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                TempData["RecordException"] = ex.Message.ToString();
                                return RedirectToAction("Index");
                            }

                        }
                    }
                }
                if (objRequestPlannerDetailsModel.objInspReqModel.Count == 0)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View();
                }
                if (objRequestPlannerDetailsModel.PlnID == 0)
                {
                    ViewBag.Employee = new SelectList(GetAllEmployee(0).ToList(), "EmpPsNo", "EmpName");
                }
                else
                {
                    ViewBag.Employee = new SelectList(GetAllEmployee(objRequestPlannerDetailsModel.PlnID).ToList(), "EmpPsNo", "EmpName");
                    ViewBag.selectedInspector = concateExternalInspector;
                    ViewBag.selectedInteralInspector = concateInternalInspector;
                }
                ViewBag.Reason = new SelectList(GetReasonMasterModels().ToList(), "RsnID", "RsnDesc");
                List<SelectListItem> RequestType = new List<SelectListItem>();
                RequestType.Add(new SelectListItem() { Text = "Internal", Value = "1", Selected = true });
                RequestType.Add(new SelectListItem() { Text = "External", Value = "2" });

                this.ViewBag.RequestType = new SelectList(RequestType, "Value", "Text");
                return View("Create", objRequestPlannerDetailsModel);
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.Message.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Create(RequestPlannerDetailsModel requestPlannerDetailsModel)
        {
            string OldEmployee = string.Empty;
            Random randomNumber = new Random();
            try
            {
                if (requestPlannerDetailsModel.PlnID != 0)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                    {
                        con.Open();
                        DataSet ds = new DataSet();
                        SqlCommand com = new SqlCommand("select T1.EmpMail from tblAssignInspct T JOIN tblEmpMst T1 on T.InsPsNo=T1.EmpPsNo where T.PlnID='" + requestPlannerDetailsModel.PlnID + "'", con);//where Status=1
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
                string ExternalInspector = string.Empty;
                string Inspector = string.Empty;
                List<string> listExternalInspector = null;
                List<string> listInspector = null;
                if (requestPlannerDetailsModel.ExternaltempValue != null)
                {
                    ExternalInspector = requestPlannerDetailsModel.ExternaltempValue.Substring(0, requestPlannerDetailsModel.ExternaltempValue.Length - 1);
                    listExternalInspector = new List<string>();
                    listExternalInspector = ExternalInspector.Split(',').ToList();
                }

                var OutputID = string.Empty;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (requestPlannerDetailsModel.objInspReqModel[0].RqTypeName == "Internal")
                {
                    if (requestPlannerDetailsModel.tempValue != null)
                    {
                        Inspector = requestPlannerDetailsModel.tempValue.Substring(0, requestPlannerDetailsModel.tempValue.Length - 1);
                        listInspector = new List<string>();
                        listInspector = Inspector.Split(',').ToList();
                    }

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(query))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                //cmd.Parameters.AddWithValue("@PlnID", requestPlannerDetailsModel.PlnID);
                                if (requestPlannerDetailsModel.objInspReqModel[0].RqStatus == "Return")
                                {
                                    cmd.Parameters.AddWithValue("@PlnID", 0);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@PlnID", requestPlannerDetailsModel.PlnID);
                                }
                                cmd.Parameters.AddWithValue("@RqID", requestPlannerDetailsModel.objInspReqModel[0].RqID);
                                cmd.Parameters.AddWithValue("@RqstType", "1");
                                cmd.Parameters.AddWithValue("@RqNo", requestPlannerDetailsModel.objInspReqModel[0].RqNo);
                                cmd.Parameters.AddWithValue("@RqRevNo", requestPlannerDetailsModel.objInspReqModel[0].RqRevNo);
                                cmd.Parameters.AddWithValue("@PlnStart", requestPlannerDetailsModel.PlnStart);
                                cmd.Parameters.AddWithValue("@PlnEnd", requestPlannerDetailsModel.PlnEnd);
                                cmd.Parameters.AddWithValue("@PlnBy", Session["EmpPsNo"].ToString());
                                cmd.Parameters.AddWithValue("@PlnDpcd", Session["EmployeeDeparment"].ToString());
                                cmd.Parameters.AddWithValue("@PlnRemark", requestPlannerDetailsModel.PlnRemark);
                                if (requestPlannerDetailsModel.PlnID != 0)
                                {
                                    cmd.Parameters.AddWithValue("@ReasonCode", requestPlannerDetailsModel.ResnID);
                                    cmd.Parameters.AddWithValue("@InsRemark", requestPlannerDetailsModel.InspRemark);
                                }
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
                            return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID, data = "" });
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
                                        ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                        TempData["RecordException"] = ex.Message.ToString();
                                        return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID, data = "" });
                                    }

                                }
                            }

                        }
                    }

                    if (listInspector != null)
                    {
                        foreach (var item in listInspector)
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
                                        return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID, data = "" });
                                    }

                                }
                            }
                        }
                    }

                    body = "Inspection request has been assigned to you for Project " + requestPlannerDetailsModel.objInspReqModel[0].PrjNo.ToString() + "/" + requestPlannerDetailsModel.objInspReqModel[0].FrgNo.ToString() + " for stage '" + requestPlannerDetailsModel.objInspReqModel[0].StageName.ToString() + "' of planned on '" + requestPlannerDetailsModel.PlnStart + "'";
                    OldEmployee += Session["EmployeeEmail"].ToString();
                    if (!EmailNotification.sendEmail(OldEmployee.ToString(), "Assigned", "Request Assigned " + requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString(), body, OutputID, requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString()))
                    {
                        TempData["RecordException"] = "Email Notification is not working";
                    }
                }


                if (requestPlannerDetailsModel.IsParallel == 1)
                {
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(query))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                if (requestPlannerDetailsModel.objInspReqModel[0].RqStatus == "Closed" || requestPlannerDetailsModel.objInspReqModel[0].RqStatus == "Return")
                                {
                                    cmd.Parameters.AddWithValue("@PlnID", 0);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@PlnID", requestPlannerDetailsModel.PlnID);
                                }
                                cmd.Parameters.AddWithValue("@RqID", requestPlannerDetailsModel.objInspReqModel[0].RqID);
                                cmd.Parameters.AddWithValue("@RqstType", "2");
                                cmd.Parameters.AddWithValue("@RqNo", requestPlannerDetailsModel.objInspReqModel[0].RqNo);
                                cmd.Parameters.AddWithValue("@RqRevNo", requestPlannerDetailsModel.objInspReqModel[0].RqRevNo);
                                cmd.Parameters.AddWithValue("@AgencyID", requestPlannerDetailsModel.AgcyID);
                                if (requestPlannerDetailsModel.AgcyID != 0)
                                {
                                    cmd.Parameters.AddWithValue("@PlnStart", requestPlannerDetailsModel.ExtenalRquestPlnStart);
                                    cmd.Parameters.AddWithValue("@PlnEnd", requestPlannerDetailsModel.ExtenalRquestPlnEnd);
                                    cmd.Parameters.AddWithValue("@PlnRemark", requestPlannerDetailsModel.ExternalPlnRemark);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@PlnStart", requestPlannerDetailsModel.PlnStart);
                                    cmd.Parameters.AddWithValue("@PlnEnd", requestPlannerDetailsModel.PlnEnd);
                                    cmd.Parameters.AddWithValue("@PlnRemark", requestPlannerDetailsModel.PlnRemark);
                                }
                                cmd.Parameters.AddWithValue("@PlnBy", Session["EmpPsNo"].ToString());
                                cmd.Parameters.AddWithValue("@PlnDpcd", Session["EmployeeDeparment"].ToString());
                                if (requestPlannerDetailsModel.PlnID != 0)
                                {
                                    cmd.Parameters.AddWithValue("@ReasonCode", requestPlannerDetailsModel.ResnID);
                                    cmd.Parameters.AddWithValue("@InsRemark", requestPlannerDetailsModel.InspRemark);
                                }
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
                            TempData["RecordException"] = ex.ToString();
                            return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID, data = "" });
                        }

                    }

                    foreach (HttpPostedFileBase file in requestPlannerDetailsModel.UploadedExternalFile)
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
                                        ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                        TempData["RecordException"] = ex.ToString();
                                        return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID, data = "" });
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
                                        ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                                        TempData["RecordException"] = ex.ToString();
                                        return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID, data = "" });
                                    }

                                }
                            }
                        }
                    }
                    body = "Inspection external request has been assigned to you for Project " + requestPlannerDetailsModel.objInspReqModel[0].PrjNo.ToString() + "/" + requestPlannerDetailsModel.objInspReqModel[0].FrgNo.ToString() + " for stage '" + requestPlannerDetailsModel.objInspReqModel[0].StageName.ToString() + "' of planned on '" + requestPlannerDetailsModel.PlnStart + "'";
                    if (OldEmployee != Session["EmployeeEmail"].ToString())
                    {
                        OldEmployee += Session["EmployeeEmail"].ToString();
                    }
                    if (!EmailNotification.sendEmail(OldEmployee.ToString(), "Assigned", "Request Assigned " + requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString(), body, OutputID, requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString()))
                    {
                        TempData["RecordException"] = "Email Notification is not working";
                    }
                }

                TempData["TransactionStatus"] = "Request Planned Successfully :" + requestPlannerDetailsModel.objInspReqModel[0].RqNo + "-" + requestPlannerDetailsModel.objInspReqModel[0].RqRevNo.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Create", new { id = requestPlannerDetailsModel.objInspReqModel[0].RqID, data = "" });
            }
        }

        private List<EmployeeModel> GetAllEmployee(int PlnID)
        {
            List<EmployeeModel> lstEmployeeModel = new List<EmployeeModel>();

            try
            {
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
                        com = new SqlCommand("Select * from vwInspectorList where EmpPsNo not in (select InsPsNo from tblAssignInspct where PlnID='" + PlnID + "')", con);//where Status=1
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
                    else
                    {
                        TempData["RecordException"] = "Please check Inspector table because there is no data in this table";
                        EmployeeModel objEmployeeModel = new EmployeeModel();
                        objEmployeeModel.EmpPsNo = "0";
                        objEmployeeModel.EmpName = "";
                        lstEmployeeModel.Add(objEmployeeModel);
                        return lstEmployeeModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = "Please check Inspector table because there is no data in this table";
                EmployeeModel objEmployeeModel = new EmployeeModel();
                objEmployeeModel.EmpPsNo = "0";
                objEmployeeModel.EmpName = "";
                lstEmployeeModel.Add(objEmployeeModel);
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return lstEmployeeModel;
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
                            TempData["RecordException"] = ex.ToString();
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
                            TempData["RecordException"] = ex.ToString();
                            return RedirectToAction("Create", new { id = data });
                        }
                    }
                }
                return RedirectToAction("Create", new { id = data });
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Create", new { id = data });
            }

        }
        [HttpGet]
        public ActionResult DeleteAssignInsp(int? id, string data)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "DELETE from tblAssignInspct where InsID='" + id + "'";
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
                            TempData["RecordException"] = ex.ToString();
                            return RedirectToAction("Create", new { id = data });
                        }
                    }
                }
                string insertQuery = "insert into tblLogs(LgRqstID,LgDttm,LgBy,LgDpcd,LgTxt) values('" + data.ToString() + "','" + DateTime.Today + "','" + Session["EmpPsNo"].ToString() + "','" + Session["EmployeeDeparment"].ToString() + "','Assocated Employee is deleted and ID is : " + id.ToString() + "')";
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
                            TempData["RecordException"] = ex.ToString();
                            return RedirectToAction("Create", new { id = data });
                        }
                    }
                }
                return RedirectToAction("Create", new { id = data });
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Create", new { id = data });
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
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return null;
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

        public ActionResult CheckInspectorAvaliable(string inspectorNo, DateTime startDate, DateTime endDate, string PlanID)
        {
            try
            {
                var result = "";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("usp_CheckInspectorAvaliable", con);//where Status=1
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    com.Parameters.AddWithValue("@InsPsNo", inspectorNo);
                    com.Parameters.AddWithValue("@StartDateTime", startDate);
                    com.Parameters.AddWithValue("@EndDateTime", endDate);
                    com.Parameters.AddWithValue("@PlanID", PlanID);
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
                            result = ds.Tables[0].Rows[i][0].ToString();
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

        [HttpGet]
        public ActionResult GenerateRequestReport()
        {
            return View("GenerateRequestReport");
        }
        [HttpGet]
        public ActionResult DownloadRequestReport(DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                //Defining the PageSize
                //Creating the ViewModel's Object
                DataSet ds = new DataSet();
                //List of the Person
                string Inspector = string.Empty;
                List<InspReqModel> lstInspReqModel = new List<InspReqModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_GenerateReport", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                    SqlDataAdapter adapt = new SqlDataAdapter(com);
                    if (StartDate == null)
                    {
                        com.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        com.Parameters.AddWithValue("@ToDate", DBNull.Value);
                    }
                    else
                    {
                        com.Parameters.AddWithValue("@FromDate", StartDate);
                        if (EndDate == null)
                        {
                            com.Parameters.AddWithValue("@ToDate", DateTime.Today);
                        }
                        else
                        {
                            com.Parameters.AddWithValue("@ToDate", EndDate);
                        }
                    }
                    //Fill the Dataset and Close the connection
                    adapt.Fill(ds);
                    con.Close();
                    //Bind the data in List of type Person
                    //We are returning Dataset with two Datatable, one contains the Person Data and Other contains the total records count
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var gv = new GridView();
                            gv.DataSource = ds;
                            gv.DataBind();
                            Response.ClearContent();
                            Response.Buffer = true;
                            Response.AddHeader("content-disposition", "attachment; filename=GenerateReport.xls");
                            Response.ContentType = "application/ms-excel";
                            Response.Charset = "";
                            StringWriter objStringWriter = new StringWriter();
                            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                            gv.RenderControl(objHtmlTextWriter);
                            Response.Output.Write(objStringWriter.ToString());
                            Response.Flush();
                            Response.End();
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    }
                }
                return View("GenerateRequestReport");
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Print");
            }
        }

        [HttpGet]
        public ActionResult GenerateCalenderReport()
        {
            return View("GenerateCalenderReport");
        }
        public JsonResult GetAssignedInspectorReport()
        {
            try
            {
                var result = new List<SelectListItem>();
                List<RequestPlannerDetailsModel> listRequestPlannerDetailsModel = new List<RequestPlannerDetailsModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("usp_GenerateCalenderReport", con);
                    com.CommandType = CommandType.StoredProcedure;
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
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                RequestPlannerDetailsModel objRequestPlannerDetailsModel = new RequestPlannerDetailsModel();
                                objRequestPlannerDetailsModel.RqID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RqID"]);
                                objRequestPlannerDetailsModel.RqNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqNo"]);
                                objRequestPlannerDetailsModel.RqRevNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["RqRevNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RqRevNo"]);
                                objRequestPlannerDetailsModel.PlnRemark = Convert.IsDBNull(ds.Tables[0].Rows[i]["PlnRemark"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PlnRemark"]);
                                objRequestPlannerDetailsModel.tempValue = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                                objRequestPlannerDetailsModel.PlnStart = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnStart"]);
                                objRequestPlannerDetailsModel.PlnEnd = Convert.ToDateTime(ds.Tables[0].Rows[i]["PlnEnd"]);
                                objRequestPlannerDetailsModel.RqstType = Convert.IsDBNull(ds.Tables[0].Rows[i]["RequestType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RequestType"]);
                                objRequestPlannerDetailsModel.ExternaltempValue = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                                objRequestPlannerDetailsModel.ProjectNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["PrjNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PrjNo"]);
                                objRequestPlannerDetailsModel.ForgingNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["FrgNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FrgNo"]);
                                listRequestPlannerDetailsModel.Add(objRequestPlannerDetailsModel);
                            }


                            return new JsonResult { Data = listRequestPlannerDetailsModel.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
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