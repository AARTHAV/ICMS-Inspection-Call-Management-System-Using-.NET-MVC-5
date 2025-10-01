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
    public class ProjectDescController : Controller
    {
        // GET: Agency
        public ActionResult Index(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 5;
                //Creating the ViewModel's Object
                ProjectDescViewModel objProjectDescModel = new ProjectDescViewModel();
                DataSet ds = new DataSet();
                //List of the Person
                List<ProjectDescModel> lstProjectDescModel = new List<ProjectDescModel>();


                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllProjectDesc", con);
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
                            ProjectDescModel objProjectDesc = new ProjectDescModel();
                            objProjectDesc.ID = Convert.IsDBNull(ds.Tables[0].Rows[i]["ID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["ID"]);
                            objProjectDesc.ProjectCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectCode"]);
                            objProjectDesc.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"]);
                            objProjectDesc.ProjectDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectDesc"]);
                            objProjectDesc.IsActive = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsActive"]) ? false : Convert.ToBoolean(ds.Tables[0].Rows[i]["IsActive"]);
                            lstProjectDescModel.Add(objProjectDesc);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objProjectDescModel.ListProjectDesc = lstProjectDescModel;
                        objProjectDescModel.pager = pager;
                    }
                }
                return View(objProjectDescModel);
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
        public ActionResult Create(ProjectDescModel ProjectDescModel)
        {
            try
            {
                string query = "usp_DMLProjectDesc";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", 0);
                        cmd.Parameters.AddWithValue("@ProjectCode", ProjectDescModel.ProjectCode);
                        cmd.Parameters.AddWithValue("@ProjectDesc", ProjectDescModel.ProjectDesc);
                        cmd.Parameters.AddWithValue("@IsActive", ProjectDescModel.IsActive);
                        cmd.Connection = con;
                        con.Open();
                        ProjectDescModel.ID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + ProjectDescModel.ProjectCode;
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
                ProjectDescModel objProjectDescModel = new ProjectDescModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneProjectDesc";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ProjectCode", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objProjectDescModel.ID = Convert.ToInt32(sdr["ID"]);
                                    objProjectDescModel.ProjectCode = Convert.ToString(sdr["ProjectCode"]);
                                    objProjectDescModel.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]);
                                    objProjectDescModel.ProjectDesc = Convert.ToString(sdr["ProjectDesc"]);
                                    objProjectDescModel.IsActive = Convert.ToBoolean(sdr["IsActive"]);
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
                if (objProjectDescModel.ID != 0)
                {
                    return View(objProjectDescModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objProjectDescModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(ProjectDescModel projectDescModel)
        {
            try
            {
                string query = "usp_DMLProjectDesc";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AgID", projectDescModel.ID);
                        cmd.Parameters.AddWithValue("@ProjectCode", projectDescModel.ProjectCode);
                        cmd.Parameters.AddWithValue("@ProjectDesc", projectDescModel.ProjectDesc);
                        cmd.Parameters.AddWithValue("@IsActive", projectDescModel.IsActive);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + projectDescModel.ID;
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
                ProjectDescModel objProjectDescModel = new ProjectDescModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneProjectDesc";
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
                                        objProjectDescModel.ID = Convert.ToInt32(sdr["ID"]);
                                        objProjectDescModel.ProjectCode = Convert.ToString(sdr["ProjectCode"]);
                                        objProjectDescModel.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]);
                                        objProjectDescModel.ProjectDesc = Convert.ToString(sdr["ProjectDesc"]);
                                        objProjectDescModel.IsActive = Convert.ToBoolean(sdr["IsActive"]);
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
                if (objProjectDescModel.ProjectCode == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objProjectDescModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}