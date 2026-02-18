using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class MOM_MeetingTypeController : Controller
    {
        #region
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

        #region
        public IActionResult MeetingTypeAddEdit(MOM_MeetingTypeModel meetType)
        {
            //ViewBag.DepartmentList = DropdowmDept();
            try
            {
                meetType.Created = DateTime.UtcNow;
                meetType.Modified = DateTime.UtcNow;

                if (!ModelState.IsValid)
                {
                    return View("MeetingTypeAddEdit", meetType);
                }

                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;


                con.Open();

                if (meetType.MeetingTypeID == 0)
                {
                    meetType.Created = DateTime.UtcNow;
                    meetType.Modified = DateTime.UtcNow;

                    cmd.CommandText = "PR_MOM_MeetingType_Insert";
                    cmd.Parameters.AddWithValue("@MeetingTypeID", meetType.MeetingTypeID);
                    cmd.Parameters.AddWithValue("@MeetingTypeName", meetType.MeetingTypeName);
                    cmd.Parameters.AddWithValue("@Remarks", meetType.Remarks);
                    cmd.Parameters.AddWithValue("@Created", meetType.Created);
                    cmd.Parameters.AddWithValue("@Modified", meetType.Modified);
                }
                else
                {
                    meetType.Modified = DateTime.UtcNow;
                    cmd.CommandText = "PR_MOM_MeetingType_UpdateByPK";
                    cmd.Parameters.AddWithValue("@MeetingTypeID", meetType.MeetingTypeID);
                    cmd.Parameters.AddWithValue("@MeetingTypeName", meetType.MeetingTypeName);
                    cmd.Parameters.AddWithValue("@Remarks", meetType.Remarks);
                    cmd.Parameters.AddWithValue("@Modified", meetType.Modified);
                }
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception ex)
            {
                return View("MeetingTypeAddEdit", meetType);
            }
        }
        #endregion

        #region
        public IActionResult DeleteMeetingType(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=LAPTOP-NQ0ROPVF\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingType_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingTypeID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Deleted Successfully.";
                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingTypeList");
            }
        }
        #endregion
    }
}
