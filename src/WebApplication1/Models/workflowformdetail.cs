using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public partial class workflowformdetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public string applicationtype { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 20)]
        public double applicationversion { get; set; }
        public Nullable<System.DateTime> effectivedate { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 30)]
        public int workfloworder { get; set; }
        public int payrollgroupid { get; set; }
        public string formcode { get; set; }
        public byte applicanttype { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 40)]
        public int empid { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 50)]
        public string querycode { get; set; }
        public string orgcode { get; set; }
        public string workflowcode { get; set; }
        public bool delegateapply { get; set; }
        public string folder_english { get; set; }
        public string folder_chinese { get; set; }
        public string folder_big5 { get; set; }
        public string folder_japanese { get; set; }
        public int uploadstatus { get; set; }
        public Nullable<System.DateTime> expiredate { get; set; }
    }
}