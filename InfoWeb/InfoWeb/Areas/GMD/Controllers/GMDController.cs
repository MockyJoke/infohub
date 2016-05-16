using InfoWeb.Areas.GMD.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InfoWeb.Areas.GMD.Controllers
{
    public class GMDController : Controller
    {
        List<GMDTalkShow> showList = GMDTalkShowUtility.Instance.ShowList;
        // GET: GMD/GMD
        public ActionResult Index()
        {
            return View(GMDTalkShowUtility.Instance.ShowList);
        }
    }
}