using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("payperiod")]
    public class payperiod
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public int payrollgroupid { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 20)]
        public int yearcode { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 30)]
        public System.DateTime begindate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<System.DateTime> attbegindate { get; set; }
        public Nullable<System.DateTime> attenddate { get; set; }
        public int monthid { get; set; }
        public string note { get; set; }
        public bool post { get; set; }
    }
}
