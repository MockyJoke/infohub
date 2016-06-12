using InfoWeb.Areas.IP.Models;
using InfoWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InfoWeb.Areas.Etc.Controllers.Api
{
    public class WolController : ApiController
    {
        // GET: api/wol
        //[Route("etc/api/wol/Get/{id:int}")]
        public int Get(string machine = "", string set = "")
        {
            if (string.IsNullOrEmpty(machine))
            {
                return 0;
            }
            if (set == "1")
            {
                WolManager.Instance.RenewMachine(machine);
            }else if (set == "0")
            {
                WolManager.Instance.RemoveMachine(machine);
            }
            return this.SmartWebReturn(WolManager.Instance.IsTargetValid(machine) ? 1 : 0);
        }
    }
}
