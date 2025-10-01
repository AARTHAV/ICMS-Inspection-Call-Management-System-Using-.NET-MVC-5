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
    public class RoleController : Controller
    {
        public ActionResult Index(int page = 1)
        {
            int PageSize = 5;
            //Creating the ViewModel's Object
            RoleMsViewModel objRoleMsViewModel = new RoleMsViewModel();
            DataSet ds = new DataSet();
            //List of the Person
            List<RoleMsModel> lstRoleMsModel = new List<RoleMsModel>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_getAllRoleMaster", con);
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
                        RoleMsModel objRoleMsModel = new RoleMsModel();
                        objRoleMsModel.RoID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RoID"]);
                        objRoleMsModel.RoDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RoDesc"]);
                        objRoleMsModel.RoRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["RoRegDttm"]);
                        objRoleMsModel.RoRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RoRegBy"]);
                        objRoleMsModel.RoStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RoStatus"]);
                        lstRoleMsModel.Add(objRoleMsModel);
                    }
                    //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                    objRoleMsViewModel.ListRoleMaster = lstRoleMsModel;
                    objRoleMsViewModel.pager = pager;
                }
            }
            return View(objRoleMsViewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(RoleMsModel roleMsModel)
        {
            try
            {
                string query = "usp_DMLRoleMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoID", 0);
                        cmd.Parameters.AddWithValue("@RoDesc", roleMsModel.RoDesc);
                        cmd.Parameters.AddWithValue("@RoRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@RoStatus", roleMsModel.RoStatus);
                        cmd.Connection = con;
                        con.Open();
                        roleMsModel.RoID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + roleMsModel.RoDesc;
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
                RoleMsModel objRoleMsModel = new RoleMsModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneRoleMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@RoID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objRoleMsModel.RoID = Convert.ToInt32(sdr["RoID"]);
                                    objRoleMsModel.RoDesc = Convert.ToString(sdr["RoDesc"]);
                                    objRoleMsModel.RoRegDttm = Convert.ToDateTime(sdr["RoRegDttm"]);
                                    objRoleMsModel.RoRegBy = Convert.ToString(sdr["RoRegBy"]);
                                    objRoleMsModel.RoStatus = Convert.ToInt32(sdr["RoStatus"]);
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
                if (objRoleMsModel.RoID != 0)
                {
                    return View(objRoleMsModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objRoleMsModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(RoleMsModel roleMsModel)
        {
            try
            {
                string query = "usp_DMLRoleMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoID", roleMsModel.RoID);
                        cmd.Parameters.AddWithValue("@RoDesc", roleMsModel.RoDesc);
                        cmd.Parameters.AddWithValue("@RoRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@RoStatus", roleMsModel.RoStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + roleMsModel.RoID;
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
                RoleMsModel objRoleMsModel = new RoleMsModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneRoleMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@RoID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objRoleMsModel.RoID = Convert.ToInt32(sdr["RoID"]);
                                        objRoleMsModel.RoDesc = Convert.ToString(sdr["RoDesc"]);
                                        objRoleMsModel.RoRegDttm = Convert.ToDateTime(sdr["RoRegDttm"]);
                                        objRoleMsModel.RoRegBy = Convert.ToString(sdr["RoRegBy"]);
                                        objRoleMsModel.RoStatus = Convert.ToInt32(sdr["RoStatus"]);
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
                if (objRoleMsModel.RoDesc == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objRoleMsModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}