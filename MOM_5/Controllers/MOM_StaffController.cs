using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_StaffController : Controller
    {
        private IConfiguration configuration;

        public MOM_StaffController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region StaffFilter
        public IActionResult StaffFilter(IFormCollection fc)
        {
            List<MOM_StaffModel> staffList = new List<MOM_StaffModel>();

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_MOM_Staff_Search";
            command.Parameters.Add("@StaffName", SqlDbType.VarChar).Value = fc["StaffName"].ToString();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())

            {
                MOM_StaffModel staff = new MOM_StaffModel();
                staff.StaffID = Convert.ToInt32(reader["StaffID"]);
                staff.StaffName = reader["StaffName"].ToString();
                staff.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                staff.Mobile = reader["Mobile"].ToString();
                staff.Email = reader["Email"].ToString();
                staff.Remarks = reader["Remarks"].ToString();
                staff.Created = Convert.ToDateTime(reader["Created"]);
                staff.Modified = Convert.ToDateTime(reader["Modified"]);


                staffList.Add(staff);
            }

            reader.Close();
            connection.Close();

            return View("StaffList", staffList);
        }
        #endregion

        #region DropdowmDept
        public List<SelectListItem> DropdowmDept()
        {
            List<SelectListItem> deptList = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_DD";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                deptList.Add(new SelectListItem(reader[ "DepartmentName"].ToString(), reader["DepartmentID"].ToString()));
            }

            reader.Close();
            con.Close();

            return deptList;
        }
        #endregion

        #region StaffList
        public IActionResult StaffList(MOM_DepartmentModel dept)
        {
            try
            {
                List<MOM_StaffModel> staffList = new List<MOM_StaffModel>();

                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "PR_MOM_Staff_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_StaffModel staff = new MOM_StaffModel();
                    staff.StaffID = Convert.ToInt32(Reader["StaffID"]);
                    staff.StaffName = Reader["StaffName"].ToString();
                    staff.DepartmentName = Reader["DepartmentName"]?.ToString();
                    staff.Mobile = Reader["Mobile"].ToString();
                    staff.Email = Reader["Email"].ToString();
                    staff.Remarks = Reader["Remarks"]?.ToString();
                    staff.Created = Convert.ToDateTime(Reader["Created"]);
                    staff.Modified = Convert.ToDateTime(Reader["Modified"]);

                    staffList.Add(staff);
                }

                Reader.Close();
                connection.Close();


                return View(staffList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion StaffAddEdit

        #region StaffAddEdit
        public IActionResult StaffAddEdit(int? id)
        {
            MOM_StaffModel model = new MOM_StaffModel();
            ViewBag.DepartmentList = DropdowmDept();

            if (id != null) // EDIT MODE
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_MOM_Staff_SelectByPK";
                command.Parameters.AddWithValue("@StaffID", id);

                SqlDataReader reader = command.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(reader);

                if (datatable.Rows.Count > 0)
                {
                    DataRow row = datatable.Rows[0];
                    model.StaffID = Convert.ToInt32(row["StaffID"]);
                    model.StaffName = row["StaffName"].ToString();
                    model.DepartmentID = Convert.ToInt32(row["DepartmentID"]);
                    model.Email = row["Email"].ToString();
                    model.Mobile = row["Mobile"].ToString();
                    model.Remarks = row["Remarks"].ToString();
                    model.Created = Convert.ToDateTime(row["Created"]);
                    model.Modified = Convert.ToDateTime(row["Modified"]);
                }
            }
            return View(model);

        }
        #endregion

        #region StaffSave
        public IActionResult StaffSave(MOM_StaffModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (model.StaffID == 0)
                {
                    command.CommandText = "PR_MOM_Staff_Insert";
                }
                else
                {
                    command.CommandText = "PR_MOM_Staff_UpdateByPK";
                    command.Parameters.Add("@StaffID", SqlDbType.Int).Value = model.StaffID;
                }
                command.Parameters.Add("@StaffName", SqlDbType.VarChar).Value = model.StaffName;
                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = model.DepartmentID;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = model.Email;
                command.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = model.Mobile;
                command.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = model.Remarks;

                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("StaffList");

            }
            return View("StaffAddEdit", model);
        }
        #endregion

        #region DeleteStaff
        public IActionResult DeleteStaff(int id)
        {
            //Console.WriteLine(id + " is the id to delete");
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //Console.WriteLine("APP DB = " + connection.Database);
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_MOM_Staff_DeleteByPK";
                    command.Parameters.Add("@StaffID", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                }


                TempData["Success"] = "Staff Member Deleted Successfully.";
                return RedirectToAction("StaffList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("StaffList");
            }
        }
        #endregion
    }
}
