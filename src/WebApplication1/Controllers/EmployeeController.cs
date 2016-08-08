using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Threading.Tasks;


using Newtonsoft.Json;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Utilities;

namespace WebApplication1.Controllers
{
    



    //public class MyDateTimeConvertor : DateTimeConverterBase
    //{
    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        return DateTime.Parse(reader.Value.ToString());

    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        string dateformat = "yyyy-MM-dd";
    //        int empid = EmployeeController.currentempid;
    //        if (EmployeeController.theUsersData != null)
    //        {
    //            User theUser = EmployeeController.theUsersData.FirstOrDefault(k => k.empid == empid);

    //            if (theUser != null)
    //            {
    //                dateformat = theUser.datemask;
    //            }

    //        }
    //        writer.WriteValue(((DateTime)value).ToString(dateformat));
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    //public class UseDateFormatterAttribute : Attribute, IControllerConfiguration
    //{
    //    public void Initialize(HttpControllerSettings controllerSettings,
    //        HttpControllerDescriptor descriptor)
    //    {
    //        JsonMediaTypeFormatter jsonFormatter = controllerSettings.Formatters.JsonFormatter;
    //        JsonSerializerSettings jSettings = new Newtonsoft.Json.JsonSerializerSettings()
    //        {
    //            Formatting = Formatting.Indented,
    //            DateTimeZoneHandling = DateTimeZoneHandling.Utc
    //        };
           
    //        jSettings.Converters.Add(new MyDateTimeConvertor());
    //        jsonFormatter.SerializerSettings = jSettings;



    //        var formatter = new JsonMediaTypeFormatter();
    //        controllerSettings.Formatters.Remove(formatter);

    //        var json = formatter.SerializerSettings;
    //        json.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;

    //        controllerSettings.Formatters.Add(formatter);
    //    }
    //}


    //[UseDateFormatter]
    //[System.Web.Mvc.RoutePrefix("employee")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmployeeController : Controller
    {
        public static List<User> theUsersData = null;
        public static int currentempid = 0;


        private HRMSDBContext db = new HRMSDBContext();

        [HttpGet("{id}")]
        public EmployeeData GetEmployee(int id = 0)
        {
            int empid = id;
            string language = "english";
            User theUser = null;

            var identity = new ClaimsIdentity();// (ClaimsIdentity)RequestContext.Principal.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim theClaim = claims.FirstOrDefault();
            if (theClaim != null)
            {
                string username = theClaim.Value;
                theUser = db.users.FirstOrDefault(b => b.usercode.ToLower() == username.ToLower());
            }
            else
            {
                theUser = db.users.FirstOrDefault(b => b.empid == id);
            }


            if (id <= 0)
            {
                if (theUser != null)
                {
                    empid = theUser.empid;
                }
                else
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    //response.RequestMessage = Request;
                    //return ResponseMessage(response);
                }
            }


            if (theUsersData==null)
            {
                theUsersData = new List<User>();
            }
            if (theUser != null)
            {
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                //response.RequestMessage = Request;
                //return ResponseMessage(response);
                User tmpUser = theUsersData.FirstOrDefault(k => k.empid == empid);
                if (tmpUser != null)
                {
                    int index = theUsersData.FindIndex(user => user.userid == theUser.userid);
                    theUsersData.RemoveAt(index);
                }
                theUsersData.Add(theUser);
                languages theLanguage = db.languages.FirstOrDefault(k => k.id == theUser.language);
                if (theLanguage != null)
                {
                    language = theLanguage.name;
                }
            }


            EmployeeUtils theUtils = new EmployeeUtils();
            EmployeeData theData = new EmployeeData();
            emphr theEmphr = db.emphrs.FirstOrDefault(b => b.empid == empid);
            if (theEmphr == null)
            {
                string msg = "Employee detail has not been configured. Please contact system administrator";
                errorcode theErrorcode = db.errorcodes.FirstOrDefault(k => String.Compare(k.code, "NoEmployee",
                                StringComparison.Ordinal) == 0);
                if (theErrorcode != null)
                {
                    PropertyInfo[] properties1 = typeof(errorcode).GetProperties();
                    foreach (PropertyInfo property1 in properties1)
                    {
                        if (property1.Name.CompareTo(language) == 0)
                        {
                            msg = property1.GetValue(theErrorcode).ToString();
                        }
                    }
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent(@"{""error"":""" + msg + @"""}");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //response.RequestMessage = Request;
                return null;
            }
            

            
            theData.RosterList = theUtils.GetRosterByEmpId(theEmphr.empid, language);
            theData.AnnualLeave = theUtils.GetAnnualLeaveByEmpId(theEmphr.empid);

            theData.msgcount = theUtils.GetMessageCountByEmpId(theEmphr.empid);

            empposition theEmpposition = db.empposition.Where(k => k.empid == theEmphr.empid).FirstOrDefault();
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
                        PropertyInfo theProperty = Array.Find(properties2, p => p.Name.CompareTo(property1.Name) == 0);
                        if (theProperty != null)
                        {
                            var value = theProperty.GetValue(thePosition);
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


                    theData.Position = thePositionDTO;
                }
            }
            //var json = new JavaScriptSerializer().Serialize(theEmphr);
            //File.WriteAllText( @"e:\\JSON.txt", json);
            
            theData.SetData(theEmphr, language);
            theData.SetUser(theUser);
            //theData.SetSubordinates();
            JsonSerializerSettings jSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            //jSettings.Converters.Add(new MyDateTimeConvertor());
            currentempid = (int) empid;
            var items = JsonConvert.SerializeObject(theData, jSettings);
            JObject json = JObject.Parse(items);
            items = items.Replace("null", "{}");
            json = JObject.Parse(items);
            return theData;
        }
    }
}
