using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication1.Models
{
    [Table("parameters")]
    public class parameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order=10)]
        public string paratype { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order=20)]
        public string paracode { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public bool issystem { get; set; }
        public string japanese { get; set; }
    }
}