using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class MOM_StaffModel
    {
        // remove [Required] from StaffID for create scenarios
        public int StaffID { get; set; }

        // either make nullable and keep [Required] or use Range to force non-zero selection
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
