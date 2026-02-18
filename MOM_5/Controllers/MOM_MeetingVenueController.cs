using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_MeetingVenueController : Controller
    {
        #region
        public IActionResult MeetingVenueList()
        {
            try
            {
                List<MOM_MeetingVenueModel> meetingVenues = new List<MOM_MeetingVenueModel>();

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingVenue_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_MeetingVenueModel venue = new MOM_MeetingVenueModel();
                    venue.MeetingVenueID = Convert.ToInt32(Reader["MeetingVenueID"]);
                    venue.MeetingVenueName = Reader["MeetingVenueName"].ToString();
                    venue.Remarks = Reader["Remarks"].ToString();
                    venue.Created = Convert.ToDateTime(Reader["Created"]);
                    venue.Modified = Convert.ToDateTime(Reader["Modified"]);

                    meetingVenues.Add(venue);
                }

                Reader.Close();
                con.Close();


                return View(meetingVenues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region
        public IActionResult MeetingVenueAddEdit(MOM_MeetingVenueModel venue)
        {
            //ViewBag.DepartmentList = DropdowmDept();
            try
            {
                venue.Created = DateTime.UtcNow;
                venue.Modified = DateTime.UtcNow;

                if (!ModelState.IsValid)
                {
                    return View("MeetingVenueAddEdit", venue);
                }

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                if (venue.MeetingVenueID == 0)
                {
                    venue.Created = DateTime.UtcNow;
                    venue.Modified = DateTime.UtcNow;
                    SqlParameter newIdParam = new SqlParameter("@NewID", SqlDbType.Int);
                    newIdParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(newIdParam);

                    cmd.CommandText = "PR_MOM_MeetingVenue_Insert";
                    cmd.Parameters.AddWithValue("@MeetingVenueName", venue.MeetingVenueName);
                    cmd.Parameters.AddWithValue("@Remarks", venue.Remarks);
                }
                else
                {
                    venue.Modified = DateTime.UtcNow;
                    cmd.CommandText = "PR_MOM_MeetingVenue_UpdateByPK";
                    cmd.Parameters.AddWithValue("@MeetingVenueName", venue.MeetingVenueName);
                    cmd.Parameters.AddWithValue("@Remarks", venue.Remarks);
                    cmd.Parameters.AddWithValue("@Modified", venue.Modified);
                }
                con.Open();
                cmd.ExecuteNonQuery();  
                con.Close();

                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception ex)
            {
                return View("MeetingVenueAddEdit", venue);
            }
        }
        #endregion

        #region
        public IActionResult DeleteMeetingVenue(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingVenue_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingVenueID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Deleted Successfully.";
                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingVenueList");
            }
        }
        #endregion
    }
}
