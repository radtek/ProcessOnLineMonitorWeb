using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MyModels.Common;
using POLM.Models.Home;

namespace POLM.Controllers
{
    public class PolmConfigController : Controller
    {
        // GET: PolmConfig
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Index(MoViewBag viewBag)
        {
            HomeModel m_HomeModel = new HomeModel();
            m_HomeModel.ServiceModel_Home(viewBag);

            return View(viewBag);
        }



    }
}