using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICMS.Models;
using PagedList;
namespace ICMS.Controllers
{
    public class StatusController : Controller
    {
        //GET: Status
        public ActionResult Index(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 5;
                //Creating the ViewModel's Object
                StatusViewModel obj = new StatusViewModel();
                DataSet ds = new DataSet();
                //List of the Person
                List<StatusModel> lstPerson = new List<StatusModel>();

                //Connecting to the Database (Here, I am using ADO.Net in order to interact with the database)
                //You can use any ORM as per your need or requirement


                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllStatus", con);
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
                            StatusModel objPerson = new StatusModel();
                            objPerson.StID = Convert.IsDBNull(ds.Tables[0].Rows[i]["StID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["StID"]);
                            objPerson.StDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["StDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StDesc"]);
                            objPerson.StRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["StRegDttm"]);
                            objPerson.StRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["StRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["StRegBy"]);
                            objPerson.StStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["StStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["StStatus"]);
                            lstPerson.Add(objPerson);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        obj.ListStatus = lstPerson;
                        obj.pager = pager;
                    }
                }
                return View(obj);
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
        public ActionResult Create(StatusModel statusModel)
        {
            try
            {
                string query = "usp_DMLStatus";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StID", 0);
                        cmd.Parameters.AddWithValue("@StDesc", statusModel.StDesc);
                        cmd.Parameters.AddWithValue("@StRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@StStatus", statusModel.StStatus);
                        cmd.Connection = con;
                        con.Open();
                        statusModel.StID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + statusModel.StDesc;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordNotExist = ex.Message.ToString();
                return View();
            }
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                StatusModel statuses = new StatusModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_GetOneStatusMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@StID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    statuses.StID = Convert.ToInt32(sdr["StID"]);
                                    statuses.StDesc = Convert.ToString(sdr["StDesc"]);
                                    statuses.StStatus = Convert.ToInt32(sdr["StStatus"]);
                                    statuses.StRegDttm = Convert.ToDateTime(sdr["StRegDttm"]);
                                    statuses.StRegBy = Convert.ToString(sdr["StRegBy"]);
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
                if (statuses != null)
                {
                    return View(statuses);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(statuses);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(StatusModel statusModel)
        {
            try
            {
                string query = "usp_DMLStatus";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StID", statusModel.StID);
                        cmd.Parameters.AddWithValue("@StDesc", statusModel.StDesc);
                        cmd.Parameters.AddWithValue("@StRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@StStatus", statusModel.StStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + statusModel.StID;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordNotExist = ex.ToString();
                return View();
            }
        }
        [HttpGet]
        public ActionResult Details(string id)
        {
            try
            {
                StatusModel statuses = new StatusModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_GetOneStatusMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@StID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        statuses.StID = Convert.ToInt32(sdr["StID"]);
                                        statuses.StDesc = Convert.ToString(sdr["StDesc"]);
                                        statuses.StStatus = Convert.ToInt32(sdr["StStatus"]);
                                        statuses.StRegDttm = Convert.ToDateTime(sdr["StRegDttm"]);
                                        statuses.StRegBy = Convert.ToString(sdr["StRegBy"]);
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
                }
                if (statuses.StDesc == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(statuses);
            }
            catch (Exception ex)
            {
                ViewBag.RecordNotExist = ex.ToString();
                return View();
            }
        }
    }
}