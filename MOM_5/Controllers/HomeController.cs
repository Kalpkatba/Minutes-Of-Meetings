using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_5.Models;
using System.Data;

namespace MOM_5.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration configuration;

        public HomeController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult Index()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");

            int totalMeetings = 0;
            int pendingMeetings = 0;
            int completedMeetings = 0;
            int cancelledMeetings = 0;

            List<string> typeLabels = new List<string>();
            List<int> typeCounts = new List<int>();

            List<string> deptLabels = new List<string>();
            List<int> deptCounts = new List<int>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Summary
                SqlCommand cmdSummary = new SqlCommand("PR_MOM_Meetings_Summary", con);
                cmdSummary.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmdSummary.ExecuteReader();

                if (reader.Read())
                {
                    totalMeetings = Convert.ToInt32(reader["TotalMeetings"]);
                    pendingMeetings = Convert.ToInt32(reader["PendingMeetings"]);
                    completedMeetings = Convert.ToInt32(reader["CompletedMeetings"]);
                    cancelledMeetings = Convert.ToInt32(reader["CancelledMeetings"]);
                }
                reader.Close();

                // Meetings By Type
                SqlCommand cmdType = new SqlCommand("PR_MOM_Meetings_ByType", con);
                cmdType.CommandType = CommandType.StoredProcedure;

                reader = cmdType.ExecuteReader();

                while (reader.Read())
                {
                    typeLabels.Add(reader["MeetingTypeName"].ToString());
                    typeCounts.Add(Convert.ToInt32(reader["Total"]));
                }
                reader.Close();

                // Meetings By Department
                SqlCommand cmdDept = new SqlCommand("PR_MOM_Meetings_ByDepartment", con);
                cmdDept.CommandType = CommandType.StoredProcedure;

                reader = cmdDept.ExecuteReader();

                while (reader.Read())
                {
                    deptLabels.Add(reader["DepartmentName"].ToString());
                    deptCounts.Add(Convert.ToInt32(reader["Total"]));
                }
                reader.Close();
            }

            ViewBag.TotalMeetings = totalMeetings;
            ViewBag.PendingMeetings = pendingMeetings;
            ViewBag.CompletedMeetings = completedMeetings;
            ViewBag.CancelledMeetings = cancelledMeetings;

            ViewBag.TypeLabels = typeLabels.ToArray();
            ViewBag.TypeCounts = typeCounts.ToArray();

            ViewBag.DeptLabels = deptLabels.ToArray();
            ViewBag.DeptCounts = deptCounts.ToArray();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
