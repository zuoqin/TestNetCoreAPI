
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class UserDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int userid { get; set; }
        public string usercode { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string usergroupcode { get; set; }
        public string userpassword { get; set; }
        public int empid { get; set; }
        public string datemask { get; set; }
        public string timemask { get; set; }
        public int language { get; set; }
        public int msgcount { get; set; }
    }
}
