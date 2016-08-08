using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("control")]
    public class control
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string companycode { get; set; }
        public string companyname_english { get; set; }
        public string companyname_chinese { get; set; }
        public string address1_english { get; set; }
        public System.DateTime ambegintime { get; set; }
        public System.DateTime amendtime { get; set; }
        public System.DateTime pmbegintime { get; set; }
        public System.DateTime pmendtime { get; set; }
    }
}
