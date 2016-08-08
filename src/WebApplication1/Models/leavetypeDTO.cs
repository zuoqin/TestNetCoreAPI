
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class leavetypeDTO
    {
        public int payrollgroupid { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string leavecode { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string category { get; set; }
        public string japanese { get; set; }
        public string name { get; set; }
    }
}
