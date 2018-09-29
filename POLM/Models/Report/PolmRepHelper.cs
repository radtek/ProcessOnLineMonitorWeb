using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyModels.Common;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

using SignalR.MessageHub;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace POLM.Models.Report
{
    public class PolmRepHelper
    {
        string SingalR_DI = "";
        #region SingalR -- Publish message to client
        private IHubConnectionContext<dynamic> Clients { get; set; }
        //private IHubCallerConnectionContext<dynamic> ClientsCaller { get; set; }
        //Outside of the hub, there obviously is no caller because the server is the one who initiates.
        //If you are worried about unique user names, you'll need to implement a custom IUserIdProvider, or you need to manage connection ids per user in some other way. Then you could call
        //context.Clients.Client(connectionId).reportProgress();
        //which would be unique.
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
                    //if (title == "sub")
                    //{
                    //    if (Clients != null)
                    //        Clients.Client(SingalR_DI).publishMsgSub(viewBag);
                    //}
                    //else
                    //{
                    if (Clients != null)
                        Clients.Client(SingalR_DI).publishMsg(viewBag);
                }
            }
            catch (Exception ex)
            {
            }
        }


        public PolmRepHelper()
        {
            try
            {
                Clients = GlobalHost.ConnectionManager.GetHubContext<MessageHubSingle>().Clients;
            }
            catch (Exception)
            {
            }
        }
        #endregion

        public List<string> List_Track = new List<string>();
        public List<string> List_Error = new List<string>();

        public void GetReport_CellParas_Main(MoViewBag viewBag)
        {
            string ErrMsg = "";
            try
            {
                SingalR_DI = viewBag.SignalR_ID;

                if (viewBag.Model == "801_Data")  
                {
                    #region 801_Data  ,首次获取当前的数据
                    List_Track.Add("--Process POLM report  v1.1");

                    BroadcastMyMessage("sub", "");
                    DataTable dt_All = GetReport_CellParas_Wave(viewBag, ref ErrMsg);
                    if (dt_All != null && dt_All.Rows.Count > 0)
                    {
                        #region tracking
                        List_Track.Add(" ");
                        List_Track.Add("----4. Last Record form All");
                        #endregion

                        DataTable dt_Last = GetReport_CellParas_Wave_LastRecord(dt_All, ref ErrMsg);

                        #region tracking
                        List_Track.Add("------Get last record result = " + ErrMsg);
                        List_Track.Add(" ");
                        List_Track.Add("----5. Chart data form All");
                        #endregion

                        #region chart data
                        BroadcastMyMessage("", "Generate chart data ...");
                        //BroadcastMyMessage("sub", "");
                        DataTable dt_Chart = GetReport_CellParas_Wave_Chart(dt_All, viewBag, ref ErrMsg);
                        if (dt_Last != null && dt_Last.Rows.Count > 0)
                        {
                            viewBag.Message = "Success to get report";
                            viewBag.jsonOut = JsonConvert.SerializeObject(dt_Last);

                            viewBag.jsonData = JsonConvert.SerializeObject(dt_Chart);

                            #region tracking
                            List_Track.Add("------Get wave chart data result = " + ErrMsg);
                            #endregion
                        }
                        else
                        {
                            viewBag.Message = "Fail to get report last record";
                            List_Track.Add(viewBag.Message);
                        } 
                        #endregion
                    }
                    else
                    {
                        viewBag.Message = ErrMsg;
                        List_Track.Add(viewBag.Message);
                    } 
                    #endregion
                }
                else if(viewBag.Model == "803_item")
                {
                    #region 803_item

                    #region tracking
                    List_Track.Add("--Process POLM report item detailed data  v1.1");
                    #endregion

                    DataTable dt = GetReport_CellParas_Wave_Item(viewBag, ref ErrMsg);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        viewBag.Message = "Success to get detailed data";
                        viewBag.jsonOut = JsonConvert.SerializeObject(dt);

                    }
                    else
                    {
                        if (ErrMsg != "")
                            viewBag.Message = ErrMsg;
                        else 
                            viewBag.Message = "Fail to get detailed data";
                        List_Track.Add(viewBag.Message);
                    }

                    #region tracing
                    List_Track.Add("  ");
                    List_Track.Add(viewBag.Message);
                    #endregion
                    #endregion
                }
                else if (viewBag.Model == "805_ConRun")
                {
                    #region 805_ConRun 和 801_Data 差不多， 连续获取当前的数据
                    DataTable dt_All = GetReport_CellParas_Wave(viewBag, ref ErrMsg);
                    if (dt_All != null && dt_All.Rows.Count > 0)
                    {
                        DataTable dt_Last = GetReport_CellParas_Wave_LastRecord(dt_All, ref ErrMsg);
                        DataTable dt_Chart = GetReport_CellParas_Wave_Chart(dt_All, viewBag, ref ErrMsg);
                        if (dt_Last != null && dt_Last.Rows.Count > 0)
                        {
                            viewBag.Message = "Success to get report";
                            viewBag.jsonOut = JsonConvert.SerializeObject(dt_Last);
                            viewBag.jsonData = JsonConvert.SerializeObject(dt_Chart);

                            viewBag.ParaRet1 = GetConfig_Para("RepInterval");
                        }
                        else
                        {
                            viewBag.Message = "Fail to get report last record";
                        }
                    }
                    else
                    {
                        if(ErrMsg == "")
                            viewBag.Message = "Fail to get report";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        #region Get Cell Parameters
        /// <summary>
        /// 获取 Wave Report All, 根据Cell, 日期，获取设备的程序，WorkI参数，对比生成报告
        /// </summary>
        private DataTable GetReport_CellParas_Wave(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region init data
                BroadcastMyMessage("", "Get config from server for " + viewBag.Para1);
                List_Track.Add("--Call GetReport_CellParas_Wave ver:1.1 @2018/08/31");
                string sCell = viewBag.Para1.Split(' ')[1].Trim();
                string sCell_Line = viewBag.Para1.Split(' ')[2].Trim();
                string sDayChart = viewBag.Para2;  //要显示数据的日期

                #region checking
                if (sCell == "")
                {
                    ErrMsg = "Fail to get cell";
                    List_Track.Add("----Fail to get cell");
                    BroadcastMyMessage("", "Fail to get cell due to " + viewBag.Para1);
                    return null;
                }
                if (sCell_Line == "")
                {
                    ErrMsg = "Fail to get line";
                    List_Track.Add("----Fail to get line");
                    BroadcastMyMessage("", "Fail to get line due to " + viewBag.Para1);
                    return null;
                } 
                #endregion

                if (sDayChart == null || sDayChart == "")
                {
                    sDayChart = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    sDayChart = Convert.ToDateTime(sDayChart).ToString("yyyy-MM-dd");
                }
                List_Track.Add("----Report cell = " + sCell + ", day=" + sDayChart); 
                #endregion

                #region 获取 config of cell from server 
                sCell = sCell.Replace("Cell", "").Trim();
                if (sCell == "")
                {
                    ErrMsg = "Fail to get cell value";
                    List_Track.Add(ErrMsg);
                    return null;
                }
                string SQL = "select * from POLM_Config where Family='Cell' and Type='Machine' and ParKey='#Cell#' and Data='Wave'";
                if (sCell_Line != "ALL")
                {
                    sCell_Line = sCell_Line.Replace("L", "");
                    SQL = "select * from POLM_Config where Family='Cell' and Type='Machine' and ParKey='#Cell#' and Data='Wave' AND ParValue='#Line#'";
                    SQL = SQL.Replace("#Line#", sCell_Line);
                }
                SQL = SQL.Replace("#Cell#", sCell);

                //获取配置 Line，for Cell=1  Wave
                DataTable dtCfg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg); 
                #endregion

                List<string> List_Machine = dtCfg.AsEnumerable().Select(o => o.Field<string>("Machine")).ToList();
                List_Track.Add("----Report line list = " + String.Join(",", List_Machine.ToArray()));

                if (List_Machine != null && List_Machine.Count > 0)
                {
                    #region process
                    List_Track.Add("----1. Program Data" );
                    BroadcastMyMessage("", "Query Program data for machine => " + String.Join(",", List_Machine.ToArray()) + " ...");

                    DataTable dt_Prg = DT_GetData_WaveProgram3(List_Machine, 5, -1 , sDayChart);
                    if (dt_Prg != null && dt_Prg.Rows.Count > 0)
                    {
                        #region tracking
                        List_Track.Add(" ");
                        List_Track.Add("----2. WorkI Data");
                        BroadcastMyMessage("", "Query WorkI data ...");
                        #endregion

                        List<string> List_Program = dt_Prg.AsEnumerable().Select(o => o.Field<string>("Program")).Distinct().ToList();
                        DataTable dt_WI = DT_GetData_WaveWI(List_Program, ref ErrMsg);

                        if (dt_WI != null && dt_WI.Rows.Count > 0)
                        {
                            #region tracking
                            List_Track.Add(" ");
                            List_Track.Add("----3. Comparision all");
                            #endregion

                            //BroadcastMyMessage("", "Compare report ...");
                            DataTable dt_Report = Polm_Report_Generate_ALL(dt_Prg, dt_WI, dtCfg);

                            return dt_Report;
                        }
                        else
                        {
                            ErrMsg = "Fail to get WI datatable";
                            List_Track.Add(ErrMsg);
                        }
                    }
                    else
                    {
                        if(ErrMsg == "")
                            ErrMsg = "Fail to get program data";
                        //BroadcastMyMessage("sub", ErrMsg);
                        List_Track.Add(ErrMsg);
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "Fail to get machine for Line = " + viewBag.Para1;
                    List_Track.Add(ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        #region Get report -- last record and chart
        private DataTable GetReport_CellParas_Wave_LastRecord(DataTable dt_Prg_All, ref string ErrMsg )
        {
            ErrMsg = "";
            try
            {

                var res_Last = dt_Prg_All.AsEnumerable().Where(p => p.Field<Int32>("num") == 1).Select(o => o);
                if (res_Last != null && res_Last.Count() > 0)
                {
                    DataTable dt_Last = res_Last.CopyToDataTable();
                    ErrMsg = "Success to get last record rows = " + dt_Last.Rows.Count;
                    return dt_Last;
                }
                
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        /// <summary>
        /// 获取 chart data=> line pass/fail 的数据
        /// </summary>
        private DataTable GetReport_CellParas_Wave_Chart(DataTable dt_Prg_All, MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                ChartOptions chartOpt = new ChartOptions();

                #region tracking
                List_Track.Add("--------------------------------------------------------------------------");
                #endregion

                DataTable dt_Chart = CreateNewDataTableForChartWave();
                List<string> List_Line = dt_Prg_All.AsEnumerable().OrderBy(p => p.Field<string>("Line")).Select(p => p.Field<string>("Line")).Distinct().ToList();
                chartOpt.LineQty = List_Line.Count;

                #region tracking
                List_Track.Add("------Get chart data for Line = " + List_Line.Count);
                #endregion
                //24 小时的数据，小时不会有重复, obsolete, using following function
                //List<Int32> List_Hour_Prg = dt_Prg_All.AsEnumerable().Where(p => p.Field<string>("Line") == Line_ONE).
                //                OrderBy(p => Convert.ToDateTime(p.Field<string>("Time"))).Select(p => p.Field<Int32>("Hour")).Distinct().ToList();
                int nLastHour_Program = dt_Prg_All.AsEnumerable().Where(p => p.Field<Int32>("num") == 1).Select(o => o.Field<Int32>("Hour")).FirstOrDefault();
                int nLastHour_Now = DateTime.Now.Hour;

                List<Int32> List_Hour = GetHoursFromNow(); //out nLastHour_Now);  //normal use this function
                if (nLastHour_Now != nLastHour_Program)
                    List_Hour.Remove(nLastHour_Now);

                    #region tracking
                if (List_Hour != null)
                    List_Track.Add("------Get chart hours = " + List_Hour.Count);
                else
                    List_Track.Add("------Fail to get chart hours");
                #endregion

                if (List_Hour != null)
                {
                    #region for ONE LINE
                    foreach (Int32 hour in List_Hour) 
                    {
                        DataRow drN = dt_Chart.NewRow();
                        drN["Time"] = hour.ToString();

                        #region set line hour value
                        for (int i = 0; i < List_Line.Count; i++)  //线别是按照从小到大排序
                        {
                            string sLine = List_Line[i];
                            string sRemark = ""; //用于显示chart marker点上的 tooltip
                            int PF_Val = GetLineHour_PF_Value(dt_Prg_All, i, sLine, hour, out sRemark);
                            switch (i)
                            {
                                case 0:
                                    chartOpt.Line1_Name = sLine;
                                    drN["Line1"] = PF_Val;
                                    drN["Remark1"] = sRemark;
                                    break;
                                case 1:
                                    chartOpt.Line2_Name = sLine;
                                    drN["Line2"] = PF_Val;
                                    drN["Remark2"] = sRemark;
                                    break;
                                case 2:
                                    chartOpt.Line3_Name = sLine;
                                    drN["Line3"] = PF_Val;
                                    drN["Remark3"] = sRemark;
                                    break;
                                case 3:
                                    chartOpt.Line4_Name = sLine;
                                    drN["Line4"] = PF_Val;
                                    drN["Remark4"] = sRemark;
                                    break;
                            }
                        } 
                        #endregion
                        dt_Chart.Rows.Add(drN);
                    } 
                    #endregion
                }
                if(dt_Chart != null && dt_Chart.Rows.Count > 0)
                {
                    ErrMsg = "Success to get chart data, rows = " + dt_Chart.Rows.Count;
                }
                //to do list: set axis limit: max and min and tick
                #region GetMaxNndMinValue
                int nMin = 140;
                int nMax = 10;
                GetMaxNndMinValue(List_Line, dt_Chart, out nMin, out nMax);
                chartOpt.nInterval = (nMax - nMin) / 10;
                chartOpt.nMin = nMin;
                chartOpt.nMax = nMax;

                #endregion

                viewBag.ParaRet1 = JsonConvert.SerializeObject(chartOpt);
                return dt_Chart;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
            }
            return null;
        }

        private DataTable GetReport_CellParas_Wave_Chart_bck_basedonActline(DataTable dt_Prg_All, MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                ChartOptions chartOpt = new ChartOptions();

                #region tracking
                List_Track.Add("--------------------------------------------------------------------------");
                #endregion

                DataTable dt_Chart = CreateNewDataTableForChartWave();
                List<string> List_Line = dt_Prg_All.AsEnumerable().OrderBy(p => p.Field<string>("Line")).Select(p => p.Field<string>("Line")).Distinct().ToList();
                chartOpt.LineQty = List_Line.Count;

                #region tracking
                List_Track.Add("------Get chart data for Line = " + List_Line.Count);
                #endregion
                //24 小时的数据，小时不会有重复, obsolete, using following function
                //List<Int32> List_Hour = dt_Prg_All.AsEnumerable().Where(p => p.Field<string>("Line") == Line_ONE).
                //                OrderBy(p => Convert.ToDateTime(p.Field<string>("Time"))).Select(p => p.Field<Int32>("Hour")).Distinct().ToList();

                List<Int32> List_Hour = GetHoursFromNow();  //normal use this function
                #region tracking
                if (List_Hour != null)
                    List_Track.Add("------Get chart hours = " + List_Hour.Count);
                else
                    List_Track.Add("------Fail to get chart hours");
                #endregion

                if (List_Hour != null)
                {
                    #region for ONE LINE
                    foreach (Int32 hour in List_Hour)
                    {
                        DataRow drN = dt_Chart.NewRow();
                        drN["Time"] = hour.ToString();

                        #region set line hour value
                        for (int i = 0; i < List_Line.Count; i++)  //线别是按照从小到大排序
                        {
                            string sLine = List_Line[i];
                            string sRemark = "";
                            int PF_Val = GetLineHour_PF_Value(dt_Prg_All, i, sLine, hour, out sRemark);
                            switch (i)
                            {
                                case 0:
                                    chartOpt.Line1_Name = sLine;
                                    drN["Line1"] = PF_Val;
                                    drN["Remark1"] = sRemark;
                                    break;
                                case 1:
                                    chartOpt.Line2_Name = sLine;
                                    drN["Line2"] = PF_Val;
                                    drN["Remark2"] = sRemark;
                                    break;
                                case 2:
                                    chartOpt.Line3_Name = sLine;
                                    drN["Line3"] = PF_Val;
                                    drN["Remark3"] = sRemark;
                                    break;
                                case 3:
                                    chartOpt.Line4_Name = sLine;
                                    drN["Line4"] = PF_Val;
                                    drN["Remark4"] = sRemark;
                                    break;
                            }
                        }
                        #endregion
                        dt_Chart.Rows.Add(drN);
                    }
                    #endregion
                }
                if (dt_Chart != null && dt_Chart.Rows.Count > 0)
                {
                    ErrMsg = "Success to get chart data, rows = " + dt_Chart.Rows.Count;
                }
                //to do list: set axis limit: max and min and tick
                #region GetMaxNndMinValue
                int nMin = 140;
                int nMax = 10;
                GetMaxNndMinValue(List_Line, dt_Chart, out nMin, out nMax);
                chartOpt.nInterval = (nMax - nMin) / 10;
                chartOpt.nMin = nMin;
                chartOpt.nMax = nMax;

                #endregion

                viewBag.ParaRet1 = JsonConvert.SerializeObject(chartOpt);
                return dt_Chart;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
            }
            return null;
        }

        private void GetMaxNndMinValue(List<string> List_Lines, DataTable dt_Chart, out int nMin, out int nMax)
        {
            nMin = 140;
            nMax = 10;

            string ErrMsg = "";
            try
            {
                for (int i = 1; i <= List_Lines.Count; i++)  //foreach(string sLine in List_Line)
                {
                    string sLine = "Line" + i.ToString(); // List_Line[nLine];
                    List<int> List_Line_Values = dt_Chart.AsEnumerable().OrderBy(p => p.Field<int>(sLine)).Select(p => p.Field<int>(sLine)).ToList();
                    int nMin_Temp = List_Line_Values.First();
                    if (nMin > nMin_Temp)
                        nMin = nMin_Temp;

                    int nMax_Temp = List_Line_Values.Last();
                    if (nMax < nMax_Temp)
                        nMax = nMax_Temp;
                }

                string sMin = nMin.ToString();
                if(sMin.Substring(sMin.Length -1, 1) == "5")
                    nMin = nMin - 15;
                else
                    nMin = nMin - 20;

                string sMax = nMax.ToString();
                if (sMax.Substring(sMax.Length - 1, 1) == "5")
                    nMax = nMax + 25;
                else
                    nMax = nMax + 20;


                //nMin = nMin % 5 == 0 ? nMin - 15 : nMin - 20;
                //nMax = nMax % 5 == 0 ? nMax + 25 : nMax + 20;

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        /// <summary>
        /// 获取Chart值, 表示 line的红绿值，3线在线， 8线在上
        /// </summary>
        private int GetLineHour_PF_Value(DataTable dt_Prg, int nLineSeq, string sLine, int nHour, out string sRemark)
        {
            string ErrMsg = "";
            int PF_Val = 0;
            sRemark = "Result=";
            try
            {
                if (nLineSeq == 0)
                    PF_Val = 30;
                else if (nLineSeq == 1)
                    PF_Val = 60;
                else if (nLineSeq == 2)
                    PF_Val = 90;
                else if (nLineSeq == 3)
                    PF_Val = 120;

                string PF = "";
                int nFail_Qty = 0;
                var res = dt_Prg.AsEnumerable().Where(p => p.Field<string>("Line") == sLine && 
                        p.Field<Int32>("Hour") == nHour).Select(o => o.Field<string>("Status")).ToList();
                if(res != null)
                {
                    if (res.Contains("fail"))
                    {
                        PF = "fail";
                        nFail_Qty = res.Count;
                    }
                    else
                        PF = "pass";
                }

                if (nLineSeq == 0)
                {
                    if(PF == "fail")
                        PF_Val = 25;
                }
                else if (nLineSeq == 1)
                {
                    if (PF == "fail")
                        PF_Val = 55;
                }
                else if (nLineSeq == 2)
                {
                    if (PF == "fail")
                        PF_Val = 85;
                }
                else if (nLineSeq == 3)
                {
                    if (PF == "fail")
                        PF_Val = 115;
                }

                if(PF == "pass")
                {
                    sRemark = "Result=Pass";
                }
                else
                {
                    sRemark = "Result=Fail, Para_Q=" + nFail_Qty;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return PF_Val;
        }

        private int GetLineHour_PF_Value_obso_Actual(DataTable dt_Prg, int nLineSeq, string sLine, int nHour, out string sRemark)
        {
            string ErrMsg = "";
            int PF_Val = 0;
            sRemark = "Result=";
            try
            {
                if (sLine == "3")
                    PF_Val = 30;
                else if (sLine == "6")
                    PF_Val = 60;
                else if (sLine == "7")
                    PF_Val = 90;
                else if (sLine == "8")
                    PF_Val = 120;

                string PF = "";
                var res = dt_Prg.AsEnumerable().Where(p => p.Field<string>("Line") == sLine &&
                        p.Field<Int32>("Hour") == nHour).Select(o => o.Field<string>("Status")).ToList();
                if (res != null)
                {
                    if (res.Contains("fail"))
                        PF = "fail";
                    else
                        PF = "pass";
                }

                if (sLine == "3")
                {
                    if (PF == "fail")
                        PF_Val = 25;
                }
                else if (sLine == "6")
                {
                    if (PF == "fail")
                        PF_Val = 55;
                }
                else if (sLine == "7")
                {
                    if (PF == "fail")
                        PF_Val = 85;
                }
                else if (sLine == "8")
                {
                    if (PF == "fail")
                        PF_Val = 115;
                }

                if (PF == "pass")
                {
                    sRemark = "Result=Pass";
                }
                else
                {
                    sRemark = "Result=Fail";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return PF_Val;
        }

        #endregion
        private DataTable Polm_Report_Generate_ALL(DataTable dt_Prg, DataTable dt_WI, DataTable dt_Cfg)
        {
            string ErrMsg = "";
            try
            {
                DataTable dt_Res = CreateNewDataTableForReport();
                #region Compare

                #region reamark fields
                //Flux_BdWid, Flux_ConvSpd, Flux_NozSpd, Flux_Volumn, Flux_NozSpray, Flux_Power, Flux_Pres,Flux_BiModel,
                //Heat_Low1, Heat_Low2, Heat_Low3, Heat_Upp1, Heat_Upp2, Heat_Upp3, SP_Temp, SP_ConWave,  SP_LdClear, Conv_Speed, Conv_Width,
                //Other_Ni, Remark , Machine, LogTime, Program, UpdateTime 
                #endregion

                List_Track.Add("--------------------------------------------------------------------------");
                List_Track.Add("------Compare rows = " + dt_Prg.Rows.Count);
                BroadcastMyMessage("", "Compare program and workI ...");

                //List<string> List_Program = dt_Prg.AsEnumerable().Select(o => o.Field<string>("Program")).Distinct().ToList();

                List<string> List_Program = new List<string>();

                /*比较程序的参数和WorkI的标准是否一致，
                 */
                foreach (DataRow dr in dt_Prg.Rows)
                {
                    List<ST_RepRes> List_ST_RepRes = new List<ST_RepRes>(); //have qty=20

                    #region overall data
                    string Program = Convert.ToString(dr[RepStr.Program.ToUpper()]);
                    if (!List_Program.Contains(Program))
                    {
                        BroadcastMyMessage("", "Compare program and workI for " + Program + " ...");
                        List_Program.Add(Program);
                    }
                    string Machine = Convert.ToString(dr[RepStr.Machine.ToUpper()]).Trim().ToUpper();
                    string Line = GetConfig_Line(dt_Cfg, Machine);
                    string logTime = Convert.ToString(dr[RepStr.LogTime.ToUpper()]);
                    Int32 rowNum = Convert.ToInt32(dr["NUM"]);
                    Int32 Hour = Convert.ToInt32(dr["HOUR"]);

                    bool Global_Row_Pass = true; //总体表示此采样点是否 pass or fail
                    #endregion

                    if (Line != "")
                    {
                        string Project = GetWIPara_Project(dt_WI, Line, Program);
                        for (int i = 0; i < 20; i++) //Wave 有参数 19个
                        {
                            #region insert data
                            string Para_Name = "", Para_Remark = "";
                            string Para_Cen = "", Para_Max = "", Para_Min = "", Para_ActVal = "";
                            switch (i)
                            {
                                #region CASE to get parameter
                                case 0:
                                    #region parameter
                                    Para_Name = "Flux_BdWid";
                                    //Para_Cen = Convert.ToString(dr["Flux_BdWid"]);
                                    //Para_Max = Convert.ToString(dr["Flux_BdWid_Max"]);
                                    //Para_Min = Convert.ToString(dr["Flux_BdWid_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 1:
                                    #region Flux_ConvSpd
                                    Para_Name = "Flux_ConvSpd";
                                    //Para_Cen = Convert.ToString(dr["Flux_BdWid"]);
                                    //Para_Max = Convert.ToString(dr["Flux_BdWid_Max"]);
                                    //Para_Min = Convert.ToString(dr["Flux_BdWid_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 2:
                                    #region Flux_NozSpd
                                    Para_Name = "Flux_NozSpd";
                                    //Para_Cen = Convert.ToString(dr["Flux_NozSpd"]);
                                    //Para_Max = Convert.ToString(dr["Flux_NozSpd_Max"]);
                                    //Para_Min = Convert.ToString(dr["Flux_NozSpd_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 3:
                                    #region Flux_Volumn
                                    Para_Name = "Flux_Volumn";
                                    //Para_Cen = Convert.ToString(dr["Flux_Volumn"]);
                                    //Para_Max = Convert.ToString(dr["Flux_Volumn_Max"]);
                                    //Para_Min = Convert.ToString(dr["Flux_Volumn_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 4:
                                    #region Flux_NozSpray
                                    Para_Name = "Flux_NozSpray";
                                    //Para_Cen = Convert.ToString(dr["Flux_NozSpray"]);
                                    //Para_Max = Convert.ToString(dr["Flux_NozSpray_Max"]);
                                    //Para_Min = Convert.ToString(dr["Flux_NozSpray_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 5:
                                    #region Flux_Power
                                    Para_Name = "Flux_Power";
                                    //Para_Cen = Convert.ToString(dr["Flux_Power"]);
                                    //Para_Max = Convert.ToString(dr["Flux_Power_Max"]);
                                    //Para_Min = Convert.ToString(dr["Flux_Power_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 6:
                                    #region Flux_Pres
                                    Para_Name = "Flux_Pres";
                                    //Para_Cen = Convert.ToString(dr["Flux_Pres"]);
                                    //Para_Max = Convert.ToString(dr["Flux_Pres_Max"]);
                                    //Para_Min = Convert.ToString(dr["Flux_Pres_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 7:
                                    #region Flux_BiModel
                                    Para_Name = "Flux_BiModel";
                                    //Para_Cen = Convert.ToString(dr["Flux_BiModel"]);
                                    //Para_Max = ""; // Convert.ToString(dr["Flux_BiModel_Max"]);
                                    //Para_Min = "";// Convert.ToString(dr["Flux_BiModel_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 8:
                                    #region Heat_Low1
                                    Para_Name = "Heat_Low1";
                                    //Para_Cen = Convert.ToString(dr["Heat_Low1"]);
                                    //Para_Max = Convert.ToString(dr["Heat_Low1_Max"]);
                                    //Para_Min = Convert.ToString(dr["Heat_Low1_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 9:
                                    #region Heat_Low2
                                    Para_Name = "Heat_Low2";
                                    //Para_Cen = Convert.ToString(dr["Heat_Low2"]);
                                    //Para_Max = Convert.ToString(dr["Heat_Low2_Max"]);
                                    //Para_Min = Convert.ToString(dr["Heat_Low2_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 10:
                                    #region Heat_Low3
                                    Para_Name = "Heat_Low3";
                                    //Para_Cen = Convert.ToString(dr["Heat_Low3"]);
                                    //Para_Max = Convert.ToString(dr["Heat_Low3_Max"]);
                                    //Para_Min = Convert.ToString(dr["Heat_Low3_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 11:
                                    #region Heat_Upp1
                                    Para_Name = "Heat_Upp1";
                                    //Para_Cen = Convert.ToString(dr["Heat_Upp1"]);
                                    //Para_Max = Convert.ToString(dr["Heat_Upp1_Max"]);
                                    //Para_Min = Convert.ToString(dr["Heat_Upp1_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 12:
                                    #region Heat_Upp2
                                    Para_Name = "Heat_Upp2";
                                    //Para_Cen = Convert.ToString(dr["Heat_Upp2"]);
                                    //Para_Max = Convert.ToString(dr["Heat_Upp2_Max"]);
                                    //Para_Min = Convert.ToString(dr["Heat_Upp2_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 13:
                                    #region Heat_Upp3
                                    Para_Name = "Heat_Upp3";
                                    //Para_Cen = Convert.ToString(dr["Heat_Upp3"]);
                                    //Para_Max = Convert.ToString(dr["Heat_Upp3_Max"]);
                                    //Para_Min = Convert.ToString(dr["Heat_Upp3_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 14:
                                    #region SP_Temp
                                    Para_Name = "SP_Temp";
                                    //Para_Cen = Convert.ToString(dr["SP_Temp"]);
                                    //Para_Max = Convert.ToString(dr["SP_Temp_Max"]);
                                    //Para_Min = Convert.ToString(dr["SP_Temp_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 15:
                                    #region SP_ConWave
                                    Para_Name = "SP_ConWave";
                                    //Para_Cen = Convert.ToString(dr["SP_ConWave"]);
                                    //Para_Max = Convert.ToString(dr["SP_ConWave_Max"]);
                                    //Para_Min = Convert.ToString(dr["SP_ConWave_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 16:
                                    #region SP_LdClear
                                    Para_Name = "SP_LdClear";
                                    //Para_Cen = Convert.ToString(dr["SP_LdClear"]);
                                    //Para_Max = Convert.ToString(dr["SP_LdClear_Max"]);
                                    //Para_Min = Convert.ToString(dr["SP_LdClear_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 17:
                                    #region Conv_Speed
                                    Para_Name = "Conv_Speed";
                                    //Para_Cen = Convert.ToString(dr["Conv_Speed"]);
                                    //Para_Max = Convert.ToString(dr["Conv_Speed_Max"]);
                                    //Para_Min = Convert.ToString(dr["Conv_Speed_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 18:
                                    #region Conv_Width
                                    Para_Name = "Conv_Width";
                                    //Para_Cen = Convert.ToString(dr["Conv_Width"]);
                                    //Para_Max = Convert.ToString(dr["Conv_Width_Max"]);
                                    //Para_Min = Convert.ToString(dr["Conv_Width_Min"]);
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
                                    #endregion
                                    break;
                                case 19:
                                    #region Other_Ni
                                    Para_Name = "Other_Ni";
                                    //Para_Cen = Convert.ToString(dr["Other_Ni"]);
                                    //Para_Max = "";
                                    //Para_Min = "";
                                    Para_Remark = "";
                                    Para_ActVal = Convert.ToString(dr[Para_Name.ToUpper()]);
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

                            ST_RepRes m_ST_RepRes = new Report.ST_RepRes();
                            m_ST_RepRes.Parameter = Para_Name;

                            string compareResult = Compare_Parameter(dt_WI, dt_Cfg, Program, Machine, Para_Name, Para_ActVal, m_ST_RepRes);
                            m_ST_RepRes.Val_Act = Para_ActVal;
                            m_ST_RepRes.Time = Machine;
                            m_ST_RepRes.Status = compareResult;
                            m_ST_RepRes.Comment = Para_Remark;

                            List_ST_RepRes.Add(m_ST_RepRes);

                            if (compareResult == "fail")
                                Global_Row_Pass = false;
                            #endregion
                        }

                        for(int i=0; i< List_ST_RepRes.Count; i++)
                        {
                            ST_RepRes st_RepRes = List_ST_RepRes[i];
                            if(st_RepRes.Parameter == "Other_Ni")
                            {
                                continue;
                            }

                            #region fill in dt and return
                            string commentThis = "";
                            if (Global_Row_Pass == true)
                                commentThis = "Hur=" + Hour + " 1 Rev=" + st_RepRes.DocRev; // rowNum + " pass";
                            else
                                commentThis = "Hur=" + Hour + " 0 Rev=" + st_RepRes.DocRev; //rowNum + " fail";

                            DataRow drN = dt_Res.NewRow();
                            drN["Line"] = Line; // st_RepRes.Line;
                            drN["Machine"] = Machine; // st_RepRes.Machine;
                            drN["Project"] = Project; //st_RepRes.Project;
                            drN["Program"] = Program; // st_RepRes.Program;
                            drN["Parameter"] = st_RepRes.Parameter;
                            drN["Val_Cen"] = st_RepRes.Val_Cen;
                            drN["Val_Min"] = st_RepRes.Val_Min;
                            drN["Val_Max"] = st_RepRes.Val_Max;
                            drN["Val_Act"] = st_RepRes.Val_Act;
                            drN["TEMPDAYS"] = st_RepRes.TEMPDAYS;
                            drN["Time"] = logTime;
                            drN["Comment"] = commentThis;
                            drN["Status"] = st_RepRes.Status;  //某个参数的状态=> pass|fail
                            drN["PF"] = Global_Row_Pass == true ? "pass" : "fail";  //整个行的状态=> pass|fail
                            drN["Num"] = rowNum;
                            drN["Hour"] = Hour;

                            dt_Res.Rows.Add(drN); 
                            #endregion
                        }
                    }
                    else
                    {
                        List_Track.Add("------Fail to get  line for Program = " + Program);
                    }
                }
                List_Track.Add("------Compare Result rows = " + dt_Res.Rows.Count);

                return dt_Res;
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private string Compare_Parameter(DataTable dtWI, DataTable dtCfg, string Program, string Machine, string ParaName, string val_Act, ST_RepRes m_ST_RepRes)
        {
            string ErrMsg = "";
            try
            {
                #region 获取线别
                var mcLine = dtCfg.AsEnumerable().Where(p => p.Field<string>("Machine") == Machine).Select(o => o.Field<string>("ParValue")).FirstOrDefault();
                if (mcLine == null && mcLine == "")
                {
                    ErrMsg = "Fail to get line for machine=" + Machine;
                    return "";
                }
                #endregion

                if (ParaName == RepStr.Flux_BdWid || ParaName == RepStr.Flux_ConvSpd || ParaName == RepStr.Flux_NozSpd || ParaName == RepStr.Flux_Volumn
                    || ParaName == RepStr.Flux_NozSpray || ParaName == RepStr.Flux_Power || ParaName == RepStr.Flux_Pres)
                {
                    //取出此行的WI的所有参数
                    #region Flux_BdWid
                    var rowWI = dtWI.AsEnumerable().Where(p => p.Field<string>("Line") == mcLine && p.Field<string>("Program").ToUpper() == Program.ToUpper() &&
                        p.Field<string>("Para_Name").ToUpper() == ParaName.ToUpper()).Select(o => o).FirstOrDefault();
                    if (rowWI != null)
                    {
                        #region Flux_BdWid
                        string Val_Cen = Convert.ToString(rowWI["VAL_CEN"]);

                        #region Check if NA, then return NA, DO NOT Compare
                        if (Val_Cen == "NA")
                        {
                            m_ST_RepRes.Val_Cen = "NA";
                            m_ST_RepRes.Val_Min = "NA";
                            m_ST_RepRes.Val_Max = "NA";
                            //drN["Val_Cen"] = "NA";
                            //drN["Val_Min"] = "NA";
                            //drN["Val_Max"] = "NA";
                            return "NA";
                        }

                        #endregion
                        string Val_Max = Convert.ToString(rowWI["VAL_MAX"]);
                        string Val_Min = Convert.ToString(rowWI["VAL_MIN"]);

                        m_ST_RepRes.Val_Cen = Val_Cen;
                        m_ST_RepRes.Val_Min = Val_Min;
                        m_ST_RepRes.Val_Max = Val_Max;
                        m_ST_RepRes.TEMPDAYS = Convert.ToString(rowWI["TEMPDAYS"]);

                        m_ST_RepRes.DocRev = Convert.ToString(rowWI["DOCREV"]);
                        //drN["Val_Cen"] = Val_Cen;
                        //drN["Val_Min"] = Val_Min;
                        //drN["Val_Max"] = Val_Max;

                        float fVal_Cen = GetValue_Float(Val_Cen, ref ErrMsg);
                        float fVal_Max = GetValue_Float(Val_Max, ref ErrMsg);
                        if (ErrMsg == "Success")
                        {
                            #region compare
                            float fVal_Min = GetValue_Float(Val_Min, ref ErrMsg);
                            if (ErrMsg == "Success")
                            {
                                float fVal_Act = GetValue_Float(val_Act, ref ErrMsg);
                                if (ErrMsg == "Success")
                                {
                                    if (fVal_Act >= fVal_Min && fVal_Act <= fVal_Max)
                                    {
                                        return "pass";
                                    }
                                    else
                                        return "fail";
                                }
                            }
                            else
                            {

                            }
                            #endregion
                        }
                        else
                        {

                        } 
                        #endregion
                    }
                    else
                    {
                        ErrMsg = "Fail to get WI data for line=" + mcLine + " Program=" + Program + " ParaName=" + ParaName;
                    }
                    #endregion
                }
                else if(ParaName == RepStr.Flux_BiModel || ParaName == RepStr.Other_Ni)
                {
                    #region Flux_BiModel
                    var rowWI = dtWI.AsEnumerable().Where(p => p.Field<string>("Line") == mcLine && p.Field<string>("Program").ToUpper() == Program.ToUpper() &&
                        p.Field<string>("Para_Name").ToUpper() == ParaName.ToUpper()).Select(o => o).FirstOrDefault();
                    if (rowWI != null)
                    {
                        #region Flux_BiModel
                        string Val_Cen = Convert.ToString(rowWI["VAL_CEN"]);
                        //drN["Val_Cen"] = Val_Cen;
                        //drN["Val_Min"] = "";
                        //drN["Val_Max"] = "";
                        m_ST_RepRes.Val_Cen = Val_Cen;
                        m_ST_RepRes.Val_Min = "";
                        m_ST_RepRes.Val_Max = "";
                        m_ST_RepRes.TEMPDAYS = Convert.ToString(rowWI["TEMPDAYS"]);
                        m_ST_RepRes.DocRev = Convert.ToString(rowWI["DOCREV"]);

                        if (Val_Cen != null && Val_Cen != "" && Val_Cen != "NA")
                        {
                            if (Val_Cen.ToLower() == val_Act.ToLower())
                            {
                                return "pass";
                            }
                            else
                            {
                                return "NA";  //暂时用NA, 还要改动
                            }
                        }
                        else
                        {
                            return "NA";
                        } 
                        #endregion
                    }
                    #endregion
                }
                else if (ParaName == RepStr.Heat_Low1 || ParaName == RepStr.Heat_Low2 || ParaName == RepStr.Heat_Low3 || 
                         ParaName == RepStr.Heat_Upp1 || ParaName == RepStr.Heat_Upp2 || ParaName == RepStr.Heat_Upp3 ||
                         ParaName == RepStr.SP_Temp || ParaName == RepStr.SP_ConWave || ParaName == RepStr.SP_LdClear ||
                         ParaName == RepStr.Conv_Speed || ParaName == RepStr.Conv_Width)
                {
                    //取出此行的WI的所有参数
                    #region Heat_Low1
                    var rowWI = dtWI.AsEnumerable().Where(p => p.Field<string>("Line") == mcLine && p.Field<string>("Program").ToUpper() == Program.ToUpper() &&
                        p.Field<string>("Para_Name").ToUpper() == ParaName.ToUpper()).Select(o => o).FirstOrDefault();
                    if (rowWI != null)
                    {
                        #region MyRegion
                        string Val_Cen = Convert.ToString(rowWI["VAL_CEN"]);

                        #region Check if NA, then return NA, DO NOT Compare
                        if (Val_Cen == "NA")
                        {
                            m_ST_RepRes.Val_Cen = "NA";
                            m_ST_RepRes.Val_Min = "NA";
                            m_ST_RepRes.Val_Max = "NA";
                            return "NA";
                        }

                        #endregion
                        string Val_Max = Convert.ToString(rowWI["VAL_MAX"]);
                        string Val_Min = Convert.ToString(rowWI["VAL_MIN"]);
                        m_ST_RepRes.Val_Cen = Val_Cen;
                        m_ST_RepRes.Val_Min = Val_Min;
                        m_ST_RepRes.Val_Max = Val_Max;
                        m_ST_RepRes.TEMPDAYS = Convert.ToString(rowWI["TEMPDAYS"]);
                        m_ST_RepRes.DocRev = Convert.ToString(rowWI["DOCREV"]);

                        float fVal_Cen = GetValue_Float(Val_Cen, ref ErrMsg);
                        float fVal_Max = GetValue_Float(Val_Max, ref ErrMsg);
                        if (ErrMsg == "Success")
                        {
                            #region compare
                            float fVal_Min = GetValue_Float(Val_Min, ref ErrMsg);
                            if (ErrMsg == "Success")
                            {
                                float fVal_Act = GetValue_Float(val_Act, ref ErrMsg);
                                if (ErrMsg == "Success")
                                {
                                    if (fVal_Act >= fVal_Min && fVal_Act <= fVal_Max)
                                    {
                                        return "pass";
                                    }
                                    else
                                        return "fail";
                                }
                            }
                            else
                            {

                            }
                            #endregion
                        }
                        else
                        {

                        } 
                        #endregion
                    }
                    else
                    {
                        ErrMsg = "Fail to get WI data for line=" + mcLine + " Program=" + Program + " ParaName=" + ParaName;
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return "NA";
        }


        private DataTable WI_GetAllParameters() //DataTable dtCfg, string sCell)
        {
            string ErrMsg = "";
            try
            {
                //获取WorkI wave的全部数据
                string SQL_WI = @"select LINE,PROJECT,OV_ID,MODEL,PROGRAM,PARA_NAME,VAL_CEN,VAL_MAX,VAL_MIN,REMARK from
                                    (
                                        SELECT a.ID, a.Line, a.Project, b.* from
                                        (
                                            SELECT ID, Line, Project from POLM_WI_OV where McType='Wave' 
                                        ) a left join 
                                        (
                                            SELECT * FROM POLM_WI_PARA   
                                        ) b
                                        ON a.ID=b.OV_ID
                                    )-- where instr('#Programs#',Program,1) > 0 --program name is not stable
                                ";
                DataTable dt_WI = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL_WI, ref ErrMsg);
                return dt_WI;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        #region 获取 Wave and WI 的参数数据

        /// <summary>
        /// 获取Cell的 24小时的所有的 wave 程序参数
        /// </summary>
        private DataTable DT_GetData_WaveProgram(List<string> List_Machine)
        {
            string ErrMsg = "";
            try
            {
                #region get DT for wave program                 
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //Convert.ToDateTime(sDateFrom).ToString("yyyy-MM-dd 00:00:00");
                string DateFrom = DateTime.Now.AddHours(-24).ToString("yyyy-MM-dd HH:mm:ss");  // Convert.ToDateTime(sDateTo).ToString("yyyy-MM-dd 23:59:59");
                string McList = string.Join(",", List_Machine.ToArray());
                //string SQL_Prg = @"select * from POLM_Wave_Program2 where 
                //        logTime > to_date('#DateFrom#','yyyy-mm-dd hh24:mi:ss') and logTime < to_date('#DateTo#','yyyy-mm-dd hh24:mi:ss') and
                //        instr('#McList#',Machine,1) > 0
                //    ";
                string SQL_Prg = @"
                    select * from (
                                    select Machine,LogTime,Program,Flux_BdWid, Flux_ConvSpd, Flux_NozSpd, Flux_Volumn, Flux_NozSpray, Flux_Power, Flux_Pres,Flux_BiModel,
                                                    Heat_Low1, Heat_Low2, Heat_Low3, Heat_Upp1, Heat_Upp2, Heat_Upp3, SP_Temp, SP_ConWave,  SP_LdClear, Conv_Speed, Conv_Width,
                                                    Other_Ni, Remark , UpdateTime, row_number() over(partition by Machine order by LogTime desc) as num FROM POLM_Wave_Program2 
                                        where logTime > to_date('#DateFrom#','yyyy-mm-dd hh24:mi:ss') and logTime < to_date('#DateTo#','yyyy-mm-dd hh24:mi:ss')
                                                and instr('#McList#',Machine,1) > 0
                                            order by Machine, LogTime desc
                        ) where MOD(num,2)=1
                    ";  //取奇数行，数量减少一半
                SQL_Prg = SQL_Prg.Replace("#DateFrom#", DateFrom);
                SQL_Prg = SQL_Prg.Replace("#DateTo#", DateTo);
                SQL_Prg = SQL_Prg.Replace("#McList#", McList);
                DataTable dt_Prg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL_Prg, ref ErrMsg);
                #endregion

                return dt_Prg;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        /// <summary>
        /// 获取Cell的 24小时的所有的 wave 程序参数, 并采样获取
        /// </summary>
        private DataTable DT_GetData_WaveProgram2(List<string> List_Machine, int nInterval)
        {
            string ErrMsg = "";
            try
            {
                #region nInterval
                string Sql_Interval = "";
                switch (nInterval)
                {
                    case 1:
                        Sql_Interval = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21";
                        break;
                    case 2:
                        Sql_Interval = "1,3,5,7,9,11,13,15,17,19,21";
                        break;
                    case 3:
                        Sql_Interval = "1,4,7,10,13,16,19";
                        break;
                    case 4:
                        Sql_Interval = "1,5,9,13,17,21";
                        break;
                    case 5:
                        Sql_Interval = "1,6,11,16,21";
                        break;
                    case 6:
                        Sql_Interval = "1,7,13,19";
                        break;
                    case 7:
                        Sql_Interval = "1,8,15";
                        break;
                } 
                #endregion


                #region get DT for wave program                 
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //Convert.ToDateTime(sDateFrom).ToString("yyyy-MM-dd 00:00:00");
                //string DateFrom = DateTime.Now.AddHours(-24).ToString("yyyy-MM-dd HH:mm:ss");  // Convert.ToDateTime(sDateTo).ToString("yyyy-MM-dd 23:59:59");
                string DateFrom = DateTime.Now.AddHours(-23).ToString("yyyy-MM-dd HH:00:00");  // Convert.ToDateTime(sDateTo).ToString("yyyy-MM-dd 23:59:59");
                string McList = string.Join(",", List_Machine.ToArray());
                //string SQL_Prg = @"select * from POLM_Wave_Program2 where 
                //        logTime > to_date('#DateFrom#','yyyy-mm-dd hh24:mi:ss') and logTime < to_date('#DateTo#','yyyy-mm-dd hh24:mi:ss') and
                //        instr('#McList#',Machine,1) > 0
                //    ";
                string SQL_Prg = @"
                            select * from
                            (
                                select Machine,LogTime,Program,Flux_BdWid, Flux_ConvSpd, Flux_NozSpd, Flux_Volumn, Flux_NozSpray, Flux_Power, Flux_Pres,Flux_BiModel,
                                                    Heat_Low1, Heat_Low2, Heat_Low3, Heat_Upp1, Heat_Upp2, Heat_Upp3, SP_Temp, SP_ConWave,  SP_LdClear, Conv_Speed, Conv_Width,
                                                    Other_Ni, Remark , UpdateTime, num, hour, row_number() over(partition by Machine,hour order by LogTime desc) as num2 from
                                (
                                    select Machine,LogTime,Program,Flux_BdWid, Flux_ConvSpd, Flux_NozSpd, Flux_Volumn, Flux_NozSpray, Flux_Power, Flux_Pres,Flux_BiModel,
                                                    Heat_Low1, Heat_Low2, Heat_Low3, Heat_Upp1, Heat_Upp2, Heat_Upp3, SP_Temp, SP_ConWave,  SP_LdClear, Conv_Speed, Conv_Width,
                                                    Other_Ni, Remark , UpdateTime, row_number() over(partition by Machine order by LogTime desc) as num,
                                                    to_char(LogTime, 'HH24') as hour
                                                    FROM POLM_Wave_Program2 
                                        where logTime > to_date('#DateFrom#','yyyy-mm-dd hh24:mi:ss') and logTime < to_date('#DateTo#','yyyy-mm-dd hh24:mi:ss')
                                            order by Machine, LogTime desc
                                ) order by num, num2
                            ) where num2 in (#Interval#)
                    ";  //取奇数行，数量减少一半
                SQL_Prg = SQL_Prg.Replace("#DateFrom#", DateFrom);
                SQL_Prg = SQL_Prg.Replace("#DateTo#", DateTo);
                SQL_Prg = SQL_Prg.Replace("#McList#", McList);
                SQL_Prg = SQL_Prg.Replace("#Interval#", Sql_Interval);

                DataTable dt_Prg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL_Prg, ref ErrMsg);
                #endregion

                return dt_Prg;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        /// <summary>
        /// 获取Cell的 24小时的所有的 wave 程序参数, 并采样获取, nHour=取某小时的数据，-1=all
        /// </summary>
        private DataTable DT_GetData_WaveProgram3(List<string> List_Machine, int nInterval, int nHour, string sDay)
        {
            string ErrMsg = "";
            try
            {
                #region nInterval
                List_Track.Add("--------------------------------------------------------------------------Get program data ...");

                string Sql_Interval = "";
                switch (nInterval)
                {
                    case 1:
                        Sql_Interval = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21";
                        break;
                    case 2:
                        Sql_Interval = "1,3,5,7,9,11,13,15,17,19,21";
                        break;
                    case 3:
                        Sql_Interval = "1,4,7,10,13,16,19";
                        break;
                    case 4:
                        Sql_Interval = "1,5,9,13,17,21";
                        break;
                    case 5:
                        Sql_Interval = "1,6,11,16,21";
                        break;
                    case 6:
                        Sql_Interval = "1,7,13,19";
                        break;
                    case 7:
                        Sql_Interval = "1,8,15";
                        break;
                }
                #endregion

                List_Track.Add("------Interval to sample = " + Sql_Interval);

                #region get DT for wave program  
                #region get date for data
                if (sDay == "" || sDay == null)
                {
                    sDay = DateTime.Now.ToString("yyyy-MM-dd");
                }
                string DateTo = "", DateFrom = "";

                if (sDay == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    DateFrom = DateTime.Now.AddHours(-23).ToString("yyyy-MM-dd HH:00:00");
                    DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    DateFrom = Convert.ToDateTime(sDay).ToString("yyyy-MM-dd 00:00:00");
                    DateTo = Convert.ToDateTime(sDay).ToString("yyyy-MM-dd 23:59:59");
                } 
                #endregion

                string McList = string.Join("','", List_Machine.ToArray());

                string SQL_Prg = @"
                            select * from
                            (
                                select Machine,LogTime,Program,Flux_BdWid, Flux_ConvSpd, Flux_NozSpd, Flux_Volumn, Flux_NozSpray, Flux_Power, Flux_Pres,Flux_BiModel,
                                                    Heat_Low1, Heat_Low2, Heat_Low3, Heat_Upp1, Heat_Upp2, Heat_Upp3, SP_Temp, SP_ConWave,  SP_LdClear, Conv_Speed, Conv_Width, Other_Ni, 
                                                    Remark , UpdateTime, num, hour, row_number() over(partition by Machine,hour order by LogTime desc) as num2 
                                from
                                (
                                    select Machine,LogTime,Program,Flux_BdWid, Flux_ConvSpd, Flux_NozSpd, Flux_Volumn, Flux_NozSpray, Flux_Power, Flux_Pres,Flux_BiModel,
                                                    Heat_Low1, Heat_Low2, Heat_Low3, Heat_Upp1, Heat_Upp2, Heat_Upp3, SP_Temp, SP_ConWave,  SP_LdClear, Conv_Speed, Conv_Width,
                                                    Other_Ni, Remark , UpdateTime, row_number() over(partition by Machine order by LogTime desc) as num,
                                                    to_char(LogTime, 'HH24') as hour
                                                    FROM POLM_Wave_Program2 
                                        where logTime > to_date('#DateFrom#','yyyy-mm-dd hh24:mi:ss') and logTime < to_date('#DateTo#','yyyy-mm-dd hh24:mi:ss')
                                            order by Machine, LogTime desc
                                ) order by num, num2
                            ) where num2 in (#Interval#) #hour# and Machine in ('#McList#')
                            order by Program
                    ";  //取奇数行，数量减少一半
                SQL_Prg = SQL_Prg.Replace("#DateFrom#", DateFrom);
                SQL_Prg = SQL_Prg.Replace("#DateTo#", DateTo);
                SQL_Prg = SQL_Prg.Replace("#McList#", McList);
                SQL_Prg = SQL_Prg.Replace("#Interval#", Sql_Interval);
                if(nHour == -1)
                    SQL_Prg = SQL_Prg.Replace("#hour#", "");
                else
                    SQL_Prg = SQL_Prg.Replace("#hour#", " and hour=" + nHour);

                List_Track.Add("------SQL = " + Environment.NewLine + SQL_Prg);

                DataTable dt_Prg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL_Prg, ref ErrMsg);

                #region Wave program 名字 不规范的 要转换
                using (Entities context = new Entities())
                {
                    List<string> List_CfgProg_Cnt = context.POLM_CONFIG.Where(p => p.FAMILY == "WorkI" && p.TYPE == "PrgMap" && p.PARKEY == "Wave").Select(o=>o.PARVALUE).ToList();
                    var res_Cfg_Cnt = context.POLM_CONFIG.Where(p => p.FAMILY == "WorkI" && p.TYPE == "PrgMap" && p.PARKEY == "Wave").Select(o => new {oldVal = o.PARVALUE, newVal = o.DATA });
                    IEnumerable<DataRow> rows = dt_Prg.AsEnumerable().Where(p=> List_CfgProg_Cnt.Contains(p.Field<string>("Program"))).Select(o=>o);
                    foreach (DataRow row in rows)
                    {
                        dt_Prg.Columns["Program"].ReadOnly = false;
                        string sOldName = Convert.ToString(row["Program"]);
                        string sNewName = res_Cfg_Cnt.Where(p => p.oldVal == sOldName).Select(o => o.newVal).FirstOrDefault();
                        //rows.Select(o => o["Program"] = sNewName);
                        row["Program"] = sNewName;
                        //row.SetModified();
                        //row.SetField<string>("Program", sNewName); //.AcceptChanges();
                        //row.AcceptChanges();
                    }

                }
                #endregion


                List_Track.Add("  ");
                List_Track.Add("------SQL Result = " + ErrMsg +  ", Rows="  + (dt_Prg == null ? "0" : dt_Prg.Rows.Count.ToString()));
                #endregion

                #region check num=1 的时间是否和当前一致， 因为
                var res_First = dt_Prg.AsEnumerable().Where(p => p.Field<Decimal>("num") == 1).Select(o=>o.Field<string>("Hour")).FirstOrDefault();
                if(res_First != null)
                {
                    int nHur_First = Convert.ToInt32(res_First);
                    int nHur_Curr = DateTime.Now.Hour;
                    if(Math.Abs(nHur_First - nHur_Curr) > 2)
                    {
                        List_Track.Add("------Fail to compare hour => first = " + nHur_First + "  Current = " + nHur_Curr);
                        BroadcastMyMessage("sub", "Fail to compare hour => first = " + nHur_First + "  Current = " + nHur_Curr);
                    }
                    else
                    {
                        List_Track.Add("------Success to compare hour => first = " + nHur_First + "  Current = " + nHur_Curr);
                    }
                }
                #endregion

                return dt_Prg;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
            }
            return null;
        }




        /// <summary>
        /// 获取Cell的 24小时的所有的 wave 程序参数的最后一笔记录
        /// </summary>
        private DataTable DT_GetData_WaveProgram_LastRecord(List<string> List_Machine)
        {
            string ErrMsg = "";
            try
            {
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string DateFrom = DateTime.Now.AddHours(-24).ToString("yyyy-MM-dd HH:mm:ss");
                string McList = string.Join(",", List_Machine.ToArray());
                string SQL = @"
                                select * from 
                                (
                                    select Machine,LogTime,Program,Flux_BdWid, Flux_ConvSpd, Flux_NozSpd, Flux_Volumn, Flux_NozSpray, Flux_Power, Flux_Pres,Flux_BiModel,
                                                    Heat_Low1, Heat_Low2, Heat_Low3, Heat_Upp1, Heat_Upp2, Heat_Upp3, SP_Temp, SP_ConWave,  SP_LdClear, Conv_Speed, Conv_Width,
                                                    Other_Ni, Remark , UpdateTime, row_number() over(partition by Machine order by LogTime desc) as num FROM POLM_Wave_Program2 
                                        where logTime > to_date('#DateFrom#','yyyy-mm-dd hh24:mi:ss') and logTime < to_date('#DateTo#','yyyy-mm-dd hh24:mi:ss')
                                                and instr('#McList#',Machine,1) > 0
                                            order by Machine, LogTime desc
                                ) 
                                where num=1
                            ";
                SQL = SQL.Replace("#DateTo#", DateTo);
                SQL = SQL.Replace("#DateFrom#", DateFrom);
                SQL = SQL.Replace("#McList#", McList);

                DataTable dt_Prg_Last = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);

                return dt_Prg_Last;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        /// <summary>
        /// Program 是Wave的程序， Model是WorkI中的产品的名字(Model基本不用)
        /// </summary>
        private DataTable DT_GetData_WaveWI(List<string> List_Program, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                //MachineProgramMapping(List_Program);  //已经移到一开始就改变程序的

                #region get DT for WorkI 
                List_Track.Add("--------------------------------------------------------------------------");

                #region get workI standard parameter
                //List<string> List_Program = dt_Prg.AsEnumerable().Select(o => o.Field<string>("Program")).Distinct().ToList();
                string ProgramNames = string.Join(",", List_Program.ToArray());

                List_Track.Add("------WokI for programs = " + ProgramNames);

                #region obsolete
                //string SQL_WI = @"
                //        select LINE,PROJECT,OV_ID,MODEL,PROGRAM,PARA_NAME,VAL_CEN,VAL_MAX,VAL_MIN,REMARK from
                //        (
                //                SELECT a.ID, a.Line, a.Project, b.* from
                //            (
                //                SELECT ID, Line, Project from POLM_WI_OV where McType='Wave' 
                //            ) a left join 
                //            (
                //                SELECT * FROM POLM_WI_PARA   
                //            ) b
                //            ON a.ID=b.OV_ID
                //        ) where  instr(UPPER('#Programs#'),UPPER(Program),1) > 0 --  UPPER('#Programs#') = UPPER(Program)
                //    ";
                //SQL_WI = SQL_WI.Replace("#Programs#", ProgramNames); 
                #endregion
                //获取WorkI 数据 from VIEW=>POLM_WAVE_REP_WI
                ///////////////////////////////////////////增加了版本筛选，根据EffDate筛选出满足要求的，选择最新的WorkI(num=1)
                string SQL_WI = @"
                        --select Line,Project,Model,Program,Para_Name,VAL_CEN,TEMPDAYS,Val_Max,Val_Min,EffDate,DocRev from POLM_WAVE_REP_WI
                        --where instr(UPPER('#Programs#'),UPPER(Program),1) > 0

                        select Line,Project,Model,Program,Para_Name,VAL_CEN,TEMPDAYS,Val_Max,Val_Min,EffDate,DocRev,num from
                        (
                            select a.Line,a.Project,a.Model,a.Program,a.Para_Name,a.VAL_CEN,a.TEMPDAYS,a.Val_Max,a.Val_Min,a.EffDate,a.DocRev, b.num from
                            (
                                select Line,Project,Model,Program,Para_Name,VAL_CEN,TEMPDAYS,Val_Max,Val_Min,EffDate,DocRev
                                 from POLM_WAVE_REP_WI
                                where instr(UPPER('#Programs#'),UPPER(Program),1) > 0 and EffDate > To_Date('2018-8-2', 'YYYY-MM-DD hh24:mi')
                             ) a   left join 
                             (
                                 select Line,Project, DocRev, row_number() over (partition by Line,Project order by DocRev desc) as num from
                                 (
                                    select Line,Project, DocRev from POLM_WAVE_REP_WI group by Line,Project, DocRev order by Line,Project,DocRev
                                )
                            ) b on a.Line=b.Line and a.Project=b.Project and a.DocRev=b.DocRev 
                        ) where num =1 -- order by line,Project, num
                ";
                //SQL_WI = SQL_WI.Replace("#Line#", Line); Line='#Line#'
                SQL_WI = SQL_WI.Replace("#Programs#", ProgramNames);
                DataTable dt_WI = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL_WI, ref ErrMsg);
                //DataTable dt_WI = WI_GetAllParameters();
                #endregion

                #region check program is in datatable
                foreach (string sItemPrg in List_Program)
                {
                    var resExt = dt_WI.AsEnumerable().Where(p => p.Field<string>("Program").ToUpper() == sItemPrg.ToUpper()).Select(o => o);
                    if(resExt == null || resExt.Count() == 0)
                    {
                        RegisterProgramIsNotInWorkI(sItemPrg);

                        ErrMsg = "Program not in WorkI => " + sItemPrg;
                        BroadcastMyMessage("sub", "Program not in WorkI " + sItemPrg );
                        List_Track.Add("------Program not in WorkI" + sItemPrg);
                    }
                }
                #endregion

                #region tracking
                List_Track.Add("------SQL = " + Environment.NewLine + SQL_WI);
                List_Track.Add("------SQL Result = " + ErrMsg + ", Rows = " + (dt_WI == null ? "0" : dt_WI.Rows.Count.ToString())); 
                #endregion
                #endregion

                return dt_WI;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        #endregion

        #region additional function

        /// <summary>
        /// 把wave的程序的名字矫正，如 wave program=RH-Settig14,  WorkI=RH-Setting14
        /// </summary>
        private void MachineProgramMapping(List<string> List_Program)
        {
            string ErrMsg = "";
            try
            {
                using (Entities context = new Entities())
                {
                    //context.Database.Connection.Open();
                    var res = context.POLM_CONFIG.Where(p => p.FAMILY == "WorkI" && p.TYPE == "PrgMap" && p.PARKEY == "Wave")
                                   .Select(o => o);

                    if (res != null && res.Count() > 0)
                    {
                        for (int i = 0; i < List_Program.Count; i++)
                        {
                            string sItem = List_Program[i];
                            var resPrg = res.Where(p => p.PARVALUE.ToUpper() == sItem.ToUpper()).Select(o => o.DATA);
                            if (resPrg != null && resPrg.Count() > 0)
                            {
                                string sNewProgName = resPrg.ToList()[0];
                                List_Program[i] = sNewProgName;
                            }
                        }
                        //foreach(string sItem in List_Program)
                        //{
                        //    var resPrg = res.Where(p => p.PARVALUE.ToUpper() == sItem.ToUpper()).Select(o => o.DATA);
                        //    if(resPrg != null && resPrg.Count() > 0)
                        //    {
                        //        List_Program[]
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        private string GetConfig_Line(DataTable dtCfg, string Machine)
        {
            string ErrMsg = "";
            try
            {
                var mcLine = dtCfg.AsEnumerable().Where(p => p.Field<string>("Machine") == Machine).Select(o => o.Field<string>("ParValue")).FirstOrDefault();
                if (mcLine == null && mcLine == "")
                {
                    ErrMsg = "Fail to get line for machine=" + Machine;
                    return "";
                }
                else
                {
                    return mcLine;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        private string GetWIPara_Project(DataTable dtWI, string Line, string Program)
        {
            string ErrMsg = "";
            try
            {
                var rowWI = dtWI.AsEnumerable().Where(p => p.Field<string>("Line") == Line && p.Field<string>("Program").ToUpper() == Program.ToUpper() 
                    ).Select(o => o.Field<string>("Project")).FirstOrDefault();
                if (rowWI != null)
                {
                    string Project = rowWI; // Convert.ToString(rowWI["Project"]);
                    return Project;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        private float GetValue_Float(string sValue, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                if (Regex.IsMatch(sValue, @"^[-]?\d+[.]?\d*$"))
                {
                    float fValue = Convert.ToSingle(sValue);
                    ErrMsg = "Success";
                    return fValue;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return 0.0f;
        }

        private bool IsWiProgramEqualWithWaveProgram(string WIProgram, string WaveProgram)
        {
            string ErrMsg = "";
            try
            {
                if (WaveProgram.IndexOf(".") > 0)
                {
                    WaveProgram = WaveProgram.Substring(0, WaveProgram.Length - 4);
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return false;
        }

        private string Program_GetPara(DataTable dtPrg, string Key_Para)
        {
            string ErrMsg = "";
            try
            {
                //var res = dtPrg.AsEnumerable().
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }
        

        private DataTable CreateNewDataTableForReport()
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "PolmRep";
                DataColumn column = new DataColumn("Line", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Machine", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Project", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Program", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Parameter", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Val_Cen", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Val_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Val_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Val_Act", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("TEMPDAYS", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                
                column = new DataColumn("Time", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Comment", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Status", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("PF", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Num", System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Hour", System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);

                #endregion

                return m_DT;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private DataTable CreateNewDataTableForChartWave()
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "PolmChartWave";
                DataColumn column = new DataColumn("Time", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Line1", System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Line2", System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Line3", System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Line4", System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Num", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Remark1", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Remark2", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Remark3", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Remark4", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                #endregion

                return m_DT;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        
        private List<Int32> GetHoursFromNow() //out int nLastHurNow)
        {
            //nLastHurNow = 0;
            string ErrMsg = "";
            try
            {
                List<Int32> List_Hrs = new List<Int32>();

                for (int i = 23; i >= 0; i--)
                {
                    List_Hrs.Add(DateTime.Now.AddHours(-i).Hour);
                }
                //for (int i = 0; i < 24; i++)
                //{
                //    List_Hrs.Add(DateTime.Now.AddHours(-i).Hour);
                //}
                //nLastHurNow = DateTime.Now.Hour;
                return List_Hrs;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }
        #endregion


        #region Judge program if have WorkI
        private bool IsProgramHaveWorkIData(string Program)
        {
            string ErrMsg = "";
            try
            {
                //using (Entities context = new Entities())
                //{
                //    context.POLM_WI_OVSet.Where(p=>p.f)

                //}
                //Program = Program.ToUpper();
                //string SQL = "select * from POLM_WI_PARA where model='" + Program + "'";
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return false;
        }
        
        private void RegisterProgramIsNotInWorkI(string sProgram)
        {
            string ErrMsg = "";
            try
            {
                string sRemark = "";
                OracleParameter[] param = new OracleParameter[4];
                param[0] = new OracleParameter("sModel", "Wave_Issue");
                param[1] = new OracleParameter("p_Para1", sProgram);
                param[2] = new OracleParameter("p_Para2", "N");
                param[3] = new OracleParameter("p_Para3", sRemark);

                MyOracleSql.Oracle_Sql.StoreProcedure_Paras("P_POLM", param, ref ErrMsg);

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }
        
        #endregion
        #endregion

        #region doubleclick -- get Marker detailed hour data
        private DataTable GetReport_CellParas_Wave_Item(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string sCell = viewBag.Para1.Split(' ')[1].Trim();
                string sCell_Line = viewBag.Para1.Split(' ')[2].Trim();

                //string sCell = viewBag.Para1;
                string sDayChart = viewBag.Para5;  //要显示数据的日期
                if (sDayChart == null || sDayChart == "")
                {
                    sDayChart = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    sDayChart = Convert.ToDateTime(sDayChart).ToString("yyyy-MM-dd");
                }
                #region tracking
                List_Track.Add("----Data for cell = " + sCell + ", day=" + sDayChart);
                #endregion

                #region get line
                string sLine = viewBag.Para2;
                sLine = sLine.Replace("Line", "");
                //if (sCell == "1")
                //{
                //    if (sLine == "1")
                //        sLine = "3";
                //    else if (sLine == "2")
                //        sLine = "6";
                //    else if (sLine == "3")
                //        sLine = "7";
                //    else if (sLine == "4")
                //        sLine = "7";
                //}

                #region tracking
                List_Track.Add("----Data for line = " + sLine);
                #endregion

                int nIndex = Convert.ToInt32(viewBag.Para3);
                #endregion

                dynamic record = JsonConvert.DeserializeObject(viewBag.Para4);
                if(sCell != "")
                {
                    #region line string
                    //var value = ""; // Convert.ToString(record.sLine);
                    //if (sLine == "Line1")
                    //    value = record.Line1;
                    //else if (sLine == "Line2")
                    //    value = record.Line2;
                    //else if (sLine == "Line3")
                    //    value = record.Line3; 
                    #endregion

                    var nHour = Convert.ToInt32(record.Time);

                    #region tracking
                    List_Track.Add("----Data for hour = " + nHour);
                    #endregion

                    if (nHour != null && sLine != "")
                    {
                        DataTable dtCfg = null;
                        List<string> List_Mc = GetMachine_CellLine(sCell, sLine, out dtCfg, ref ErrMsg);

                        #region tracking
                        List_Track.Add("----Get machine for cell and line = " + String.Join(",", List_Mc.ToArray()));
                        List_Track.Add(" ");
                        List_Track.Add("----Get Program data");
                        #endregion

                        DataTable dt_Prg = DT_GetData_WaveProgram3(List_Mc, 5, nHour, sDayChart);

                        if (dt_Prg != null && dt_Prg.Rows.Count > 0)
                        {
                            List<string> List_Program = dt_Prg.AsEnumerable().Select(o => o.Field<string>("Program")).Distinct().ToList();

                            #region tracking
                            List_Track.Add(" ");
                            List_Track.Add("----Get WorkI data");
                            List_Track.Add("------Program list = " + String.Join(",", List_Program.ToArray()));
                            #endregion


                            DataTable dt_WI = DT_GetData_WaveWI(List_Program, ref ErrMsg);

                            if (dt_WI != null && dt_WI.Rows.Count > 0)
                            {
                                DataTable dt_Report = Polm_Report_Generate_ALL(dt_Prg, dt_WI, dtCfg);

                                #region tracking
                                List_Track.Add(" ");
                                List_Track.Add("----Get Detailed data");
                                List_Track.Add("------Rows = " + (dt_Report == null ? "0" : dt_Report.Rows.Count.ToString()));
                                #endregion

                                return dt_Report;
                            }
                            else
                            {
                                #region tracking
                                if(ErrMsg == "")
                                    ErrMsg = "Fail to get WI datatable";
                                List_Track.Add(ErrMsg);
                                #endregion
                            }
                        }
                        else
                        {
                            #region tracking
                            ErrMsg = "Fail to get program data";
                            List_Track.Add(ErrMsg);
                            #endregion
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        private List<string> GetMachine_CellLine(string sCell, string sLine, out DataTable dtCfg, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region 获取 config of cell from server 
                sCell = sCell.Replace("Cell", "").Trim();
                if (sCell == "")
                {
                    ErrMsg = "Fail to get cell value";
                    dtCfg = null;
                    return null;
                }
                string SQL = "select * from POLM_Config where Family='Cell' and Type='Machine' and ParKey='#Cell#' and Data='Wave'";
                SQL = SQL.Replace("#Cell#", sCell);

                //获取配置 Line，for Cell=1  Wave
                dtCfg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                #endregion

                if (sLine == "ALL")
                {
                    List<string> List_Machine = dtCfg.AsEnumerable().Select(o => o.Field<string>("Machine")).ToList();
                    if (List_Machine != null && List_Machine.Count > 0)
                    {
                        return List_Machine;
                    }
                }
                else
                {
                    List<string> List_Machine = dtCfg.AsEnumerable().Where(p=>p.Field<string>("ParValue") == sLine).Select(o => o.Field<string>("Machine")).ToList();
                    if (List_Machine != null && List_Machine.Count > 0)
                    {
                        return List_Machine;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            dtCfg = null;
            return null;
        }
        #endregion


        #region Get config data

        public string GetConfig_Para(string sKey)
        {
            string ErrMsg = "";
            try
            {
                string SQL = "select * from POLM_Config";
                DataTable dtCfg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                if(dtCfg != null && dtCfg.Rows.Count > 0)
                {
                    if(sKey == "RepInterval")
                    {
                        var RepInt = dtCfg.AsEnumerable().Where(p => p.Field<string>("Family") == "Wave" && p.Field<string>("Type") == "Run" &&
                            p.Field<string>("ParKey") == "Interval" && p.Field<string>("Machine") == "ALL").Select(o=>o.Field<string>("ParValue")).FirstOrDefault();
                        if (RepInt != null)
                        {
                            return RepInt;
                        }
                        else
                        {
                            return "3";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        #endregion
    }
}