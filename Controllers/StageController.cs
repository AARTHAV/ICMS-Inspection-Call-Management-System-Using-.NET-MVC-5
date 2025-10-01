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
    public class StageController : Controller
    {
        // GET: Stage

        public ActionResult Index(int page = 1)
        {
            //Defining the PageSize
            int PageSize = 5;
            //Creating the ViewModel's Object
            RqStageViewModel objRqStageViewModel = new RqStageViewModel();
            DataSet ds = new DataSet();
            //List of the Person
            List<StageModel> lstRqStageModel = new List<StageModel>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_getAllStage", con);
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
                        StageModel objRqStageModel = new StageModel();
                        objRqStageModel.StgID = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["StgID"]);
                        objRqStageModel.StgDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgDesc"]);
                        objRqStageModel.StgRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["StgRegDttm"]);
                        objRqStageModel.StgRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StgRegBy"]);
                        objRqStageModel.StgStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StgStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["StgStatus"]);
                        lstRqStageModel.Add(objRqStageModel);
                    }
                    //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                    objRqStageViewModel.ListStage = lstRqStageModel;
                    objRqStageViewModel.pager = pager;
                }
            }
            return View(objRqStageViewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(StageModel rqStageModel)
        {
            try
            {
                string query = "usp_DMLStage";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StgID", 0);
                        cmd.Parameters.AddWithValue("@StgDesc", rqStageModel.StgDesc);
                        cmd.Parameters.AddWithValue("@StgStatus", rqStageModel.StgStatus);
                        cmd.Parameters.AddWithValue("@StgRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Connection = con;
                        con.Open();
                        rqStageModel.StgID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + rqStageModel.StgDesc;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.Message.ToString();
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
                StageModel objRqStageModel = new StageModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneStage";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@StgID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objRqStageModel.StgID = Convert.ToInt32(sdr["StgID"]);
                                    objRqStageModel.StgDesc = Convert.ToString(sdr["StgDesc"]);
                                    objRqStageModel.StgRegDttm = Convert.ToDateTime(sdr["StgRegDttm"]);
                                    objRqStageModel.StgRegBy = Convert.ToString(sdr["StgRegBy"]);
                                    objRqStageModel.StgStatus = Convert.ToInt32(sdr["StgStatus"]);
                                }
                            }
                            con.Close();

                        }
                        catch(Exception ex)
                        {
                            TempData["RecordException"] = ex.Message.ToString();
                            return RedirectToAction("Index");

                        }
                        
                    }
                }
                if (objRqStageModel.StgID != 0)
                {
                    return View(objRqStageModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objRqStageModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(StageModel rqStageModel)
        {
            try
            {
                string query = "usp_DMLStage";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StgID", rqStageModel.StgID);
                        cmd.Parameters.AddWithValue("@StgDesc", rqStageModel.StgDesc);
                        cmd.Parameters.AddWithValue("@StgRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@StgStatus", rqStageModel.StgStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + rqStageModel.StgID;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordNotExist = ex.ToString();
                return View();
            }
        }
        [HttpGet]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                StageModel objRqStageModel = new StageModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneStage";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@StgID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objRqStageModel.StgID = Convert.ToInt32(sdr["StgID"]);
                                        objRqStageModel.StgDesc = Convert.ToString(sdr["StgDesc"]);
                                        objRqStageModel.StgRegDttm = Convert.ToDateTime(sdr["StgRegDttm"]);
                                        objRqStageModel.StgRegBy = Convert.ToString(sdr["StgRegBy"]);
                                        objRqStageModel.StgStatus = Convert.ToInt32(sdr["StgStatus"]);
                                    }
                                }
                                con.Close();
                            }
                            catch(Exception ex)
                            {
                                TempData["RecordException"] = ex.Message.ToString();
                                return RedirectToAction("Index");
                            }
                           
                        }
                    }
                }
                if (objRqStageModel.StgDesc == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objRqStageModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}