using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("fields")]
    public class field
    {
        public int payrollgroupid { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public string tablecode { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 20)]
        public string fieldcode { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public int fieldid { get; set; }
        public byte fieldtype { get; set; }
        public string typename { get; set; }
        public int fieldlength { get; set; }
        public bool isprimarykey { get; set; }
        public bool allownull { get; set; }
        public bool autonumber { get; set; }
        public string reftable { get; set; }
        public string refcode { get; set; }
        public bool auditchange { get; set; }
        public bool showchange { get; set; }
        public bool canedit { get; set; }
        public string selfdef { get; set; }
        public bool custom { get; set; }
        public byte timeformat { get; set; }
        public string refbeforetable { get; set; }
        public string refbeforefield { get; set; }
        public bool multiselected { get; set; }
        public string japanese { get; set; }
        public bool delayeffect { get; set; }
        public string refbeforesourcefield { get; set; }
        public int createuser { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public int moduser { get; set; }
        public Nullable<System.DateTime> moddate { get; set; }
    }
}
