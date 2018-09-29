using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POLM.Models.Home;
using MyModels.Common;

namespace POLM.Controllers
{
    public class PolmRepController : Controller
    {
        // GET: CompareParas
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult ActionOperation(MoViewBag viewBag)
        {
            HomeModel m_HomeModel = new HomeModel();
            m_HomeModel.ServiceModel_Home(viewBag);
            return Json(viewBag, JsonRequestBehavior.AllowGet);
        }



    }
}