using ICMS.App_Start;
using ICMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace ICMS.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(int page = 1)
        {
            //Defining the PageSize
            int PageSize = 5;
            //Creating the ViewModel's Object
            UserMstViewModel objUserMstViewModel = new UserMstViewModel();
            DataSet ds = new DataSet();
            //List of the Person
            List<UserMstModel> lstUserMstModel = new List<UserMstModel>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_getAllUserMaster", con);
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
                        UserMstModel objUserMstModel = new UserMstModel();
                        objUserMstModel.EmpNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpNo"]);
                        objUserMstModel.UsrRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["UsrRegDttm"]);
                        objUserMstModel.UsrRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["UsrRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["UsrRegBy"]);
                        objUserMstModel.UsrStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["UsrStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["UsrStatus"]);
                        lstUserMstModel.Add(objUserMstModel);
                    }
                    //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                    objUserMstViewModel.ListUserMst = lstUserMstModel;
                    objUserMstViewModel.pager = pager;
                }
            }
            return View(objUserMstViewModel);
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            return View("../Login/Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(UserMstModel userMstModel)
        {
            try
            {
                string query = "usp_DMLUserMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", 0);
                        cmd.Parameters.AddWithValue("@EmpNo", userMstModel.EmpNo);
                        cmd.Parameters.AddWithValue("@UsrRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@UsrStatus", userMstModel.UsrStatus);
                        cmd.Connection = con;
                        con.Open();
                        userMstModel.EmpNo = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + userMstModel.EmpNo;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.Message.ToString();
                return View();
            }
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                UserMstModel objUserMstModel = new UserMstModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneUserMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EmpNo", id.ToString());
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objUserMstModel.EmpNo = Convert.ToString(sdr["EmpNo"]);
                                    objUserMstModel.UsrRegBy = Convert.ToString(sdr["UsrRegBy"]);
                                    objUserMstModel.UsrRegDttm = Convert.ToDateTime(sdr["UsrRegDttm"]);
                                    objUserMstModel.UsrStatus = Convert.ToInt32(sdr["UsrStatus"]);
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
                if (objUserMstModel.EmpNo != "")
                {
                    return View(objUserMstModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objUserMstModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(UserMstModel userMstModel)
        {
            try
            {
                string query = "usp_DMLUserMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", 1);
                        cmd.Parameters.AddWithValue("@EmpNo", userMstModel.EmpNo);
                        cmd.Parameters.AddWithValue("@UsrRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@UsrStatus", userMstModel.UsrStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + userMstModel.EmpNo;
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
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                UserMstModel objUserMstModel = new UserMstModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneUserMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@EmpNo", id.ToString());
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objUserMstModel.EmpNo = Convert.ToString(sdr["EmpNo"]);
                                        objUserMstModel.UsrStatus = Convert.ToInt32(sdr["UsrStatus"]);
                                        objUserMstModel.UsrRegBy = Convert.ToString(sdr["UsrRegBy"]);
                                        objUserMstModel.UsrRegDttm = Convert.ToDateTime(sdr["UsrRegDttm"]);
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
                if (objUserMstModel.EmpNo == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objUserMstModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }

        public ActionResult CheckLogin(UserMstModel userMstModel)
        {
            try
            {
                UserMstViewModel objUserMstViewModel = new UserMstViewModel();
                string psNo = System.Web.HttpContext.Current.User.Identity.Name.Substring(6).ToString();
                DataSet ds = new DataSet();
                string RoleName = string.Empty;
                string EmpPsNo = string.Empty;
                string EmployeeEmail = string.Empty;
                string EmployeeDeparment = string.Empty;
                string EmpName = string.Empty;
                string department = string.Empty;
                //List of the Person
                List<UserMstModel> lstUserMstModel = new List<UserMstModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("usp_CheckLoginInformation", con);
                    com.CommandType = CommandType.StoredProcedure;
                    //Passing the Offset value in the procedure
                   //com.Parameters.AddWithValue("@UserName", psNo);
                    com.Parameters.AddWithValue("@UserName", userMstModel.EmpPsNo);
                    com.Parameters.AddWithValue("@password", userMstModel.Password);
                   //com.Parameters.AddWithValue("@Role", userMstModel.RoleName);
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
                            UserMstModel objUserMstModel = new UserMstModel();
                            objUserMstModel.EmpNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpID"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpID"]);
                            objUserMstModel.EmpName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            objUserMstModel.EmpPsNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpPsNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpPsNo"]);
                            objUserMstModel.EmployeeEmail = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpMail"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpMail"]);
                            objUserMstModel.EmployeeDeparment = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpDpcd"]);
                            objUserMstModel.RoleName = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RoDesc"]);
                            RoleName = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RoDesc"]);
                            EmpPsNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpPsNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpPsNo"]);
                            EmployeeEmail = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpMail"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpMail"]);
                            EmployeeDeparment = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpDpcd"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpDpcd"]);
                            EmpName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            //EmpName = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                            lstUserMstModel.Add(objUserMstModel);
                        }
                        //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                        objUserMstViewModel.ListUserMst = lstUserMstModel;
                    }
                }
                if (objUserMstViewModel.ListUserMst.Count == 0)
                {
                    TempData["LoginError"] = "Invalid Username or Password";
                }
                Session["Role"] = RoleName;
                Session["EmpPsNo"] = EmpPsNo;
                Session["EmployeeEmail"] = EmployeeEmail;
                Session["EmployeeDeparment"] = EmployeeDeparment;
                Session["EmpName"] = EmpName;
                //UsersRoleProvider usersRoleProvider = new UsersRoleProvider();
                //string[] roles = usersRoleProvider.GetRolesForUser(lstUserMstModel[0].EmpNo);
                //String[] newRoles = { "INT" };
                //GenericIdentity newIdentity = new GenericIdentity(EmpPsNo);
                //GenericPrincipal newPrincipal = new GenericPrincipal(newIdentity, newRoles);
                //Thread.CurrentPrincipal = newPrincipal;
                //System.Web.HttpContext.Current.User = newPrincipal;
                //String name = newPrincipal.Identity.Name;
                //bool auth = newPrincipal.Identity.IsAuthenticated;
                //bool isInRole = newPrincipal.IsInRole("INT");
                if (RoleName == "INT")
                {
                    return RedirectToAction("../Initiator/Index");
                }
                if (RoleName == "QCP")
                {
                    return RedirectToAction("../RequestPlanner/Index");
                }
                if (RoleName == "QCI")
                {
                    return RedirectToAction("../Inspector/Index");
                }
                if (RoleName == "HOD")
                {
                    return RedirectToAction("../HOD/Index");
                }
                return View("../Login/Index");
            }
            catch (Exception ex)
            {
                TempData["LoginError"] = ex.ToString();
                return View("../Login/Index");
            }
        }
    }
}