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
    public class ReasonController : Controller
    {
        // GET: Resn
        public ActionResult Index(int page = 1)
        {
            //Defining the PageSize
            int PageSize = 5;
            //Creating the ViewModel's Object
            ReasonMstViewModel objResnMstViewModel = new ReasonMstViewModel();
            DataSet ds = new DataSet();
            //List of the Person
            List<ReasonMstModel> lstResnMstModel = new List<ReasonMstModel>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_getAllResnMaster", con);
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
                        ReasonMstModel objResnMstModel = new ReasonMstModel();
                        objResnMstModel.RsnID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnID"]) ? 0: Convert.ToInt32(ds.Tables[0].Rows[i]["RsnID"]);
                        objResnMstModel.RsnDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RsnDesc"]);
                        objResnMstModel.RsnRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["RsnRegDttm"]);
                        objResnMstModel.RsnRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RsnRegBy"]);
                        objResnMstModel.RsnStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["RsnStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RsnStatus"]);
                        objResnMstModel.IsReqiredFileUpload = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsReqiredFileUpload"]) ? false : Convert.ToBoolean(ds.Tables[0].Rows[i]["IsReqiredFileUpload"]);
                        lstResnMstModel.Add(objResnMstModel);
                    }
                    //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                    objResnMstViewModel.ListResnMaster = lstResnMstModel;
                    objResnMstViewModel.pager = pager;
                }
            }
            return View(objResnMstViewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ReasonMstModel resnMstModel)
        {
            try
            {
                string query = "usp_DMLResnMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RsnID", 0);
                        cmd.Parameters.AddWithValue("@RsnDesc", resnMstModel.RsnDesc);
                        cmd.Parameters.AddWithValue("@IsReqiredFileUpload", resnMstModel.IsReqiredFileUpload);
                        cmd.Parameters.AddWithValue("@RsnRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@RsnStatus", resnMstModel.RsnStatus);
                        cmd.Connection = con;
                        con.Open();
                        resnMstModel.RsnID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + resnMstModel.RsnDesc;
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
                ReasonMstModel objResnMstModel = new ReasonMstModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneResnMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@RsnID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objResnMstModel.RsnID = Convert.ToInt32(sdr["RsnID"]);
                                    objResnMstModel.RsnDesc = Convert.ToString(sdr["RsnDesc"]);
                                    objResnMstModel.RsnRegDttm = Convert.ToDateTime(sdr["RsnRegDttm"]);
                                    objResnMstModel.RsnRegBy = Convert.ToString(sdr["RsnRegBy"]);
                                    objResnMstModel.RsnStatus = Convert.ToInt32(sdr["RsnStatus"]);
                                    objResnMstModel.IsReqiredFileUpload = Convert.ToBoolean(sdr["IsReqiredFileUpload"]);
                                }
                            }
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            TempData["RecordException"] = ex.Message.ToString();
                            return RedirectToAction("Index");
                        }

                    }
                }
                if (objResnMstModel.RsnID != 0)
                {
                    return View(objResnMstModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objResnMstModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(ReasonMstModel resnMstModel)
        {
            try
            {
                string query = "usp_DMLResnMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RsnID", resnMstModel.RsnID);
                        cmd.Parameters.AddWithValue("@RsnDesc", resnMstModel.RsnDesc);
                        cmd.Parameters.AddWithValue("@IsReqiredFileUpload", resnMstModel.IsReqiredFileUpload);
                        cmd.Parameters.AddWithValue("@RsnRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@RsnStatus", resnMstModel.RsnStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + resnMstModel.RsnID;
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
                ReasonMstModel objResnMstModel = new ReasonMstModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneResnMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@RsnID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objResnMstModel.RsnID = Convert.ToInt32(sdr["RsnID"]);
                                        objResnMstModel.RsnDesc = Convert.ToString(sdr["RsnDesc"]);
                                        objResnMstModel.RsnRegDttm = Convert.ToDateTime(sdr["RsnRegDttm"]);
                                        objResnMstModel.RsnRegBy = Convert.ToString(sdr["RsnRegBy"]);
                                        objResnMstModel.RsnStatus = Convert.ToInt32(sdr["RsnStatus"]);
                                        objResnMstModel.IsReqiredFileUpload = Convert.ToBoolean(sdr["IsReqiredFileUpload"]);
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
                if (objResnMstModel.RsnDesc == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objResnMstModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}