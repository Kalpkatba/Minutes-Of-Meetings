using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class MOM_MeetingsModel
    {
        [Required]
        public int MeetingID { get; set; }
        [Required]
        public DateTime MeetingDate { get; set; }
        [Required]
        public int MeetingTypeID { get; set; }
        [Required]
        public int DepartmentID { get; set; }
        [Required]
        public int MeetingVenueID { get; set; }
        public string? MeetingDescription { get; set; }
        public string? DocumentPath { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancellationDateTime { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public string? MeetingTypeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? MeetingVenueName { get; set; }

    }
}
