using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Collections.Generic;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_MeetingsController : Controller
    {

        #region
        public IActionResult MeetingList(MOM_DepartmentModel dept,MOM_MeetingTypeModel memtype,MOM_MeetingVenueModel venue)
        {
            try
            {
                List<MOM_MeetingsModel> meetingList = new List<MOM_MeetingsModel>();

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Meetings_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_MeetingsModel meet = new MOM_MeetingsModel();
                    meet.MeetingID = Convert.ToInt32(Reader["MeetingID"]);
                    meet.MeetingDate = Convert.ToDateTime(Reader["MeetingDate"]);
                    meet.MeetingTypeName = Reader["MeetingTypeName"].ToString();
                    meet.DepartmentName = Reader["DepartmentName"].ToString();
                    meet.MeetingVenueName = Reader["MeetingVenueName"].ToString();
                    meet.MeetingDescription = Reader["MeetingDescription"].ToString();
                    meet.CancellationReason = Reader["CancellationReason"].ToString();
                    meet.IsCancelled = Convert.ToBoolean(Reader["IsCancelled"]);
                    if (Reader["CancellationDateTime"] != DBNull.Value)
                    {
                        meet.CancellationDateTime = Convert.ToDateTime(Reader["CancellationDateTime"]);
                    }
                    meet.CancellationReason = Reader["CancellationReason"].ToString();

                    meetingList.Add(meet);
                }

                Reader.Close();
                con.Close();

                return View(meetingList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region
        public IActionResult MeeetingAddEdit(MOM_MeetingsModel meet)
        {
            ViewBag.DepartmentList = DropdowmDept();
            ViewBag.MeetingTypeList = DropdownMT();
            ViewBag.MeetingVenueList = DropdownVenue();
            try
            {
                meet.Created = DateTime.UtcNow;
                meet.Modified = DateTime.UtcNow;

                if (!ModelState.IsValid)
                {
                    return View("MeetingAddEdit", meet);
                }

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                if (meet.MeetingID == 0)
                {
                    meet.Created = DateTime.UtcNow;
                    meet.Modified = DateTime.UtcNow;

                    cmd.CommandText = "PR_MOM_Meetings_Insert";
                    cmd.Parameters.AddWithValue("@MeetingID", meet.MeetingID);
                    cmd.Parameters.AddWithValue("@MeetingDate", meet.MeetingDate);
                    cmd.Parameters.AddWithValue("@DepartmentID", meet.DepartmentID);
                    cmd.Parameters.AddWithValue("@MeetingTypeID", meet.MeetingTypeID);
                    cmd.Parameters.AddWithValue("@MeetingVenueID", meet.MeetingVenueID);
                    cmd.Parameters.AddWithValue("@MeetingDescription", meet.MeetingDescription);
                    cmd.Parameters.AddWithValue("@DocumentPath", meet.DocumentPath);
                    cmd.Parameters.AddWithValue("@IsCancelled", meet.IsCancelled);
                    cmd.Parameters.AddWithValue("@CancellationDateTime", meet.CancellationDateTime);
                    cmd.Parameters.AddWithValue("@CancellationReason", meet.CancellationReason);
                    cmd.Parameters.AddWithValue("@Created", meet.Created);
                    cmd.Parameters.AddWithValue("@Modified", meet.Modified);
                }
                else
                {
                    meet.Modified = DateTime.UtcNow;
                    cmd.Parameters.AddWithValue("@MeetingID", meet.MeetingID);
                    cmd.Parameters.AddWithValue("@MeetingDate", meet.MeetingDate);
                    cmd.Parameters.AddWithValue("@DepartmentID", meet.DepartmentID);
                    cmd.Parameters.AddWithValue("@MeetingTypeID", meet.MeetingTypeID);
                    cmd.Parameters.AddWithValue("@MeetingVenueID", meet.MeetingVenueID);
                    cmd.Parameters.AddWithValue("@MeetingDescription", meet.MeetingDescription);
                    cmd.Parameters.AddWithValue("@DocumentPath", meet.DocumentPath);
                    cmd.Parameters.AddWithValue("@IsCancelled", meet.IsCancelled);
                    cmd.Parameters.AddWithValue("@CancellationDateTime", meet.CancellationDateTime);
                    cmd.Parameters.AddWithValue("@CancellationReason", meet.CancellationReason);
                    cmd.Parameters.AddWithValue("@Modified", meet.Modified);
                }
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("MeetingList");
            }
            catch (Exception ex)
            {
                return View("MeetingAddEdit", meet);
            }
        }
        #endregion

        #region
        public IActionResult DeleteMeeting(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Meetings_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Deleted Successfully.";
                return RedirectToAction("MeetingList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingList");
            }
        }
        #endregion

        #region
        public List<SelectListItem> DropdownMT()
        {
            List<SelectListItem> mtList = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");


            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_DT";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                mtList.Add(new SelectListItem(reader["MeetingTypeName"].ToString(), reader["MeetingTypeID"].ToString()));
            }

            reader.Close();
            con.Close();

            return mtList;
        }
        #endregion
        #region
        public List<SelectListItem> DropdownVenue()
        {
            List<SelectListItem> venueList = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");


            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_DV";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                venueList.Add(new SelectListItem(reader["MeetingVenueName"].ToString(), reader["MeetingVenueID"].ToString()));
            }

            reader.Close();
            con.Close();

            return venueList;
        }
        #endregion

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
                deptList.Add(new SelectListItem(reader["DepartmentName"].ToString(), reader["DepartmentID"].ToString()));
            }

            reader.Close();
            con.Close();

            return deptList;
        }
        #endregion

        #region
        public IActionResult QuickSchedule(MOM_MeetingsModel meet)
        {
            MeeetingAddEdit(meet);
            return View();
        }
        #endregion
    }
}
