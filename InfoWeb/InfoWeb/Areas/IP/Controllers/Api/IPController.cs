using InfoWeb.Models;
using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace InfoWeb.Areas.IP.Controllers.Api
{
    public class IPController : ApiController
    {
        // GET: api/ip
        //[Route("ip/api/ip/Get/{id:int}")]
        public string Get(string machine = "", string type = "", string timeStamp = "")
        {
            string clientIp = GetClientIp();
            TelemetryClient telemetry = new TelemetryClient();
            Dictionary<string, string> propDict = new Dictionary<string, string>() {
                { "ip" , clientIp },
                { "machine" , machine },
                { "type", type},
                { "timeStamp", timeStamp}
            };
            telemetry.TrackEvent("IP_Get", propDict, null);
            return this.SmartWebReturn(clientIp);
        }
        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
