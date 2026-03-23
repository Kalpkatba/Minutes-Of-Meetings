using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class MOM_StaffModel
    {
        public int StaffID { get; set; }

        [Required]
        public int? DepartmentID { get; set; }

        [Required]
        public string StaffName { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(250)]
        public string? Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string? DepartmentName { get; set; }
    }
}
