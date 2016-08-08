
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class PayrollGroupsDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int payrollgroupid { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string japanese { get; set; }
        public string name { get; set; }
    }
}