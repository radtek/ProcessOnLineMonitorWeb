using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyModels.Common;
using System.IO;
using System.Net;
using POLM.Models.Home;

namespace POLM.Controllers
{
    public class UploadWIController : Controller
    {
        // GET: UploadWI
        public ActionResult Index()
        {
            return View();
        }

        // GET: WIReview
        public ActionResult WIReview()
        {
            return View();
        }


        [HttpPost]
        public ActionResult uploadFiles()
        {
            string ErrMsg = "";
            MoViewBag m_MoViewBag = new MoViewBag();
            try
            {
                m_MoViewBag.List_Track.Add("Call uploadFiles @" + DateTime.Now.ToString());
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;

                        #region and optionally write the file to disk
                        var fileName = fileContent.FileName;// Path.GetFileName(file);
                        m_MoViewBag.List_Track.Add("--upload file: " + fileName);
                        m_MoViewBag.List_Track.Add("--Create folder on server: ");
                        #region create folder
                        m_MoViewBag.List_Track.Add("----1. Create App_Data folder on server ");
                        string appData = Server.MapPath("~/App_Data");
                        Directory.CreateDirectory(appData);
                        m_MoViewBag.List_Track.Add("----2. Create UploadFiles folder on server ");
                        string UploadFiles = Server.MapPath("~/App_Data/UploadFiles");
                        Directory.CreateDirectory(UploadFiles);
                        #endregion


                        var path = Path.Combine(Server.MapPath("~/App_Data/UploadFiles/"), fileName);

                        m_MoViewBag.List_Track.Add("--server map path: " + Server.MapPath("~/App_Data/UploadFiles/"));

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);

                            m_MoViewBag.List_Track.Add("--Success to copy file to server: " + path);
                        }

                        if (System.IO.File.Exists(path))
                        {
                            string Para1 = Request["para1"].ToString();
                            //string Para2 = Request["para2"].ToString();
                            string LocalFoder = "";
                            if (Para1 == "Wave") //根据设备不同，上传的文件放在相应的文件夹
                            {
                                //@"D:\JianMing\Project\2018_Application\ProcessOnLineMonitor\ExcelUpload\Wave\";
                                LocalFoder = DbConfigHelper.GetConfigPara(CfgPara.Wave_Up_Path);

                                m_MoViewBag.List_Track.Add("--Get wave saved folder from config: " + LocalFoder);
                            }
                            if (LocalFoder != "")
                            {
                                if(Directory.Exists(LocalFoder))
                                {
                                    m_MoViewBag.List_Track.Add("--Success to wave excel saved folder: " + LocalFoder);

                                    #region copy and move
                                    m_MoViewBag.List_Track.Add("----Success to get folder" + LocalFoder);

                                string LocalFile = LocalFoder + fileName;
                                if (System.IO.File.Exists(LocalFile))
                                {
                                    System.IO.File.Delete(LocalFile);
                                    System.Threading.Thread.Sleep(200);
                                }
                                System.IO.File.Move(path, LocalFile);
                                m_MoViewBag.List_Track.Add("----Success to move fiel to server=>" + LocalFile);

                                System.Threading.Thread.Sleep(200);
                                if (System.IO.File.Exists(LocalFile))
                                {
                                    ErrMsg = "Success to upload file " + fileName;
                                } 
                                else
                                {
                                    ErrMsg = "Fail to upload file " + fileName;
                                    m_MoViewBag.List_Track.Add("----" + ErrMsg);
                                }
                                    #endregion

                                }
                                else
                                {
                                    m_MoViewBag.List_Track.Add("--Fail to wave excel saved folder: " + LocalFoder);
                                }
                            }
                            else
                            {
                                ErrMsg = "Fail to get local folder";

                                m_MoViewBag.List_Track.Add("----" + ErrMsg);
                            }
                        }
                        else
                        {
                            m_MoViewBag.List_Track.Add("--Fail to copy file to server: " + path);
                        }
                        #endregion

                        m_MoViewBag.Message = ErrMsg;

                        return Json(m_MoViewBag, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        m_MoViewBag.List_Track.Add("Fail to get file.");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                m_MoViewBag.List_Track.Add(ex.Message + Environment.NewLine + ex.StackTrace);
                m_MoViewBag.Message = ErrMsg;
                //return Json(m_MoViewBag, JsonRequestBehavior.AllowGet);
                //return Json("Upload failed");
            }
            return Json(m_MoViewBag, JsonRequestBehavior.AllowGet);
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