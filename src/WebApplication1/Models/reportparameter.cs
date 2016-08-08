using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("reportparameter")]
    public class reportparameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public string parametercode { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 20)]
        public string reporttype { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 30)]
        public int userid { get; set; }
        public string selfreportcode { get; set; }
        public int payrollgroupid { get; set; }
        public string querycode { get; set; }
        public string reportnote { get; set; }
        public string paraset { get; set; }
        public string exportfile { get; set; }
    }
}
