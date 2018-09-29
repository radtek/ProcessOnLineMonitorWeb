using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyModels.Common;
using POLM.Models.Home;

namespace POLM.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult RealData()
        {
            ViewBag.Message = "View Real Data";

            return View();
        }


        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult ActionOperation(MoViewBag viewBag)
        {
            HomeModel m_HomeModel = new HomeModel();
            m_HomeModel.ServiceModel_Home(viewBag);
            return Json(viewBag, JsonRequestBehavior.AllowGet);
        }


        /////////////////////////////////////////////////////////////////
    }
}