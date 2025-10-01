using ICMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICMS.Content
{
    public class EmailTemplateController : Controller
    {
        // GET: EmailTemplate
        public ActionResult Index(int page = 1)
        {
            try
            {
                //Defining the PageSize
                int PageSize = 5;
                //Creating the ViewModel's Object
                EmailTempViewModel objEmailTempViewModel = new EmailTempViewModel();
                DataSet ds = new DataSet();
                //List of the Person
                List<EmailTempModel> lstEmailTempModel = new List<EmailTempModel>();


                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_getAllEmailTemplateMaster", con);
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
                            EmailTempModel objEmailTempModel = new EmailTempModel();
                            objEmailTempModel.EtempID = Convert.IsDBNull(ds.Tables[0].Rows[i]["EtempID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["EtempID"]);
                            objEmailTempModel.EtempType = Convert.IsDBNull(ds.Tables[0].Rows[i]["EtempType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EtempType"]);
                            objEmailTempModel.EtempSub = Convert.IsDBNull(ds.Tables[0].Rows[i]["EtempSub"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EtempSub"]);
                            objEmailTempModel.EmtpCont = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmtpCont"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmtpCont"]);
                            objEmailTempModel.EmtpTo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmtpTo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmtpTo"]);
                            objEmailTempModel.EmtpCc = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmtpCc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmtpCc"]);
                            objEmailTempModel.RegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["RegDttm"]);
                            objEmailTempModel.RegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["RegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RegBy"]);
                            objEmailTempModel.Status = Convert.IsDBNull(ds.Tables[0].Rows[i]["Status"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["Status"]);
                            lstEmailTempModel.Add(objEmailTempModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                        objEmailTempViewModel.ListEmailTemplate = lstEmailTempModel;
                        objEmailTempViewModel.pager = pager;
                    }
                }
                return View(objEmailTempViewModel);
            }
            catch (Exception ex)
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
        public ActionResult Create(EmailTempModel emailTempModel)
        {
            try
            {
                string query = "usp_DMLEmailTemplateMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EtempID", 0);
                        cmd.Parameters.AddWithValue("@EtempType", emailTempModel.EtempType);
                        cmd.Parameters.AddWithValue("@EtempSub", emailTempModel.EtempSub);
                        cmd.Parameters.AddWithValue("@EmtpCont", emailTempModel.EmtpCont);
                        cmd.Parameters.AddWithValue("@EmtpTo", emailTempModel.EmtpTo);
                        cmd.Parameters.AddWithValue("@EmtpCc", emailTempModel.EmtpCc);
                        cmd.Parameters.AddWithValue("@RegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@Status", emailTempModel.Status);
                        cmd.Connection = con;
                        con.Open();
                        emailTempModel.EtempID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + emailTempModel.EtempType;
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
                EmailTempModel objEmailTempModel = new EmailTempModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneEmailTemplateMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EtempID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objEmailTempModel.EtempID = Convert.ToInt32(sdr["EtempID"]);
                                    objEmailTempModel.EtempType = Convert.ToString(sdr["EtempType"]);
                                    objEmailTempModel.EtempSub = Convert.ToString(sdr["EtempSub"]);
                                    objEmailTempModel.EmtpCont = Convert.ToString(sdr["EmtpCont"]);
                                    objEmailTempModel.EmtpTo = Convert.ToString(sdr["EmtpTo"]);
                                    objEmailTempModel.EmtpCc = Convert.ToString(sdr["EmtpCc"]);
                                    objEmailTempModel.RegDttm = Convert.ToDateTime(sdr["RegDttm"]);
                                    objEmailTempModel.RegBy = Convert.ToString(sdr["RegBy"]);
                                    objEmailTempModel.Status = Convert.ToInt32(sdr["Status"]);
                                }
                            }
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            ViewBag.RecordException = ex.Message.ToString();
                            return RedirectToAction("Index");
                        }
                    }
                }
                if (objEmailTempModel.EtempID != 0)
                {
                    return View(objEmailTempModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objEmailTempModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(EmailTempModel emailTempModel)
        {
            try
            {
                string query = "usp_DMLEmailTemplateMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EtempID", emailTempModel.EtempID);
                        cmd.Parameters.AddWithValue("@EtempType", emailTempModel.EtempType);
                        cmd.Parameters.AddWithValue("@EtempSub", emailTempModel.EtempSub);
                        cmd.Parameters.AddWithValue("@EmtpCont", emailTempModel.EmtpCont);
                        cmd.Parameters.AddWithValue("@EmtpTo", emailTempModel.EmtpTo);
                        cmd.Parameters.AddWithValue("@EmtpCc", emailTempModel.EmtpCc);
                        cmd.Parameters.AddWithValue("@RegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@Status", emailTempModel.Status);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + emailTempModel.EtempID;
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
                EmailTempModel objEmailTempModel = new EmailTempModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneEmailTemplateMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@EtempID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objEmailTempModel.EtempID = Convert.ToInt32(sdr["EtempID"]);
                                        objEmailTempModel.EtempType = Convert.ToString(sdr["EtempType"]);
                                        objEmailTempModel.EtempSub = Convert.ToString(sdr["EtempSub"]);
                                        objEmailTempModel.EmtpCont = Convert.ToString(sdr["EmtpCont"]);
                                        objEmailTempModel.EmtpTo = Convert.ToString(sdr["EmtpTo"]);
                                        objEmailTempModel.EmtpCc = Convert.ToString(sdr["EmtpCc"]);
                                        objEmailTempModel.RegDttm = Convert.ToDateTime(sdr["RegDttm"]);
                                        objEmailTempModel.RegBy = Convert.ToString(sdr["RegBy"]);
                                        objEmailTempModel.Status = Convert.ToInt32(sdr["Status"]);
                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                ViewBag.RecordException = ex.Message.ToString();
                                return RedirectToAction("Index");
                            }

                        }
                    }
                }
                if (objEmailTempModel.EtempID == 0)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objEmailTempModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}