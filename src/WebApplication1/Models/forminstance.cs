using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class forminstance
    {
        public string applicationtype { get; set; }
        public double applicationversion { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int forminstanceid { get; set; }
        public int workflowinstanceid { get; set; }
        public int payrollgroupid { get; set; }
        public string formcode { get; set; }
        public byte formstatus { get; set; }
        public int applicant { get; set; }
        public Nullable<System.DateTime> submittime { get; set; }
        public int delegateapplicant { get; set; }
        public string formparameters { get; set; }
        public string rowflags { get; set; }
    }
}