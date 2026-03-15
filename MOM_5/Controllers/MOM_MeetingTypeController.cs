using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_MeetingTypeController : Controller
    {
        private IConfiguration configuration;

        public MOM_MeetingTypeController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region MeetingTypeList
        public IActionResult MeetingTypeList()
        {
            try
            {
                List<MOM_MeetingTypeModel> meetTypeList = new List<MOM_MeetingTypeModel>();

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    MOM_MeetingTypeModel meetType = new MOM_MeetingTypeModel();
                    meetType.MeetingTypeID = Convert.ToInt32(Reader["MeetingTypeID"]);
                    meetType.MeetingTypeName = Reader["MeetingTypeName"].ToString();
                    meetType.Remarks = Reader["Remarks"].ToString();
                    meetType.Created = Convert.ToDateTime(Reader["Created"]);
                    meetType.Modified = Convert.ToDateTime(Reader["Modified"]);

                    meetTypeList.Add(meetType);
                }

                Reader.Close();
                con.Close();


                return View(meetTypeList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region MeetingTypeAddEdit
        public IActionResult MeetingTypeAddEdit(int? id)
        {
            MOM_MeetingTypeModel model = new MOM_MeetingTypeModel();

            if (id != null) // EDIT MODE
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_MOM_MeetingType_SelectByPK";
                command.Parameters.AddWithValue("@MeetingTypeID", id);

                SqlDataReader reader = command.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(reader);

                if (datatable.Rows.Count > 0)
                {
                    DataRow row = datatable.Rows[0];
                    model.MeetingTypeID = Convert.ToInt32(row["MeetingTypeID"]);
                    model.MeetingTypeName = row["MeetingTypeName"]?.ToString();
                    model.Remarks = row["Remarks"]?.ToString();
                }
            }
            return View(model);

        }
        #endregion

        #region MeetingTypeSave
        public IActionResult MeetingTypeSave(MOM_MeetingTypeModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.MeetingTypeID == 0)
                {
                    command.CommandText = "PR_MOM_MeetingType_Insert";
                }
                else
                {
                    command.CommandText = "PR_MOM_MeetingType_UpdateByPK    ";
                    command.Parameters.Add("@MeetingTypeID", SqlDbType.Int).Value = model.MeetingTypeID;
                }
                command.Parameters.Add("@MeetingTypeName", SqlDbType.VarChar).Value = model.MeetingTypeName;
                command.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = model.Remarks;

                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("MeetingTypeList");

            }
            return View("MeetingTypeAddEdit", model);
        }

        #endregion

        #region DeleteMeetingType
        public IActionResult DeleteMeetingType(int id)
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
                    command.CommandText = "PR_MOM_MeetingType_DeleteByPK";
                    command.Parameters.Add("@MeetingTypeID", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                }


                TempData["Success"] = "Meeting Type Deleted Successfully.";
                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingTypeList");
            }
        }
        #endregion

        //#region
        //public IActionResult MeetingTypeAddEdit(MOM_MeetingTypeModel meetType)
        //{
        //    //ViewBag.DepartmentList = DropdowmDept();
        //    try
        //    {
        //        meetType.Created = DateTime.UtcNow;
        //        meetType.Modified = DateTime.UtcNow;

        //        if (!ModelState.IsValid)
        //        {
        //            return View("MeetingTypeAddEdit", meetType);
        //        }

        //        SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;
        //        cmd.CommandType = CommandType.StoredProcedure;


        //        con.Open();

        //        if (meetType.MeetingTypeID == 0)
        //        {
        //            meetType.Created = DateTime.UtcNow;
        //            meetType.Modified = DateTime.UtcNow;

        //            cmd.CommandText = "PR_MOM_MeetingType_Insert";
        //            cmd.Parameters.AddWithValue("@MeetingTypeID", meetType.MeetingTypeID);
        //            cmd.Parameters.AddWithValue("@MeetingTypeName", meetType.MeetingTypeName);
        //            cmd.Parameters.AddWithValue("@Remarks", meetType.Remarks);
        //            cmd.Parameters.AddWithValue("@Created", meetType.Created);
        //            cmd.Parameters.AddWithValue("@Modified", meetType.Modified);
        //        }
        //        else
        //        {
        //            meetType.Modified = DateTime.UtcNow;
        //            cmd.CommandText = "PR_MOM_MeetingType_UpdateByPK";
        //            cmd.Parameters.AddWithValue("@MeetingTypeID", meetType.MeetingTypeID);
        //            cmd.Parameters.AddWithValue("@MeetingTypeName", meetType.MeetingTypeName);
        //            cmd.Parameters.AddWithValue("@Remarks", meetType.Remarks);
        //            cmd.Parameters.AddWithValue("@Modified", meetType.Modified);
        //        }
        //        cmd.ExecuteNonQuery();
        //        con.Close();

        //        return RedirectToAction("MeetingTypeList");
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("MeetingTypeAddEdit", meetType);
        //    }
        //}
        //#endregion

        //#region
        //public IActionResult DeleteMeetingType(int id)
        //{
        //    try
        //    {
        //        SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;
        //        cmd.CommandText = "PR_MOM_MeetingType_DeleteByPK";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter p = new SqlParameter();
        //        p.ParameterName = "@MeetingTypeID";
        //        p.SqlDbType = SqlDbType.Int;
        //        p.Value = id;

        //        cmd.Parameters.Add(p);

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();

        //        TempData["Success"] = "Deleted Successfully.";
        //        return RedirectToAction("MeetingTypeList");
        //    }
        //    catch (Exception)
        //    {
        //        TempData["Error"] = "Foreign Key Constraint Violated.";
        //        return RedirectToAction("MeetingTypeList");
        //    }
        //}
        //#endregion
    }
}
