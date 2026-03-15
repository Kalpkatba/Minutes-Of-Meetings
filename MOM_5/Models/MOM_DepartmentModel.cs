using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class MOM_DepartmentModel
    {
        [Required]
        public int DepartmentID { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        [StringLength(250)]
        public string Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
