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
    public class TpiController : Controller
    {
        // GET: Tpi
        public ActionResult Index(int page = 1)
        {
            //Defining the PageSize
            int PageSize = 5;
            //Creating the ViewModel's Object
            TpiMstViewModel objTpiMstViewModel = new TpiMstViewModel();
            DataSet ds = new DataSet();
            //List of the Person
            List<TpiMstModel> lstTpiMstModel = new List<TpiMstModel>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_getAllTpiMaster", con);
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
                        TpiMstModel objTpiMstModel = new TpiMstModel();
                        objTpiMstModel.TpID = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["TpID"]);
                        objTpiMstModel.AgName = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgName"]);
                        objTpiMstModel.TpName = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["TpName"]);
                        objTpiMstModel.TpMob = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpMob"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["TpMob"]);
                        objTpiMstModel.TpMail = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpMail"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["TpMail"]);
                        objTpiMstModel.TpPrm = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpPrm"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["TpPrm"]);
                        objTpiMstModel.TpRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["TpRegDttm"]);
                        objTpiMstModel.TpRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["TpRegBy"]);
                        objTpiMstModel.TpStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["TpStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["TpStatus"]);
                        lstTpiMstModel.Add(objTpiMstModel);
                    }
                    //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                    objTpiMstViewModel.ListTpiMst = lstTpiMstModel;
                    objTpiMstViewModel.pager = pager;
                }
            }
            return View(objTpiMstViewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
            return View();
        }

        private List<AgencyMstModel> GetAgencyMstModels()
        {
            try
            {
                List<AgencyMstModel> lstAgencyMstModel = new List<AgencyMstModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlCommand com = new SqlCommand("Select AgID,AgName from tblAgencyMst", con);//where Status=1
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
                            AgencyMstModel objAgencyMstModel = new AgencyMstModel();
                            objAgencyMstModel.AgID= Convert.IsDBNull(ds.Tables[0].Rows[i]["AgID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["AgID"]);
                            objAgencyMstModel.AgName = Convert.IsDBNull(ds.Tables[0].Rows[i]["AgName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["AgName"]);
                            lstAgencyMstModel.Add(objAgencyMstModel);
                        }
                    }
                    if (lstAgencyMstModel.Count > 0)
                    {
                        return lstAgencyMstModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [HttpPost]
        public ActionResult Create(TpiMstModel tpiMstModel)
        {
            try
            {
                if (tpiMstModel.AgID == 0)
                {
                    ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
                    ViewBag.RecordException = "Please select agency";
                    return View();
                }
                
                string query = "usp_DMLTpiMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TpID", 0);
                        cmd.Parameters.AddWithValue("@AgID", tpiMstModel.AgID);
                        cmd.Parameters.AddWithValue("@TpName", tpiMstModel.TpName);
                        cmd.Parameters.AddWithValue("@TpMob", tpiMstModel.TpMob);
                        cmd.Parameters.AddWithValue("@TpMail", tpiMstModel.TpMail);
                        cmd.Parameters.AddWithValue("@TpPrm", tpiMstModel.TpPrm);
                        cmd.Parameters.AddWithValue("@TpRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@TpStatus", tpiMstModel.TpStatus);
                        cmd.Connection = con;
                        con.Open();
                        tpiMstModel.TpID = Convert.ToInt32(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + tpiMstModel.TpName;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
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
                TpiMstModel objTpiMstModel = new TpiMstModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneTpiMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TpID", id);
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {

                                while (sdr.Read())
                                {
                                    objTpiMstModel.TpID = Convert.ToInt32(sdr["TpID"]);
                                    objTpiMstModel.AgID = Convert.ToInt32(sdr["AgID"]);
                                    objTpiMstModel.AgName = Convert.ToString(sdr["AgName"]);
                                    objTpiMstModel.TpName = Convert.ToString(sdr["TpName"]);
                                    objTpiMstModel.TpMob = Convert.ToString(sdr["TpMob"]);
                                    objTpiMstModel.TpMail = Convert.ToString(sdr["TpMail"]);
                                    objTpiMstModel.TpPrm = Convert.ToInt32(sdr["TpPrm"]);
                                    objTpiMstModel.TpRegDttm = Convert.ToDateTime(sdr["TpRegDttm"]);
                                    objTpiMstModel.TpRegBy = Convert.ToString(sdr["TpRegBy"]);
                                    objTpiMstModel.TpStatus = Convert.ToInt32(sdr["TpStatus"]);
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
                if (objTpiMstModel.AgID != 0)
                {
                    ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
                    return View(objTpiMstModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objTpiMstModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(TpiMstModel tpiMstModel)
        {
            try
            {
                if (tpiMstModel.AgID == 0)
                {
                    ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
                    ViewBag.RecordException = "Please select agency";
                    return View();
                }
                if (tpiMstModel.TpPrm == 0)
                {
                    ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
                    ViewBag.RecordException = "Please select TpPrm";
                    return View();
                }
                string query = "usp_DMLTpiMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TpID", tpiMstModel.TpID);
                        cmd.Parameters.AddWithValue("@AgID", tpiMstModel.AgID);
                        cmd.Parameters.AddWithValue("@TpName", tpiMstModel.TpName);
                        cmd.Parameters.AddWithValue("@TpMob", tpiMstModel.TpMob);
                        cmd.Parameters.AddWithValue("@TpMail", tpiMstModel.TpMail);
                        cmd.Parameters.AddWithValue("@TpPrm", tpiMstModel.TpPrm);
                        cmd.Parameters.AddWithValue("@TpRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@TpStatus", tpiMstModel.TpStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + tpiMstModel.TpID;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Agency = new SelectList(GetAgencyMstModels().ToList(), "AgID", "AgName");
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
                TpiMstModel objTpiMstModel = new TpiMstModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneTpiMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TpID", id);
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objTpiMstModel.TpID = Convert.ToInt32(sdr["TpID"]);
                                        objTpiMstModel.AgID = Convert.ToInt32(sdr["AgID"]);
                                        objTpiMstModel.AgName = Convert.ToString(sdr["AgName"]);
                                        objTpiMstModel.TpName = Convert.ToString(sdr["TpName"]);
                                        objTpiMstModel.TpMob = Convert.ToString(sdr["TpMob"]);
                                        objTpiMstModel.TpMail = Convert.ToString(sdr["TpMail"]);
                                        objTpiMstModel.TpPrm = Convert.ToInt32(sdr["TpPrm"]);
                                        objTpiMstModel.TpRegDttm = Convert.ToDateTime(sdr["TpRegDttm"]);
                                        objTpiMstModel.TpRegBy = Convert.ToString(sdr["TpRegBy"]);
                                        objTpiMstModel.TpStatus = Convert.ToInt32(sdr["TpStatus"]);
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
                if (objTpiMstModel.TpID == 0)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objTpiMstModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}