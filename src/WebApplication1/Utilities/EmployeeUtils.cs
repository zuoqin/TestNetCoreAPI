using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Utilities
{



    public class EmployeeInfoShort
    {
        public int empid { get; set; }
        public string empcode { get; set; }
        public PayrollGroupsDTO payrollgroup { get; set; }
        public string pinyin { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public string big5 { get; set; }
        public string servicestatus { get; set; }
        public string hrstatus_english { get; set; }
        public string hrstatus_big5 { get; set; }
        public string hrstatus_chinese { get; set; }
        public string hrstatus_japanese { get; set; }
        public string hrstatus_name { get; set; }
        
        public string homephone { get; set; }
        public string mobile { get; set; }
        public string businessphone { get; set; }
        public string cemail { get; set; }
        public string pemail { get; set; }
        public string portrait { get; set; }
        public string EmpName { get; set; }


        public EmployeeInfoShort(emphr theEmphr = null)
        {
            HRMSDBContext db = new HRMSDBContext();
            if (theEmphr != null)
            {
                typeof (EmployeeInfoShort).GetProperties().Select(i =>
                {
                    var Content = typeof (emphr).GetProperties().FirstOrDefault(k => k.Name == i.Name);
                    if (Content != null)
                    {
                        i.SetValue(this, i.Name.Contains("portrait") ?
                            @"http://api.t5p.hk/Content/Portrait/" + Content.GetValue(theEmphr) :
                            Content.GetValue(theEmphr));
                    }
                    return 0;
                });

                parameter theHrStatus = db.parameters
                    .FirstOrDefault(k => (
                        (k.paratype == "HRSTATUS") &&
                        (k.paracode.ToLower().Contains(theEmphr.hrstatus.ToLower()))
                        ));

                this.hrstatus_chinese = theHrStatus.chinese;
                this.hrstatus_english = theHrStatus.english;
                this.hrstatus_big5 = theHrStatus.big5;
                this.hrstatus_japanese = theHrStatus.japanese;
            }
        }
    }




    public class SubordinatesCTE
    {
        public int empid { get; set; }
        public int supervisorempid { get; set; }
        public int level { get; set; }
    }
    public class SubordinatesShort
    {
        public int level { get; set; }
        public EmployeeData employee { get; set; }
        public EmployeeData supervisor { get; set; }
    }

    public class EmployeePaySlip
    {
        private HRMSDBContext db = new HRMSDBContext();
        List<DateTime> paySlipDates = new List<DateTime>();
        public List<PaySlipData> GetNetPayment(int empid, DateTime date, string language = "")
        {
            if (language.Length == 0)
            {
                //language = WebConfigurationManager.AppSettings["language"] ?? "english";
            }
            DateTime createDate = new DateTime(date.Year, date.Month, date.Day);
            string getNetIncomeSql = $@"
                SELECT perioddate,
                    'net' code,
                    (SELECT TOP 1 {language} FROM fields WHERE tablecode = 'emppayroll' AND fieldcode = 'net') [name],
                    net amount
                FROM
                    emppayroll_his
                WHERE
                    empid = {empid}
                    AND perioddate in (select max(perioddate) from emppayroll_his
                    where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})";
            
                        //DateTime.DaysInMonth(date.Year, date.Month));

            string sql = getNetIncomeSql;//String.Format(GetNetIncomeSql, empid, createDate.ToString("yyyy-MM-dd"));
            List<PaySlipData> paySlips = new List<PaySlipData>();//   db.payslipdata.FromSql(sql).ToList();
            return paySlips; 
        }

        private string GetSrchString(int empid, DateTime date, string srch = "")
        {
            string result = "";
            emphr theEmphr = db.emphrs.FirstOrDefault(k => k.empid == empid);
            if (theEmphr == null)
            {
                return "";
            }
            User theUser = db.users.FirstOrDefault(k => k.empid == empid);
            if (theUser == null)
            {
                return "";
            }

            int payrollgroupid = GetPayrollgroupID(empid, date);

            string command = $@"declare @sql nvarchar(1000)
                SET @sql =N'select *  from reportparameter
                    where (userid = {theUser.userid} or userid =1) and reporttype = 1
                    and payrollgroupid = {payrollgroupid}
                    order by ' +  dbo.ufn_GetLanguagesAPI()
                    

                EXECUTE sp_executesql   @sql , N''";


            string sql = command;
            List<reportparameter> theReportparameters = null;
            try
            {
                theReportparameters = db.reportparameter.FromSql(sql).ToList();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            if (theReportparameters == null)
            {
                return "";
            }

            reportparameter theReportparameter = null;
            theReportparameters.ForEach(x =>
            {
                if (x.querycode.Length > 0)
                {
                    string GetCommand = $@"
								select empid from {x.querycode}";

                    sql = GetCommand;

                    try
                    {
                        List<IntResult> EmployeeIDs = db.intresult.FromSql(sql).ToList();
                        if (EmployeeIDs.Count(q => q.result == empid) > 0)
                        {
                            if (theReportparameter == null)
                            {
                                theReportparameter = x;
                                return;
                            }
                        }


                    }
                    catch (Exception e)
                    {
                        Console.Write(e.ToString());
                    }
                }
                else
                {
                    if (theReportparameter == null)
                    {
                        theReportparameter = x;
                        return;
                    }
                }
            });
            if (theReportparameter != null)
            {
                result = theReportparameter.paraset;
                int pos1 = result.IndexOf(srch, StringComparison.Ordinal);
                int pos2 = result.IndexOf("/", pos1 + 1);
                if (pos2 < 0 && pos1 > 0)
                {
                    pos2 = result.Length;
                }
                if (pos1 > 0 && pos2 > 0)
                {
                    pos1 += srch.Length;
                    result = result.Substring(pos1, pos2 - pos1);
                }
            }
            return result;
        }
        public int GetPayrollgroupID(int empid, DateTime date)
        {
            string PayrollgroupCommand = $@"
                select top(1) payrollgroupid  from (
                select  CAST(newdata as int) as payrollgroupid, effectivedate,
                1 as flag from empdata_his where empid = {empid} and fieldcode = 'payrollgroupid' and
                effectivedate in (select max(effectivedate) from empdata_his where empid = {empid} and
                fieldcode = 'payrollgroupid' and  effectivedate <= '{date.ToString("yyyy-MM-dd HH:mm:ss")}')
                union 
                select payrollgroupid, getdate(), 0 from emphr where empid = {empid}
                ) as b
                order by flag desc ";


            string sql = PayrollgroupCommand;
            int payrollgroupid = -1;
            try
            {
                payrollgroupid = db.intresult.FromSql(sql).FirstOrDefault().result;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return payrollgroupid;
        }

        public int GetPayrollgroupID(int empid, int index)
        {
            
            string payrollgroupCommand = $@"
                select payrollgroupid  from (
                select  CAST(newdata as int) as payrollgroupid, effectivedate,
                1 as flag from empdata_his where empid = {empid} and fieldcode = 'payrollgroupid' and
                effectivedate in (select max(effectivedate) from empdata_his where empid = {empid} and
                fieldcode = 'payrollgroupid' and  effectivedate <= '{paySlipDates[index].ToString("yyyy-MM-dd HH:mm:ss")}')
                union 
                select payrollgroupid, getdate(), 0 from emphr where empid = {empid}
                ) as b
                order by flag desc, effectivedate desc
                OFFSET  0 ROWS 
                FETCH NEXT 1 ROWS ONLY ";


            string sql = payrollgroupCommand;
            int payrollgroupid = -1;
            try
            {
                payrollgroupid = db.intresult.FromSql(sql).FirstOrDefault().result;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return payrollgroupid;
        }


        public List<PaySlipData> GetDeductionPayment(int empid, int payrollgroupid, DateTime date, string language = "")
        {
            if (language.Length == 0)
            {
                language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
            }
            List<PaySlipData> paySlips = new List<PaySlipData>();
            string inStr = GetSrchString(empid, date, "[Deduction]=");
            string showZero = GetSrchString(empid, date, "[ShowZero]=");
            if (inStr.Length > 0)
            {
                DateTime createDate = new DateTime(date.Year, date.Month, date.Day);
                string getDeductionSql = $@"
                SELECT perioddate,
                    salary.salary_item code,
                    salary.amount
	                ,his.name
                FROM 
                    (SELECT
                        empsalary_his.*, {payrollgroupid} as payrollgroupid
                    FROM
                        empsalary_his inner join emphr on empsalary_his.empid = emphr.empid
                    WHERE
                        perioddate in (select max(perioddate) from empsalary_his
                        where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
                        AND emphr.empid = {empid}) p
                    UNPIVOT
                    (amount FOR salary_item IN ({inStr})) AS salary
                    INNER JOIN (SELECT salarycode, {language} as name, payrollgroupid
                                FROM salaryitems_his
                                WHERE perioddate in (select max(perioddate) from empsalary_his
                                where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
				                ) his
                        ON salary.salary_item = his.salarycode
		                and  his.payrollgroupid = salary.payrollgroupid";


                
                //DateTime.DaysInMonth(date.Year, date.Month));

                string sql = getDeductionSql;//String.Format(GetDeductionSql, empid, createDate.ToString("yyyy-MM-dd"), inStr);

                try
                {
                    paySlips = db.payslipdata.FromSql(sql).ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }
            }
            if (showZero == "0")
            {
                paySlips.RemoveAll(x => x.amount < 0.1);
            }
            return paySlips;
        }
        public List<PaySlipData> GetAllowancePayment(int empid, int payrollgroupid, DateTime date, string language = "")
        {
            if (language.Length == 0)
            {
                language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
            }
            string inStr = GetSrchString(empid, date, "[Allowance]=");
            string showZero = GetSrchString(empid, date, "[ShowZero]=");
            List<PaySlipData> paySlips = new List<PaySlipData>();
            if (inStr.Length > 0)
            {
                DateTime createDate = new DateTime(date.Year, date.Month, date.Day);
                string getAllowanceSql = $@"
                SELECT perioddate,
                    salary.salary_item code,
                    salary.amount
	                ,his.name
                FROM 
                    (SELECT
                        empsalary_his.*, {payrollgroupid} as payrollgroupid
                    FROM
                        empsalary_his inner join emphr on empsalary_his.empid = emphr.empid
                    WHERE
                        perioddate in (select max(perioddate) from empsalary_his
                        where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
                        AND emphr.empid = {empid}) p
                    UNPIVOT
                    (amount FOR salary_item IN ({inStr})) AS salary
                    INNER JOIN (SELECT salarycode, {language} as name, payrollgroupid
                                FROM salaryitems_his
                                WHERE perioddate in (select max(perioddate) from empsalary_his
                                where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
				                ) his
                        ON salary.salary_item = his.salarycode
		                and  his.payrollgroupid = salary.payrollgroupid";



                string sql = getAllowanceSql;//String.Format(GetAllowanceIncomeSql, empid, createDate.ToString("yyyy-MM-dd"), inStr);

                try
                {
                    paySlips = db.payslipdata.FromSql(sql).ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }

            }
            if (showZero == "0")
            {
                paySlips.RemoveAll(x => x.amount < 0.1);
            }
            return paySlips;
        }
        public List<PaySlipData> GetBonusPayment(int empid, int payrollgroupid, DateTime date, string language = "")
        {
            if (language.Length == 0)
            {
                language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
            }
            DateTime createDate = new DateTime(date.Year, date.Month, date.Day);
            string inStr = GetSrchString(empid, date, "[Bonus]=");
            string showZero = GetSrchString(empid, date, "[ShowZero]=");
            List<PaySlipData> paySlips = new List<PaySlipData>();
            if (inStr.Length > 0)
            {
                string getBonusSql = $@"
                SELECT perioddate,
                    salary.salary_item code,
                    salary.amount
	                ,his.name
                FROM 
                    (SELECT
                        empsalary_his.*, {payrollgroupid} as payrollgroupid
                    FROM
                        empsalary_his inner join emphr on empsalary_his.empid = emphr.empid
                    WHERE
                        perioddate in (select max(perioddate) from empsalary_his
                        where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
                        AND emphr.empid = {empid}) p
                    UNPIVOT
                    (amount FOR salary_item IN ({inStr})) AS salary
                    INNER JOIN (SELECT salarycode, {language} as name, payrollgroupid
                                FROM salaryitems_his
                                WHERE perioddate in (select max(perioddate) from empsalary_his
                                where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
				                ) his
                        ON salary.salary_item = his.salarycode
		                and  his.payrollgroupid = salary.payrollgroupid";


                string sql = getBonusSql;

                try
                {
                    paySlips = db.payslipdata.FromSql(sql).ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }
            }
            if (showZero == "0")
            {
                paySlips.RemoveAll(x => x.amount < 0.1);
            }
            return paySlips;
        }
        public List<PaySlipData> GetTaxPayment(int empid, int payrollgroupid, DateTime date, string language = "")
        {
            if (language.Length == 0)
            {
                language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
            }
            DateTime createDate = new DateTime(date.Year, date.Month, date.Day);
            string inStr = GetSrchString(empid, date, "[Tax]=");
            string showZero = GetSrchString(empid, date, "[ShowZero]=");
            List<PaySlipData> paySlips = new List<PaySlipData>();
            if (inStr.Length > 0)
            {
                string[] words;
                string[] separators = new string[] {"."};
                words = inStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                string code = words.Last();
                string getTaxSql = $@"
                SELECT perioddate,
                    '{code}' code,
                    (SELECT TOP 1 {language} FROM fields WHERE tablecode = 'emppayroll' AND fieldcode = '{code}') [name],
                    {code} amount
                FROM
                    emppayroll_his
                WHERE
                    empid = {empid}
                    AND perioddate in (select max(perioddate) from emppayroll_his
                    where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})";


                string sql = getTaxSql;
                
                try
                {
                    paySlips = db.payslipdata.FromSql(sql).ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }

            }
            if (showZero == "0")
            {
                paySlips.RemoveAll(x => x.amount < 0.1);
            }
            return paySlips;
        }

        public List<DateTime> GetPayPeriods(int empid)
        {
            const string getPaySlipsByEmpIdSql = @"
                 WITH DateSelect ( perioddate, myRowNumber )
                    AS
                    ( 
                        select  perioddate, ROW_NUMBER() over (order by perioddate) As myRowNumber from 
                            (select distinct perioddate from empsalary_his where empid = {0} ) as A

                     ) 

                     select perioddate from DateSelect order by perioddate desc";
            string sql = String.Format(getPaySlipsByEmpIdSql, empid);


            //List<DateTime> paySlips = new List<DateTime>();
            try
            {
                paySlipDates = db.datetimeresult.FromSql(sql).Select(x =>x.result).ToList();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return paySlipDates;
        }
        public List<PaySlipData> GetIncomePayment(int empid, int payrollgroupid, int index, string language = "english")
        {
            if (paySlipDates.Count == 0)
            {
                paySlipDates = GetPayPeriods(empid);
            }

            if (paySlipDates.Count > index)
            {
                
                return GetIncomePayment(empid, payrollgroupid, paySlipDates[index], language);
            }
            return null;
        }

        public List<PaySlipData> GetIncomePayment(int empid, int index, string language = "english")
        { 
            if (paySlipDates.Count == 0)
            {
                paySlipDates = GetPayPeriods(empid);
            }

            if (paySlipDates.Count > index)
            {
                int payrollgroupid = GetPayrollgroupID(empid, index);
                return GetIncomePayment(empid, payrollgroupid, paySlipDates[index], language);
            }
            return null;
        }


        public List<PaySlipData> GetIncomePayment(int empid, int payrollgroupid, DateTime date, string language = "english")
        {
            DateTime createDate = new DateTime(date.Year, date.Month, date.Day);
            if (language.Length == 0)
            {
                language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
            }
            string inStr = GetSrchString(empid, date, "[Income]=");
            string showZero = GetSrchString(empid, date, "[ShowZero]=");
            List<PaySlipData> paySlips = new List<PaySlipData>();
            if (inStr.Length > 0)
            {
                string getPaySlipsByEmpIdSql =
                    $@"
                SELECT perioddate,
                    salary.salary_item code,
                    salary.amount
	                ,his.name
                FROM 
                    (SELECT
                        empsalary_his.*, {payrollgroupid} as payrollgroupid
                    FROM
                        empsalary_his inner join emphr on empsalary_his.empid = emphr.empid
                    WHERE
                        perioddate in (select max(perioddate) from empsalary_his where
                        perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
                        AND emphr.empid = {empid}) p
                    UNPIVOT
                    (amount FOR salary_item IN ({inStr})) AS salary
                    INNER JOIN (SELECT salarycode, {language} as name, payrollgroupid
                                FROM salaryitems_his
                                WHERE perioddate in (select max(perioddate) from empsalary_his
                                where perioddate <= '{createDate.ToString("yyyy-MM-dd")}' and empid = {empid})
				                ) his
                        ON salary.salary_item = his.salarycode
		                and  his.payrollgroupid = salary.payrollgroupid";

                //DateTime.DaysInMonth(date.Year, date.Month));

                string sql = getPaySlipsByEmpIdSql;
                    //String.Format(GetPaySlipsByEmpIdSql, empid, createDate.ToString("yyyy-MM-dd"));                
                try
                {
                    paySlips = db.payslipdata.FromSql(sql).ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }
            }
            if (showZero == "0")
            {
                paySlips.RemoveAll(x => x.amount < 0.1);
            }
            return paySlips;
        }
    }

    public class EmployeeData
    {
        private HRMSDBContext db = new HRMSDBContext();
        public EmployeeInfoShort Emphr;
        public AnuualLeaveInfo AnnualLeave;
        public UserDTO User;
        public int msgcount;
        public PositionDTO Position;
        public List<RosterInfo> RosterList;
        public List<EmployeeData> Subordinates;

        public bool SetSubordinates(string language = "", int level = 0, int maxlevel = 2)
        {
            if (level <= maxlevel)
            {
                EmployeeUtils theUtils = new EmployeeUtils();
                Subordinates = theUtils.GetSubordinatesByCurrentEmpidSql(Emphr.empid, 1, language, level + 1, maxlevel);
            }

            return true;
        }
        public bool SetUser(User theUser = null)
        {
            if (theUser != null)
            {
                UserDTO theUserDTO = new UserDTO();
                PropertyInfo[] properties1 = typeof(UserDTO).GetProperties();
                PropertyInfo[] properties2 = typeof(User).GetProperties();


                foreach (PropertyInfo property1 in properties1)
                {
                    PropertyInfo theProperty = Array.Find(properties2, p => String.Compare(p.Name, property1.Name, StringComparison.Ordinal) == 0);
                    if (theProperty != null)
                    {
                        var value = theProperty.GetValue(theUser);
                        if (theProperty.Name.Contains("language"))
                        {
                            int languageId = Int32.Parse(value.ToString());
                            languages theLanguage = db.languages.FirstOrDefault(k => k.id == languageId);
                            value = theLanguage.id;
                        }
                        property1.SetValue(theUserDTO, value);
                    }
                }
                this.User = theUserDTO;
            }

            
            return true;
        }
        public bool SetData(emphr theEmphr = null, string language = "")
        {
            if (theEmphr != null)
            {
                Emphr = new EmployeeInfoShort();
                PropertyInfo[] properties1 = typeof(EmployeeInfoShort).GetProperties();
                PropertyInfo[] properties2 = typeof(emphr).GetProperties();

                if (language == "")
                {
                    User theUser = db.users.FirstOrDefault(k => k.empid == theEmphr.empid);

                    if (theUser != null)
                    {
                        languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);

                        if (theLanguage != null)
                        {
                            language = theLanguage.name;
                        }
                        else
                        {
                            language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
                        }
                    }
                    else
                    {
                        language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
                    }
                }

                foreach (PropertyInfo property1 in properties2)
                {
                    PropertyInfo theProperty = Array.Find(properties1, p => String.Compare(p.Name, property1.Name, StringComparison.Ordinal) == 0);
                    var value = property1.GetValue(theEmphr);
                    if (theProperty != null)
                    {
                        

                        if (property1.Name.Contains("portrait"))
                        {
                            string host = "";// HttpContext.Current.Request.Url.AbsoluteUri;
                            //host.Substring(0, host.IndexOf("api")) + "Content/Portrait/";

                            value = host.Substring(0, host.IndexOf("/api/")) + "/Content/Portrait/" + value;
                        }
                        theProperty.SetValue(Emphr, value);



                    }
                    if (property1.Name.CompareTo(language) == 0)
                    {
                        theProperty = Array.Find(properties1, p => p.Name.CompareTo("EmpName") == 0);
                        if (theProperty != null)
                        {
                            theProperty.SetValue(Emphr, value);
                        }
                    }
                }
                Emphr.businessphone = theEmphr.countrycode + theEmphr.businessphone;
                Emphr.mobile = theEmphr.countrycode + theEmphr.mobile;
                Emphr.homephone = theEmphr.countrycode + theEmphr.homephone;

                parameter theHrStatus = db.parameters
                    .FirstOrDefault(k => (
                        (k.paratype == "HRSTATUS") &&
                        (k.paracode.ToLower().Contains(theEmphr.hrstatus.ToLower()))
                        ));

                if (theHrStatus != null)
                {
                    Emphr.hrstatus_chinese = theHrStatus.chinese;
                    Emphr.hrstatus_english = theHrStatus.english;
                    Emphr.hrstatus_big5 = theHrStatus.big5;
                    Emphr.hrstatus_japanese = theHrStatus.japanese;

                    PropertyInfo theProperty1 = Array.Find(properties1,
                        p => p.Name.CompareTo("hrstatus_" + language) == 0);
                    if (theProperty1 != null)
                    {
                        var value = theProperty1.GetValue(Emphr);
                        theProperty1 = Array.Find(properties1, p => p.Name.CompareTo("hrstatus_name") == 0);
                        if (theProperty1 != null)
                        {
                            theProperty1.SetValue(Emphr, value);
                        }
                    }
                }
                else
                {
                    Emphr.hrstatus_name = "";
                }
            }
            return true;
        }
    }





    class EmployeeUtils
    {
        private HRMSDBContext db = new HRMSDBContext();

        public MsgCount GetUnreadMessageCount(int empid)
        {
            string GetMessagesSql = @"
                SELECT count(*) as unreadcount
                FROM
                    [messages] m
                    LEFT JOIN messagerecipient r
                        ON m.messageid = r.messageid
                    LEFT JOIN emphr sender
                        ON m.senderempid = sender.empid
                WHERE
                    m.[status] > 1
                    AND r.receivedate IS NULL
                    AND r.empid = {0}
                ";

            string sql = String.Format(GetMessagesSql, empid);
            MsgCount theMsgcount = db.msgcount.FromSql(sql).FirstOrDefault();
            return theMsgcount;
        }

        public string GetErrorCode(int empid, string errorCode)
        {
            string msg1 = "";
            string theLanguage = "english";
            User theUser = db.users.FirstOrDefault(k => k.empid == empid);
            if (theUser != null)
            {
                languages language = db.languages.FirstOrDefault(k => k.id == theUser.language);
                if (language != null)
                {
                    theLanguage = language.name;
                }
            }

            errorcode theErrorcode = db.errorcodes.FirstOrDefault(k => String.Compare(k.code, errorCode,
                            StringComparison.Ordinal) == 0);
            if (theErrorcode != null)
            {
                PropertyInfo[] properties1 = typeof(errorcode).GetProperties();
                foreach (PropertyInfo property1 in properties1)
                {
                    if (property1.Name.CompareTo(theLanguage) == 0)
                    {
                        msg1 = property1.GetValue(theErrorcode).ToString();
                    }
                }
            }
            return msg1;
        }
        public double CalculateLeaveHours(int empId, string leaveCode, DateTime beginDate, DateTime endDate)
        {
            var leave = new LeaveApplicationModel(empId, leaveCode, beginDate, endDate);
            double result = leave.CalcLeaveHours();
            //if (result == 0.0)
            //{
            //    if (endDate > beginDate)
            //    {
            //        TimeSpan timespan = endDate - beginDate;
            //        if (timespan.TotalDays > 0)
            //        {
            //            DateTime dt = beginDate;//.AddDays(1);
            //            dt = new DateTime(dt.Year, dt.Month, dt.Day, endDate.Hour, endDate.Minute, endDate.Second);
            //            TimeSpan timespan1 = dt - beginDate;
            //            result = timespan.Days * 8 + timespan1.TotalHours;
            //        }
            //        else
            //        {
            //            result = timespan.TotalHours;
            //        }
            //    }
            //}
            return result;
        }

        public double GetReferenceDays(int empid, LeaveApplication theLeaveApplication)
        {
            const string GetCommand = @"
                                SET DATEFIRST 1
								DECLARE @stadays FLOAT,
										@stapos INT,
										@standardWorkingHours FLOAT;
								SET @stadays = dbo.ufn_calc_GetStatoryDays({0},'{1}','{2}','{3}');
								SET @stapos =  dbo.ufn_calc_GetStatoryDaysEdge({0},'{1}','{2}','{3}');
								SELECT @standardWorkingHours = dbo.ufn_get_dayhours({0},'{2}');
								SELECT 0 as maxref, @stadays stadays, @stapos stapos, @standardWorkingHours standardWorkingHours;";

            string sql = string.Format(GetCommand, empid, theLeaveApplication.leavecode, theLeaveApplication.leavefromdate.ToString("yyyy-MM-dd"),
                theLeaveApplication.leavetodate.ToString("yyyy-MM-dd"));
            CalcLeaveDaysHours theResult = new CalcLeaveDaysHours();
            try
            {
                theResult = db.calcLeavedayshours.FromSql(sql).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            //      ufn_calc_GetStatoryDaysEdge：
            //
            //      if @begincount > 0 and @endcount> 0
            //          set @stapos = 3--前后都是休息
            //      else if @begincount > 0
            //          set @stapos = 1--只有第一天是休息
            //      else if @endcount > 0
            //          set @stapos = 2--只有最后一天休息
            //      else 
            //          set @stapos = -1--第一天和最后一天都不是休息日


            TimeSpan ts = theLeaveApplication.leavetodate - theLeaveApplication.leavefromdate;
            double TotalDays = Math.Floor(ts.TotalDays) + 1;
            theLeaveApplication.calcres = theResult.stapos;

            if (Math.Floor(ts.TotalDays) == 0)
            {
                if (!theLeaveApplication.todatemorning || !theLeaveApplication.fromdatemorning)
                {
                    theLeaveApplication.todatemorning = false;
                    theLeaveApplication.fromdatemorning = false;
                }

                if (!theLeaveApplication.todateafternoon || !theLeaveApplication.fromdateafternoon)
                {
                    theLeaveApplication.todateafternoon = false;
                    theLeaveApplication.fromdateafternoon = false;
                }

            }

            if (theLeaveApplication.fromdatemorning == false)
            {
                if(theResult.stapos == 2 || theResult.stapos == -1)
                {
                    if (theLeaveApplication.todatemorning == false && Math.Floor(ts.TotalDays) == 0)
                    {
                        TotalDays = TotalDays + 0;
                    }
                    else
                    {
                        TotalDays -= 0.5;
                    }

                }
            }

            if (theLeaveApplication.fromdateafternoon == false)
            {
                if (theResult.stapos == 2 || theResult.stapos == -1)
                {
                    if (theLeaveApplication.todateafternoon == false && Math.Floor(ts.TotalDays) == 0)
                    {
                        TotalDays = TotalDays + 0;
                    }
                    else
                    {
                        TotalDays -= 0.5;
                    }                    
                }
            }

            if (theLeaveApplication.todatemorning == false)
            {
                if (theResult.stapos == 1 || theResult.stapos == -1)
                {
                    TotalDays -= 0.5;
                }
            }

            if (theLeaveApplication.todateafternoon == false)
            {
                if (theResult.stapos == 1 || theResult.stapos == -1)
                {
                    TotalDays -= 0.5;
                }
            }

            theLeaveApplication.referencedays = TotalDays;
            theLeaveApplication.leavedays = theResult.stadays >= 0 ? TotalDays - theResult.stadays : -1;
            return theResult.stadays >= 0 ? TotalDays - theResult.stadays : -1;
        }


        public List<MyWorkflow> GetMyWorkflows(int empid, string language = "english")
        {
            User theUser = db.users.FirstOrDefault(k => k.empid == empid);
            if (theUser == null)
            {
                return null;
            }
            string command = $@"declare @p8 bit
                set @p8=NULL
                declare @p9 bit
                set @p9=NULL
                declare @p10 bit
                set @p10=NULL
                declare @p11 nvarchar(50)
                set @p11=NULL
                exec usp_wf_GetMyWorkflow @in_applicantempid={empid},@in_curusergroup=N'{theUser.usergroupcode}',@in_curlan=N'{language}',
                    @in_applicationtype=N'',@in_applicationversion=0,@in_formid=N'',@in_isdelegate=0,
                    @out_isruntimeapprover=@p8 output,@out_isallapprove=@p9 output,@out_isnextruntime=@p10 output,
                    @out_positioncode=@p11 output
                --select @p8, @p9, @p10, @p11";


            string sql = string.Format(command, empid, language);
            List<MyWorkflow> results = null;
            try
            {
                results = db.myworkflow.FromSql(sql).ToList();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return results;
        }

        public List<LeavetypesFields> GetLeaveTypesByEmpid2(int empid, string language = "")
        {
            User theUser = db.users.FirstOrDefault(k => k.empid == empid);
            emphr theEmphr = db.emphrs.FirstOrDefault(k => k.empid == empid);
            if (theEmphr == null)
            {
                return null;
            }
            List<LeavetypesFields> theList = new List<LeavetypesFields>();
            PropertyInfo[] properties1 = typeof(LeavetypesFields).GetProperties();
            PropertyInfo[] properties2 = typeof(leavetype).GetProperties();
            List<string> theLeaves = db.parameters.Where(k => k.paratype == "LEAVECATEGORY"
                                                                //&&(k.paracode == "00" || k.paracode == "01" ||
                                                                //k.paracode == "01")
                                                                ).Select(k => k.paracode).ToList();
            if (language == "")
            {                
                if (theUser != null)
                {
                    languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                    if (theLanguage != null)
                    {
                        language = theLanguage.name;
                    }
                }
                else
                {
                    language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
                }
            }

            string leavecodes3 = "";// WebConfigurationManager.AppSettings["3leavecodes"] ?? "";
            List<string> leavecodes = new List<string>();
            if (leavecodes3.Length == 0)
            {
                string bUseAllLeavesinMobile = "0";// WebConfigurationManager.AppSettings["UseAllLeaveCodes"] ?? "0";
                if (bUseAllLeavesinMobile == "1")
                {
                    leavecodes = db.leavetype.Where(k =>
                        (k.payrollgroupid == 0 || k.payrollgroupid == theEmphr.payrollgroupid)
                        && k.leavecode.Length > 0
                        )
                        .OrderBy(k => k.leavecode).Select(k => k.leavecode).ToList();
                }
            }
            else
            {
                leavecodes = leavecodes3.Split(new char[] { ',' }).ToList();
            }

            List<string> tmpleavecodes = db.leavetype.Where(k =>
                (k.payrollgroupid == 0 || k.payrollgroupid == theEmphr.payrollgroupid) &&
                theLeaves.Contains(k.category) &&
                k.ismobile && k.leavecode.Length > 0).Select(k => k.leavecode).ToList();


            IEnumerable<string> tmpleavecodes2 = Enumerable.Range(1, 30).Select(i =>
            {
                string tmp = "";
                if (leavecodes != null)
                {
                    tmp = leavecodes.FirstOrDefault(
                        k => tmpleavecodes.Contains(k) == false) ?? "";
                }


                if (tmpleavecodes.Count() < i && tmp.Length > 0)
                {
                    tmpleavecodes.Add(tmp);
                    return tmpleavecodes.FirstOrDefault(k => k == tmp);
                }

                if (tmpleavecodes != null && tmpleavecodes.Count >= i)
                    return tmpleavecodes[i - 1];
                else
                {
                    return null;
                }
            });

            tmpleavecodes = tmpleavecodes2.ToList();
            tmpleavecodes.RemoveAll(k => k == null);
            foreach (var pos in db.leavetype.Where(k =>
                tmpleavecodes.Contains(k.leavecode)).Take(30))
            {
                LeavetypesFields theLeaveType = new LeavetypesFields();

                foreach (PropertyInfo property1 in properties2)
                {
                    if (property1.Name.CompareTo(language) == 0)
                    {
                        var value = property1.GetValue(pos);
                        theLeaveType.name = value.ToString();
                    }
                    else
                    {

                        PropertyInfo theProperty = Array.Find(properties1, p => p.Name.CompareTo(property1.Name) == 0);
                        if (theProperty != null)
                        {
                            try
                            {
                                PropertyInfo theProperty2 = Array.Find(properties2, p => p.Name.CompareTo(property1.Name) == 0);

                                var value = theProperty2.GetValue(pos);
                                theProperty.SetValue(theLeaveType, value);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        }
                    }
                }

                string FormCommand = $@"exec sp_executesql N'SELECT TOP(1) subpayrollgroupid,subformcode,[seq] FROM workflowformsecurity WHERE applicationtype=@applicationtype AND [seq] in (select max(seq) from workflowformsecurity where applicationtype = @applicationtype)',N'@applicationtype nvarchar(20)',@applicationtype={pos.leaveworkflow}";
                string sql = FormCommand;
                SubleaveForm theSubForm = null;
                try
                {
                    theSubForm = db.subleaveform.FromSql(FormCommand).FirstOrDefault();
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }

                if (theSubForm != null)
                {


                    List<WFFormLong> theFields = GetFormFields(theSubForm.subformcode, theSubForm.subpayrollgroupid, theUser.usergroupcode);
                    PropertyInfo[]  WFFormLong_properties = typeof(WFFormLong).GetProperties();
                    PropertyInfo[] formfield_properties = typeof(FormField).GetProperties();
                    List<FormField> theFormFields = new List<FormField>();
                    foreach (var field in theFields)
                    {
                        FormField theFormField = new FormField();
                        theFormField.values = new List<ParamField>();

                        foreach (PropertyInfo property in WFFormLong_properties)
                        {
                            PropertyInfo theProperty = null;
                            if (property.Name.CompareTo(language) == 0)
                            {
                                var value = property.GetValue(field);
                                theFormField.name = value.ToString();
                            }
                            else if(property.Name.CompareTo("fieldtype") == 0) {
                                var value = property.GetValue(field);
                                if (value.ToString() == "0" //&& field.fieldcode.CompareTo("leavecode") != 0
                                    )
                                {
                                    if (field.reftable == "parameters")
                                    {
                                        sql = $@"SELECT paracode as code, {language} as value from {field.reftable} where paratype=N'{field.refcode}'";
                                    }
                                    else
                                    {
                                        sql = $@"SELECT {field.refcode} as code, {language} as value from {field.reftable}";
                                    }
                                    List<ParamField> theFieldValues = new List<ParamField>();
                                    try
                                    {
                                        theFieldValues = db.paramfield.FromSql(sql).ToList();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.Write(e.ToString());
                                    }
                                    foreach (var param in theFieldValues)
                                    {
                                        theFormField.values.Add(param);
                                    }
                                    
                                }
                                theProperty = Array.Find(formfield_properties, p => p.Name.CompareTo(property.Name) == 0);
                            }
                            else
                            {
                                theProperty = Array.Find(formfield_properties, p => p.Name.CompareTo(property.Name) == 0);
                            }
                            if (theProperty != null)
                            {
                                try
                                {
                                    var value = property.GetValue(field);
                                    theProperty.SetValue(theFormField, value);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                            }
                            
                        }
                        theFormFields.Add(theFormField);
                    }
                    theLeaveType.fields = theFormFields;
                }
                theList.Add(theLeaveType);
            }
            return theList;
        }

        public List<WFFormLong> GetFormFields(string formcode, int payrollgroupid, string usergroupcode)
        {
            string FormCommand = $@"exec sp_executesql N'SELECT a.* 
                FROM formfields a INNER JOIN fields b ON b.tablecode=a.tablecode AND b.fieldcode=a.fieldcode 
                INNER JOIN tables c ON c.tablecode=b.tablecode  LEFT JOIN fields rf ON a.reftable=rf.tablecode AND 
                a.refcode=rf.fieldcode  LEFT JOIN forms lf ON lf.payrollgroupid=a.linkformpayrollgroupid AND 
                lf.formcode=a.linkformcode  LEFT JOIN salaryitems salitem on salitem.salarycode = a.fieldcode AND
                (salitem.payrollgroupid=@payrollgroupid OR salitem.payrollgroupid=0) LEFT JOIN parameters salitemcate
                on salitem.itemcategory = salitemcate.paracode and salitemcate.paratype = ''SALARYITEMCATEGORY'' 
                LEFT JOIN form_security lfs ON lfs.payrollgroupid=lf.payrollgroupid AND lfs.formcode=lf.formcode AND 
                lfs.usergroupcode=''{usergroupcode}'' LEFT JOIN fieldsecurity fs ON fs.fieldcode=a.tablecode+''.''+a.fieldcode 
                AND fs.usergroupcode=@curusergroup LEFT JOIN fieldsecurity hs ON hs.fieldcode=CASE WHEN LEN(a.tablecode)>4 
                THEN LEFT(a.tablecode,LEN(a.tablecode)-4) ELSE a.tablecode END+''.''+a.fieldcode AND 
                hs.usergroupcode=@curusergroup LEFT JOIN fieldsecurity hd ON hd.fieldcode=''emppayroll.''+a.fieldcode 
                AND hd.usergroupcode=@curusergroup LEFT JOIN salaryitem_security ss ON ss.salarycode=a.fieldcode 
                AND (ss.payrollgroupid=@payrollgroupid OR ss.payrollgroupid=0) AND ss.usergroupcode=@curusergroup 
                WHERE a.payrollgroupid=@payrollgroupid AND a.formcode=@formcode AND
                (a.tablecode<>''organization'' OR a.fieldcode<>''parentorg'')
                and hide  = 0
                ORDER BY CASE WHEN a.areaid=0 THEN 99 ELSE a.areaid END,a.fieldorder',
                N'@payrollgroupid int,@curusergroup nvarchar(20),@formcode nvarchar(50)',
                @payrollgroupid={payrollgroupid},@curusergroup=N'{usergroupcode}',@formcode=N'{formcode}'";


            string sql = FormCommand;
            List<WFFormLong> theFormLong = null;
            try
            {
                theFormLong = db.wfformlong.FromSql(sql).ToList();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            return theFormLong;

        }
        public List<leavetypeDTO> GetLeaveTypesByEmpid(int empid, string language = "")
        {
            emphr theEmphr = db.emphrs.FirstOrDefault(k => k.empid == empid);
            if (theEmphr == null)
            {
                return null;
            }
            List<leavetypeDTO> theList = new List<leavetypeDTO>();
            PropertyInfo[] properties1 = typeof(leavetypeDTO).GetProperties();
            PropertyInfo[] properties2 = typeof(leavetype).GetProperties();
            List<string> theLeaves = db.parameters.Where(k => k.paratype == "LEAVECATEGORY"
                                                                //&&(k.paracode == "00" || k.paracode == "01" ||
                                                                //k.paracode == "01")
                                                                ).Select(k => k.paracode).ToList();
            if (language == "")
            {
                User theUser = db.users.FirstOrDefault(k => k.empid == empid);
                if (theUser != null)
                {
                    languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                    if (theLanguage != null)
                    {
                        language = theLanguage.name;
                    }
                }
                else
                {
                    //language = WebConfigurationManager.AppSettings["language"] ?? "english";
                }
            }

            string leavecodes3 = "";// WebConfigurationManager.AppSettings["3leavecodes"] ?? "";
            List<string> leavecodes = new List<string>();
            if (leavecodes3.Length == 0)
            {
                string bUseAllLeavesinMobile = "0";// WebConfigurationManager.AppSettings["UseAllLeaveCodes"] ?? "0";
                if (bUseAllLeavesinMobile == "1")
                {
                    leavecodes = db.leavetype.Where(k =>
                        (k.payrollgroupid == 0 || k.payrollgroupid == theEmphr.payrollgroupid)
                        && k.leavecode.Length > 0
                        )
                        .OrderBy(k => k.leavecode).Select(k => k.leavecode).ToList();
                }
            }
            else
            {
                leavecodes = leavecodes3.Split(new char[] { ',' }).ToList();
            }

            List<string> tmpleavecodes = db.leavetype.Where(k =>
                (k.payrollgroupid == 0 || k.payrollgroupid == theEmphr.payrollgroupid) &&
                theLeaves.Contains(k.category) &&
                k.ismobile && k.leavecode.Length > 0).Select(k=>k.leavecode).ToList();


            IEnumerable<string> tmpleavecodes2 = Enumerable.Range(1, 30).Select(i =>
            {
                string tmp = "";
                if (leavecodes != null)
                {
                    tmp = leavecodes.FirstOrDefault(
                        k => tmpleavecodes.Contains(k) == false) ?? "";
                }
                    

                if (tmpleavecodes.Count() < i && tmp.Length > 0)
                {                    
                    tmpleavecodes.Add(tmp);
                    return tmpleavecodes.FirstOrDefault(k => k == tmp);
                }

                if (tmpleavecodes != null && tmpleavecodes.Count >= i)
                    return tmpleavecodes[i-1];
                else
                {
                    return null;
                }
            });

            tmpleavecodes = tmpleavecodes2.ToList();
            tmpleavecodes.RemoveAll(k => k == null);
            foreach (var pos in db.leavetype.Where(k =>
                tmpleavecodes.Contains(k.leavecode)).Take(30))
            {
                leavetypeDTO theLeaveType = new leavetypeDTO();

                foreach (PropertyInfo property1 in properties1)
                {
                    PropertyInfo theProperty = Array.Find(properties2, p => p.Name.CompareTo(property1.Name) == 0);
                    if (theProperty != null)
                    {
                        var value = theProperty.GetValue(pos);
                        property1.SetValue(theLeaveType, value);


                        if (theProperty.Name.CompareTo(language) == 0)
                        {
                            theLeaveType.name = value.ToString();
                        }

                    }
                }
                theList.Add(theLeaveType);
            }
            return theList;
        }

        public AnuualLeaveInfo GetAnnualLeaveByEmpId(int empid)
        {
            const string GetAnnualLeaveBalanceByEmpIdSql = @"
                SELECT
                    year_begin_date,
	                year_end_date,
                    year_entitlement,
                    year_balance,
                    year_taken,
                    ytd_balance,
                    ytd_taken,
                    exprv_prev,
                    expire_thismonth
                FROM empanlv 
                WHERE empid= {0}";

            string sql = String.Format(GetAnnualLeaveBalanceByEmpIdSql, empid);
            AnuualLeaveInfo leaveInfo = db.annualleaveinfo.FromSql(sql).FirstOrDefault();
            if (leaveInfo == null)
            {
                leaveInfo = new AnuualLeaveInfo();
            }
            return leaveInfo;
        }
        public CompanyLeaveInfo GetCompanyLeaveByEmpId(int empid)
        {
            const string GetCompanyLeaveBalanceByEmpIdSql = @"
                SELECT
                    perioddate,
	                begindate,
                    ISNULL ( expiredate , '9999-12-31' ) as expiredate,
                    isexpiremonth,
                    curquota,
                    curearndays,
                    curbalance,
                    curused,
                    curexpired,
                    lastquotaused,
                    quotaused
                FROM emp_companysick 
                WHERE empid= {0} and perioddate in 
                (select max(perioddate) from emp_companysick where empid = {0} and perioddate < GetDate())";

            string sql = String.Format(GetCompanyLeaveBalanceByEmpIdSql, empid);
            CompanyLeaveInfo leaveInfo = db.companyleaveinfo.FromSql(sql).FirstOrDefault();
            return leaveInfo;
        }
        public SickLeaveInfo GetSickLeaveByEmpId(int empid)
        {
            const string GetSickLeaveBalanceByEmpIdSql = @"
                SELECT
                    perioddate,
	                begindate,
                    enddate,
                    curcate1days,
                    curcate2days,
                    curused,
                    adj
                FROM emp_sick 
                WHERE empid= {0} and perioddate in 
                (select max(perioddate) from emp_sick where empid = {0} and perioddate < GetDate())";

            string sql = String.Format(GetSickLeaveBalanceByEmpIdSql, empid);
            SickLeaveInfo leaveInfo = db.sickleaveinfo.FromSql(sql).FirstOrDefault();
            return leaveInfo;
        }

        public PositionDTO GetEmpPositionByEmpId(int empid, string language = "")
        {
            PositionDTO Position = null;
            if (language == "")
            {
                User theUser = db.users.FirstOrDefault(k => k.empid == empid);
                if (theUser != null)
                {
                    languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                    if (theLanguage != null)
                    {
                        language = theLanguage.name;
                    }
                    else
                    {
                        language = "english";// WebConfigurationManager.AppSettings["language"] ?? "english";
                    }
                }
                else
                {
                    language = "english";// WebConfigurationManager.AppSettings["language"] ?? "english";
                }

            }

            empposition theEmpposition = db.empposition.FirstOrDefault(k => k.empid == empid);
            if (theEmpposition != null)
            {
                position thePosition = db.positions.Where((k => k.positioncode == theEmpposition.positioncode)).FirstOrDefault();
                if (thePosition != null)
                {
                    PositionDTO thePositionDTO = new PositionDTO();
                    PropertyInfo[] properties1 = typeof(PositionDTO).GetProperties();
                    PropertyInfo[] properties2 = typeof(position).GetProperties();


                    foreach (PropertyInfo property1 in properties1)
                    {
                        var value = new object();
                        PropertyInfo theProperty = Array.Find(properties2, p => p.Name.CompareTo(property1.Name) == 0);
                        if (theProperty != null)
                        {
                            value = theProperty.GetValue(thePosition);
                            property1.SetValue(thePositionDTO, value);


                            if (theProperty.Name.CompareTo(language) == 0)
                            {
                                theProperty = Array.Find(properties1, p => p.Name.CompareTo("positionname") == 0);
                                if (theProperty != null)
                                {
                                    theProperty.SetValue(thePositionDTO, value);
                                }
                            }

                        }


                    }
                    Position = thePositionDTO;
                }
            }
            return Position;
        }



        public List<PendingApplications> GetMyApplicationsByEmpId(int empid, string language = "")
        {
            
            if (language.Length == 0)
            {
                User theUser = db.users.FirstOrDefault(k => k.empid == empid);
                if (theUser != null)
                {
                    languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                    if (theLanguage != null)
                    {
                        language = theLanguage.name;
                    }
                    else
                    {
                        language = "english"; // WebConfigurationManager.AppSettings["language"] ?? "english";
                    }
                }
                else
                {
                    language = "english"; // WebConfigurationManager.AppSettings["language"] ?? "english";
                }
                
            }

            string GetMyApplicationsByEmpIdSQL =
                $@"SELECT lfd.leavefromdate, ltd.leavetodate, lt.{language} as leavetype, h.{language} as empname, c.submittime, ltt.leavetotime, lft.leavefromtime, isnull(g.leavefromdate,'1900-01-01') as leavefromdate,
lc.leavecode, ln.notes, 
isnull(g.leavetodate,'1900-01-01') as leavetodate, isnull(g.leavedays,0) as leavedays, isnull(g.leavehours,0) as leavehours,
a.workflowinstanceid,b.forminstanceid,p.[{language}] AS typename,b.applicationtype,
b.applicationversion,b.workflowstatus,
b.applicant,h.[{language}] AS applicantname,ap.positioncode,ap.[{language}] AS positionname,a.allapprove,a.workflowstep,a.starttime,a.endtime,
a.stepstatus,a.delegateapprove,a.delegateapprover,b.workflowcode,c.payrollgroupid,c.formcode,a.inconsult,a.isnextruntimeapprover,
(CASE WHEN aa.workflowinstanceid IS NULL THEN 0 ELSE 1 END) AS [nextautoapprove],a.nextallapprove,
c.workflowinfo_{language} AS workflowinfo,f.folder, '' as empinformcontent 
FROM workflowinstancedetail a 
INNER JOIN workflowinstance b ON b.workflowinstanceid=a.workflowinstanceid INNER JOIN forminstance c ON c.forminstanceid=b.forminstanceid 
 INNER JOIN [parameters] p ON p.paratype='APPLICATIONTYPE' 
AND p.paracode=b.applicationtype INNER JOIN emphr h ON h.empid=b.applicant INNER JOIN approvalposition ap ON ap.positioncode=a.approvalpositioncode 
INNER JOIN workflow wf ON b.workflowcode = wf.workflowcode 
LEFT JOIN workflowinstancedetail aa ON aa.workflowinstanceid=a.workflowinstanceid AND aa.workflowstep=a.workflowstep+1 AND aa.isruntimeapprover<>0 AND a.isnextruntimeapprover<>0 AND aa.autoapprove<>0 
LEFT JOIN (SELECT DISTINCT applicationtype,applicationversion,folder_{language} AS folder FROM workflowformdetail) AS f ON 
f.applicationtype=b.applicationtype AND f.applicationversion=b.applicationversion 


left join (select forminstanceid, min(leavefromdate) as leavefromdate, 
max(leavetodate) as leavetodate, sum(leavedays) as leavedays, sum(leavehours) as leavehours
 from empleavedetail_ongoing group by forminstanceid) as g on
 c.forminstanceid = g.forminstanceid

left join (select min(newvalue) as leavefromtime, forminstanceid from forminstancedetail where 
fieldcode ='leavefromtime' and tablecode = 'empleavedata' 
group by forminstanceid) as lft on lft.forminstanceid = c.forminstanceid

left join (select min(newvalue) as leavetotime, forminstanceid from forminstancedetail where 
fieldcode ='leavetotime' and tablecode = 'empleavedata' 
group by forminstanceid) as ltt on ltt.forminstanceid = c.forminstanceid

left join (select min(newvalue) as leavecode, forminstanceid from forminstancedetail where 
fieldcode ='leavecode' and tablecode = 'empleavedata'
group by forminstanceid) as lc on lc.forminstanceid = c.forminstanceid

left join (select min(newvalue) as notes, forminstanceid from forminstancedetail where 
fieldcode ='notes' and tablecode = 'empleavedata'
group by forminstanceid) as ln on ln.forminstanceid = c.forminstanceid

left join (select min(newvalue) as leavefromdate, forminstanceid from forminstancedetail where 
fieldcode ='leavefromdate' and tablecode = 'empleavedata'
group by forminstanceid) as lfd on lfd.forminstanceid = c.forminstanceid

left join (select min(newvalue) as leavetodate, forminstanceid from forminstancedetail where 
fieldcode ='leavetodate' and tablecode = 'empleavedata'
group by forminstanceid) as ltd on ltd.forminstanceid = c.forminstanceid

join leavetype lt on lt.leavecode = lc.leavecode

WHERE c.applicant={empid}
    and b.forminstanceid in (select distinct forminstanceid from forminstancedetail where tablecode in ( 'empleavedata', 'empleavedetail'))
	
	AND c.formstatus IN (1) 
    AND a.stepstatus = 1
ORDER BY ISNULL(b.submittime,c.submittime) DESC";


            string sql = GetMyApplicationsByEmpIdSQL;
            List<PendingApplications> applications = db.pendingapplications.FromSql(sql).ToList();
            return applications;
        }



        public List<PendingApplications> GetPendingApplicationsByEmpId(int empid, string language = "")
        {
            if (language.Length == 0)
            {
                language = "english"; // WebConfigurationManager.AppSettings["language"] ?? "english";
            }
            string GetPendingApplicationsByEmpIdSQL =
                $@"SELECT lfd.leavefromdate, ltd.leavetodate, lt.{language} as leavetype, h.{language} as empname, c.submittime, ltt.leavetotime, lft.leavefromtime, isnull(g.leavefromdate,'1900-01-01') as leavefromdate,
lc.leavecode, ln.notes, 
isnull(g.leavetodate,'1900-01-01') as leavetodate, isnull(g.leavedays,0) as leavedays, isnull(g.leavehours,0) as leavehours,
a.workflowinstanceid,b.forminstanceid,p.[{language}] AS typename,b.applicationtype,
b.applicationversion,b.workflowstatus,
b.applicant,h.[{language}] AS applicantname,ap.positioncode,ap.[{language}] AS positionname,a.allapprove,a.workflowstep,a.starttime,a.endtime,
a.stepstatus,a.delegateapprove,a.delegateapprover,b.workflowcode,c.payrollgroupid,c.formcode,a.inconsult,a.isnextruntimeapprover,
(CASE WHEN aa.workflowinstanceid IS NULL THEN 0 ELSE 1 END) AS [nextautoapprove],a.nextallapprove,
c.workflowinfo_{language} AS workflowinfo,f.folder, '' as empinformcontent 
FROM workflowinstancedetail a 
INNER JOIN workflowinstance b ON b.workflowinstanceid=a.workflowinstanceid INNER JOIN forminstance c ON c.forminstanceid=b.forminstanceid 
 INNER JOIN [parameters] p ON p.paratype='APPLICATIONTYPE' 
AND p.paracode=b.applicationtype INNER JOIN emphr h ON h.empid=b.applicant INNER JOIN approvalposition ap ON ap.positioncode=a.approvalpositioncode 
INNER JOIN workflow wf ON b.workflowcode = wf.workflowcode 
LEFT JOIN workflowinstancedetail aa ON aa.workflowinstanceid=a.workflowinstanceid AND aa.workflowstep=a.workflowstep+1 AND aa.isruntimeapprover<>0 AND a.isnextruntimeapprover<>0 AND aa.autoapprove<>0 
LEFT JOIN (SELECT DISTINCT applicationtype,applicationversion,folder_{language} AS folder FROM workflowformdetail) AS f ON 
f.applicationtype=b.applicationtype AND f.applicationversion=b.applicationversion 


left join (select forminstanceid, min(leavefromdate) as leavefromdate, 
max(leavetodate) as leavetodate, sum(leavedays) as leavedays, sum(leavehours) as leavehours
 from empleavedetail_ongoing group by forminstanceid) as g on
 c.forminstanceid = g.forminstanceid

left join (select newvalue as leavefromtime, forminstanceid from forminstancedetail where 
fieldcode ='leavefromtime' and tablecode = 'empleavedata') as lft on lft.forminstanceid = c.forminstanceid

left join (select newvalue as leavetotime, forminstanceid from forminstancedetail where 
fieldcode ='leavetotime' and tablecode = 'empleavedata' ) as ltt on ltt.forminstanceid = c.forminstanceid

left join (select newvalue as leavecode, forminstanceid from forminstancedetail where 
fieldcode ='leavecode' and tablecode = 'empleavedata') as lc on lc.forminstanceid = c.forminstanceid

left join (select newvalue as notes, forminstanceid from forminstancedetail where 
fieldcode ='notes' and tablecode = 'empleavedata') as ln on ln.forminstanceid = c.forminstanceid

left join (select newvalue as leavefromdate, forminstanceid from forminstancedetail where 
fieldcode ='leavefromdate' and tablecode = 'empleavedata') as lfd on lfd.forminstanceid = c.forminstanceid

left join (select newvalue as leavetodate, forminstanceid from forminstancedetail where 
fieldcode ='leavetodate' and tablecode = 'empleavedata') as ltd on ltd.forminstanceid = c.forminstanceid

join leavetype lt on lt.leavecode = lc.leavecode

WHERE (CASE WHEN wf.can_see_auto_approved_application = 1 THEN 1 ELSE 0 END = 1
    OR CASE WHEN wf.can_see_auto_approved_application = 0 THEN 0 ELSE 1 END = a.autoapprove) 
    and b.forminstanceid in (select distinct forminstanceid from forminstancedetail where tablecode in ( 'empleavedata', 'empleavedetail'))
	AND (a.approver={empid} OR a.delegateapprover={empid} OR a.supervisorapprover={empid}) 
	AND a.stepstatus IN (1) ORDER BY a.starttime DESC";



            //@"exec usp_wf_Get_MyApprovalList_Leave @firstdatemaskchar = N'y',@datesep = N'/',@curemp = {0},@showcode = 0,
            //@namefield = N'english',@empname = N'h.[english]',@empcodeincontent = N'',@empcodewhere = N''";

            string sql = GetPendingApplicationsByEmpIdSQL;//String.Format(GetPendingApplicationsByEmpIdSQL, empid);
            List<PendingApplications> applications = db.pendingapplications.FromSql(sql).ToList();
            return applications;
        }

        public List<EmployeeData> GetSubordinatesByCurrentEmpidSql(int empid, int page = 1, string language = "", int level = 0, int maxlevel = 2)
        {
            List<EmployeeData> theData = new List<EmployeeData>();
            if (empid == 0)
            {
                return theData;
            }
            //if (page < 1)
            //{
            //    page = 1;
            //}
            string subordinatescount = "10"; // WebConfigurationManager.AppSettings["subordinatescount"] ?? "10";
            string GetMySubordinatesByCurrentEmpidSql = $@"
                SELECT 
                    p.empid 
                FROM empposition p inner join AllActiveEmp a on p.empid = a.empid
                WHERE (supervisorempid = {empid} or supervisorempid1 = {empid} or  supervisorempid2 = {empid})
                --and exists(select * from users u where u.empid = p.empid)
                order by 1
                ";

            if (page > 0)
            {
                GetMySubordinatesByCurrentEmpidSql += $@" OFFSET { (page - 1) * Int32.Parse(subordinatescount)} ROWS ";

                GetMySubordinatesByCurrentEmpidSql += $@" FETCH NEXT {subordinatescount} ROWS ONLY ";
            }
            //string sql = String.Format(GetMySubordinatesByCurrentEmpidSql, empid);
            List<int> subordinates = new List<int>();// db.intresult.FromSql(GetMySubordinatesByCurrentEmpidSql).ToList();

            if (language == "")
            {
                User theUser = db.users.FirstOrDefault(b => b.empid == empid);
                if (theUser != null)
                {
                    languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                    if (theLanguage != null)
                    {
                        language = theLanguage.name;
                    }
                }
                else
                {
                    language = "english"; //WebConfigurationManager.AppSettings["language"] ?? "english";
                }
            }

            List<emphr> emphrs = db.emphrs.Where(k => subordinates.Contains(k.empid)).ToList();

            
            emphrs.ForEach(x =>
            {
                EmployeeData employee = new EmployeeData();
                employee.RosterList = GetRosterByEmpId(x.empid, language);
                employee.Position = GetEmpPositionByEmpId(x.empid, language);
                employee.AnnualLeave = GetAnnualLeaveByEmpId(x.empid);
                employee.SetData(x, language);
                employee.SetSubordinates(language, level, maxlevel);
                theData.Add(employee);
            }
            );

            return theData;
        }

        public int GetMessageCountByEmpId(int empid)
        {
            const string GetMessagesSql = @"
                SELECT count(*)
                FROM
                    [messages] m
                    LEFT JOIN messagerecipient r
                        ON m.messageid = r.messageid
                    LEFT JOIN emphr sender
                        ON m.senderempid = sender.empid
                WHERE
                    m.[status] > 1
                    AND r.receivedate IS NULL 
                    AND r.empid = {0}";
            string sql = String.Format(GetMessagesSql, empid);
            int result = db.intresult.FromSql(sql).FirstOrDefault().result;
            return result;
        }

        public bool UpdateMessage(int empid, int messageid)
        {
            const string Command1 = @"
                exec sp_executesql N'UPDATE messagerecipient SET receivedate=@date1 WHERE messageid=@messageid 
                    AND empid=@empid',N'@messageid bigint,@empid int,@date1 datetime',@messageid={1},
                    @empid={0},@date1='{2}'
            ";

            string sql = string.Format(Command1, empid, messageid,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                );
            try
            {
                db.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return true;
        }
        public EmployeeMsg GetMessageById(int empid, int messageid, string language = "")
        {
            if (language.Length == 0)
            {
                User theUser = db.users.FirstOrDefault(k => k.empid == empid);
                if (theUser == null)
                {
                    return null;
                }
                languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                if (theLanguage == null)
                {
                    return null;
                }
                language = theLanguage.name;
            }
            string GetSingleMessageSql = $@"
                SELECT
                    m.messageid,    
                    m.subject_" + language + $@" as subject,
                    m.senddate,           
                    m.body,    
                    m.[status],
                    m.senderempid
                FROM
                    [messages] m
                    LEFT JOIN messagerecipient r
                        ON m.messageid = r.messageid
                    LEFT JOIN emphr sender
                        ON m.senderempid = sender.empid
                WHERE
                    --m.[status] = 2
                    --AND
                    m.messageid = {messageid}
                    AND r.empid = {empid}";
            string sql = GetSingleMessageSql;//String.Format(GetSingleMessageSql, empid, messageid);
            EmployeeMsgDTO msgDTO = db.employeemsg.FromSql(sql).FirstOrDefault();
            EmployeeMsg msg = new EmployeeMsg();
            msg.To = GetMessageRecipients(messageid, 0, language);
            msg.Bcc = GetMessageRecipients(messageid, 2, language);
            msg.Cc = GetMessageRecipients(messageid, 1, language);

            msg.Sender = new EmployeeMsgInfo(msg.senderempid);

            emphr theEmphr = db.emphrs.FirstOrDefault(k => k.empid == msg.Sender.empid);
            if (theEmphr != null)
            {
                msg.Sender.empcode = theEmphr.empcode;
                var EmphrContent = typeof(emphr).GetProperties().FirstOrDefault(k => k.Name == language);
                if (EmphrContent != null)
                {
                    msg.Sender.empname =
                        (EmphrContent.GetValue(theEmphr) ?? "").ToString();

                }
            }
            return msg;

        }



        private List<EmployeeMsgInfo> GetMessageRecipients(int messageid, byte type, string language)
        {
            string GetToRecipientsMessageSql = @"
                SELECT
                    r.empid
                FROM
                    [messages] m
                    LEFT JOIN messagerecipient r
                        ON m.messageid = r.messageid
                WHERE
                    m.messageid = {0}
                    AND r.recipienttype = {1}";
            string sql = String.Format(GetToRecipientsMessageSql, messageid, type);
            List<IntResult> To = db.intresult.FromSql(sql).ToList();
            List<EmployeeMsgInfo> result = new List<EmployeeMsgInfo>();



            To.All(p =>
            {
                EmployeeMsgInfo theInfo = new EmployeeMsgInfo(p.result);
                theInfo.empid = p.result;

                emphr theEmphr = db.emphrs.FirstOrDefault(k => k.empid == p.result);
                if (theEmphr != null)
                {
                    theInfo.empcode = theEmphr.empcode;
                    if (language.Length == 0)
                    {
                        User theUser = db.users.FirstOrDefault(k => k.empid == p.result);
                        if (theUser != null)
                        {
                            languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                            if (theLanguage != null)
                            {
                                language = theLanguage.name;
                            }
                        }
                    }
                    PropertyInfo[] properties = typeof(emphr).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (String.Compare(property.Name, language, StringComparison.Ordinal) == 0)
                        {
                            theInfo.empname = property.GetValue(theEmphr).ToString();
                            result.Add(theInfo);
                            break;
                        }
                    }

                }
                return true;
            });

            return result;
        }


        public EmployeeMsg GetMessage(int empid, int messageid, string language)
        {
            EmployeeMsg msg = GetMessageById(empid, messageid, language.Length == 0 ? "english" : language);
            //msg.Sender = new EmployeeMsgInfo(empid);

            emphr theEmphr = db.emphrs.FirstOrDefault(k => k.empid == msg.senderempid);
            if (theEmphr != null)
            {
                msg.Sender.empid = theEmphr.empid;
                msg.Sender.empcode = theEmphr.empcode;

                if (language.Length == 0)
                {
                    User theUser = db.users.FirstOrDefault(k => k.empid == empid);
                    if (theUser != null)
                    {
                        languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                        if (theLanguage != null)
                        {
                            language = theLanguage.name;
                        }
                    }
                }
                PropertyInfo[] properties = typeof(emphr).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name.CompareTo(language) == 0)
                    {
                        msg.Sender.empname = property.GetValue(theEmphr).ToString();
                    }
                }
            }
            return msg;
        }
        public List<RosterInfo> GetRosterByEmpId(int empid, string language)
        {
            if (language.Length == 0)
            {
                User theUser = db.users.FirstOrDefault(e => e.empid == empid);
                if (theUser != null)
                {
                    languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                    if (theLanguage != null)
                    {
                        language = theLanguage.name;
                    }
                }
            }


            string GetRosterByEmpIdSql = $@"
                DECLARE @days INT
                DECLARE @row_count INT
                DECLARE @row_date DATETIME
                DECLARE @now_date DATETIME
                SET @days = 13  --获取2周排版天数
                SET @row_count = @days  
                SET @now_date = CONVERT(DATETIME,CONVERT(NVARCHAR(10),GETDATE(),112))
                --set @now_date = '2015-01-01 00:00:00'


                --创建临时表
                CREATE TABLE #days (
		                sequence		INT,
		                empid			INT,
		                auto_date		DATETIME,
		                weekend_days	FLOAT NOT NULL DEFAULT(0),			--值为0，表示当天不是周末，或者被换成工作日或节假日
		                holiday_days	FLOAT NOT NULL DEFAULT(0),			--值为0，表示当天不是法定节假日，或者被换成节假日或周末
		                ph_days			FLOAT NOT NULL DEFAULT(0),			--值为0，表示当天不是公众节假日，或者被换成节假日或周末
		                leave_code		NVARCHAR(200),
		                leave_name		NVARCHAR(500)
                );

                --获取每天日期
                INSERT INTO	#days(empid, sequence, auto_date)
                SELECT
                    {empid},
                    number,
                    CONVERT(VARCHAR(10),DATEADD(DAY, number, @now_date),112)
                FROM  master..spt_values
                WHERE [TYPE] = 'P' AND number <= DATEDIFF(DAY, @now_date, DATEADD(DAY,@days,@now_date));

                --法定假
                UPDATE a 
                SET		a.holiday_days = 1
                FROM	#days a
                WHERE	dbo.ufn_ats_GetDateType({empid},NULL,NULL,a.auto_date) = 2;

                --公众假
                UPDATE	a
                SET		a.ph_days = 1
                FROM	#days a
                WHERE	a.holiday_days = 0 AND dbo.ufn_ats_GetDateType({empid},NULL,NULL,a.auto_date) = 3;

                --周末休息
                UPDATE	a
                SET		a.weekend_days = 1
                FROM	#days a
                WHERE	a.holiday_days = 0 AND a.ph_days = 0 AND dbo.ufn_ats_GetDateType({empid},NULL,NULL,a.auto_date) = 1;

                SELECT 
                    a.workdate,
                    b.{language} roster_name,
                    c.locationcode location_code,
                    c.{language} location_name,
                    d.workstarthour,
                    d.nextdayws,
                    d.workendhour,
                    d.nextdaywe,
                    e.{language} job_name
                INTO #temp_workdate
                FROM ats_locationrosterdetail a 
                INNER JOIN ats_roster b ON a.rostercode=b.rostercode 
                INNER JOIN ats_locations c ON a.locationcode=c.locationcode
                INNER JOIN ats_rosterbasic d ON a.rostercode=d.rostercode AND d.rowno = 1
                LEFT JOIN ats_locationjobs e ON c.locationcode = e.locationcode AND a.jobcode = e.jobcode
                WHERE a.empid = {empid} 
                AND a.workdate BETWEEN @now_date AND DATEADD(DAY,@days,@now_date)
                ORDER BY a.workdate,d.workstarthour;

                SELECT 
                    a.empid,
	                (CASE WHEN a.holiday_days = 1 THEN 'SH' ELSE '' END) AS holiday,
	                (CASE WHEN a.ph_days = 1 THEN 'RO' ELSE '' END) AS phday,
	                (CASE WHEN a.weekend_days = 1 THEN 'RO' ELSE '' END) AS weekendday,
	                ISNULL(a.leave_code,'') AS leave_code,
                    ISNULL(a.leave_name,'') AS leave_name,
	                ISNULL(b.workdate,a.auto_date) AS workdate,
                    ISNULL(b.roster_name,'') AS roster_name,
                    ISNULL(b.location_code,'') AS location_code,
                    ISNULL(b.location_name,'') AS location_name,
                    ISNULL(b.workstarthour,'') AS workstarthour,
                    ISNULL(b.nextdayws,0) AS nextdayws,
                    ISNULL(b.workendhour,'') AS workendhour,
                    ISNULL(b.nextdaywe,0) AS nextdaywe,
                    ISNULL(b.job_name,'') AS job_name
                INTO #temp_days    
                FROM #days a LEFT JOIN #temp_workdate b ON a.auto_date = b.workdate;

                --获取休假记录
                SELECT 
	                a.empid, 
	                a.leavefromdate,
	                a.leavetodate,
	                a.leavecode AS leave_code,
	                b.{language} AS leave_name
                INTO #temp_empleavedate
                FROM empleavedata a 
	                INNER JOIN leavetype b ON a.leavecode = b.leavecode
                WHERE a.empid = {empid} AND a.leavefromdate >= @now_date AND a.leavetodate <= DATEADD(DAY,@days,@now_date);

                WHILE(@row_count > 0)
                BEGIN
	                SET @row_date = DATEADD(DAY,@row_count,@now_date)
	
	                UPDATE a SET a.leave_code = b.leave_code,a.leave_name = b.leave_name 
	                FROM #temp_days a 
		                INNER JOIN #temp_empleavedate b ON a.empid = b.empid 
	                WHERE a.workdate = @row_date AND leavefromdate >= @row_date AND leavetodate <= @row_date

	                SET @row_count = @row_count - 1
                END

                SELECT * FROM #temp_days

                DROP TABLE #days;
                DROP TABLE #temp_days;
                DROP TABLE #temp_workdate;
                DROP TABLE #temp_empleavedate;";
            string sql = GetRosterByEmpIdSql;// String.Format(GetRosterByEmpIdSql, empid, language);
            List<RosterInfo> rosterInfo = db.rosterinfo.FromSql(sql).ToList();
            return rosterInfo;

        }
    }
}
