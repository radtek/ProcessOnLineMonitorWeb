using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MyModels.Common;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

using SignalR.MessageHub;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace POLM.Models.Home
{
    public class UploadWiHelper
    {
        string Global_xlsLocalFolder = @"D:\JianMing\Project\2018_Application\ProcessOnLineMonitor\ExcelUpload\Wave\";
        public List<string> List_Track = new List<string>();
        public List<string> List_Error = new List<string>();

        #region SingalR -- Publish message to client
        private IHubConnectionContext<dynamic> Clients { get; set; }

        public string PcName { get; set; }

        public string UserName { get; set; }

        private readonly object _updateStockPricesLock = new object();

        private void BroadcastMyMessage(string title, string msg)
        {
            try
            {
                lock (_updateStockPricesLock)
                {
                    MoViewBag viewBag = new MoViewBag();
                    viewBag.Title = title;
                    viewBag.Message = msg;

                    if (UserName == "")
                    {
                        //Clients.All.publishMsg(viewBag);
                    }
                    else
                    {
                        // 发送给自己消息
                        //Clients.Groups(new List<string> { "GROUP-" + PcName }).publishMsg(viewBag);
                        Clients.Group(UserName).publishMsg(viewBag);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public UploadWiHelper()
        {
            try
            {
                Clients = GlobalHost.ConnectionManager.GetHubContext<MessageHubSingle>().Clients;
                PcName = "";
                UserName = "";
            }
            catch (Exception)
            {
            }
        }

        public UploadWiHelper(string para)
        {
            try
            {
                Clients = GlobalHost.ConnectionManager.GetHubContext<MessageHubSingle>().Clients;
                PcName = para;
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region for read excel list

        public DataTable GetWI_Excel_List(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                List_Track.Add("Call GetWI_Excel_List v1.1 @" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                string Machine = "";
                if (viewBag.Para1 == "Wave")
                {
                    Global_xlsLocalFolder = DbConfigHelper.GetConfigPara(CfgPara.Wave_Up_Path);
                    Machine = "Wave";

                    List_Track.Add("--Machine = " + Machine + " , 文件夹= " + Global_xlsLocalFolder);
                }

                List<string> List_Project = DbConfigHelper.GetProjectList();
                List_Track.Add("--Project list = " + String.Join(",", List_Project.ToArray()));


                DataTable dt = CreateNewDataTabletForXlsList();

                if (Directory.Exists(Global_xlsLocalFolder))
                {
                    string[] arryFiles = Directory.GetFiles(Global_xlsLocalFolder, "*.xls*", SearchOption.TopDirectoryOnly);

                    #region tracking
                    BroadcastMyMessage("XlsList", "Get xls list Q=" + arryFiles.Length);
                    List_Track.Add("--获取文件清单 = " + arryFiles.Length);
                    #endregion

                    #region dafdsf
                    for (int i = 0; i < arryFiles.Length; i++)
                    {
                        string filePath = arryFiles[i];
                        string sFileFull = Path.GetFileName(arryFiles[i]);
                        string sFileExt = Path.GetExtension(sFileFull);

                        #region tracking
                        BroadcastMyMessage("XlsList", "Get xls file =" + sFileFull);
                        List_Track.Add("----获取文件 = " + sFileFull);
                        #endregion

                        string ProjectCurrent = "";

                        if (sFileFull.IndexOf("-") > 0)
                        {
                            if (Regex.IsMatch(sFileFull, @"[-]+\s*"))
                            {
                                string ProjAll = Regex.Split(sFileFull, @"[-]+\s*")[1].Trim();
                                ProjectCurrent = ProjAll.Split(' ')[0].Trim().ToUpper();

                                //Match mstr = Regex.Match(sFileFull, @"[-]+\s*");
                                //if (mstr.Groups.Count > 0)
                                //{
                                //    string ProAll = mstr.Groups[0].ToString().Trim();
                                //}
                                if(!List_Project.Contains(ProjectCurrent))
                                {
                                    ProjectCurrent = "";
                                }
                            }
                            List_Track.Add("----项目名 = " + ProjectCurrent);
                        }
                        else
                        {
                            List_Track.Add("----文件名没有标志符： - ，无法获取项目名" );
                        }

                        xlsSc_Title m_xlsTit = new Home.xlsSc_Title();
                        string xlsMsg = "";
                        UploadWi_Excel m_UploadWi_Excel = new Home.UploadWi_Excel();
                        List_Track.Add("  ");
                        if (sFileExt == ".xlsx")
                        {
                            List_Track.Add("----Call WI_GetOverPage_xlsx");

                            xlsMsg = m_UploadWi_Excel.WI_GetOverPage_xlsx(filePath, m_xlsTit);   //key for xlsx
                        }
                        else if (sFileExt == ".xls")
                        {
                            List_Track.Add("----Call WI_GetOverPage");

                            xlsMsg = m_UploadWi_Excel.WI_GetOverPage(filePath, m_xlsTit);     //key for xls
                        }
                        List_Track.AddRange(m_UploadWi_Excel.List_Track);

                        FileInfo fi = new FileInfo(arryFiles[i]);
                        string sLengthText = (fi.Length / 1024).ToString() + "kb";

                        string dateTime = m_xlsTit.Etq_EffDate;  // fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

                        if (m_xlsTit != null)
                        {
                            DataRow dr = dt.NewRow();
                            dr["File"] = Path.GetFileName(sFileFull);
                            dr["Category"] = Machine;
                            dr["Doc"] = m_xlsTit.Etq_Doc;
                            dr["Rev"] = m_xlsTit.Etq_Rev;
                            dr["Remark"] = Path.GetFileName(sFileFull) + " " + xlsMsg;
                            dr["DateTime"] = dateTime;
                            dr["View"] = "read";
                            dr["Update"] = "update";

                            dr["Project"] = ProjectCurrent;

                            dt.Rows.Add(dr);
                        }
                    }
                    #endregion

                    #region adsfas
                    if (dt.Rows.Count > 0)
                    {
                        var res = from b in dt.AsEnumerable()
                                  orderby b.Field<string>("DateTime") descending
                                  select b;
                        DataTable dtRes = res.CopyToDataTable();

                        return dtRes;
                    }
                    else
                    {
                        ErrMsg = "fail,没有找到一个.xls 文件在文件夹中," + Global_xlsLocalFolder;
                        BroadcastMyMessage("XlsList", ErrMsg);

                        List_Track.Add("----" + ErrMsg);
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "fail,没有找到文件夹：" + Global_xlsLocalFolder;
                    BroadcastMyMessage("XlsList", ErrMsg);
                    List_Track.Add("----" + ErrMsg);
                }

            }
            catch (Exception ex)
            {
                #region tracking
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
                List_Error.Add(ErrMsg); 
                #endregion
            }
            return null;
        }

        public string GetLine_FromFileName(string sFileName)
        {
            string ErrMsg = "";
            try
            {
                //machine Wave_Lin e3_ELE500_RoHS
                if (Regex.IsMatch(sFileName, @"_L[ine\s]*[0-9A-B]+_", RegexOptions.IgnoreCase))
                {
                    //Match mstr = Regex.Match(sFileName, @"_L[0-9A-B]+_");
                    //if (mstr.Groups.Count > 0)
                    //{
                    //    string sLine = mstr.Groups[0].ToString().Trim();

                    //}
                    string sLine = Regex.Match(sFileName, @"_L[ine\s]*[0-9A-B]+_").Value;
                    if(sLine.Length > 0)
                    {
                        sLine = sLine.Replace("_", "").ToUpper();
                        sLine = sLine.Replace("L", "").Trim();
                        string sLineNum = Regex.Match(sLine, @"\d+").Value;
                        if (sLineNum.Length > 0)
                        {
                            //int nLine = Convert.ToInt16()
                            #region to deal with Line3B 这种格式的线别
                            string[] sLineSubFixArry = Regex.Split(sLine, sLineNum);
                            if (sLineSubFixArry.Length > 1)
                            {
                                string subFix = sLineSubFixArry[1].Trim();
                                if (subFix != "")
                                {
                                    string sLineNumABC = sLineNum + subFix;
                                    return sLineNumABC;
                                }
                            } 
                            #endregion
                            return sLineNum;
                        }
                        sLine.Replace(" ", "");
                        sLine.Replace("INE", "");
                        return sLine;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        public DataTable CreateNewDataTabletForXlsList()
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "xlsFiles";
                DataColumn column = new DataColumn("File", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Category", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Doc", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Rev", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Remark", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("DateTime", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("View", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Update", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Project", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                #endregion

                return m_DT;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        #endregion

        #region fore read excel work sheet
        public DataTable GetWI_Excel_Sheets(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region get excel content
                string sCategory = viewBag.Para1;
                string sFileName = viewBag.Para2;

                if (viewBag.Para1 == "Wave")
                {
                    Global_xlsLocalFolder = DbConfigHelper.GetConfigPara(CfgPara.Wave_Up_Path);
                }

                DataTable dt = CreateNewDataTable_Sheets();

                if (Directory.Exists(Global_xlsLocalFolder))
                {
                    #region read excel file
                    string xlsFile = Global_xlsLocalFolder + sFileName;
                    if(File.Exists(xlsFile))
                    {
                        #region exists file
                        List<string> List_WS = new UploadWi_Excel().WI_GetContent_SheetList(xlsFile, ref ErrMsg);
                        foreach(string WorkSheet in List_WS)
                        {
                            string LineCurrent = GetLine_FromFileName(WorkSheet);

                            DataRow dr = dt.NewRow();
                            dr["File"] = sFileName;
                            dr["Sheet"] = WorkSheet;
                            dr["Line"] = LineCurrent;
                            dr["Remark"] = "";
                            dr["View"] = "read";
                            dt.Rows.Add(dr);
                        }
                        #endregion
                    }
                    else
                    {
                        ErrMsg = "fail to get file：" + xlsFile;
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "fail,没有找到文件夹：" + Global_xlsLocalFolder;
                }
                return dt;
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }
        public DataTable CreateNewDataTable_Sheets()
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "WorkSheets";
                DataColumn column = new DataColumn("File", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Sheet", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Line", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Remark", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("View", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                #endregion

                return m_DT;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        #endregion

        #region for read excel sheet content
        public DataTable GetWI_Excel_Content(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region get excel content
                string sCategory = viewBag.Para1;
                string sFileName = viewBag.Para2;
                string WorkSheet = viewBag.Para3;
                string Line = viewBag.Para4;
                //string WorkSheetLineAll = viewBag.Para3;
                //if(WorkSheetLineAll.IndexOf("|") < 0)
                //{
                //    ErrMsg = "Fail to get WS name and Line";
                //    return null;
                //}
                //string WorkSheet = WorkSheetLineAll.Split('|')[0];
                //string Line = WorkSheetLineAll.Split('|')[1];

                if (viewBag.Para1 == "Wave")
                {
                    Global_xlsLocalFolder = DbConfigHelper.GetConfigPara(CfgPara.Wave_Up_Path);
                }

                //DataTable dt = CreateNewDataTable_Sheets();

                if (Directory.Exists(Global_xlsLocalFolder))
                {
                    #region read excel file
                    string xlsFile = Global_xlsLocalFolder + sFileName;
                    if (File.Exists(xlsFile))
                    {
                        #region exists file
                        //List<string> List_WS = new UploadWi_Excel().WI_GetContent_SheetList(xlsFile);
                        DataTable dt_T = new UploadWi_Excel().WI_GetContent_Wave(xlsFile, WorkSheet,ref ErrMsg);
                        return dt_T;
                        #endregion
                    }
                    else
                    {
                        ErrMsg = "fail to get file：" + xlsFile;
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "fail,没有找到文件夹：" + Global_xlsLocalFolder;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        #endregion


        #region upload parameter into server
        public void Update_Para_Wave(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region parameter
                bool bIsChecked = Convert.ToBoolean(viewBag.Para9);
                if (bIsChecked)
                {
                    string RowIndex = viewBag.Para1;
                    string McType = viewBag.Para2;
                    string Project = viewBag.Para3;
                    string FileName = viewBag.Para4;
                    string SheetName = viewBag.Para5.Split('|')[0];
                    string Line = viewBag.Para5.Split('|')[1];
                    if (Line == null)
                        Line = "";
                    string DocNum = viewBag.Para6;
                    string DocRev = viewBag.Para7;
                    string Remark = "";

                    dynamic objJson = JsonConvert.DeserializeObject(viewBag.Para8);
                    #region get row data
                    string MsgOut = "";
                    List<string> List_Track = new List<string>();

                    OracleParameter[] param = new OracleParameter[9];
                    param[0] = new OracleParameter("sModel", "Wave_Para_OV");
                    param[1] = new OracleParameter("p_Para1", McType);
                    param[2] = new OracleParameter("p_Para2", Project);
                    param[3] = new OracleParameter("p_Para3", DocNum);
                    param[4] = new OracleParameter("p_Para4", DocRev);
                    param[5] = new OracleParameter("p_Para5", Line);
                    param[6] = new OracleParameter("p_Para6", FileName);
                    param[7] = new OracleParameter("p_Para7", SheetName);
                    param[8] = new OracleParameter("p_Para8", Remark);

                    MyOracleSql.Oracle_Sql.StoreProcedure_Paras("P_POLM", param, ref MsgOut);

                    if (MsgOut.IndexOf("Success") == 0)
                    {
                        string IID = MsgOut.Split('=')[1].Trim();

                        string ModelName = objJson.ModelName;
                        string ProgName = objJson.ProgName;
                        string ErrMsg_Update_All = "Success to update Para for Row=" + RowIndex;
                        for (int i = 0; i < 20; i++)
                        {
                            #region update
                            string Para_Name = "";
                            string Para_Cen = "";
                            string Para_Max = "";
                            string Para_Min = "";
                            string Para_Remark = "";
                            switch (i)
                            {
                                #region CASE to get parameter
                                case 0:
                                    #region parameter
                                    Para_Name = "Flux_BdWid";
                                    Para_Cen = objJson.Flux_BdWid;
                                    Para_Max = objJson.Flux_BdWid_Max;
                                    Para_Min = objJson.Flux_BdWid_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 1:
                                    #region Flux_ConvSpd
                                    Para_Name = "Flux_ConvSpd";
                                    Para_Cen = objJson.Flux_ConvSpd;
                                    Para_Max = objJson.Flux_ConvSpd_Max;
                                    Para_Min = objJson.Flux_ConvSpd_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 2:
                                    #region Flux_NozSpd
                                    Para_Name = "Flux_NozSpd";
                                    Para_Cen = objJson.Flux_NozSpd;
                                    Para_Max = objJson.Flux_NozSpd_Max;
                                    Para_Min = objJson.Flux_NozSpd_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 3:
                                    #region Flux_Volumn
                                    Para_Name = "Flux_Volumn";
                                    Para_Cen = objJson.Flux_Volumn;
                                    Para_Max = objJson.Flux_Volumn_Max;
                                    Para_Min = objJson.Flux_Volumn_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 4:
                                    #region Flux_Volumn
                                    Para_Name = "Flux_NozSpray";
                                    Para_Cen = objJson.Flux_NozSpray;
                                    Para_Max = objJson.Flux_NozSpray_Max;
                                    Para_Min = objJson.Flux_NozSpray_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 5:
                                    #region Flux_Power
                                    Para_Name = "Flux_Power";
                                    Para_Cen = objJson.Flux_Power;
                                    Para_Max = objJson.Flux_Power_Max;
                                    Para_Min = objJson.Flux_Power_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 6:
                                    #region Flux_Pres
                                    Para_Name = "Flux_Pres";
                                    Para_Cen = objJson.Flux_Pres;
                                    Para_Max = objJson.Flux_Pres_Max;
                                    Para_Min = objJson.Flux_Pres_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 7:
                                    #region Flux_Pres
                                    Para_Name = "Flux_BiModel";
                                    Para_Cen = objJson.Flux_BiModel;
                                    Para_Max = "";
                                    Para_Min = "";
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 8:
                                    #region Heat_Low1
                                    Para_Name = "Heat_Low1";
                                    Para_Cen = objJson.Heat_Low1;
                                    Para_Max = objJson.Heat_Low1_Max;
                                    Para_Min = objJson.Heat_Low1_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 9:
                                    #region Heat_Low2
                                    Para_Name = "Heat_Low2";
                                    Para_Cen = objJson.Heat_Low2;
                                    Para_Max = objJson.Heat_Low2_Max;
                                    Para_Min = objJson.Heat_Low2_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 10:
                                    #region Heat_Low3
                                    Para_Name = "Heat_Low3";
                                    Para_Cen = objJson.Heat_Low3;
                                    Para_Max = objJson.Heat_Low3_Max;
                                    Para_Min = objJson.Heat_Low3_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 11:
                                    #region Heat_Upp1
                                    Para_Name = "Heat_Upp1";
                                    Para_Cen = objJson.Heat_Upp1;
                                    Para_Max = objJson.Heat_Upp1_Max;
                                    Para_Min = objJson.Heat_Upp1_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 12:
                                    #region Heat_Upp2
                                    Para_Name = "Heat_Upp2";
                                    Para_Cen = objJson.Heat_Upp2;
                                    Para_Max = objJson.Heat_Upp2_Max;
                                    Para_Min = objJson.Heat_Upp2_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 13:
                                    #region Heat_Upp3
                                    Para_Name = "Heat_Upp3";
                                    Para_Cen = objJson.Heat_Upp3;
                                    Para_Max = objJson.Heat_Upp3_Max;
                                    Para_Min = objJson.Heat_Upp3_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 14:
                                    #region SP_Temp
                                    Para_Name = "SP_Temp";
                                    Para_Cen = objJson.SP_Temp;
                                    Para_Max = objJson.SP_Temp_Max;
                                    Para_Min = objJson.SP_Temp_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 15:
                                    #region SP_ConWave
                                    Para_Name = "SP_ConWave";
                                    Para_Cen = objJson.SP_ConWave;
                                    Para_Max = objJson.SP_ConWave_Max;
                                    Para_Min = objJson.SP_ConWave_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 16:
                                    #region SP_LdClear
                                    Para_Name = "SP_LdClear";
                                    Para_Cen = objJson.SP_LdClear;
                                    Para_Max = objJson.SP_LdClear_Max;
                                    Para_Min = objJson.SP_LdClear_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 17:
                                    #region Conv_Speed
                                    Para_Name = "Conv_Speed";
                                    Para_Cen = objJson.Conv_Speed;
                                    Para_Max = objJson.Conv_Speed_Max;
                                    Para_Min = objJson.Conv_Speed_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 18:
                                    #region Conv_Width
                                    Para_Name = "Conv_Width";
                                    Para_Cen = objJson.Conv_Width;
                                    Para_Max = objJson.Conv_Width_Max;
                                    Para_Min = objJson.Conv_Width_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 19:
                                    #region Other_Ni
                                    Para_Name = "Other_Ni";
                                    Para_Cen = objJson.Other_Ni;
                                    Para_Max = "";
                                    Para_Min = "";
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 20:
                                    #region Remark  //不用上传 Remark
                                    Para_Name = "Remark";
                                    Para_Cen = objJson.Remark;
                                    Para_Max = "";
                                    Para_Min = "";
                                    Para_Remark = "";
                                    #endregion
                                    break;
                               #endregion
                            }
                            string sResult = Update_Para_Wave_Action(IID, ModelName, ProgName, Para_Name, Para_Cen, Para_Max, Para_Min, Para_Remark);
                            List_Track.Add(Para_Name + "|" + sResult);
                            if (sResult.IndexOf("Success") != 0)
                            {
                                ErrMsg_Update_All = sResult;
                                break;
                            }
                            #endregion
                        }

                        viewBag.Message = ErrMsg_Update_All;
                    }
                    else
                    {
                        viewBag.Message = MsgOut;
                    }
                    #endregion
                }
                else
                {
                    viewBag.Message = "Success,No need update this row";
                }
                #endregion
                #region dddd
                // param[9] = new OracleParameter("p_Para6", objJson.ModelName);  
                //param[10] = new OracleParameter("p_Para7", objJson.ProgramName); 
                //param[11] = new OracleParameter("p_Para8", objJson.Flux_BdWid);  //0
                //param[11] = new OracleParameter("p_Para9", objJson.Flux_BdWid_Max);
                //param[11] = new OracleParameter("p_Para10", objJson.Flux_BdWid_Min);
                //param[11] = new OracleParameter("p_Para11", objJson.Flux_ConvSpd);  //1
                //param[11] = new OracleParameter("p_Para12", objJson.Flux_ConvSpd_Max);
                //param[11] = new OracleParameter("p_Para13", objJson.Flux_ConvSpd_Min);
                //param[11] = new OracleParameter("p_Para14", objJson.Flux_NozSpd); //2
                //param[11] = new OracleParameter("p_Para15", objJson.Flux_NozSpd_Max);
                //param[11] = new OracleParameter("p_Para16", objJson.Flux_NozSpd_Min);
                //param[11] = new OracleParameter("p_Para17", objJson.Flux_Volumn);
                //param[11] = new OracleParameter("p_Para18", objJson.Flux_Volumn_Max);
                //param[11] = new OracleParameter("p_Para19", objJson.Flux_Volumn_Min);
                //param[11] = new OracleParameter("p_Para20", objJson.Flux_NozSpray);
                //param[11] = new OracleParameter("p_Para21", objJson.Flux_NozSpray_Max);
                //param[11] = new OracleParameter("p_Para22", objJson.Flux_NozSpray_Min);
                //param[11] = new OracleParameter("p_Para23", objJson.Flux_Power);
                //param[11] = new OracleParameter("p_Para24", objJson.Flux_Power_Max);
                //param[11] = new OracleParameter("p_Para25", objJson.Flux_Power_Min);
                //param[11] = new OracleParameter("p_Para26", objJson.Flux_Pres);
                //param[11] = new OracleParameter("p_Para27", objJson.Flux_Pres_Max);
                //param[11] = new OracleParameter("p_Para28", objJson.Flux_Pres_Min);
                //param[11] = new OracleParameter("p_Para29", objJson.Flux_BiModel);
                //param[11] = new OracleParameter("p_Para30", objJson.Heat_Low1);
                //param[11] = new OracleParameter("p_Para31", objJson.Heat_Low1_Max);
                //param[11] = new OracleParameter("p_Para32", objJson.Heat_Low1_Min);
                //param[11] = new OracleParameter("p_Para33", objJson.Heat_Low2);
                //param[11] = new OracleParameter("p_Para34", objJson.Heat_Low2_Max);
                //param[11] = new OracleParameter("p_Para35", objJson.Heat_Low2_Min);
                //param[11] = new OracleParameter("p_Para36", objJson.Heat_Low3);
                //param[11] = new OracleParameter("p_Para37", objJson.Heat_Low3_Max);
                //param[11] = new OracleParameter("p_Para38", objJson.Heat_Low3_Min);
                //param[11] = new OracleParameter("p_Para39", objJson.Heat_Upp1);
                //param[11] = new OracleParameter("p_Para40", objJson.Heat_Upp1_Max);
                //param[11] = new OracleParameter("p_Para41", objJson.Heat_Upp1_Min);
                //param[11] = new OracleParameter("p_Para42", objJson.Heat_Upp2);
                //param[11] = new OracleParameter("p_Para43", objJson.Heat_Upp2_Max);
                //param[11] = new OracleParameter("p_Para44", objJson.Heat_Upp2_Min);
                //param[11] = new OracleParameter("p_Para45", objJson.Heat_Upp3);
                //param[11] = new OracleParameter("p_Para46", objJson.Heat_Upp3_Max);
                //param[11] = new OracleParameter("p_Para47", objJson.Heat_Upp3_Min);
                //param[11] = new OracleParameter("p_Para48", objJson.SP_Temp);
                //param[11] = new OracleParameter("p_Para49", objJson.SP_Temp_Max);
                //param[11] = new OracleParameter("p_Para50", objJson.SP_Temp_Min);
                //param[11] = new OracleParameter("p_Para51", objJson.SP_ConWave);
                //param[11] = new OracleParameter("p_Para52", objJson.SP_ConWave_Max);
                //param[11] = new OracleParameter("p_Para53", objJson.SP_ConWave_Min);
                //param[11] = new OracleParameter("p_Para54", objJson.SP_LdClear);
                //param[11] = new OracleParameter("p_Para55", objJson.SP_LdClear_Max);
                //param[11] = new OracleParameter("p_Para56", objJson.SP_LdClear_Min);
                //param[11] = new OracleParameter("p_Para57", objJson.Conv_Speed);
                //param[11] = new OracleParameter("p_Para58", objJson.Conv_Speed_Max);
                //param[11] = new OracleParameter("p_Para59", objJson.Conv_Speed_Min);
                //param[11] = new OracleParameter("p_Para60", objJson.Conv_Width);
                //param[11] = new OracleParameter("p_Para61", objJson.Conv_Width_Max);
                //param[11] = new OracleParameter("p_Para62", objJson.Conv_Width_Min);
                //param[11] = new OracleParameter("p_Para63", objJson.Other_Ni);
                #endregion


            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        public void Update_Para_Wave2(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region parameter
                string RowIndex = viewBag.Para1;
                string McType = viewBag.Para2;
                string Project = viewBag.Para3;
                string FileName = viewBag.Para4;
                string SheetName = viewBag.Para5.Split('|')[0];
                string Line = viewBag.Para5.Split('|')[1];
                if (Line == null)
                    Line = "";
                string DocNum = viewBag.Para6;
                string DocRev = viewBag.Para7;
                string Remark = "";

                dynamic objJsonAll = JsonConvert.DeserializeObject(viewBag.Para8);

                for (int j = 0; j < objJsonAll.Count; j++)
                {
                    dynamic objJson = objJsonAll[j];

                    #region get row data
                    string MsgOut = "";

                    BroadcastMyMessage("UpdatePara", "Update WS =" + SheetName);

                    List<string> List_Track = new List<string>();

                    OracleParameter[] param = new OracleParameter[9];
                    param[0] = new OracleParameter("sModel", "Wave_Para_OV");
                    param[1] = new OracleParameter("p_Para1", McType);
                    param[2] = new OracleParameter("p_Para2", Project);
                    param[3] = new OracleParameter("p_Para3", DocNum);
                    param[4] = new OracleParameter("p_Para4", DocRev);
                    param[5] = new OracleParameter("p_Para5", Line);
                    param[6] = new OracleParameter("p_Para6", FileName);
                    param[7] = new OracleParameter("p_Para7", SheetName);
                    param[8] = new OracleParameter("p_Para8", Remark);

                    MyOracleSql.Oracle_Sql.StoreProcedure_Paras("P_POLM", param, ref MsgOut);
                    BroadcastMyMessage("UpdatePara", "Update Worksheet OV result=" + MsgOut);

                    if (MsgOut.IndexOf("Success") == 0)
                    {
                        string IID = MsgOut.Split('=')[1].Trim();

                        string ModelName = objJson.ModelName;
                        string ProgName = objJson.ProgName;
                        string ErrMsg_Update_All = "Success to update Para for Row=" + j;

                        BroadcastMyMessage("UpdatePara", "update Para for Row=" + j + " ...");

                        for (int i = 0; i < 20; i++)
                        {
                            #region update
                            string Para_Name = "";
                            string Para_Cen = "";
                            string Para_Max = "";
                            string Para_Min = "";
                            string Para_Remark = "";
                            switch (i)
                            {
                                #region CASE to get parameter
                                case 0:
                                    #region parameter
                                    Para_Name = "Flux_BdWid";
                                    Para_Cen = objJson.Flux_BdWid;
                                    Para_Max = objJson.Flux_BdWid_Max;
                                    Para_Min = objJson.Flux_BdWid_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 1:
                                    #region Flux_ConvSpd
                                    Para_Name = "Flux_ConvSpd";
                                    Para_Cen = objJson.Flux_ConvSpd;
                                    Para_Max = objJson.Flux_ConvSpd_Max;
                                    Para_Min = objJson.Flux_ConvSpd_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 2:
                                    #region Flux_NozSpd
                                    Para_Name = "Flux_NozSpd";
                                    Para_Cen = objJson.Flux_NozSpd;
                                    Para_Max = objJson.Flux_NozSpd_Max;
                                    Para_Min = objJson.Flux_NozSpd_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 3:
                                    #region Flux_Volumn
                                    Para_Name = "Flux_Volumn";
                                    Para_Cen = objJson.Flux_Volumn;
                                    Para_Max = objJson.Flux_Volumn_Max;
                                    Para_Min = objJson.Flux_Volumn_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 4:
                                    #region Flux_NozSpray
                                    Para_Name = "Flux_NozSpray";
                                    Para_Cen = objJson.Flux_NozSpray;
                                    Para_Max = objJson.Flux_NozSpray_Max;
                                    Para_Min = objJson.Flux_NozSpray_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 5:
                                    #region Flux_Power
                                    Para_Name = "Flux_Power";
                                    Para_Cen = objJson.Flux_Power;
                                    Para_Max = objJson.Flux_Power_Max;
                                    Para_Min = objJson.Flux_Power_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 6:
                                    #region Flux_Pres
                                    Para_Name = "Flux_Pres";
                                    Para_Cen = objJson.Flux_Pres;
                                    Para_Max = objJson.Flux_Pres_Max;
                                    Para_Min = objJson.Flux_Pres_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 7:
                                    #region Flux_BiModel
                                    Para_Name = "Flux_BiModel";
                                    Para_Cen = objJson.Flux_BiModel;
                                    Para_Max = "";
                                    Para_Min = "";
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 8:
                                    #region Heat_Low1
                                    Para_Name = "Heat_Low1";
                                    Para_Cen = objJson.Heat_Low1;
                                    Para_Max = objJson.Heat_Low1_Max;
                                    Para_Min = objJson.Heat_Low1_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 9:
                                    #region Heat_Low2
                                    Para_Name = "Heat_Low2";
                                    Para_Cen = objJson.Heat_Low2;
                                    Para_Max = objJson.Heat_Low2_Max;
                                    Para_Min = objJson.Heat_Low2_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 10:
                                    #region Heat_Low3
                                    Para_Name = "Heat_Low3";
                                    Para_Cen = objJson.Heat_Low3;
                                    Para_Max = objJson.Heat_Low3_Max;
                                    Para_Min = objJson.Heat_Low3_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 11:
                                    #region Heat_Upp1
                                    Para_Name = "Heat_Upp1";
                                    Para_Cen = objJson.Heat_Upp1;
                                    Para_Max = objJson.Heat_Upp1_Max;
                                    Para_Min = objJson.Heat_Upp1_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 12:
                                    #region Heat_Upp2
                                    Para_Name = "Heat_Upp2";
                                    Para_Cen = objJson.Heat_Upp2;
                                    Para_Max = objJson.Heat_Upp2_Max;
                                    Para_Min = objJson.Heat_Upp2_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 13:
                                    #region Heat_Upp3
                                    Para_Name = "Heat_Upp3";
                                    Para_Cen = objJson.Heat_Upp3;
                                    Para_Max = objJson.Heat_Upp3_Max;
                                    Para_Min = objJson.Heat_Upp3_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 14:
                                    #region SP_Temp
                                    Para_Name = "SP_Temp";
                                    Para_Cen = objJson.SP_Temp;
                                    Para_Max = objJson.SP_Temp_Max;
                                    Para_Min = objJson.SP_Temp_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 15:
                                    #region SP_ConWave
                                    Para_Name = "SP_ConWave";
                                    Para_Cen = objJson.SP_ConWave;
                                    Para_Max = objJson.SP_ConWave_Max;
                                    Para_Min = objJson.SP_ConWave_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 16:
                                    #region SP_LdClear
                                    Para_Name = "SP_LdClear";
                                    Para_Cen = objJson.SP_LdClear;
                                    Para_Max = objJson.SP_LdClear_Max;
                                    Para_Min = objJson.SP_LdClear_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 17:
                                    #region Conv_Speed
                                    Para_Name = "Conv_Speed";
                                    Para_Cen = objJson.Conv_Speed;
                                    Para_Max = objJson.Conv_Speed_Max;
                                    Para_Min = objJson.Conv_Speed_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 18:
                                    #region Conv_Width
                                    Para_Name = "Conv_Width";
                                    Para_Cen = objJson.Conv_Width;
                                    Para_Max = objJson.Conv_Width_Max;
                                    Para_Min = objJson.Conv_Width_Min;
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 19:
                                    #region Other_Ni
                                    Para_Name = "Other_Ni";
                                    Para_Cen = objJson.Other_Ni;
                                    Para_Max = "";
                                    Para_Min = "";
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                case 20:
                                    #region Remark  //不用上传 Remark
                                    Para_Name = "Remark";
                                    Para_Cen = objJson.Remark;
                                    Para_Max = "";
                                    Para_Min = "";
                                    Para_Remark = "";
                                    #endregion
                                    break;
                                    #endregion
                            }
                            string sResult = Update_Para_Wave_Action(IID, ModelName, ProgName, Para_Name, Para_Cen, Para_Max, Para_Min, Para_Remark);
                            List_Track.Add(Para_Name + "|" + sResult);

                            //BroadcastMyMessage("UpdatePara", "Result=" + sResult + " for Row=" + j);

                            if (sResult.IndexOf("Success") != 0)
                            {
                                ErrMsg_Update_All = sResult;
                                break;
                            }
                            #endregion
                        }

                        BroadcastMyMessage("UpdatePara", ErrMsg_Update_All);

                        viewBag.Message = ErrMsg_Update_All;
                    }
                    else
                    {
                        viewBag.Message = MsgOut;
                    }
                    #endregion

                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        public string Update_Para_Wave_Action(string IID, string Model, string Program, string Par_Name, string Par_Cen, string Par_Max, string Par_Min, string Remark)
        {
            string ErrMsg = "";
            try
            {
                Program = Program.Replace(" ", "");
                string MsgOut_Para = "";
                OracleParameter[] paramD = new OracleParameter[9];
                paramD[0] = new OracleParameter("sModel", "Wave_Para_In");
                paramD[1] = new OracleParameter("p_Para1", IID);
                paramD[2] = new OracleParameter("p_Para2", Model);
                paramD[3] = new OracleParameter("p_Para3", Program);

                paramD[4] = new OracleParameter("p_Para4", Par_Name);
                paramD[5] = new OracleParameter("p_Para5", Par_Cen);
                paramD[6] = new OracleParameter("p_Para6", Par_Max);
                paramD[7] = new OracleParameter("p_Para7", Par_Min);
                paramD[8] = new OracleParameter("p_Para8", Remark);
                MyOracleSql.Oracle_Sql.StoreProcedure_Paras("P_POLM", paramD, ref MsgOut_Para);
                ErrMsg = MsgOut_Para;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return ErrMsg;
        }

        /// <summary>
        /// 一次性更新 excel的表到数据库中
        /// </summary>
        /// <param name="viewBag"></param>
        /// <param name="ErrMsg"></param>
        public void Update_Para_Wave3(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region get excel content
                string McType = viewBag.Para1;
                string sFileName = viewBag.Para2;
                string Project = viewBag.Project;
                string DocNum = viewBag.Para3;
                string DocRev = viewBag.Para4;
                string EffDate = viewBag.Para5;
                //DateTime dEffDate = Convert.ToDateTime(EffDate);

                List_Track.Add("--Process Update parameters to server v1.1");
                List_Track.Add("----File name = " + sFileName);

                if (viewBag.Para1 == "Wave")
                {
                    Global_xlsLocalFolder = DbConfigHelper.GetConfigPara(CfgPara.Wave_Up_Path);
                }

                if (Directory.Exists(Global_xlsLocalFolder))
                {
                    List_Track.Add("----Folder name= " + Global_xlsLocalFolder);

                    #region read excel file
                    string xlsFile = Global_xlsLocalFolder + sFileName;
                    if (File.Exists(xlsFile))
                    {
                        #region exists file
                        List_Track.Add("----Get File full name= " + xlsFile);

                        List<string> List_WS = new UploadWi_Excel().WI_GetContent_SheetList(xlsFile, ref ErrMsg);
                        List_Track.Add("----File work sheet length = " + List_WS.Count);
                        foreach (string WorkSheet in List_WS)
                        {
                            string msgNeed = "";
                            bool IsCanUpdate = IsNeedUpdateParameters(WorkSheet, ref msgNeed);
                            List_Track.Add("----Check if need update = " + msgNeed + " , for Sheet=" + WorkSheet);
                            if(IsCanUpdate)
                            {
                                #region update worksheet data into server
                                List_Track.Add("------------------------------------------Begin to for " + WorkSheet + "------------------------------------------------");
                                UploadWi_Excel m_UploadWi_Excel = new UploadWi_Excel();
                                DataTable dtSrc = m_UploadWi_Excel.WI_GetContent_Wave(xlsFile, WorkSheet, ref ErrMsg);
                                List_Track.Add("------get data source =" + ErrMsg);

                                string LineCurrent = GetLine_FromFileName(WorkSheet);
                                List_Track.Add("------get worksheet line =" + LineCurrent);

                                string Remark = ""; // "EffDay=" + EffDate;
                                string MsgOut = "";
                                OracleParameter[] param = new OracleParameter[10];
                                param[0] = new OracleParameter("sModel", "Wave_Para_OV_DelPar"); //插入OverView数据后删除所有的此ID的参数数据
                                param[1] = new OracleParameter("p_Para1", McType);
                                param[2] = new OracleParameter("p_Para2", Project);
                                param[3] = new OracleParameter("p_Para3", DocNum);
                                param[4] = new OracleParameter("p_Para4", DocRev);
                                param[5] = new OracleParameter("p_Para5", LineCurrent);
                                param[6] = new OracleParameter("p_Para6", sFileName);
                                param[7] = new OracleParameter("p_Para7", WorkSheet);
                                param[8] = new OracleParameter("p_Para8", Remark);
                                param[9] = new OracleParameter("p_Para9", EffDate);

                                MyOracleSql.Oracle_Sql.StoreProcedure_Paras("P_POLM", param, ref MsgOut);
                                if (MsgOut.IndexOf("Success") != 0)
                                    List_Error.Add(MsgOut);

                                List_Track.Add("------Update server overview = " + MsgOut);
                                BroadcastMyMessage("UpdatePara", "Update Worksheet OV result=" + MsgOut);

                                if (MsgOut.IndexOf("Success") == 0)
                                {
                                    string IID = MsgOut.Split('=')[1].Trim();
                                    List_Track.Add("------Update server overview ID = " + IID);

                                    BroadcastMyMessage("UpdatePara", "Get datatable to save in db ...");

                                    DataTable dtSrv = ConvertSrcDataToSrv(dtSrc, IID, ref ErrMsg);
                                    List_Track.Add("------Convert source data into updated server data = " + dtSrv.Rows.Count);
                                    if (ErrMsg.IndexOf("Success") != 0)
                                        List_Error.Add(ErrMsg);

                                    if (dtSrv != null && dtSrv.Rows.Count > 0)
                                    {
                                        BroadcastMyMessage("UpdatePara", "Saving datatable to db ...");

                                        string sResult = MyOracleSql.SqlExtension.BulkToDB(dtSrv, "POLM_WI_PARA");
                                        List_Track.Add("------Update server parameter result = " + sResult);

                                        BroadcastMyMessage("UpdatePara", sResult);
                                        ErrMsg = sResult;
                                        if (ErrMsg.IndexOf("Success") != 0)
                                            List_Error.Add(ErrMsg);
                                    }
                                }
                                else
                                {
                                    List_Track.Add("------Fail Update server overview");
                                }
                                List_Track.Add("                                  ");
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        ErrMsg = "fail to get file：" + xlsFile;
                        List_Track.Add("----Fail get file full name= " + xlsFile);
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "fail,没有找到文件夹：" + Global_xlsLocalFolder;
                    List_Track.Add("----Fail to get folder name= " + Global_xlsLocalFolder);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                List_Track.Add("----" + ErrMsg);
            }
            finally
            {
                #region Save each log file
                string sTrackingFile = AppDomain.CurrentDomain.BaseDirectory + "UpdateWI_" + DateTime.Now.ToString("HH-mm-ss") + ".txt";
                using (StreamWriter sw = new StreamWriter(sTrackingFile))
                {
                    for (int i = 0; i < List_Track.Count; i++)
                    {
                        sw.WriteLine(List_Track[i]);
                    }
                }
                POLM_CommonFun.BackupLogFile(sTrackingFile, "");
                #endregion
            }
        }


        #region Update_Para_Wave3 for additional function
        private bool IsNeedUpdateParameters(string WorkSheetName, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                if(Regex.IsMatch(WorkSheetName, @"[WRD]+\w*[_L]+\w*[_]+", RegexOptions.IgnoreCase))
                {
                    ErrMsg = @"true, Success for regex=[WRD]+\w*[_L]+\w*[_]+";
                    return true;
                }

                ErrMsg = @"false, Fail for regex=[WRD]+\w*[_L]+\w*[_]+";
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return false;
        }

        private DataTable CreateNewDataTable_ParaUpdate3()
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "POLM_WI_PARA";
                DataColumn column = new DataColumn("OV_ID", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("MODEL", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("PROGRAM", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("PARA_NAME", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("VAL_CEN", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("VAL_MAX", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("VAL_MIN", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("REMARK", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                #endregion

                return m_DT;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private DataTable ConvertSrcDataToSrv(DataTable dtSrc, string IID, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                DataTable dtServer = CreateNewDataTable_ParaUpdate3();
                foreach (DataRow dr in dtSrc.Rows)
                {
                    string modeName = Convert.ToString(dr["ModelName"]);
                    string progName = Convert.ToString(dr["ProgName"]);
                    progName = progName.Replace(" ", "");
                    for (int i = 0; i < 20; i++)
                    {
                        #region insert data
                        string Para_Name = "";
                        string Para_Cen = "";
                        string Para_Max = "";
                        string Para_Min = "";
                        string Para_Remark = "";
                        switch (i)
                        {
                            #region CASE to get parameter
                            case 0:
                                #region parameter
                                Para_Name = "Flux_BdWid";
                                Para_Cen = Convert.ToString(dr["Flux_BdWid"]); 
                                Para_Max = Convert.ToString(dr["Flux_BdWid_Max"]);
                                Para_Min = Convert.ToString(dr["Flux_BdWid_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 1:
                                #region Flux_ConvSpd
                                Para_Name = "Flux_ConvSpd";
                                Para_Cen = Convert.ToString(dr["Flux_ConvSpd"]);
                                Para_Max = Convert.ToString(dr["Flux_ConvSpd_Max"]);
                                Para_Min = Convert.ToString(dr["Flux_ConvSpd_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 2:
                                #region Flux_NozSpd
                                Para_Name = "Flux_NozSpd";
                                Para_Cen = Convert.ToString(dr["Flux_NozSpd"]);
                                Para_Max = Convert.ToString(dr["Flux_NozSpd_Max"]);
                                Para_Min = Convert.ToString(dr["Flux_NozSpd_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 3:
                                #region Flux_Volumn
                                Para_Name = "Flux_Volumn";
                                Para_Cen = Convert.ToString(dr["Flux_Volumn"]);
                                Para_Max = Convert.ToString(dr["Flux_Volumn_Max"]);
                                Para_Min = Convert.ToString(dr["Flux_Volumn_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 4:
                                #region Flux_NozSpray
                                Para_Name = "Flux_NozSpray";
                                Para_Cen = Convert.ToString(dr["Flux_NozSpray"]);
                                Para_Max = Convert.ToString(dr["Flux_NozSpray_Max"]);
                                Para_Min = Convert.ToString(dr["Flux_NozSpray_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 5:
                                #region Flux_Power
                                Para_Name = "Flux_Power";
                                Para_Cen = Convert.ToString(dr["Flux_Power"]);
                                Para_Max = Convert.ToString(dr["Flux_Power_Max"]);
                                Para_Min = Convert.ToString(dr["Flux_Power_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 6:
                                #region Flux_Pres
                                Para_Name = "Flux_Pres";
                                Para_Cen = Convert.ToString(dr["Flux_Pres"]);
                                Para_Max = Convert.ToString(dr["Flux_Pres_Max"]);
                                Para_Min = Convert.ToString(dr["Flux_Pres_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 7:
                                #region Flux_BiModel
                                Para_Name = "Flux_BiModel";
                                Para_Cen = Convert.ToString(dr["Flux_BiModel"]);
                                Para_Max = ""; // Convert.ToString(dr["Flux_BiModel_Max"]);
                                Para_Min = "";// Convert.ToString(dr["Flux_BiModel_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 8:
                                #region Heat_Low1
                                Para_Name = "Heat_Low1";
                                Para_Cen = Convert.ToString(dr["Heat_Low1"]);
                                Para_Max = Convert.ToString(dr["Heat_Low1_Max"]);
                                Para_Min = Convert.ToString(dr["Heat_Low1_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 9:
                                #region Heat_Low2
                                Para_Name = "Heat_Low2";
                                Para_Cen = Convert.ToString(dr["Heat_Low2"]);
                                Para_Max = Convert.ToString(dr["Heat_Low2_Max"]);
                                Para_Min = Convert.ToString(dr["Heat_Low2_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 10:
                                #region Heat_Low3
                                Para_Name = "Heat_Low3";
                                Para_Cen = Convert.ToString(dr["Heat_Low3"]);
                                Para_Max = Convert.ToString(dr["Heat_Low3_Max"]);
                                Para_Min = Convert.ToString(dr["Heat_Low3_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 11:
                                #region Heat_Upp1
                                Para_Name = "Heat_Upp1";
                                Para_Cen = Convert.ToString(dr["Heat_Upp1"]);
                                Para_Max = Convert.ToString(dr["Heat_Upp1_Max"]);
                                Para_Min = Convert.ToString(dr["Heat_Upp1_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 12:
                                #region Heat_Upp2
                                Para_Name = "Heat_Upp2";
                                Para_Cen = Convert.ToString(dr["Heat_Upp2"]);
                                Para_Max = Convert.ToString(dr["Heat_Upp2_Max"]);
                                Para_Min = Convert.ToString(dr["Heat_Upp2_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 13:
                                #region Heat_Upp3
                                Para_Name = "Heat_Upp3";
                                Para_Cen = Convert.ToString(dr["Heat_Upp3"]);
                                Para_Max = Convert.ToString(dr["Heat_Upp3_Max"]);
                                Para_Min = Convert.ToString(dr["Heat_Upp3_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 14:
                                #region SP_Temp
                                Para_Name = "SP_Temp";
                                Para_Cen = Convert.ToString(dr["SP_Temp"]);
                                Para_Max = Convert.ToString(dr["SP_Temp_Max"]);
                                Para_Min = Convert.ToString(dr["SP_Temp_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 15:
                                #region SP_ConWave
                                Para_Name = "SP_ConWave";
                                Para_Cen = Convert.ToString(dr["SP_ConWave"]);
                                Para_Max = Convert.ToString(dr["SP_ConWave_Max"]);
                                Para_Min = Convert.ToString(dr["SP_ConWave_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 16:
                                #region SP_LdClear
                                Para_Name = "SP_LdClear";
                                Para_Cen = Convert.ToString(dr["SP_LdClear"]);
                                Para_Max = Convert.ToString(dr["SP_LdClear_Max"]);
                                Para_Min = Convert.ToString(dr["SP_LdClear_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 17:
                                #region Conv_Speed
                                Para_Name = "Conv_Speed";
                                Para_Cen = Convert.ToString(dr["Conv_Speed"]);
                                Para_Max = Convert.ToString(dr["Conv_Speed_Max"]);
                                Para_Min = Convert.ToString(dr["Conv_Speed_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 18:
                                #region Conv_Width
                                Para_Name = "Conv_Width";
                                Para_Cen = Convert.ToString(dr["Conv_Width"]);
                                Para_Max = Convert.ToString(dr["Conv_Width_Max"]);
                                Para_Min = Convert.ToString(dr["Conv_Width_Min"]);
                                Para_Remark = "";
                                #endregion
                                break;
                            case 19:
                                #region Other_Ni
                                Para_Name = "Other_Ni";
                                Para_Cen = Convert.ToString(dr["Other_Ni"]);
                                Para_Max = "";
                                Para_Min = "";
                                Para_Remark = "";
                                #endregion
                                break;
                            case 20:
                                #region Remark  //不用上传 Remark
                                Para_Name = "Remark";
                                Para_Cen = "";
                                Para_Max = "";
                                Para_Min = "";
                                Para_Remark = "";
                                #endregion
                                break;
                                #endregion
                        }

                        DataRow drN = dtServer.NewRow();
                        drN["OV_ID"] = IID;
                        drN["MODEL"] = modeName;
                        drN["PROGRAM"] = progName;
                        drN["PARA_NAME"] = Para_Name;
                        drN["VAL_CEN"] = Para_Cen;
                        drN["VAL_MAX"] = Para_Max;
                        drN["VAL_MIN"] = Para_Min;
                        drN["REMARK"] = Para_Remark;
                        dtServer.Rows.Add(drN);
                        #endregion
                    }
                }
                ErrMsg = "Success to get data";
                return dtServer;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }
        #endregion
        #endregion


        #region additional function
        #region 1 delete excel
        public void Additional_DeleteXls(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string McType = viewBag.Para1;
                string fileName = viewBag.Para2;

                string LocalFoder = "";
                if (McType == "Wave") //根据设备不同，上传的文件放在相应的文件夹
                {
                    //@"D:\JianMing\Project\2018_Application\ProcessOnLineMonitor\ExcelUpload\Wave\";
                    LocalFoder = DbConfigHelper.GetConfigPara(CfgPara.Wave_Up_Path);
                }
                if (LocalFoder != "")
                {
                    #region copy and move
                    string LocalFile = LocalFoder + fileName;
                    if (System.IO.File.Exists(LocalFile))
                    {
                        System.IO.File.Delete(LocalFile);
                        ErrMsg = "Success to delete file " + fileName;
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "Fail to get local folder";
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }
        #endregion
        #endregion
    }
}