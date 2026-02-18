using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class LoginController : Controller
    {

        private IConfiguration configuration;

        #region
        public LoginController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region
        public IActionResult SignIn()
        {
            return View();
        }
        #endregion

        #region
        public IActionResult Register()
        {
            return View();
        }
        #endregion

        #region
        public IActionResult Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please provide valid login details.";
                    return RedirectToAction("SignIn");
                }

                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                //SqlConnection connectionString = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");


                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.CommandText = "PR_User_ValidateLogin";
                        sqlCommand.Parameters.Add("@username", SqlDbType.VarChar).Value = model.Username;
                        sqlCommand.Parameters.Add("@password", SqlDbType.VarChar).Value = model.Password;

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        using (DataTable dataTable = new DataTable())
                        {
                            dataTable.Load(sqlDataReader);

                            if (dataTable.Rows.Count > 0)
                            {
                                DataRow dr = dataTable.Rows[0];


                                HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                                HttpContext.Session.SetString("Username", dr["Username"].ToString());


                                return RedirectToAction("Index", "Home");

                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Invalid username or password.";
                                return RedirectToAction("SignIn");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred. Please try again later.";
            }
            return RedirectToAction("SignIn");
        }
        #endregion
    }
}
