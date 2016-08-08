using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class EmployeeMsgInfo
    {
        public string empname { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int empid { get; set; }
        public string empcode { get; set; }

        public EmployeeMsgInfo(int empid )
        {
            this.empid = empid;
        }
    }


    public class EmployeeMsgDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long messageid { get; set; }
        [JsonConverter(typeof(DateAndTimeConverter))]
        public DateTime senddate { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public byte status { get; set; }
        public int senderempid { get; set; }
        //public EmployeeMsgInfo Sender { get; set; }
        //public List<EmployeeMsgInfo> To { get; set; }
        ///public List<EmployeeMsgInfo> Bcc { get; set; }
        //public List<EmployeeMsgInfo> Cc { get; set; }
    }

    public class EmployeeMsg
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long messageid { get; set; }
        [JsonConverter(typeof(DateAndTimeConverter))]
        public DateTime senddate { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public byte status { get; set; }
        public int senderempid { get; set; }
        public EmployeeMsgInfo Sender { get; set; }
        public List<EmployeeMsgInfo> To { get; set; }
        public List<EmployeeMsgInfo> Bcc { get; set; }
        public List<EmployeeMsgInfo> Cc { get; set; }
    }
    public class DateAndTimeConverter : IsoDateTimeConverter
    {
        public DateAndTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
    public class EmployeeMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long messageid { get; set; }
        public int sender_empid { get; set; }
        public string sender_empcode { get; set; }
        public string sender_english { get; set; }
        public string sender_chinese { get; set; }
        public string sender_big5 { get; set; }
        public string sender_name { get; set; }

        [JsonConverter(typeof(DateAndTimeConverter))]
        public DateTime senddate { get; set; }
        public string subject_english { get; set; }
        public string subject_chinese { get; set; }
        public string subject_big5 { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public byte status { get; set; }
        public EmployeeMsgInfo Sender { get; set; }
    }
}