using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class MOM_MeetingTypeModel
    {
        [Required]
        public int MeetingTypeID { get; set; }
        [Required]
        public string MeetingTypeName { get; set; }
        public string Remarks { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime Modified { get; set; }
    }
}
