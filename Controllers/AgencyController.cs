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
    public class AgencyController : Controller
    {
        // GET: Agency
        public ActionResult Index(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 5;
                //Creating the ViewModel's Object
                AgencyMstViewModel objAgencyMstViewModel = new AgencyMstViewModel();
                DataSet ds = new DataSet();
                //List of the Person
                List<AgencyMstModel> lstAgencyMstModel = new List<AgencyMstModel>();


                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllAgencyMaster", con);
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
                            AgencyMstModel objAgencyMstModel = new AgencyMstModel();
                            objAgencyMstModel.AgID = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["AgID"]);
                            objAgencyMstModel.AgName = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgName"]);
                            objAgencyMstModel.AgRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["AgRegDttm"]);
                            objAgencyMstModel.AgRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgRegBy"]);
                            objAgencyMstModel.AgStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["AgStatus"]);
                            lstAgencyMstModel.Add(objAgencyMstModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objAgencyMstViewModel.ListAgencyMst = lstAgencyMstModel;
                        objAgencyMstViewModel.pager = pager;
                    }
                }
                return View(objAgencyMstViewModel);
            }
            catch(Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
            
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(AgencyMstModel agencyMstModel)
        {
            try
            {
                string query = "usp_DMLAgencyMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AgID", 0);
                        cmd.Parameters.AddWithValue("@AgName", agencyMstModel.AgName);
                        cmd.Parameters.AddWithValue("@AgRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@AgStatus", agencyMstModel.AgStatus);
                        cmd.Connection = con;
                        con.Open();
                        agencyMstModel.AgID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + agencyMstModel.AgName;
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
                AgencyMstModel objAgencyMstModel = new AgencyMstModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneAgencyMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@AgID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objAgencyMstModel.AgID = Convert.ToInt32(sdr["AgID"]);
                                    objAgencyMstModel.AgName = Convert.ToString(sdr["AgName"]);
                                    objAgencyMstModel.AgRegDttm = Convert.ToDateTime(sdr["AgRegDttm"]);
                                    objAgencyMstModel.AgRegBy = Convert.ToString(sdr["AgRegBy"]);
                                    objAgencyMstModel.AgStatus = Convert.ToInt32(sdr["AgStatus"]);
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
                if (objAgencyMstModel.AgID != 0)
                {
                    return View(objAgencyMstModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objAgencyMstModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(AgencyMstModel agencyMstModel)
        {
            try
            {
                string query = "usp_DMLAgencyMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AgID", agencyMstModel.AgID);
                        cmd.Parameters.AddWithValue("@AgName", agencyMstModel.AgName);
                        cmd.Parameters.AddWithValue("@AgRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@AgStatus", agencyMstModel.AgStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + agencyMstModel.AgID;
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
                AgencyMstModel objAgencyMstModel = new AgencyMstModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneAgencyMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@AgID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objAgencyMstModel.AgID = Convert.ToInt32(sdr["AgID"]);
                                        objAgencyMstModel.AgName = Convert.ToString(sdr["AgName"]);
                                        objAgencyMstModel.AgRegDttm = Convert.ToDateTime(sdr["AgRegDttm"]);
                                        objAgencyMstModel.AgRegBy = Convert.ToString(sdr["AgRegBy"]);
                                        objAgencyMstModel.AgStatus = Convert.ToInt32(sdr["AgStatus"]);
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
                if (objAgencyMstModel.AgName == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objAgencyMstModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}