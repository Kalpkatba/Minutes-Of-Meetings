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
        private IConfiguration configuration;

        public MOM_MeetingsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }


        #region MeetingList
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
                    meet.Created = Convert.ToDateTime(Reader["Created"]);
                    meet.Modified = Convert.ToDateTime(Reader["Modified"]);

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

        #region MeetingAddEdit
        public IActionResult MeetingAddEdit(int? id)
        {
            MOM_MeetingsModel model = new MOM_MeetingsModel();
            ViewBag.DepartmentList = DropdowmDept();
            ViewBag.MeetingTypeList = DropdownMT();
            ViewBag.MeetingVenueList = DropdownVenue();

            if (id != null) // EDIT MODE
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_MOM_Meetings_SelectByPK";
                command.Parameters.AddWithValue("@MeetingID", id);

                SqlDataReader reader = command.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(reader);

                if (datatable.Rows.Count > 0)
                {
                    DataRow row = datatable.Rows[0];
                    model.MeetingID = Convert.ToInt32(row["MeetingID"]);
                    model.MeetingDate = Convert.ToDateTime(row["MeetingDate"]);
                    model.MeetingTypeID = row["MeetingTypeID"] != DBNull.Value ? Convert.ToInt32(row["MeetingTypeID"]) : 0;
                    // get name from DropdownMT result
                    var mt = DropdownMT().FirstOrDefault(i => i.Value == model.MeetingTypeID.ToString());
                    model.MeetingTypeName = mt?.Text;
                    model.DepartmentID = row["DepartmentID"] != DBNull.Value ? Convert.ToInt32(row["DepartmentID"]) : 0;
                    // get name from DropdowmDept result
                    var dept = DropdowmDept().FirstOrDefault(i => i.Value == model.DepartmentID.ToString());
                    model.DepartmentName = dept?.Text;
                    model.MeetingVenueID = row["MeetingVenueID"] != DBNull.Value ? Convert.ToInt32(row["MeetingVenueID"]) : 0;
                    // get name from DropdownVenue result
                    var venue = DropdownVenue().FirstOrDefault(i => i.Value == model.MeetingVenueID.ToString());
                    model.MeetingVenueName = venue?.Text;
                    model.MeetingDescription = row["MeetingDescription"]?.ToString();
                    model.IsCancelled = Convert.ToBoolean(row["IsCancelled"]);
                    model.CancellationReason = row["CancellationReason"]?.ToString();
                    model.DocumentPath = row["DocumentPath"]?.ToString();
                    model.CancellationDateTime = row["CancellationDateTime"] != DBNull.Value ? Convert.ToDateTime(row["CancellationDateTime"]) : DateTime.MinValue;
                }
            }
            return View(model);

        }
        #endregion

        #region MeeitngSave
        public IActionResult MeeitngSave(MOM_MeetingsModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.MeetingID == 0)
                {
                    command.CommandText = "PR_MOM_Meetings_Insert";
                }
                else
                {
                    command.CommandText = "PR_MOM_Meetings_UpdateByPK";
                    command.Parameters.Add("@MeetingID", SqlDbType.Int).Value = model.MeetingID;
                }
                // Auto-set cancellation fields based on IsCancelled toggle
                if (model.IsCancelled)
                {
                    if (!model.CancellationDateTime.HasValue)
                        model.CancellationDateTime = DateTime.Now;
                }
                else
                {
                    model.CancellationDateTime = null;
                    model.CancellationReason = null;
                }

                command.Parameters.Add("@MeetingDate", SqlDbType.DateTime).Value = model.MeetingDate;
                command.Parameters.Add("@MeetingTypeID", SqlDbType.Int).Value = model.MeetingTypeID;
                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = model.DepartmentID;
                command.Parameters.Add("@MeetingVenueID", SqlDbType.Int).Value = model.MeetingVenueID;
                command.Parameters.Add("@MeetingDescription", SqlDbType.NVarChar).Value = model.MeetingDescription ?? "";
                command.Parameters.Add("@DocumentPath", SqlDbType.NVarChar).Value = model.DocumentPath ?? "";
                command.Parameters.Add("@IsCancelled", SqlDbType.Bit).Value = model.IsCancelled;
                command.Parameters.Add("@CancellationDateTime", SqlDbType.DateTime).Value =
                    model.CancellationDateTime.HasValue ? (object)model.CancellationDateTime.Value : DBNull.Value;
                command.Parameters.Add("@CancellationReason", SqlDbType.NVarChar).Value =
                    (object?)model.CancellationReason ?? DBNull.Value;

                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("MeetingList");
            }

            // Re-populate dropdowns if model is invalid
            ViewBag.DepartmentList = DropdowmDept();
            ViewBag.MeetingTypeList = DropdownMT();
            ViewBag.MeetingVenueList = DropdownVenue();
            return View("MeetingAddEdit", model);
        }

        #endregion

        #region DeleteMeeting
        public IActionResult DeleteMeeting(int id)
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
                    command.CommandText = "PR_MOM_Meetings_DeleteByPK";
                    command.Parameters.Add("@MeetingID", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                }

                TempData["Success"] = "Meeting Venue Deleted Successfully.";
                return RedirectToAction("MeetingList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingList");
            }
        }
        #endregion

        //#region
        //public IActionResult MeeetingAddEdit(MOM_MeetingsModel meet)
        //{
        //    ViewBag.DepartmentList = DropdowmDept();
        //    ViewBag.MeetingTypeList = DropdownMT();
        //    ViewBag.MeetingVenueList = DropdownVenue();
        //    try
        //    {
        //        meet.Created = DateTime.UtcNow;
        //        meet.Modified = DateTime.UtcNow;

        //        if (!ModelState.IsValid)
        //        {
        //            return View("MeetingAddEdit", meet);
        //        }

        //        SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        con.Open();

        //        if (meet.MeetingID == 0)
        //        {
        //            meet.Created = DateTime.UtcNow;
        //            meet.Modified = DateTime.UtcNow;

        //            cmd.CommandText = "PR_MOM_Meetings_Insert";
        //            cmd.Parameters.AddWithValue("@MeetingID", meet.MeetingID);
        //            cmd.Parameters.AddWithValue("@MeetingDate", meet.MeetingDate);
        //            cmd.Parameters.AddWithValue("@DepartmentID", meet.DepartmentID);
        //            cmd.Parameters.AddWithValue("@MeetingTypeID", meet.MeetingTypeID);
        //            cmd.Parameters.AddWithValue("@MeetingVenueID", meet.MeetingVenueID);
        //            cmd.Parameters.AddWithValue("@MeetingDescription", meet.MeetingDescription);
        //            cmd.Parameters.AddWithValue("@DocumentPath", meet.DocumentPath);
        //            cmd.Parameters.AddWithValue("@IsCancelled", meet.IsCancelled);
        //            cmd.Parameters.AddWithValue("@CancellationDateTime", meet.CancellationDateTime);
        //            cmd.Parameters.AddWithValue("@CancellationReason", meet.CancellationReason);
        //            cmd.Parameters.AddWithValue("@Created", meet.Created);
        //            cmd.Parameters.AddWithValue("@Modified", meet.Modified);
        //        }
        //        else
        //        {
        //            meet.Modified = DateTime.UtcNow;
        //            cmd.Parameters.AddWithValue("@MeetingID", meet.MeetingID);
        //            cmd.Parameters.AddWithValue("@MeetingDate", meet.MeetingDate);
        //            cmd.Parameters.AddWithValue("@DepartmentID", meet.DepartmentID);
        //            cmd.Parameters.AddWithValue("@MeetingTypeID", meet.MeetingTypeID);
        //            cmd.Parameters.AddWithValue("@MeetingVenueID", meet.MeetingVenueID);
        //            cmd.Parameters.AddWithValue("@MeetingDescription", meet.MeetingDescription);
        //            cmd.Parameters.AddWithValue("@DocumentPath", meet.DocumentPath);
        //            cmd.Parameters.AddWithValue("@IsCancelled", meet.IsCancelled);
        //            cmd.Parameters.AddWithValue("@CancellationDateTime", meet.CancellationDateTime);
        //            cmd.Parameters.AddWithValue("@CancellationReason", meet.CancellationReason);
        //            cmd.Parameters.AddWithValue("@Modified", meet.Modified);
        //        }
        //        cmd.ExecuteNonQuery();
        //        con.Close();

        //        return RedirectToAction("MeetingList");
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("MeetingAddEdit", meet);
        //    }
        //}
        //#endregion

        //#region
        //public IActionResult DeleteMeeting(int id)
        //{
        //    try
        //    {
        //        SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;
        //        cmd.CommandText = "PR_MOM_Meetings_DeleteByPK";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter p = new SqlParameter();
        //        p.ParameterName = "@MeetingID";
        //        p.SqlDbType = SqlDbType.Int;
        //        p.Value = id;

        //        cmd.Parameters.Add(p);

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();

        //        TempData["Success"] = "Deleted Successfully.";
        //        return RedirectToAction("MeetingList");
        //    }
        //    catch (Exception)
        //    {
        //        TempData["Error"] = "Foreign Key Constraint Violated.";
        //        return RedirectToAction("MeetingList");
        //    }
        //}
        //#endregion

        #region DropdownMT
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

        #region DropdownVenue
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
                deptList.Add(new SelectListItem(reader["DepartmentName"].ToString(), reader["DepartmentID"].ToString()));
            }

            reader.Close();
            con.Close();

            return deptList;
        }
        #endregion
    }
}
