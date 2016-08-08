using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class workflowinstance
    {
        public string applicationtype { get; set; }
        public double applicationversion { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int workflowinstanceid { get; set; }
        public int forminstanceid { get; set; }
        public string workflowcode { get; set; }
        public byte workflowstatus { get; set; }
        public string errormsg { get; set; }
        public int applicant { get; set; }
        public Nullable<System.DateTime> submittime { get; set; }
        public Nullable<System.DateTime> endtime { get; set; }
        public bool sendmail { get; set; }
        public int delegateapplicant { get; set; }
        public bool skipnextstep { get; set; }
        public string monitorempid { get; set; }
        public bool monitorsendmail { get; set; }
        public bool monitorapp { get; set; }

        public workflowinstance()
        {
            workflowinstanceid = 0;
        }
    }
}