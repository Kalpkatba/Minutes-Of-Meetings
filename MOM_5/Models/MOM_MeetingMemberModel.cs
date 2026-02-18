using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class MOM_MeetingMemberModel
    {
        public int MeetingMemberID { get; set; }
        public int MeetingID { get; set; }
        public string StaffID { get; set; }
        public bool IsPresent { get; set; }
        public string Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string? StaffName { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? MeetingDate { get; set; }

    }
}
