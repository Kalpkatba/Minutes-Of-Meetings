using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region Login Page (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region UserLogin (POST)
        [HttpPost]
        public IActionResult UserLogin(UserLoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("PR_User_Login", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserName", model.UserName);
                        cmd.Parameters.AddWithValue("@Password", model.Password);

                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            HttpContext.Session.SetString("UserID", dt.Rows[0]["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dt.Rows[0]["UserName"].ToString());

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Invalid Username or Password";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Login");
        }
        #endregion

        #region Register Page (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        #endregion

        #region UserRegister (POST)
        [HttpPost]
        public IActionResult UserRegister(UserRegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("PR_User_Register", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserName", model.UserName);
                        cmd.Parameters.AddWithValue("@Password", model.Password);
                        cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);

                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0 && dt.Columns.Contains("ErrorMessage"))
                        {
                            TempData["ErrorMessage"] = dt.Rows[0]["ErrorMessage"].ToString();
                            return RedirectToAction("Register");
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Registration successful! Please login.";
                            return RedirectToAction("Login");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Register");
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        #endregion
    }
}
