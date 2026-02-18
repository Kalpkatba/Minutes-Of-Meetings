using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_StaffController : Controller
    {
        #region
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

        #region
        public IActionResult StaffList(MOM_DepartmentModel dept)
        {
            try
            {
                List<MOM_StaffModel> staffList = new List<MOM_StaffModel>();

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Staff_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_StaffModel staff = new MOM_StaffModel();
                    staff.StaffID = Convert.ToInt32(Reader["StaffID"]);
                    staff.StaffName = Reader["StaffName"].ToString();
                    staff.DepartmentName = Reader["DepartmentName"].ToString();
                    staff.Mobile = Reader["Mobile"].ToString();
                    staff.Email = Reader["Email"].ToString();
                    staff.Remarks = Reader["Remarks"].ToString();

                    staffList.Add(staff);
                }

                Reader.Close();
                con.Close();


                return View(staffList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region
        public IActionResult StaffAddEdit(MOM_StaffModel staff)
        {
            ViewBag.DepartmentList = DropdowmDept();
            try
            {
                staff.Created = DateTime.UtcNow;
                staff.Modified = DateTime.UtcNow;

                if (!ModelState.IsValid)
                {
                    return View("StaffAddEdit", staff);
                }

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;


                con.Open();

                if (staff.StaffID == 0)
                {
                    staff.Created = DateTime.UtcNow;
                    staff.Modified = DateTime.UtcNow;

                    cmd.CommandText = "PR_MOM_Staff_Insert";
                    cmd.Parameters.AddWithValue("@StaffID", staff.StaffID);
                    cmd.Parameters.AddWithValue("@DepartmentID", staff.DepartmentID);
                    cmd.Parameters.AddWithValue("@StaffName", staff.StaffName);
                    cmd.Parameters.AddWithValue("@Mobile", staff.Mobile);
                    cmd.Parameters.AddWithValue("@Email", staff.Email);
                    cmd.Parameters.AddWithValue("@Remarks", staff.Remarks);
                    cmd.Parameters.AddWithValue("@Created", staff.Created);
                    cmd.Parameters.AddWithValue("@Modified", staff.Modified);
                }
                else
                {
                    staff.Modified = DateTime.UtcNow;
                    cmd.CommandText = "PR_MOM_Staff_UpdateByPK";
                    cmd.Parameters.AddWithValue("@StaffID", staff.StaffID);
                    cmd.Parameters.AddWithValue("@DepartmentID", staff.DepartmentID);
                    cmd.Parameters.AddWithValue("@StaffName", staff.StaffName);
                    cmd.Parameters.AddWithValue("@Mobile", staff.Mobile);
                    cmd.Parameters.AddWithValue("@Email", staff.Email);
                    cmd.Parameters.AddWithValue("@Remarks", staff.Remarks);
                    cmd.Parameters.AddWithValue("@Modified", staff.Modified);
                }
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("StaffList");
            }
            catch (Exception ex)
            {
                return View("StaffAddEdit", staff);
            }
        }
        #endregion

        #region
        public IActionResult DeleteStaff(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Staff_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@StaffID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Deleted Successfully.";
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
