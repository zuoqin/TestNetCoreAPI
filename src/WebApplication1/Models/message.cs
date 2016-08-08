using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long messageid { get; set; }
        public int payrollgroupid { get; set; }
        public byte messagetype { get; set; }
        public string extcode { get; set; }
        public Nullable<System.DateTime> senddate { get; set; }
        public int senderempid { get; set; }
        public string subject_english { get; set; }
        public string subject_chinese { get; set; }
        public string subject_big5 { get; set; }
        public string body { get; set; }
        public bool sendemail { get; set; }
        public byte status { get; set; }
        public Nullable<System.DateTime> duedate { get; set; }
        public bool deleted { get; set; }
        public bool mergemail { get; set; }
        public bool outercontact { get; set; }
        public string subject_japanese { get; set; }
        public bool candidate { get; set; }
        public long extid { get; set; }
    }
}