using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("approvalposition")]
    public class approvalposition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string positioncode { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public bool issupervisor { get; set; }
        public string querycode { get; set; }
        public string defaultempid { get; set; }
        public bool isapplicant { get; set; }
        public bool isruntimeapprover { get; set; }
        public string japanese { get; set; }
        public bool issupervisor1 { get; set; }
        public bool issupervisor2 { get; set; }
        public bool isapprover { get; set; }
        public bool approvalpositiononly { get; set; }
        public int approverorglevel { get; set; }
        public int createuser { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public int moduser { get; set; }
        public Nullable<System.DateTime> moddate { get; set; }
    }
}
