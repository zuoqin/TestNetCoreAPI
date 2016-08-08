using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class messagerecipient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public long messageid { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 20)]
        public int empid { get; set; }
        public byte recipienttype { get; set; }
        public Nullable<System.DateTime> receivedate { get; set; }
        public string body { get; set; }
    }
}