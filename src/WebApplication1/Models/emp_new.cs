using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("emp_new")]
    public class emp_new
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int empid { get; set; }

        public string positioncode { get; set; }
        public int positionid { get; set; }
        public string countrycode { get; set; }
        public int orgid { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string address { get; set; }
        public string raddress { get; set; }
        public string pemail { get; set; }
        public string pid { get; set; }
        public string ethnic { get; set; }
        //public DateTime? birthday { get; set; }
        public string gender { get; set; }
        public string mobile { get; set; }
        public string bankid1 { get; set; }
        public string  bankname1 { get; set; }
        public double @base { get; set; }
        public string portrait { get; set; }
        public DateTime? createdate { get; set; }
        public DateTime? hiredate { get; set; }
    }
}
