﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<emphr> Get()
        {
            using (var db = new HRMSDBContext())
            {                
                var emphrs = db.emphrs
                    .Where(b => b.empid > 0)
                    .OrderBy(b => b.empcode)
                    .ToList();
                //var json = JsonConvert.SerializeObject(emphrs);
                return emphrs;
            }
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
