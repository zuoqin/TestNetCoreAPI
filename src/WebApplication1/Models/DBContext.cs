using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class CompanyLeaveInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Nullable<System.DateTime> perioddate { get; set; } //System.DateTime
        public Nullable<System.DateTime> begindate { get; set; }   //
        //public string enddate { get; set; } //Nullable<System.DateTime>
        public Nullable<System.DateTime> expiredate { get; set; } //Nullable<System.DateTime>
        public byte isexpiremonth { get; set; }
        public double curquota { get; set; }
        public double curearndays { get; set; }
        public double curbalance { get; set; }
        public double curused { get; set; }
        public double curexpired { get; set; }
        public double lastquotaused { get; set; }
        public double quotaused { get; set; }
        public int adj { get; set; }

        public CompanyLeaveInfo()
        {
            perioddate = new DateTime();
            begindate = new DateTime();
            expiredate = new DateTime();
        }
    }
    public class SickLeaveInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime perioddate { get; set; }
        public DateTime? begindate { get; set; }
        public DateTime? enddate { get; set; }
        public double curcate1days { get; set; }
        public double curcate2days { get; set; }
        public double curused { get; set; }
        public int adj { get; set; }
        public SickLeaveInfo()
        {
            perioddate = new DateTime();
            begindate = new DateTime();
            enddate = new DateTime();
        }
    }

    public class LeavetypesFields
    {
        public int payrollgroupid { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string leavecode { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public List<FormField> fields { get; set; }
    }
    public class FormField
    {
        public int payrollgroupid;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public string formcode { get; set; }
        public string tablecode { get; set; }
        public string fieldcode { get; set; }
        public string name { get; set; }
        public int fieldorder { get; set; }
        public byte fieldtype { get; set; }
        public bool @readonly { get; set; }
        public bool required { get; set; }
        public bool hide { get; set; }
        public byte timeformat { get; set; }
        public List<ParamField> values { get; set; }
    }
    public class ParamField
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public string code { get; set; }
        public string value { get; set; }
    }
    public class SubleaveForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public int subpayrollgroupid { get; set; }
        public string subformcode { get; set; }
    }
    public class MyWorkflow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public string applicationtype { get; set; }
        public string formcode { get; set; }
        public string folder { get; set; }
        public string typename { get; set; }
        public string positioncode { get; set; }
        public string workflowcode { get; set; }
        public bool isruntimeapprover { get; set; }
        public bool allapprove { get; set; }
        public bool nextstepruntime { get; set; }
        public int payrollgroupid { get; set; }
    }
    public class CalcLeaveDaysHours
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public int maxref { get; set; }
        public double stadays { get; set; }
        public int stapos { get; set; }
        public double standardWorkingHours { get; set; }

    }

    public class EmployeeBasicInfo1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string CompanyEmail { get; set; }
        public string PersonalEmail { get; set; }
        public int payrollgroupid { get; set; }
    }


    public class EmployeeBasicInfo
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string CompanyEmail { get; set; }
        public string PersonalEmail { get; set; }
        public PayrollGroupInfo PayrollGroup { get; set; }
    }


    public class EmployeeBasicInfoDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string CompanyEmail { get; set; }
        public string PersonalEmail { get; set; }
    }
    public class PayrollGroupInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayrollGroupID { get; set; }
        public string GroupName { get; set; }

        public string BankName { get; set; }

        public string BankID { get; set; }

        public string Taxnumber { get; set; }

        public bool Use_gl { get; set; }

        public bool Gl_reverse { get; set; }

        public bool Use_costcenter { get; set; }

        public bool Use_negative { get; set; }

        public DateTime Pay_curyearbegin { get; set; }

        public DateTime Pay_curyearend { get; set; }

        public DateTime Pay_curperiodbegin { get; set; }

        public DateTime Pay_curperiodend { get; set; }


        public DateTime Attbegin { get; set; }

        public DateTime Attend { get; set; }

        public string Restday { get; set; }

        public int Gl_length { get; set; }

        public string Gl_mask { get; set; }

        public string Gl_sample { get; set; }

        public string Gl_taxexpense { get; set; }

        public string Gl_taxcoexpense { get; set; }

        public string Gl_taxpayable { get; set; }

        public string Gl_vacationaccrued { get; set; }

        public string Gl_vacationexpense { get; set; }

        public string Gl_vacationpayable { get; set; }

        public double Calendardays { get; set; }

        public double Workdays { get; set; }

        public double Dayspermonth { get; set; }

        public double Hoursperday { get; set; }

        public bool Viewsalary { get; set; }

        public int Pay_curyear { get; set; }

        public bool MultiplePayment { get; set; }
        public bool UseSalaryLockWorkflow { get; set; }

        //新加坡版专用
        public string UenNo { get; set; }

        public string NricNo { get; set; }

        public string FinNo { get; set; }
        public PayrollGroupInfo()
        {
        }
    }

    public class LeaveTypeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        public string Name { get; set; }
        public int PayrollGroupId { get; set; }
        public string Category { get; set; }
        public bool IsExcludeHoliday { get; set; }
        public string ExcludeHolidayNotApplyTo { get; set; }
        public bool IsExcludeWeekend { get; set; }
        public string ExcludeWeekendNotApplyTo { get; set; }
        public int LeaveApplyPolicy { get; set; }
        public bool ApplyToEachDay { get; set; }
    }


    public class WorkingDayProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime ambegintime { get; set; }
        public DateTime amendtime { get; set; }
        public DateTime pmbegintime { get; set; }
        public DateTime pmendtime { get; set; }
    }

    public class ExcludeDays
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public double weekend_days { get; set; }
        public double holiday_days { get; set; }
        public double ph_days { get; set; }
        public DateTime date { get; set; }
    }
    public class WFFormLong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int payrollgroupid { get; set; }
        public string formcode { get; set; }
        public string tablecode { get; set; }
        public string fieldcode { get; set; }
        public byte fieldtype { get; set; }
        public int fieldorder { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string japanese { get; set; }
        public bool hide { get; set; }
        public byte timeformat { get; set; }
        public string value { get; set; }
        public bool @readonly { get; set; }
        public bool required { get; set; }

        public string reftable { get; set; }
        public string refcode { get; set; }
    }
    public class DateTimeResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime result { get; set; }
    }
    public class StringResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string result { get; set; }
    }

    public class IntResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int result { get; set; }
    }
    public class RosterInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int empid { get; set; }
        public string holiday { get; set; }
        public string phday { get; set; }
        public string weekendday { get; set; }
        public string leave_code { get; set; }
        public string leave_name { get; set; }
        public DateTime? workdate { get; set; }
        public string roster_name { get; set; }
        public string location_code { get; set; }
        public string location_name { get; set; }
        public string workstarthour { get; set; }
        public bool nextdayws { get; set; }
        public string workendhour { get; set; }
        public bool nextdaywe { get; set; }
        public string job_name { get; set; }
    }
    public class PendingApplications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int workflowinstanceid { get; set; }
        public int forminstanceid { get; set; }
        public string application_code { get; set; }
        public string folder { get; set; }
        public string typename { get; set; }
        public double applicationversion { get; set; }
        public byte workflowstatus { get; set; }
        public int applicant { get; set; }
        public string applicantname { get; set; }
        public Nullable<System.DateTime> submittime { get; set; }
        public Nullable<System.DateTime> starttime { get; set; }
        public Nullable<System.DateTime> endtime { get; set; }
        public string leavefromtime { get; set; }
        public string leavetotime { get; set; }
        public string leavefromdate { get; set; }
        public string leavetodate { get; set; }
        public string notes { get; set; }
        public double leavedays { get; set; }
        public double leavehours { get; set; }
        public int payrollgroupid { get; set; }
        public string formcode { get; set; }
        public string leavecode { get; set; }
        public string positioncode { get; set; }
        public string positionname { get; set; }
        public bool allapprove { get; set; }
        public int workflowstep { get; set; }
        public int nextautoapprove { get; set; }
        public bool nextallapprove { get; set; }
        public string workflowcode { get; set; }
        public byte stepstatus { get; set; }
        public string leavetype { get; set; }
        public string empname { get; set; }

    }
    public class AnuualLeaveInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 10)]
        public DateTime year_begin_date { get; set; }
        public DateTime year_end_date { get; set; }
        public decimal year_entitlement { get; set; }
        public decimal year_balance { get; set; }
        public decimal year_taken { get; set; }
        public decimal ytd_balance { get; set; }
        public decimal ytd_taken { get; set; }
        public decimal exprv_prev { get; set; }
        public decimal expire_thismonth { get; set; }

    }
    public class PaySlipData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string code { get; set; }
        public double amount { get; set; }
        public string name { get; set; }
        public DateTime perioddate { get; set; }
    }
    public class MsgCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int unreadcount { get; set; }
    }
}
