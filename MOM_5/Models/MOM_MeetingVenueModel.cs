
using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class MOM_MeetingVenueModel
    {
        [Required]
        public int MeetingVenueID { get; set; }
        [Required]
        public string MeetingVenueName { get; set; }
        [Required]
        public string Remarks { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime Modified { get; set; }
    }
}
