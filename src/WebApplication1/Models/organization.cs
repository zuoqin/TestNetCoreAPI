using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication1.Models
{
    [Table("organization")]
    public class organization
    {
        public int orglevel { get; set; }
        public string orgcode { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string parentorg { get; set; }
        public string japanese { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int orgid { get; set; }
        public int managerposition { get; set; }
        public int budgetheadcount { get; set; }
        public int actualheadcount { get; set; }
        public int parentorgid { get; set; }

    }
}