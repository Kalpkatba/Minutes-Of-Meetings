using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Collections;
using System.Data;
using System.Reflection;

namespace MOM_5.Controllers
{
    public class MOM_DepartmentController : Controller
    {
        private IConfiguration configuration;

        public MOM_DepartmentController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region DepartmentFilter
        public IActionResult DepartmentFilter(IFormCollection fc)
        {
            List<MOM_DepartmentModel> dept = new List<MOM_DepartmentModel>();

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_MOM_Department_Search";
            command.Parameters.Add("@DepartmentName", SqlDbType.VarChar).Value = fc["DepartmentName"].ToString();

              SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())

            {
                MOM_DepartmentModel department = new MOM_DepartmentModel();
                department.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                department.DepartmentName = reader["DepartmentName"].ToString();
                department.Remarks = reader["Remarks"].ToString();
                department.Created = Convert.ToDateTime(reader["Created"]);
                department.Modified = Convert.ToDateTime(reader["Modified"]);

                dept.Add(department);
            }

            reader.Close();
            connection.Close();

            return View("DepartmentList", dept);
        }
        #endregion

        #region DepartmentList
        public IActionResult DepartmentList(string searchText)
        {
            try
            {
                List<MOM_DepartmentModel> dept = new List<MOM_DepartmentModel>();

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_DEPARTMENT_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                if (searchText != null)
                {
                    cmd.Parameters.AddWithValue("@SearchText", searchText);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);
                }

                con.Open();

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_DepartmentModel department = new MOM_DepartmentModel();
                    department.DepartmentID = Convert.ToInt32(Reader["DepartmentID"]);
                    department.DepartmentName = Reader["DepartmentName"].ToString();
                    department.Remarks = Reader["Remarks"].ToString();
                    department.Created = Convert.ToDateTime(Reader["Created"]);
                    department.Modified = Convert.ToDateTime(Reader["Modified"]);

                    dept.Add(department);
                }

                Reader.Close();
                con.Close();

                return View(dept);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DepartmentAddEdit
        public IActionResult DepartmentAddEdit(int? id)
        {
            MOM_DepartmentModel model = new MOM_DepartmentModel();

            if (id != null) // EDIT MODE
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_MOM_Department_SelectByPK";
                command.Parameters.AddWithValue("@DepartmentID", id);

                SqlDataReader reader = command.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(reader);

                if (datatable.Rows.Count > 0)
                {
                    DataRow row = datatable.Rows[0];
                    model.DepartmentID = Convert.ToInt32(row["DepartmentID"]);
                    model.DepartmentName = row["DepartmentName"]?.ToString();
                    model.Remarks = row["Remarks"]?.ToString();
                }
            }
            return View(model);

        }
        #endregion

        #region DepartmentSave
        public IActionResult DepartmentSave(MOM_DepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.DepartmentID == 0)
                {
                    command.CommandText = "PR_MOM_Department_Insert";
                }
                else
                {
                    command.CommandText = "PR_MOM_Department_UpdateByPK";
                    command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = model.DepartmentID;
                }
                command.Parameters.Add("@DepartmentName", SqlDbType.VarChar).Value = model.DepartmentName;
                command.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = model.Remarks;

                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("DepartmentList");

            }
            return View("DepartmentAddEdit", model);
        }
        #endregion

        #region DeleteDepartment
        public IActionResult DeleteDepartment(int id)
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
                    command.CommandText = "PR_MOM_Department_DeleteByPK";
                    command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                }


                TempData["Success"] = "Department Deleted Successfully.";
                return RedirectToAction("DepartmentList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("DepartmentList");
            }
        }
        #endregion

        //#region GetDepartmentById
        //public MOM_DepartmentModel GetDepartmentById(int id)
        //{
        //    MOM_DepartmentModel department = new MOM_DepartmentModel();

        //    SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = con;
        //    cmd.CommandText = "PR_MOM_Department_SelectByPK";
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    SqlParameter p = new SqlParameter();
        //    p.ParameterName = "@DepartmentID";
        //    p.SqlDbType = SqlDbType.Int;
        //    p.Value = id;

        //    cmd.Parameters.Add(p);

        //    con.Open();

        //    SqlDataReader reader = cmd.ExecuteReader();

        //    if (reader.Read())
        //    {
        //        department.DepartmentID = Convert.ToInt32(reader["DepartmentId"]);
        //        department.DepartmentName = reader["DepartmentName"].ToString();
        //        department.Remarks = reader["Remarks"].ToString();
        //        department.Created = Convert.ToDateTime(reader["Created"]);
        //        department.Modified = Convert.ToDateTime(reader["Modified"]);
        //    }

        //    reader.Close();
        //    con.Close();

        //    return department;
        //}
        //#endregion

        //#region DepartmentAddEdit
        //[HttpPost]
        //public void DepartmentAddEdit(MOM_DepartmentModel dept)
        //{
        //    bool isEditing = false;

        //    SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = con;

        //    if (dept.DepartmentID > 0)
        //    {
        //        isEditing = true;
        //        cmd.CommandText = "PR_MOM_Department_UpdateByPK";
        //    }
        //    else
        //    {
        //        cmd.CommandText = "PR_MOM_Department_Insert";
        //    }

        //    cmd.CommandType = CommandType.StoredProcedure;

        //    SqlParameter pName = new SqlParameter();
        //    pName.ParameterName = "@DepartmentName";
        //    pName.SqlDbType = SqlDbType.VarChar;
        //    pName.Value = dept.DepartmentName;

        //    SqlParameter remarks = new SqlParameter();
        //    remarks.ParameterName = "@Remarks";
        //    remarks.SqlDbType = SqlDbType.VarChar;
        //    remarks.Value = dept.Remarks;

        //    SqlParameter pId = new SqlParameter();
        //    pId.ParameterName = "@DepartmentID";
        //    pId.SqlDbType = SqlDbType.Int;
        //    pId.Value = dept.DepartmentID;

        //    cmd.Parameters.Add(pName);
        //    cmd.Parameters.Add(remarks);

        //    if (isEditing)
        //    {
        //        cmd.Parameters.Add(pId);
        //    }

        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //    con.Close();
        //}
        //#endregion

        //#region AddEdit Get- This method is called by both Add and Edit GET actions
        //[HttpGet]
        //public IActionResult DepartmentAddEdit(int? id)
        //{
        //    if (id > 0)
        //    {
        //        // Edit Mode
        //        MOM_DepartmentModel department = GetDepartmentById(id.Value);
        //        return View("DepartmentAddEdit", department);
        //    }
        //    else
        //    {
        //        // Add Mode
        //        return View("DepartmentAddEdit", new MOM_DepartmentModel());
        //    }
        //}
        //#endregion

        //#region AddEdit Post- This method is called by both Add and Edit POST actions
        //[HttpPost]
        //public IActionResult AddEdit(MOM_DepartmentModel dept)
        //{
        //    DepartmentAddEdit(dept);
        //    return RedirectToAction("DepartmentList");
        //}
        //#endregion


    }
}