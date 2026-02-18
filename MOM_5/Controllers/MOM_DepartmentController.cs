using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_DepartmentController : Controller
    {
        #region DepartmentList
        public IActionResult DepartmentList()
        {
            try
            {
                List<MOM_DepartmentModel> dept = new List<MOM_DepartmentModel>();

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_DEPARTMENT_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_DepartmentModel department = new MOM_DepartmentModel();
                    department.DepartmentID = Convert.ToInt32(Reader["DepartmentID"]);
                    department.DepartmentName = Reader["DepartmentName"].ToString();
                    department.Remarks = Reader["Remarks"].ToString();

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

        #region GetDepartmentById
        public MOM_DepartmentModel GetDepartmentById(int id)
        {
            MOM_DepartmentModel department = new MOM_DepartmentModel();

            SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectByPK";
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter p = new SqlParameter();
            p.ParameterName = "@DepartmentID";
            p.SqlDbType = SqlDbType.Int;
            p.Value = id;

            cmd.Parameters.Add(p);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                department.DepartmentID = Convert.ToInt32(reader["DepartmentId"]);
                department.DepartmentName = reader["DepartmentName"].ToString();
                department.Remarks = reader["Remarks"].ToString();
                department.Created = Convert.ToDateTime(reader["Created"]);
                department.Modified = Convert.ToDateTime(reader["Modified"]);
            }

            reader.Close();
            con.Close();

            return department;
        }
        #endregion

        #region DepartmentAddEdit
        public void DepartmentAddEdit(MOM_DepartmentModel dept)
        {
                bool isEditing = false;

            SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (dept.DepartmentID > 0)
                {
                    isEditing = true;
                    cmd.CommandText = "PR_MOM_Department_UpdateByPK";
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Department_Insert";
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter pName = new SqlParameter();
                pName.ParameterName = "@DepartmentName";
                pName.SqlDbType = SqlDbType.VarChar;
                pName.Value = dept.DepartmentName;

                SqlParameter remarks = new SqlParameter();
                remarks.ParameterName = "@Remarks";
                remarks.SqlDbType = SqlDbType.VarChar;
                remarks.Value = dept.Remarks;

                SqlParameter pId = new SqlParameter();
                pId.ParameterName = "@DepartmentID";
                pId.SqlDbType = SqlDbType.Int;
                pId.Value = dept.DepartmentID;

                cmd.Parameters.Add(pName);
                cmd.Parameters.Add(remarks);

                if (isEditing)
                {
                    cmd.Parameters.Add(pId);
                }

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
        }
        #endregion

        #region AddEdit Get- This method is called by both Add and Edit GET actions
        [HttpGet]
        public IActionResult AddEdit(int? id)
        {
            if (id > 0)
            {
                // Edit Mode
                MOM_DepartmentModel department = GetDepartmentById(id.Value);
                return View("DepartmentAddEdit",department);
            }
            else
            {
                // Add Mode
                return View("DepartmentAddEdit", new MOM_DepartmentModel());
            }
        }
        #endregion

        #region AddEdit Post- This method is called by both Add and Edit POST actions
        [HttpPost]
        public IActionResult AddEdit(MOM_DepartmentModel dept)
        {
            DepartmentAddEdit(dept);
            return RedirectToAction("DepartmentList");
        }
        #endregion

        #region DeleteDepartment
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Department_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@DepartmentID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Deleted Successfully.";
                return RedirectToAction("DepartmentList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("DepartmentList");
            }
        }
        #endregion
    }
}
