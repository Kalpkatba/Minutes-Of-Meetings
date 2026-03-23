using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_MeetingMemberController : Controller
    {
        private IConfiguration configuration;

        public MOM_MeetingMemberController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region MeetingMemberList
        public IActionResult MeetingMemberList(MOM_DepartmentModel dept,MOM_StaffModel staff)
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
                    member.Created = Convert.ToDateTime(Reader["Created"]);
                    member.Modified = Convert.ToDateTime(Reader["Modified"]);

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

        #region MeetingMemberAddEdit
        public IActionResult MeetingMemberAddEdit(int? id)
        {
            MOM_MeetingMemberModel model = new MOM_MeetingMemberModel();

            if (id != null) // EDIT MODE
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_MOM_MeetingMember_SelectByPK";
                command.Parameters.AddWithValue("@MeetingMemberID", id);

                SqlDataReader reader = command.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(reader);

                if (datatable.Rows.Count > 0)
                {
                    DataRow row = datatable.Rows[0];
                    model.MeetingMemberID = Convert.ToInt32(row["MeetingMemberID"]);
                    model.MeetingID = Convert.ToInt32(row["MeetingID"]);
                    model.StaffID = row["StaffID"]?.ToString();
                    model.MeetingDate = row["MeetingDate"] != DBNull.Value ? Convert.ToDateTime(row["MeetingDate"]) : (DateTime?)null;
                    model.StaffName = row["StaffName"]?.ToString();
                    model.DepartmentName = row["DepartmentName"]?.ToString();
                    model.IsPresent = row["IsPresent"] != DBNull.Value ? Convert.ToBoolean(row["IsPresent"]) : false;
                    model.Remarks = row["Remarks"].ToString();

                    model.Remarks = row["Remarks"]?.ToString();
                }
            }
            return View(model);

        }
        #endregion

        #region MeetingMemberSave
        public IActionResult MeetingMemberSave(MOM_MeetingMemberModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.MeetingMemberID == 0)
                {
                    command.CommandText = "PR_MOM_MeetingMember_Insert";
                }
                else
                {
                    command.CommandText = "PR_MOM_MeetingMember_UpdateByPK";
                    command.Parameters.Add("@MeetingMemberID", SqlDbType.Int).Value = model.MeetingMemberID;
                }
                command.Parameters.Add("@MeetingID", SqlDbType.Int).Value = model.MeetingID;
                command.Parameters.Add("@StaffID", SqlDbType.Int).Value = model.StaffID;
                command.Parameters.Add("@IsPresent", SqlDbType.Bit).Value = model.IsPresent;
                command.Parameters.Add("@StaffName", SqlDbType.VarChar).Value = model.StaffName;
                command.Parameters.Add("@DepartmentName", SqlDbType.VarChar).Value = model.DepartmentName;
                command.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = model.Remarks;
                command.Parameters.Add("@MeetingDate", SqlDbType.DateTime).Value = model.MeetingDate;

                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("MeetingMemberList");

            }
            return View("MeetingMemberAddEdit", model);
        }
        #endregion

        #region DeleteMeetingMember
        public IActionResult DeleteMeetingMember(int id)
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
                    command.CommandText = "PR_MOM_MeetingMember_DeleteByPK";
                    command.Parameters.Add("@MeetingMemberID", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                }


                TempData["Success"] = "Meeting Member Deleted Successfully.";
                return RedirectToAction("MeetingMemberList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingMemberList");
            }
        }
        #endregion
    }
}
