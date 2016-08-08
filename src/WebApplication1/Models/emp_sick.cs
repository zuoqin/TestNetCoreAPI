using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("emp_sick")]
    public class emp_sick
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int empid { get; set; }
        public DateTime perioddate { get; set; }
        public Nullable<System.DateTime> begindate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<double> curcate1days { get; set; }
        public Nullable<double> curcate1used { get; set; }
        public Nullable<double> curcate1balance { get; set; }
        public Nullable<double> curcate2days { get; set; }
        public Nullable<double> curcate2used { get; set; }
        public Nullable<double> curcate2balance { get; set; }
        public int adj { get; set; }
        public int curaccured { get; set; }
        public Nullable<double> curused { get; set; }
        public Nullable<double> lastcate1used { get; set; }
        public Nullable<double> lastcate2used { get; set; }
        public Nullable<double> cate1used { get; set; }
        public Nullable<double> cate2used { get; set; }
        public Nullable<System.DateTime> combegindate { get; set; }
        public Nullable<System.DateTime> comenddate { get; set; }
        public Nullable<double> comcurquota { get; set; }
        public Nullable<double> comcurearndays { get; set; }
        public Nullable<double> comcurbalance { get; set; }
        public Nullable<double> comcurused { get; set; }
    }
}
