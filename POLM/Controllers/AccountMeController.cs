using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyModels.Common;
using MyModels.Account;

namespace POLM.Controllers
{
    public class AccountMeController : Controller
    {

        public ActionResult Index()
        {
            string ErrMsg = "";
            ViewBagAcnt viewBag = new ViewBagAcnt();
            viewBag.Title = "Log in";

            MoAccnt m_MoAccnt = new MoAccnt();
            m_MoAccnt.GetLoginProject(viewBag, ref ErrMsg);

            return View("Login", viewBag);
        }

        public ActionResult login(string Para)
        {
            string ErrMsg = "";
            ViewBagAcnt viewBag = new ViewBagAcnt();
            viewBag.Title = "Log in";
            MoAccnt m_MoAccnt = new MoAccnt();
            viewBag.Project = Para;
            m_MoAccnt.GetLoginUsers(viewBag, ref ErrMsg);

            return PartialView("Login_data", viewBag);
        }

        public ActionResult UserPwConfirm(ViewBagAcnt viewBag)
        {
            string ErrMsg = "";
            MoAccnt m_MoAccnt = new MoAccnt();
            m_MoAccnt.ConfirmUserPw(viewBag, ref ErrMsg);
            viewBag.Message = ErrMsg;
            return Json(viewBag.Message, JsonRequestBehavior.AllowGet);
        }



        ///////////////////////////////////////////////////////////////////////////
        public ActionResult LoginData(ViewBagAcnt viewBag)
        {
            string ErrMsg = "";
            MoAccnt m_MoAccnt = new MoAccnt();
            if (viewBag.Model == "login_Project")
                m_MoAccnt.GetLoginProject(viewBag, ref ErrMsg);
            else if (viewBag.Model == "login_User")
                m_MoAccnt.GetLoginUsers(viewBag, ref ErrMsg);

            return Json(viewBag, JsonRequestBehavior.AllowGet);
        }


        ////////////////////////////////////////////////////////////////////user maintenance

        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult UserOperation(ViewBagAcnt viewBag)
        {
            string ErrMsg = "";
            return PartialView("UserOperation", viewBag);
        }

        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult UserOperation_Data(ViewBagAcnt viewBag)
        {
            string ErrMsg = "";
            MoAccnt m_MoAccnt = new MoAccnt();
            m_MoAccnt.GetLoginUsersAll(viewBag, ref ErrMsg);
            return Json(viewBag, JsonRequestBehavior.AllowGet);

            //return Json(new { moBag.Result, moBag.jsonOut }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult UserOperation_Action(ViewBagAcnt viewBag)      ///////////////////////插入更新用户
        {
            string ErrMsg = "";
            MoAccnt m_MoAccnt = new MoAccnt();
            m_MoAccnt.UserOperation_Action(viewBag, ref ErrMsg);
            viewBag.Message = ErrMsg;
            return Json(viewBag, JsonRequestBehavior.AllowGet);
        }

	}
}