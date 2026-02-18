using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_MeetingMemberController : Controller
    {
        #region MeetingMemberList
        public IActionResult MeetingMemberList(MOM_DepartmentModel dept)
        {
            try
            { 
                List<MOM_MeetingMemberModel> mem = new List<MOM_MeetingMemberModel>();


                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingMember_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_MeetingMemberModel member = new MOM_MeetingMemberModel();
                    member.MeetingMemberID = Convert.ToInt32(Reader["MeetingMemberID"]);
                    member.MeetingID = Convert.ToInt32(Reader["MeetingID"]);
                    member.StaffID = Reader["StaffID"].ToString();
                    member.MeetingDate = Convert.ToDateTime(Reader["MeetingDate"]);
                    member.StaffName = Reader["StaffName"].ToString();
                    member.DepartmentName = Reader["DepartmentName"].ToString();
                    member.IsPresent = Convert.ToBoolean(Reader["IsPresent"]);
                    member.Remarks = Reader["Remarks"].ToString();

                    mem.Add(member);
                }

                Reader.Close();
                con.Close();


                return View(mem);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetMemberById
        public MOM_MeetingMemberModel GetMemberById(int id)
        {
            MOM_MeetingMemberModel member = new MOM_MeetingMemberModel();

            SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectByPK";
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter p = new SqlParameter();
            p.ParameterName = "@MeetingMemberID";
            p.SqlDbType = SqlDbType.Int;
            p.Value = id;

            cmd.Parameters.Add(p);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                member.MeetingMemberID = Convert.ToInt32(reader["DepartmentId"]);
                member.DepartmentName = reader["DepartmentName"].ToString();
                member.Remarks = reader["Remarks"].ToString();
                member.Created = Convert.ToDateTime(reader["Created"]);
                member.Modified = Convert.ToDateTime(reader["Modified"]);
            }

            reader.Close();
            con.Close();

            return member;
        }
        #endregion

        #region DepartmentAddEdit
        public void MeetingMemberAddEdit(MOM_MeetingMemberModel member)
        {
            bool isEditing = false;

            SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (member.MeetingMemberID > 0)
            {
                isEditing = true;
                cmd.CommandText = "PR_MOM_MeetingMember_UpdateByPK";
            }
            else
            {
                cmd.CommandText = "PR_MOM_MeetingMember_Insert";
            }

            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter remarks = new SqlParameter();
            remarks.ParameterName = "@Remarks";
            remarks.SqlDbType = SqlDbType.VarChar;
            remarks.Value = member.Remarks;

            SqlParameter mmId = new SqlParameter();
            mmId.ParameterName = "@MeetingMemberID";
            mmId.SqlDbType = SqlDbType.Int;
            mmId.Value = member.MeetingMemberID;

            SqlParameter isPresent = new SqlParameter();
            isPresent.ParameterName = "@IsPresent";
            isPresent.SqlDbType = SqlDbType.Bit;

            cmd.Parameters.Add(isPresent);
            cmd.Parameters.Add(remarks);

            if (isEditing)
            {
                cmd.Parameters.Add(mmId);
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
                MOM_MeetingMemberModel member = GetMemberById(id.Value);
                return View("MeetingMemberAddEdit", member);
            }
            else
            {
                // Add Mode
                return View("MeetingMemberAddEdit", new MOM_MeetingMemberModel());
            }
        }
        #endregion

        #region AddEdit Post- This method is called by both Add and Edit POST actions
        [HttpPost]
        public IActionResult AddEdit(MOM_MeetingMemberModel member)
        {
            MeetingMemberAddEdit(member);
            return RedirectToAction("MeetingMemberList");
        }
        #endregion
        #region
        public IActionResult DeleteMeetingMember(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingMember_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingMemberID";
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
