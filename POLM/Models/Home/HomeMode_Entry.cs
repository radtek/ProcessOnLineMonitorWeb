using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyModels.Common;
using System.Data;
using Newtonsoft.Json;
using POLM.Models.Report;
using POLM.Models.eMail;

namespace POLM.Models.Home
{
    public class HomeModel
    {
        public void ServiceModel_Home(MoViewBag viewBag)
        {
            string ErrMsg = "";
            try
            {
                HomeHelper m_HomeHelper = new HomeHelper();
                if (viewBag.Model.IndexOf("10") == 0)
                {
                    #region 10
                    if (viewBag.Model == "101_Cagegory")
                    {
                        m_HomeHelper.GetCategoryFromServer(viewBag, ref ErrMsg);
                    }
                    else if (viewBag.Model == "103_Items")
                    {
                        m_HomeHelper.GetCategory_Machine(viewBag, ref ErrMsg);
                    } 
                    #endregion
                }
                else if (viewBag.Model == "301_RealTime")
                {
                    #region 301_RealTime
                    DataTable dt = m_HomeHelper.GetRealTimeData(viewBag, ref ErrMsg);
                    viewBag.Message = ErrMsg;
                    if (dt != null)
                    {
                        viewBag.jsonOut = JsonConvert.SerializeObject(dt);
                        if (ErrMsg.IndexOf("Success") == 0)
                        {
                            viewBag.Message = "Success to get real time data";
                        }
                    }
                    #endregion
                }
                else if (viewBag.Model.IndexOf("50") == 0)
                {
                    #region for upload WI
                    UploadWiHelper m_UploadWiHelper = new UploadWiHelper();
                    m_UploadWiHelper.UserName = viewBag.User;
                    if (viewBag.Model == "501_xlsList")
                    {
                        #region 501_xlsList
                        DataTable dt = m_UploadWiHelper.GetWI_Excel_List(viewBag, ref ErrMsg);
                        viewBag.Message = ErrMsg;
                        if (dt != null)
                        {
                            viewBag.jsonOut = JsonConvert.SerializeObject(dt);
                            viewBag.Message = "Success to get WI list.";
                        }
                        else
                        {
                            viewBag.Message = "Fail to get WI list.";
                        }
                        viewBag.List_Track = m_UploadWiHelper.List_Track;
                        #endregion
                    }
                    else if (viewBag.Model == "503_xlsSheet")
                    {
                        #region 503_xlsCont
                        DataTable dt = m_UploadWiHelper.GetWI_Excel_Sheets(viewBag, ref ErrMsg);
                        viewBag.Message = ErrMsg;
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            viewBag.jsonOut = JsonConvert.SerializeObject(dt);
                            viewBag.Message = "Success to get WS list";
                        }
                        else
                        {
                            viewBag.Message = "Fail to get WS list";
                        }

                        #endregion
                    }
                    else if (viewBag.Model == "505_xlsCont")
                    {
                        #region 503_xlsCont
                        DataTable dt = m_UploadWiHelper.GetWI_Excel_Content(viewBag, ref ErrMsg);
                        viewBag.Message = ErrMsg;
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            viewBag.jsonOut = JsonConvert.SerializeObject(dt);
                            viewBag.Message = "Success to get WS content";
                        }

                        #endregion
                    }
                    else if(viewBag.Model == "507_xlsDel")
                    {
                        m_UploadWiHelper.Additional_DeleteXls(viewBag, ref ErrMsg);
                        viewBag.Message = ErrMsg;

                    }

                    #endregion
                }
                else if (viewBag.Model.IndexOf("60") == 0)
                {
                    #region Upload WorkI into database 一键更新
                    UploadWiHelper m_UploadWiHelper = new UploadWiHelper();
                    m_UploadWiHelper.UserName = viewBag.User;
                    if (viewBag.Model == "601_UpPara")  //this is one by one update
                    {
                        m_UploadWiHelper.Update_Para_Wave(viewBag, ref ErrMsg);
                    }
                    else if (viewBag.Model == "603_UpPara2") //all data update once
                    {
                        m_UploadWiHelper.Update_Para_Wave2(viewBag, ref ErrMsg);
                    }
                    else if(viewBag.Model == "605_UpPara3")  //直接更新excel表参数到数据库中, available,
                    {
                        #region 605_UpPara3
                        m_UploadWiHelper.Update_Para_Wave3(viewBag, ref ErrMsg);
                        viewBag.Message = ErrMsg;
                        viewBag.List_Track = m_UploadWiHelper.List_Track;// JsonConvert.SerializeObject(m_UploadWiHelper.List_Track)
                        if (m_UploadWiHelper.List_Error.Count > 0)
                        {
                            viewBag.Message = "There are errors occurred, please check tracking record";
                        } 
                        #endregion
                    }
                    #endregion
                }
                else if (viewBag.Model.IndexOf("70") == 0)
                {
                    #region Review parameters
                    ReviewWiHelper m_ReviewHelper = new ReviewWiHelper();
                    if (viewBag.Model == "701_Project")
                    {
                        #region 701_Project
                        List<string> List_Project = m_ReviewHelper.WI_Para_Project(viewBag, ref ErrMsg);
                        if (List_Project != null && List_Project.Count > 0)
                        {
                            viewBag.ParaRet1 = JsonConvert.SerializeObject(List_Project);
                            viewBag.Message = "Success to get Project.";
                        }
                        else
                        {
                            viewBag.Message = "Fail to get Project";
                        }
                        List<string> List_Line = m_ReviewHelper.WI_Para_Line(viewBag, ref ErrMsg);
                        if (List_Line != null && List_Line.Count > 0)
                        {
                            viewBag.ParaRet2 = JsonConvert.SerializeObject(List_Line);
                            viewBag.Message = "Success to get line.";
                        }
                        else
                        {
                            viewBag.Message = "Fail to get line";
                        }
                        #endregion
                    }
                    else if(viewBag.Model == "701_PojLine")
                    {
                        #region 701_PojLine
                        List<string> List_Line = m_ReviewHelper.WI_Para_Line_BaseProject(viewBag, ref ErrMsg);
                        if (List_Line != null && List_Line.Count > 0)
                        {
                            viewBag.ParaRet2 = JsonConvert.SerializeObject(List_Line);
                            viewBag.Message = "Success to get line list";
                        }
                        else
                        {
                            viewBag.Message = "Fail to get line";
                        }
                        #endregion
                    }
                    else if (viewBag.Model == "703_Paras")
                    {
                        #region 703_Paras
                        DataTable dt = m_ReviewHelper.GetReviewedData(viewBag, ref ErrMsg);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            viewBag.jsonOut = JsonConvert.SerializeObject(dt);
                        }
                        viewBag.Message = ErrMsg;
                        #endregion
                    }
                    else if(viewBag.Model == "705_TempWIUpdate")
                    {
                        #region 705_TempWIUpdate
                        m_ReviewHelper.WI_Tempory_Para_Update(viewBag, ref ErrMsg);
                        viewBag.Message = ErrMsg;
                        #endregion
                    }
                    #endregion
                }
                else if (viewBag.Model.IndexOf("80") == 0) {  //POLM 的report的功能
                    PolmRepHelper m_PolmRepHelper = new PolmRepHelper();
                    m_PolmRepHelper.GetReport_CellParas_Main(viewBag);
                    viewBag.List_Track = m_PolmRepHelper.List_Track;
                }
                else if(viewBag.Model.IndexOf("40") == 0)  //////email reminder
                {
                    #region email
                    myEmail m_myEmail = new eMail.myEmail();
                    m_myEmail.SendMailToOwner(viewBag, ref ErrMsg);
                    viewBag.Message = ErrMsg;
                    viewBag.List_Track = m_myEmail.List_Track;
                    #endregion
                }
                else if(viewBag.Model.IndexOf("90") == 0)  //POLM 的 conifg
                {

                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            finally
            {
                #region tracking
                string sTrackAll = String.Join("\n", viewBag.List_Track.ToArray());
                string SN = viewBag.Model;
                string sRemark = viewBag.Message;
                if (sRemark.Length > 50)
                    sRemark = sRemark.Substring(0, 49);
                DataBaseOperation.PublishMSMQ_Tracking(SN, "", viewBag.Title, "", viewBag.User, "POLM", sRemark, DateTime.Now.ToString(), sTrackAll);
                #endregion
            }
        }

    }
}