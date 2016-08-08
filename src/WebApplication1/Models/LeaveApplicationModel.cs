using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WebApplication1.Models
{
    public class LeaveDay
    {
        private readonly DateTime _leaveStartTimeAM;
        private readonly DateTime _leaveEndTimeAM;
        private readonly DateTime _leaveStartTimePM;
        private readonly DateTime _leaveEndTimePM;

        public LeaveDay() { }

        public LeaveDay(DateTime leaveStartTimeAM, DateTime leaveEndTimeAM, DateTime leaveStartTimePM, DateTime leaveEndTimePM)
        {
            _leaveStartTimeAM = leaveStartTimeAM;
            _leaveEndTimeAM = leaveEndTimeAM;
            _leaveStartTimePM = leaveStartTimePM;
            _leaveEndTimePM = leaveEndTimePM;
        }

        public double CalcLeaveHours()
        {
            double startAM = GetHourWithMin(_leaveStartTimeAM),
            endAM = GetHourWithMin(_leaveEndTimeAM),
            startPM = GetHourWithMin(_leaveStartTimePM),
            endPM = GetHourWithMin(_leaveEndTimePM);
            return (endAM - startAM) + (endPM - startPM);
        }

        private double GetHourWithMin(DateTime dt)
        {
            return dt.Hour + dt.Minute / 60.0;
        }
    }
    class SplitLeaveHours
    {
        private WorkingDayProperty _workingDayProperty;
        private readonly bool _applyToEachDay;
        private readonly DateTime _beginDate;
        private readonly DateTime _endDate;
        private IList<DateTime> _excludeDays;

        public SplitLeaveHours() { }

        public SplitLeaveHours(bool applyToEachDay, DateTime beginDate, DateTime endDate,
        WorkingDayProperty workingDayProperty, IList<DateTime> excludeDays)
        {
            _applyToEachDay = applyToEachDay;
            _beginDate = beginDate;
            _endDate = endDate;
            _workingDayProperty = workingDayProperty;
            _excludeDays = excludeDays;
        }

        internal List<LeaveDay> Split()
        {
            List<LeaveDay> leaveDays = new List<LeaveDay>();

            if (UnSetWorkTime())
            {
                return leaveDays;
            }

            if (_applyToEachDay)
            {
                if (GetHourWithMin(_endDate) >= GetHourWithMin(_beginDate))
                {
                    GetLeaveDaysEachDayNor(leaveDays);
                }
                else
                {
                    GetLeaveDaysEachDaySpe(leaveDays);
                }
                return leaveDays;
            }

            if (SameDay(_beginDate, _endDate))
            {
                if (!isExcludedDay(_beginDate))
                {
                    leaveDays.Add(GetLeaveOneDay(_beginDate, _endDate));
                }
                return leaveDays;
            }

            GetLeaveDays(leaveDays);
            return leaveDays;
        }

        private void GetLeaveDays(IList<LeaveDay> leaveDays)
        {
            if (!isExcludedDay(_beginDate))
            {
                leaveDays.Add(GetLeaveOneDay(_beginDate, _workingDayProperty.pmendtime));
            }
            if (!isExcludedDay(_endDate))
            {
                leaveDays.Add(GetLeaveOneDay(_workingDayProperty.ambegintime, _endDate));
            }
            int middleDays = MiddleDays();
            if (middleDays > 0)
            {
                DateTime middleDay = _beginDate.AddDays(1);
                for (int i = 0; i < middleDays; i++)
                {
                    if (isExcludedDay(middleDay))
                    {
                        middleDay = middleDay.AddDays(1);
                        continue;
                    }
                    leaveDays.Add(GetMiddleDay());
                    middleDay = middleDay.AddDays(1);
                }
            }
        }

        private void GetLeaveDaysEachDaySpe(IList<LeaveDay> leaveDays)
        {
            if (!isExcludedDay(_beginDate))
            {
                leaveDays.Add(GetLeaveOneDay(_beginDate, _workingDayProperty.pmendtime));
            }
            int mDays = MiddleDays();
            if (mDays > 0)
            {
                DateTime middleDay = _beginDate.AddDays(1);
                for (int i = 0; i < mDays; i++)
                {
                    if (isExcludedDay(middleDay))
                    {
                        middleDay = middleDay.AddDays(1);
                        continue;
                    }
                    leaveDays.Add(GetLeaveOneDay(_workingDayProperty.ambegintime, _endDate));
                    leaveDays.Add(GetLeaveOneDay(_beginDate, _workingDayProperty.pmendtime));
                    middleDay = middleDay.AddDays(1);
                }
            }
            if (!isExcludedDay(_endDate))
            {
                leaveDays.Add(GetLeaveOneDay(_workingDayProperty.ambegintime, _endDate));
            }
        }

        private void GetLeaveDaysEachDayNor(IList<LeaveDay> leaveDays)
        {
            int allDaysCnt = _endDate.Subtract(_beginDate).Days + 1;
            if (allDaysCnt > 0)
            {
                DateTime everyDay = _beginDate;
                for (int i = 0; i < allDaysCnt; i++)
                {
                    if (isExcludedDay(everyDay))
                    {
                        everyDay = everyDay.AddDays(1);
                        continue;
                    }
                    leaveDays.Add(GetLeaveOneDay(_beginDate, _endDate));
                    everyDay = everyDay.AddDays(1);
                }
            }
        }

        private int MiddleDays()
        {
            if (GetHourWithMin(_endDate) < GetHourWithMin(_beginDate))
            {
                return _endDate.Subtract(_beginDate).Days;
            }
            else
            {
                return _endDate.Subtract(_beginDate).Days - 1;
            }
        }

        private bool isExcludedDay(DateTime day)
        {
            DateTime tempDay = _excludeDays.FirstOrDefault(d => SameDay(d, day));
            return tempDay.Year == 1 ? false : true;
        }

        private bool SameDay(DateTime day1, DateTime day2)
        {
            return day1.Day == day2.Day && day1.Month == day2.Month && day1.Year == day2.Year;
        }

        private LeaveDay GetMiddleDay()
        {
            double startAM = 0, endAM = 0, startPM = 0, endPM = 0;
            GetDoubleWorkTime(ref startAM, ref endAM, ref startPM, ref endPM);
            return new LeaveDay(_workingDayProperty.ambegintime, _workingDayProperty.amendtime
                , _workingDayProperty.pmbegintime, _workingDayProperty.pmendtime);
        }

        private LeaveDay GetLeaveOneDay(DateTime fromTime, DateTime toTime)
        {
            DateTime bankDate = new DateTime();

            double beginTime = GetHourWithMin(fromTime);
            double endTime = GetHourWithMin(toTime);

            if (beginTime >= endTime)
                return new LeaveDay(bankDate, bankDate, bankDate, bankDate);

            DateTime realStartAM = bankDate, realEndAM = bankDate, realStartPM = bankDate, realEndPM = bankDate;
            double startAM = 0, endAM = 0, startPM = 0, endPM = 0;
            GetDoubleWorkTime(ref startAM, ref endAM, ref startPM, ref endPM);

            if (beginTime < endAM)
            {
                realStartAM = beginTime > startAM ? fromTime : _workingDayProperty.ambegintime;
                realEndAM = endTime < endAM ? toTime : _workingDayProperty.amendtime;
            }

            if (endTime > startPM)
            {
                realStartPM = beginTime > startPM ? fromTime : _workingDayProperty.pmbegintime;
                realEndPM = endTime < endPM ? toTime : _workingDayProperty.pmendtime;
            }

            return new LeaveDay(realStartAM, realEndAM, realStartPM, realEndPM);
        }

        private double GetHourWithMin(DateTime dt)
        {
            return dt.Hour + dt.Minute / 60.0;
        }

        private void GetDoubleWorkTime(ref double startAM, ref double endAM, ref double startPM, ref double endPM)
        {
            startAM = GetHourWithMin(_workingDayProperty.ambegintime);
            endAM = GetHourWithMin(_workingDayProperty.amendtime);
            startPM = GetHourWithMin(_workingDayProperty.pmbegintime);
            endPM = GetHourWithMin(_workingDayProperty.pmendtime);
        }

        private bool UnSetWorkTime()
        {
            double startAM = 0, endAM = 0, startPM = 0, endPM = 0;
            GetDoubleWorkTime(ref startAM, ref endAM, ref startPM, ref endPM);
            return (startAM == 0 && endAM == 0 && startPM == 0 && endPM == 0) ? true : false;
        }
    }
    public class TextInfo
    {
        private string[] Texts;
    }



    public class LeaveCalendarService
    {
        private HRMSDBContext db = new HRMSDBContext();
        public LeaveCalendarService()
        {

        }

        public List<DateTime> GetExcludedDays(int employeeId, string leaveCode, DateTime beginDate, DateTime endDate)
        {
            const string command = @"EXEC dbo.usp_get_excluded_days_of_leave_period @empid={0},
                @leaveCode=N'{1}',@beginDate=N'{2}',@endDate=N'{3}'";


            string sql = string.Format(command, employeeId, leaveCode,
                beginDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            List<DateTime> excludedDays = new List<DateTime>();
            List<ExcludeDays> theExcludeDays = null;
            try
            {
                theExcludeDays = db.excludedays.FromSql(sql).ToList();
                if (theExcludeDays.Count > 0)
                {
                    excludedDays = new List<DateTime>();
                }
                foreach (var VARIABLE in theExcludeDays)
                {
                    excludedDays.Add(VARIABLE.date);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //if (DbTemplate.DBCompany == null)
            //{
            //    throw new NullReferenceException("Cannot open database");
            //}

            //DbTemplate.SQL.Append("usp_get_excluded_days_of_leave_period");
            //DbTemplate.AddParameter("@empid", DataType.Int, employeeId);
            //DbTemplate.AddParameter("@leaveCode", DataType.NVarChar, 200, ParameterDirection.Input, leaveCode);
            //DbTemplate.AddParameter("@beginDate", DataType.DateTime, beginDate);
            //DbTemplate.AddParameter("@endDate", DataType.DateTime, endDate);

            //DataTable excludedDays = DbTemplate.GetDataTableSP();

            //DbTemplate.Dispose();

            //return excludedDays.Select().Select(s => DbTemplate.DB2DateTime(s["date"])).ToList();
            return excludedDays;
        }
    }


    public class CompanyInformationService
    {
        private HRMSDBContext db = new HRMSDBContext();
        public CompanyInformationService()
        {

        }

        private const string SqlForWorkingDayTimeDefinition =
            @"SELECT ambegintime, amendtime, pmbegintime, pmendtime FROM control";
        public WorkingDayProperty GetWorkingDayTimeDefinition()
        {
            string sql = string.Format(SqlForWorkingDayTimeDefinition);
            WorkingDayProperty result = null;
            try
            {
                result = db.workingdayproperty.FromSql(sql).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            //if (DbTemplate.DBCompany == null)
            //{
            //    throw new NullReferenceException("Cannot open database");
            //}

            //DbTemplate.SQL.Append(SqlForWorkingDayTimeDefinition);

            //WorkingDayProperty workingDayProperty = DbTemplate.QueryForObject(new WorkingDayTimeDefinitionRowMapper<WorkingDayProperty>());
            //DbTemplate.Dispose();

            return result;// workingDayProperty;
        }
    }


    public class LeaveTypeService
    {
        private HRMSDBContext db = new HRMSDBContext();
        public LeaveTypeService()
        {
        }

        private const string SqlForGetAllMaternityLeaveCodes = "SELECT leavecode FROM leavetype WHERE category = '03'";
        public List<string> GetAllMaternityLeaveCodes()
        {
            string sql = string.Format(SqlForGetAllMaternityLeaveCodes);
            List<string> result = null;
            try
            {
                result = db.stringresult.FromSql(sql).Select(x=>x.result).ToList();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            
            
            //if (DbTemplate.DBCompany == null)
            //{
            //    throw new NullReferenceException("Cannot open database");
            //}

            //DbTemplate.SQL.Append(SqlForGetAllMaternityLeaveCodes);

            //DataTable dt = DbTemplate.GetDataTable();

            //DbTemplate.Dispose();

            //if (dt == null)
            //{
            //    return new List<string>();
            //}

            //IList<string> allCodes = dt.Select().Select(row => DbTemplate.DB2String(row["leavecode"])).ToList();

            return result;// allCodes;
        }

        private static string SqlForGetLeaveType = @"
SELECT
    leavecode
	,payrollgroupid
	,english as [name]
	,category
	,exclude_holiday
	,exclude_holiday_not_apply_to
	,exclude_weekend
	,exclude_weekend_not_apply_to
	,leaveapppolicy
    ,apply_to_each_day
FROM
    leavetype
WHERE
    leavecode = '{0}'";

        public LeaveTypeModel GetLeaveType(string leaveCode)
        {
            string sql = string.Format(SqlForGetLeaveType, leaveCode);
            LeaveTypeModel result = null;
            try
            {
                result = db.leavetypemodel.FromSql(sql).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            //if (DbTemplate.DBCompany == null)
            //{
            //    throw new NullReferenceException("Cannot open database");
            //}

            //DbTemplate.SQL.Append(string.Format(SqlForGetLeaveType, DbTemplate.TextFieldName));
            //DbTemplate.AddParameter("@leaveCode", DataType.NVarChar, 200, ParameterDirection.Input, leaveCode);
            //LeaveTypeModel leaveType = DbTemplate.QueryForObject(new LeaveTypeRowMapper<LeaveTypeModel>());

            //DbTemplate.Dispose();

            return result;// leaveType;
        }
    }

    public class PayrollGroup
    {

        private string SQL_SELECTONE = "SELECT * FROM payrollgroups WHERE payrollgroupid={0}";
        private HRMSDBContext db = new HRMSDBContext();
        public PayrollGroup()
        {
        }
        public PayrollGroupInfo Get(int payrollgroupid)
        {
            
            PayrollGroupInfo objInfo = null;
            //if (base.DBCompany != null)
            //{
            //    base.SQL.Append(SQL_SELECTONE);
            //    base.AddParameter("@payrollgroupid", DataType.Int, payrollgroupid);
            //    DataTable dt = base.GetDataTable();
            //    if (base.IsSuccess && dt.Rows.Count > 0)
            //    {
            //        objInfo = AssemblyModel(dt.Rows[0]);
            //    }
            //}
            //base.Dispose();
            string sql = string.Format(SQL_SELECTONE, payrollgroupid);
            try
            {
                objInfo = db.paygroupinfo.FromSql(sql).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return objInfo;
        }
    }

    public class EmployeeMaintenance
    {
        private HRMSDBContext db = new HRMSDBContext();
        private static string SqlForGetEmployee = @"
SELECT
                empid as EmployeeId, 
                empcode as EmployeeCode, 
                english as Name, 
                cemail as CompanyEmail, 
                pemail as PersonalEmail, 
                payrollgroupid
            FROM
                emphr
            WHERE
                empid = {0}";

        public EmployeeBasicInfo GetEmployee(int employeeId)
        {
            string sql = string.Format(SqlForGetEmployee, employeeId);
            EmployeeBasicInfo1 info = null;
            try
            {
                info = db.employeebasicinfo1.FromSql(sql).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }


            //            if (base.DBCompany == null)
            //            {
            //                throw new NullReferenceException("Cannot open database");
            //            }

            //            base.SQL.AppendFormat(@"
            //SELECT
            //    empid, 
            //    empcode, 
            //    {0} empname, 
            //    cemail, 
            //    pemail, 
            //    payrollgroupid
            //FROM
            //    emphr
            //WHERE
            //    empid = @empid", base.EmpNameFieldName(string.Empty));

            //            base.AddParameter("@empid", DataType.Int, employeeId);

            //            DataTable dt = base.GetDataTable();
            //            base.Dispose();

            //            if (dt == null)
            //            {
            //                _log.Debug("Datatable is null");
            //                return null;
            //            }

            //            if (dt.Rows.Count != 1)
            //            {
            //                _log.Debug("Row count in datatable is " + dt.Rows.Count);
            //                return null;
            //            }

            var employee = new EmployeeBasicInfo();
            employee.EmployeeId = info.EmployeeId;
            employee.EmployeeCode = info.EmployeeCode;
            employee.Name = info.Name;
            employee.CompanyEmail = info.CompanyEmail;
            employee.PersonalEmail = info.PersonalEmail;

            PayrollGroup payrollGroup = new PayrollGroup();
            employee.PayrollGroup = payrollGroup.Get(info.payrollgroupid);

            return employee;
        }
    }

    public class LeaveApplicationModel
    {
        private EmployeeBasicInfo _employee;
        private LeaveTypeModel _leaveType;
        private readonly int _employeeId;
        private readonly string _leaveCode;
        private DateTime _beginDate;
        private DateTime _endDate;
        private WorkingDayProperty _workingDayProperty;
        private IList<DateTime> _excludeDays;

        public EmployeeMaintenance EmployeeService { get; set; }
        public LeaveTypeService LeaveTypeService { get; set; }
        public CompanyInformationService CompanyInformationService { get; set; }
        public LeaveCalendarService LeaveCalendarService { get; set; }

        public LeaveApplicationModel(int employeeId, string leaveCode, DateTime beginDate, DateTime endDate)
        {
            _employeeId = employeeId;
            _leaveCode = leaveCode;
            _beginDate = beginDate;
            _endDate = endDate;
        }

        public double CalcLeaveHours()
        {
            if (EmployeeService == null)
            {
                EmployeeService = new EmployeeMaintenance();
            }

            if (LeaveTypeService == null)
            {
                LeaveTypeService = new LeaveTypeService();
            }

            if (CompanyInformationService == null)
            {
                CompanyInformationService = new CompanyInformationService();
            }

            if (LeaveCalendarService == null)
            {
                LeaveCalendarService = new LeaveCalendarService();
            }

            if (string.IsNullOrEmpty(_leaveCode))
            {
                return 0;
            }

            _employee = EmployeeService.GetEmployee(_employeeId);

            if (_employee == null)
            {
                return 0;
            }

            _leaveType = LeaveTypeService.GetLeaveType(_leaveCode);
            _workingDayProperty = CompanyInformationService.GetWorkingDayTimeDefinition();
            _excludeDays = LeaveCalendarService.GetExcludedDays(_employeeId, _leaveCode, _beginDate, _endDate);

            double value = 0;
            try
            {
                value = Calc();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return value;
        }

        public EmployeeBasicInfo Employee
        {
            get { return _employee; }
        }

        public LeaveTypeModel LeaveType
        {
            get { return _leaveType; }
        }

        public DateTime BeginDate
        {
            set { _beginDate = value; }
            get { return _beginDate; }
        }

        public DateTime EndDate
        {
            set { _endDate = value; }
            get { return _endDate; }
        }

        public IList<DateTime> ExcludeDays
        {
            get { return _excludeDays; }
        }

        private double Calc()
        {            
            if (_endDate.Subtract(_beginDate).TotalDays < 0)
            {
                return 0;
            }
            SplitLeaveHours splitHours = new SplitLeaveHours(_leaveType.ApplyToEachDay, _beginDate, _endDate,
                _workingDayProperty, _excludeDays);

            IList<LeaveDay> calcDays = splitHours.Split();

            double sum = GetPrecision(calcDays.Sum(s => s.CalcLeaveHours()));
            return sum;
        }

        private double GetPrecision(double value)
        {
            int integerPart = (int)value;
            double decimalPart = value - integerPart;
            double dot5Part = 0;
            if (decimalPart == 0)
            {
                dot5Part = 0;
            }
            else if (decimalPart > 0 && decimalPart <= 0.5)
            {
                dot5Part = 0.5;
            }
            else if (decimalPart > 0.5 && decimalPart < 1)
            {
                dot5Part = 1;
            }
            return integerPart + dot5Part;
        }
    }
}
