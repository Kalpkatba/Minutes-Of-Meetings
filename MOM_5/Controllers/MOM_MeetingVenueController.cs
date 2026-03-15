using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_MeetingVenueController : Controller
    {
        private IConfiguration configuration;

        public MOM_MeetingVenueController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region MeetingVenueList
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

        #region MeetingVenueAddEdit
        public IActionResult MeetingVenueAddEdit(int? id)
        {
            MOM_MeetingVenueModel model = new MOM_MeetingVenueModel();

            if (id != null) // EDIT MODE
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_MOM_MeetingVenue_SelectByPK";
                command.Parameters.AddWithValue("@MeetingVenueID", id);

                SqlDataReader reader = command.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(reader);

                if (datatable.Rows.Count > 0)
                {
                    DataRow row = datatable.Rows[0];
                    model.MeetingVenueID = Convert.ToInt32(row["MeetingVenueID"]);
                    model.MeetingVenueName = row["MeetingVenueName"]?.ToString();
                    model.Remarks = row["Remarks"]?.ToString();
                }
            }
            return View(model);

        }
        #endregion

        #region MeeitngVenueSave
        public IActionResult MeeitngVenueSave(MOM_MeetingVenueModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.MeetingVenueID == 0)
                {
                    command.CommandText = "PR_MOM_MeetingVenue_Insert";
                }
                else
                {
                    command.CommandText = "PR_MOM_MeetingVenue_UpdateByPK";
                    command.Parameters.Add("@MeetingVenueID", SqlDbType.Int).Value = model.MeetingVenueID;
                }
                command.Parameters.Add("@MeetingVenueName", SqlDbType.VarChar).Value = model.MeetingVenueName;
                command.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = model.Remarks;

                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("MeetingVenueList");

            }
            return View("MeetingVenueAddEdit", model);
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
                    command.CommandText = "PR_MOM_MeetingVenue_DeleteByPK";
                    command.Parameters.Add("@MeetingVenueID", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                }


                TempData["Success"] = "Meeting Venue Deleted Successfully.";
                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingVenueList");
            }
        }
        #endregion

        //#region
        //public IActionResult MeetingVenueAddEdit(MOM_MeetingVenueModel venue)
        //{
        //    //ViewBag.DepartmentList = DropdowmDept();
        //    try
        //    {
        //        venue.Created = DateTime.UtcNow;
        //        venue.Modified = DateTime.UtcNow;

        //        if (!ModelState.IsValid)
        //        {
        //            return View("MeetingVenueAddEdit", venue);
        //        }

        //        SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        if (venue.MeetingVenueID == 0)
        //        {
        //            venue.Created = DateTime.UtcNow;
        //            venue.Modified = DateTime.UtcNow;
        //            SqlParameter newIdParam = new SqlParameter("@NewID", SqlDbType.Int);
        //            newIdParam.Direction = ParameterDirection.Output;
        //            cmd.Parameters.Add(newIdParam);

        //            cmd.CommandText = "PR_MOM_MeetingVenue_Insert";
        //            cmd.Parameters.AddWithValue("@MeetingVenueName", venue.MeetingVenueName);
        //            cmd.Parameters.AddWithValue("@Remarks", venue.Remarks);
        //        }
        //        else
        //        {
        //            venue.Modified = DateTime.UtcNow;
        //            cmd.CommandText = "PR_MOM_MeetingVenue_UpdateByPK";
        //            cmd.Parameters.AddWithValue("@MeetingVenueName", venue.MeetingVenueName);
        //            cmd.Parameters.AddWithValue("@Remarks", venue.Remarks);
        //            cmd.Parameters.AddWithValue("@Modified", venue.Modified);
        //        }
        //        con.Open();
        //        cmd.ExecuteNonQuery();  
        //        con.Close();

        //        return RedirectToAction("MeetingVenueList");
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("MeetingVenueAddEdit", venue);
        //    }
        //}
        //#endregion

        //#region
        //public IActionResult DeleteMeetingVenue(int id)
        //{
        //    try
        //    {
        //        SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;
        //        cmd.CommandText = "PR_MOM_MeetingVenue_DeleteByPK";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter p = new SqlParameter();
        //        p.ParameterName = "@MeetingVenueID";
        //        p.SqlDbType = SqlDbType.Int;
        //        p.Value = id;

        //        cmd.Parameters.Add(p);

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();

        //        TempData["Success"] = "Deleted Successfully.";
        //        return RedirectToAction("MeetingVenueList");
        //    }
        //    catch (Exception)
        //    {
        //        TempData["Error"] = "Foreign Key Constraint Violated.";
        //        return RedirectToAction("MeetingVenueList");
        //    }
        //}
        //#endregion
    }
}
