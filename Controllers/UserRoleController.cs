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
    public class UserRoleController : Controller
    {
        // GET: UserRole
        public ActionResult Index(int page = 1)
        {
            //Defining the PageSize
            int PageSize = 5;
            //Creating the ViewModel's Object
            UserRoleMasterViewModel objUserRoleMasterViewModel = new UserRoleMasterViewModel();
            DataSet ds = new DataSet();
            //List of the Person
            List<UserRoleMasterModel> lstUserRoleMasterModel = new List<UserRoleMasterModel>();


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("usp_getAllUserRoleMaster", con);
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
                        UserRoleMasterModel objUserRoleMasterModel = new UserRoleMasterModel();
                        objUserRoleMasterModel.EmpNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpNo"]);
                        objUserRoleMasterModel.RoID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoID"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RoID"]);
                        objUserRoleMasterModel.RoDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RoDesc"]);
                        objUserRoleMasterModel.UrRegDttm = Convert.ToDateTime(ds.Tables[0].Rows[i]["UrRegDttm"]);
                        objUserRoleMasterModel.UrRegBy = Convert.IsDBNull(ds.Tables[0].Rows[i]["UrRegBy"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["UrRegBy"]);
                        objUserRoleMasterModel.UrStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["UrStatus"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["UrStatus"]);
                        lstUserRoleMasterModel.Add(objUserRoleMasterModel);
                    }
                    //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRecords"]) : 0, page, PageSize);
                    objUserRoleMasterViewModel.ListUserRoleMst = lstUserRoleMasterModel;
                    objUserRoleMasterViewModel.pager = pager;
                }
            }
            return View(objUserRoleMasterViewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(GetRoleList().ToList(), "RoID", "RoDesc");
            ViewBag.User = new SelectList(GetUserList().ToList(), "EmpNo", "EmpNo");
            return View();
        }
        [HttpPost]
        public ActionResult Create(UserRoleMasterModel userRoleMasterModel)
        {
            try
            {
                if (userRoleMasterModel.RoID == null || userRoleMasterModel.EmpNo == null)
                {
                    ViewBag.RecordException = "Please select Role";
                    ViewBag.Role = new SelectList(GetRoleList().ToList(), "RoID", "RoDesc");
                    ViewBag.User = new SelectList(GetUserList().ToList(), "EmpNo", "EmpNo");
                    return View();
                }
                string query = "usp_DMLUserRoleMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", 0);
                        cmd.Parameters.AddWithValue("@EmpNo", userRoleMasterModel.EmpNo);
                        cmd.Parameters.AddWithValue("@RoleID", userRoleMasterModel.RoID);
                        cmd.Parameters.AddWithValue("@UrStatus", userRoleMasterModel.UrStatus);
                        cmd.Parameters.AddWithValue("@UrRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Connection = con;
                        con.Open();
                        userRoleMasterModel.EmpNo = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Created Successfully :" + userRoleMasterModel.EmpNo;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Role = new SelectList(GetRoleList().ToList(), "RoID", "RoDesc");
                ViewBag.User = new SelectList(GetUserList().ToList(), "EmpNo", "EmpNo");
                ViewBag.RecordException = ex.Message.ToString();
                return View();
            }
        }
        private List<RoleMsModel> GetRoleList()
        {
            try
            {
                List<RoleMsModel> lstRoleMsModel = new List<RoleMsModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    SqlCommand com = new SqlCommand("Select RoID,RoDesc from tblRoleMst", con);//where Status=1
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
                        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            RoleMsModel objRoleMsModel = new RoleMsModel();
                            objRoleMsModel.RoID = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoID"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["RoID"]);
                            objRoleMsModel.RoDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["RoDesc"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RoDesc"]);
                            lstRoleMsModel.Add(objRoleMsModel);
                        }
                    }
                    if (lstRoleMsModel.Count > 0)
                    {
                        return lstRoleMsModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private List<UserMstModel> GetUserList()
        {
            try
            {
                List<UserMstModel> lstUserMstModel = new List<UserMstModel>();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    //SqlCommand com = new SqlCommand("Select ID,PrjNo+'-'+PrjDesc ProjectInfo from tblProject", con);//where Status=1
                    SqlCommand com = new SqlCommand("Select EmpNo,UsrStatus from tblUserMst", con);//where Status=1
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
                        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            UserMstModel objUserMstModel = new UserMstModel();
                            objUserMstModel.EmpNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["EmpNo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["EmpNo"]);
                            lstUserMstModel.Add(objUserMstModel);
                        }
                    }
                    if (lstUserMstModel.Count > 0)
                    {
                        return lstUserMstModel;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult Edit(string id, string data)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                UserRoleMasterModel objUserRoleMasterModel = new UserRoleMasterModel();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string query = "usp_getOneUserRoleMaster";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EmpNo", id.ToString());
                            cmd.Parameters.AddWithValue("@RoleID", data.ToString());
                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    objUserRoleMasterModel.EmpNo = Convert.ToString(sdr["EmpNo"]);
                                    objUserRoleMasterModel.RoID = Convert.ToString(sdr["RoleID"]);
                                    objUserRoleMasterModel.UrRegBy = Convert.ToString(sdr["UrRegBy"]);
                                    objUserRoleMasterModel.UrRegDttm = Convert.ToDateTime(sdr["UrRegDttm"]);
                                    objUserRoleMasterModel.UrStatus = Convert.ToInt32(sdr["UrStatus"]);
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
                if (objUserRoleMasterModel.EmpNo != "")
                {
                    ViewBag.Role = new SelectList(GetRoleList().ToList(), "RoID", "RoDesc");
                    ViewBag.User = new SelectList(GetUserList().ToList(), "EmpNo", "EmpNo");
                    return View(objUserRoleMasterModel);
                }
                else
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                    return View(objUserRoleMasterModel);
                }
            }
            catch (Exception ex)
            {
                TempData["RecordException"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(UserRoleMasterModel userRoleMasterModel)
        {
            try
            {
                if (userRoleMasterModel.RoID == null || userRoleMasterModel.EmpNo == null)
                {
                    ViewBag.RecordException = "Please select Role";
                    ViewBag.Role = new SelectList(GetRoleList().ToList(), "RoID", "RoDesc");
                    ViewBag.User = new SelectList(GetUserList().ToList(), "EmpNo", "EmpNo");
                    return View();
                }
                string query = "usp_DMLUserRoleMaster";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", 1);
                        cmd.Parameters.AddWithValue("@EmpNo", userRoleMasterModel.EmpNo);
                        cmd.Parameters.AddWithValue("@RoleID", userRoleMasterModel.RoID);
                        cmd.Parameters.AddWithValue("@UrRegBy", Session["EmpPsNo"].ToString());
                        cmd.Parameters.AddWithValue("@RqDpcd", Session["EmployeeDeparment"].ToString());
                        cmd.Parameters.AddWithValue("@UrStatus", userRoleMasterModel.UrStatus);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["TransactionStatus"] = "Record Update Successfully :" + userRoleMasterModel.EmpNo;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Role = new SelectList(GetRoleList().ToList(), "RoID", "RoDesc");
                ViewBag.User = new SelectList(GetUserList().ToList(), "EmpNo", "EmpNo");
                ViewBag.RecordNotExist = ex.ToString();
                return View();
            }
        }
        [HttpGet]
        public ActionResult Details(string id, string data)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                UserRoleMasterModel objUserRoleMasterModel = new UserRoleMasterModel();
                if (ModelState.IsValid)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string query = "usp_getOneUserRoleMaster";
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@EmpNo", id.ToString());
                                cmd.Parameters.AddWithValue("@RoleID", data.ToString());
                                cmd.Connection = con;
                                con.Open();
                                using (SqlDataReader sdr = cmd.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        objUserRoleMasterModel.EmpNo = Convert.ToString(sdr["EmpNo"]);
                                        objUserRoleMasterModel.RoID = Convert.ToString(sdr["RoleID"]);
                                        objUserRoleMasterModel.RoDesc = Convert.ToString(sdr["RoDesc"]);
                                        objUserRoleMasterModel.UrRegBy = Convert.ToString(sdr["UrRegBy"]);
                                        objUserRoleMasterModel.UrRegDttm = Convert.ToDateTime(sdr["UrRegDttm"]);
                                        objUserRoleMasterModel.UrStatus = Convert.ToInt32(sdr["UrStatus"]);
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
                if (objUserRoleMasterModel.EmpNo == null)
                {
                    ViewBag.RecordNotExist = "Sorry Record is not avaliable into DB";
                }
                return View(objUserRoleMasterModel);
            }
            catch (Exception ex)
            {
                ViewBag.RecordException = ex.ToString();
                return View();
            }
        }
    }
}