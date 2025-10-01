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
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index(int page = 1)
        {
            //Defining the PageSize
            int PageSize = 5;
            //Creating the ViewModel's Object
            LocationMstViewModel objLocationMstViewModel = new LocationMstViewModel();
            DataSet ds = new DataSet();
            //List of the Person
            List<LocMstModel> lstLocMstModel = new List<LocMstModel>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_getAllLocationMaster", con);
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
                        LocMstModel objRqStageModel = new LocMstModel();
                        objRqStageModel.LcID = Convert.IsDBNull(ds.Tables[0].Rows[i]["LcID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["LcID"]);
                        objRqStageModel.LcName = Convert.IsDBNull(ds.Tables[0].Rows[i]["LcName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["LcName"]);
                        objRqStageModel.LcRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["LcRegDttm"]);
                        objRqStageModel.LcRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["LcRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["LcRegBy"]);
                        objRqStageModel.LcStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["LcStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["LcStatus"]);
                        lstLocMstModel.Add(objRqStageModel);
                    }
                    //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                    objLocationMstViewModel.ListLocationMaster = lstLocMstModel;
                    objLocationMstViewModel.pager = pager;
                }
            }
            return View(objLocationMstViewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(LocMstModel locMstModel)
        {
            try
            {
                string query = "usp_DMLLocationMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@LcID", 0);
                        cmd.Parameters.AddWithValue("@LcName", locMstModel.LcName);
                        cmd.Parameters.AddWithValue("@LcRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@LcStatus", locMstModel.LcStatus);
                        cmd.Connection = con;
                        con.Open();
                        locMstModel.LcID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + locMstModel.LcName;
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
                LocMstModel objLocMstModel = new LocMstModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneLocationMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@LcID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objLocMstModel.LcID = Convert.ToInt32(sdr["LcID"]);
                                    objLocMstModel.LcName = Convert.ToString(sdr["LcName"]);
                                    objLocMstModel.LcRegDttm = Convert.ToDateTime(sdr["LcRegDttm"]);
                                    objLocMstModel.LcRegBy = Convert.ToString(sdr["LcRegBy"]);
                                    objLocMstModel.LcStatus = Convert.ToInt32(sdr["LcStatus"]);
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
                if (objLocMstModel.LcID != 0)
                {
                    return View(objLocMstModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objLocMstModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(LocMstModel locMstModel)
        {
            try
            {
                string query = "usp_DMLLocationMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@LcID", locMstModel.LcID);
                        cmd.Parameters.AddWithValue("@LcName", locMstModel.LcName);
                        cmd.Parameters.AddWithValue("@LcRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@LcStatus", locMstModel.LcStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + locMstModel.LcID;
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
                LocMstModel objLocMstModel = new LocMstModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneLocationMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@LcID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objLocMstModel.LcID = Convert.ToInt32(sdr["LcID"]);
                                        objLocMstModel.LcName = Convert.ToString(sdr["LcName"]);
                                        objLocMstModel.LcRegDttm = Convert.ToDateTime(sdr["LcRegDttm"]);
                                        objLocMstModel.LcRegBy = Convert.ToString(sdr["LcRegBy"]);
                                        objLocMstModel.LcStatus = Convert.ToInt32(sdr["LcStatus"]);
                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                TempData["RecordException"] = ex.ToString();
                                return RedirectToAction("Index");
                            }
                           
                        }
                    }
                }
                if (objLocMstModel.LcName == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objLocMstModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}