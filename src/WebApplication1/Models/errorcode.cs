using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("errorcodes")]
    public class errorcode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string code { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string japanese { get; set; }

        public errorcode()
        {
            
        }

        public errorcode(string code, string english, string chinese, string big5, string japanese)
        {
            this.code = code;
            this.english = english;
            this.chinese = chinese;
            this.big5 = big5;
            this.japanese = japanese;
        }
    }
}
