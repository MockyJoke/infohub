using HtmlAgilityPack;
using OnBoardService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnBoardService.Controllers
{
    public class GMDController : ApiController
    {
        // GET: api/GMD
        public IEnumerable<GMDTalkShow> Get()
        {
            return this.SmartWebReturn(GMDTalkShowUtility.Instance.ShowList);
        }

        // GET: api/GMD/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/GMD
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/GMD/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GMD/5
        public void Delete(int id)
        {
        }

        
    }
}
