using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1
{
    public class HRMSDBContext : DbContext
    {
        public DbSet<EmployeeMsgDTO> employeemsg { get; set; }
        
        public DbSet<SickLeaveInfo> sickleaveinfo { get; set; }
        public DbSet<CompanyLeaveInfo> companyleaveinfo { get; set; }
        public DbSet<ParamField> paramfield { get; set; }
        public DbSet<SubleaveForm> subleaveform { get; set; }
        public DbSet<MyWorkflow> myworkflow { get; set; }
        public DbSet<CalcLeaveDaysHours> calcLeavedayshours { get; set; }
        public DbSet<EmployeeBasicInfoDTO> employeebasicinfo { get; set; }
        public DbSet<EmployeeBasicInfo1> employeebasicinfo1 { get; set; }
        public DbSet<PayrollGroupInfo> paygroupinfo { get; set; }
        public DbSet<LeaveTypeModel> leavetypemodel { get; set; }
        public DbSet<WorkingDayProperty> workingdayproperty { get; set; }
        public DbSet<ExcludeDays> excludedays { get; set; }
        public DbSet<WFFormLong> wfformlong { get; set; }
        public DbSet<RosterInfo> rosterinfo { get; set; }
        public DbSet<StringResult> stringresult { get; set; }
        public DbSet<IntResult> intresult { get; set; }
        public DbSet<DateTimeResult> datetimeresult { get; set; }
        public DbSet<PendingApplications> pendingapplications { get; set; }
        public DbSet<AnuualLeaveInfo> annualleaveinfo { get; set; }
        public DbSet<MsgCount> msgcount { get; set; }
        public DbSet<PaySlipData> payslipdata { get; set; }
        public DbSet<payrollgroups> payrollgroups { get; set; }
        public DbSet<emphr> emphrs { get; set; }
        public DbSet<parameter> parameters { get; set; }
        public DbSet<organization> organizations { get; set; }
        public DbSet<position> positions { get; set; }
        public DbSet<empposition> empposition { get; set; }
        public DbSet<empleavedetail_ongoing> empleavedetail_ongoing { get; set; }
        public DbSet<empleavedata> empleavedata { get; set; }
        public DbSet<empleavedata_cancel> empleavedata_cancel { get; set; }
        public DbSet<empanlv> empanlv { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<emp_sick> emp_sick { get; set; }
        public DbSet<workflowinstance> workflowinstance { get; set; }
        public DbSet<workflowinstancedetail> workflowinstancedetail { get; set; }
        public DbSet<forminstance> forminstance { get; set; }
        public DbSet<forminstancedetail> forminstancedetail { get; set; }
        public DbSet<leavetype> leavetype { get; set; }
        public DbSet<message> Messages { get; set; }
        public DbSet<messagerecipient> MessageRecipients { get; set; }
        public DbSet<workflowformdetail> workflowformdetail { get; set; }
        public DbSet<approvalposition> approvalposition { get; set; }
        public DbSet<languages> languages { get; set; }
        public DbSet<errorcode> errorcodes { get; set; }
        public DbSet<control> control { get; set; }
        public DbSet<payperiod> payperiod { get; set; }
        public DbSet<reportparameter> reportparameter { get; set; }
        public DbSet<field> fields { get; set; }
        public DbSet<storesalaryrange> storesalaryranges { get; set; }
        public DbSet<emp_new> emp_news { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=zuoqinhp\SQLSERVER2016;Database=ccmp1;User Id=zuoqin;Password=Qwerty123;");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
