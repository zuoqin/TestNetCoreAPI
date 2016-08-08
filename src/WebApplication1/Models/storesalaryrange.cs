using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("storesalaryrange")]
    public class storesalaryrange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int autoid { get; set; }
        public double minsalary { get; set; }
        public double maxsalary { get; set; }
        public int positionid { get; set; }
        public string storecode { get; set; }

    }
}
