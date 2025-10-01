using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICMS.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ICMS.App_Start;
using System.Globalization;
using System.IO;
using System.Web.Security;
using PagedList;

namespace ICMS.Controllers
{
    public class MaintainEmailController : Controller
    {
        // GET: MaintainEmail
        public ActionResult Index()
        {
            //Creating the ViewModel's Object
            MaintainEmailViewModel obj = new MaintainEmailViewModel();
            DataSet ds = new DataSet();
            //List of the Email
            List<MaintainEmailModel> lstmaintainEmails = new List<MaintainEmailModel>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_GetAllMaintEmail", con);
                com.CommandType = CommandType.StoredProcedure;
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
                        MaintainEmailModel objEmailModel = new MaintainEmailModel();
                        // objEmailModel.Flag = Convert.IsDBNull(ds.Tables[0].Rows[i][2]) ? 0: Convert.ToInt32(ds.Tables[0].Rows[i][2]);

                        // objEmailModel.Flag = 2;
                        objEmailModel.HedID = Convert.IsDBNull(ds.Tables[0].Rows[i]["HedID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["HedID"]);
                        objEmailModel.Project = Convert.IsDBNull(ds.Tables[0].Rows[i]["Project"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Project"]);
                        objEmailModel.EmailAddr = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmailAddr"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmailAddr"]);
                        objEmailModel.EmailType = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmailType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmailType"]);
                        objEmailModel.RegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["RegDttm"]);
                        objEmailModel.RegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["RegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RegBy"]);
                        objEmailModel.IsActive = Convert.IsDBNull(ds.Tables[0].Rows[i]["IsActive"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["IsActive"]);
                        lstmaintainEmails.Add(objEmailModel);
                    }
                    obj.ListEmail = lstmaintainEmails;
                }
            }
            return View(obj);
        }


        // GET: MaintainEmail/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                MaintainEmailModel objEmailModel = new MaintainEmailModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneEmailAddress";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@HedID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objEmailModel.HedID = Convert.ToInt32(sdr["HedID"]);
                                        objEmailModel.Project = Convert.ToString(sdr["Project"]);
                                        objEmailModel.EmailAddr = Convert.ToString(sdr["EmailAddr"]);
                                        objEmailModel.EmailType = Convert.ToString(sdr["EmailType"]);
                                        objEmailModel.RegDttm = Convert.ToDateTime(sdr["RegDttm"]);
                                        objEmailModel.RegBy = Convert.ToString(sdr["RegBy"]);
                                        objEmailModel.IsActive = Convert.ToInt32(sdr["IsActive"]);
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
                if (objEmailModel.EmailAddr == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objEmailModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
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
                    SqlCommand com = new SqlCommand("Select distinct EmailAddr from vwEmaillst", con);
                    com.CommandType = CommandType.Text;
                    SqlDataAdapter adapt = new SqlDataAdapter(com);
                    adapt.Fill(ds);
                    con.Close();
                    if (ds != null)
                    {
                        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            result.Add(new SelectListItem
                            {
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmailAddr"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmailAddr"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmailAddr"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmailAddr"])
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
         private List<SelectListItem> GetProject()
        {
            try
            {
                var result = new List<SelectListItem>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select distinct ProjectNo from vwProject", con);
                    com.CommandType = CommandType.Text;
                    SqlDataAdapter adapt = new SqlDataAdapter(com);
                    adapt.Fill(ds);
                    con.Close();
                    if (ds != null)
                    {
                        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            result.Add(new SelectListItem
                            {
                                Value = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectNo"]),
                                Text = Convert.IsDBNull(ds.Tables[0].Rows[i]["ProjectNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ProjectNo"])
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

        // GET: MaintainEmail/Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = new MaintainEmailModel();
                model.listvwMail = GetProjectList();
                model.listvwProject = GetProject();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                ExceptionCls.SaveException(ex, this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                return RedirectToAction("Index");
            }

        }

        // POST: MaintainEmail/Create
        [HttpPost]
        public ActionResult Create(MaintainEmailModel maintainEmailModel)
        {
            try
            {
                string query = "usp_DMLMaintEmail";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", 0);
                        cmd.Parameters.AddWithValue("@HedID", "");
                        cmd.Parameters.AddWithValue("@Project", maintainEmailModel.Project);
                        cmd.Parameters.AddWithValue("@EmailAddr", maintainEmailModel.EmailAddr);
                        cmd.Parameters.AddWithValue("@EmailType", maintainEmailModel.EmailType);
                       //cmd.Parameters.AddWithValue("@RegDttm", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@IsActive", maintainEmailModel.IsActive);
                        cmd.Connection = con;
                        con.Open();
                        maintainEmailModel.EmailAddr = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully " + maintainEmailModel.EmailAddr;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.Message.ToString();
                return View();
            }
        }

        // GET: MaintainEmail/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                //var model = new MaintainEmailModel();
                //model.listvwMail = GetProjectList();
                //model.listvwProject = GetProject();

                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                MaintainEmailModel objEmailModel = new MaintainEmailModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneEmailAddr";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@HedID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    // objEmailModel.Flag = Convert.ToInt32(sdr[2]);
                                    objEmailModel.HedID = Convert.ToInt32(sdr["HedID"]);
                                    objEmailModel.Project = Convert.ToString(sdr["Project"]);
                                    objEmailModel.EmailAddr = Convert.ToString(sdr["EmailAddr"]);
                                    objEmailModel.EmailType = Convert.ToString(sdr["EmailType"]);
                                    //objEmailModel.RegDttm = Convert.ToDateTime(sdr["RegDttm"]);
                                    objEmailModel.RegBy = Convert.ToString(sdr["RegBy"]);
                                    objEmailModel.IsActive = Convert.ToInt32(sdr["IsActive"]);
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
                if (objEmailModel.HedID != 0)
                {
                    return View(objEmailModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objEmailModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        // POST: MaintainEmail/Edit/5
        [HttpPost]
        public ActionResult Edit(MaintainEmailModel maintainEmailModel)
        {

            try
            {
                string query = "usp_DMLMaintEmail";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", 1);
                        cmd.Parameters.AddWithValue("@HedID", maintainEmailModel.HedID);
                        cmd.Parameters.AddWithValue("@Project", maintainEmailModel.Project);
                        cmd.Parameters.AddWithValue("@EmailAddr", maintainEmailModel.EmailAddr);
                        cmd.Parameters.AddWithValue("@EmailType", maintainEmailModel.EmailType);
                        //cmd.Parameters.AddWithValue("@RegDttm", maintainEmailModel.RegDttm);
                        cmd.Parameters.AddWithValue("@RegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@IsActive", maintainEmailModel.IsActive);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + maintainEmailModel.HedID;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordNotExist = ex.ToString();
                return View();
            }
        }

        // GET: MaintainEmail/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MaintainEmail/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
