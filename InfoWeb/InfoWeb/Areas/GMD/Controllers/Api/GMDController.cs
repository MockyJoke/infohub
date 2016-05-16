using InfoWeb.Areas.GMD.Models;
using InfoWeb.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace InfoWeb.Areas.GMD.Controllers.Api
{
    public class GMDController : ApiController
    {
        // GET: api/GMD
        //[Route("GMD/api/GMD/Get/{id:int}")]
        public IEnumerable<GMDTalkShow> Get()
        {
            return this.SmartWebReturn(GMDTalkShowUtility.Instance.ShowList);
        }
    }
}
