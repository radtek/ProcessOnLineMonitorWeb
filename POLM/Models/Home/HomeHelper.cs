using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MyModels.Common;
using Newtonsoft.Json;

namespace POLM.Models.Home
{
    public class HomeHelper
    {
        public void GetCategoryFromServer(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string SQL = "select * from POLM_CATEGORY order by Category,ViewOrder";
                //string sCategory = viewBag.Para1.Trim();
                //if (viewBag.Para1 != "ALL" && viewBag.Para1 != "")
                //{
                //    SQL = "select * from Web_Config where Category='" + sCategory + "' order by Category,ViewOrder";
                //}

                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);


                DataTable dtJs = CreateNewDataTableForSoftList();
                #region add data into table
                foreach (DataRow dr in dt.Rows)
                {
                    string item = Convert.ToString(dr["ITEM"]);
                    DataRow drNew = dtJs.NewRow();
                    drNew["Category"] = Convert.ToString(dr["CATEGORY"]);
                    drNew["Item"] = item;
                    drNew["Link"] = Convert.ToString(dr["LINK"]);
                    drNew["Desc"] = Convert.ToString(dr["ITEMDESC"]);
                    drNew["ViewOrder"] = Convert.ToString(dr["VIEWORDER"]);
                    //drNew["Remark"] = Convert.ToString(dr["REMARK"]);
                    string sRemark = Convert.ToString(dr["REMARK"]);
                    drNew["View"] = sRemark;
                    drNew["iid"] = Convert.ToString(dr["IID"]);
                    drNew["Train"] = Convert.ToString(dr["ITEMCOMM"]);
                    dtJs.Rows.Add(drNew);
                }

                var dtJs_Wave = dtJs.AsEnumerable().Where(p => p.Field<string>("CATEGORY").ToUpper() == "WAVE" ).Select(o=>o).CopyToDataTable();
                #endregion

                if (dtJs != null && dtJs.Rows.Count > 0)
                {
                    var v_Category = dtJs.AsEnumerable().OrderByDescending(p => p.Field<string>("CATEGORY")).Select(p => p.Field<string>("CATEGORY")).Distinct();
                    viewBag.jsonData = JsonConvert.SerializeObject(v_Category.ToList());
                }

                if (dtJs_Wave.Rows.Count > 0)
                {
                    viewBag.jsonOut = JsonConvert.SerializeObject(dtJs_Wave);
                    viewBag.Message = "Success to get category";

                }
                else
                {
                    viewBag.Message = "Fail to get category";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                viewBag.Message = ErrMsg;
            }
        }

        public void GetCategory_Machine(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string SQL = "select * from POLM_CATEGORY order by Category,ViewOrder";
                string sMcType = viewBag.Para1.Trim();
                if (viewBag.Para1 != "ALL" && viewBag.Para1 != "")
                {
                    SQL = "select * from POLM_CATEGORY where Category='" + sMcType + "' order by Category,ViewOrder";
                }

                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);

                DataTable dtJs = CreateNewDataTableForSoftList();
                #region add data into table
                foreach (DataRow dr in dt.Rows)
                {
                    string item = Convert.ToString(dr["ITEM"]);
                    DataRow drNew = dtJs.NewRow();
                    drNew["Category"] = Convert.ToString(dr["CATEGORY"]);
                    drNew["Item"] = item;
                    drNew["Link"] = Convert.ToString(dr["LINK"]);
                    drNew["Desc"] = Convert.ToString(dr["ITEMDESC"]);
                    drNew["ViewOrder"] = Convert.ToString(dr["VIEWORDER"]);
                    //drNew["View"] = Convert.ToString(dr["REMARK"]);
                    string sRemark = Convert.ToString(dr["REMARK"]);
                    drNew["View"] = sRemark;
                    drNew["iid"] = Convert.ToString(dr["IID"]);
                    drNew["Train"] = Convert.ToString(dr["ITEMCOMM"]);
                    dtJs.Rows.Add(drNew);
                }
                #endregion

                if (dtJs != null && dtJs.Rows.Count > 0)
                {
                    viewBag.jsonOut = JsonConvert.SerializeObject(dtJs);
                    viewBag.Message = "Success to get List for " + sMcType;
                }
                else
                {
                    viewBag.Message = "Fail to get list for " + sMcType;
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        private DataTable CreateNewDataTableForSoftList()
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "SoftList";
                DataColumn column = new DataColumn("Category", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Item", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Link", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Desc", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("ViewOrder", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Remark", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("View", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("iid", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Train", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                #endregion

                return m_DT;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        #region for real time data

        public DataTable GetRealTimeData(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region GetRealTimeData
                string date_Sel = viewBag.Para1;
                string timeFrom = viewBag.Para2;
                string timeTo = viewBag.Para3;

                string time_Beg = date_Sel + " " + timeFrom + ":00:00";
                string time_End = date_Sel + " " + timeTo + ":59:59";

                time_Beg = Convert.ToDateTime(time_Beg).ToString("yyyy-MM-dd HH:mm:ss");
                time_End = Convert.ToDateTime(time_End).ToString("yyyy-MM-dd HH:mm:ss");

                //time_End = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");
                string SQL = @"select * from POLM_Wave_Program2 where updateTime > To_Date('" + time_Beg + "', 'yyyy-MM-dd HH24:mi:ss') and updateTime < To_Date('" +
                    time_End + "','yyyy-MM-dd HH24:mi:ss') order by LogTime desc";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);

                DataTable dt_Fin = CreateNewDataTable_Wave();
                foreach(DataRow dr in dt.Rows)
                {
                    DataRow drN = dt_Fin.NewRow();
                    drN["Machine"] = Convert.ToString(dr["MACHINE"]);
                    drN["ProgName"] = Convert.ToString(dr["PROGRAM"]);
                    drN["Flux_BdWid"] = Convert.ToString(dr["Flux_BdWid"]);
                    drN["Flux_ConvSpd"] = Convert.ToString(dr["Flux_ConvSpd"]);
                    drN["Flux_NozSpd"] = Convert.ToString(dr["Flux_NozSpd"]);
                    drN["Flux_Volumn"] = Convert.ToString(dr["Flux_Volumn"]);
                    drN["Flux_NozSpray"] = Convert.ToString(dr["Flux_NozSpray"]);
                    drN["Flux_Power"] = Convert.ToString(dr["Flux_Power"]);
                    drN["Flux_Pres"] = Convert.ToString(dr["Flux_Pres"]);
                    drN["Flux_BiModel"] = Convert.ToString(dr["Flux_BiModel"]);
                    drN["Heat_Low1"] = Convert.ToString(dr["Heat_Low1"]);
                    drN["Heat_Low2"] = Convert.ToString(dr["Heat_Low2"]);
                    drN["Heat_Low3"] = Convert.ToString(dr["Heat_Low3"]);
                    drN["Heat_Upp1"] = Convert.ToString(dr["Heat_Upp1"]);
                    drN["Heat_Upp2"] = Convert.ToString(dr["Heat_Upp2"]);
                    drN["Heat_Upp3"] = Convert.ToString(dr["Heat_Upp3"]);
                    drN["SP_Temp"] = Convert.ToString(dr["SP_Temp"]);
                    drN["SP_ConWave"] = Convert.ToString(dr["SP_ConWave"]);
                    drN["SP_LdClear"] = Convert.ToString(dr["SP_LdClear"]);
                    drN["Conv_Speed"] = Convert.ToString(dr["Conv_Speed"]);
                    drN["Conv_Width"] = Convert.ToString(dr["Conv_Width"]);

                    dt_Fin.Rows.Add(drN);
                }

                return dt_Fin;
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        public DataTable CreateNewDataTable_Wave()   //copy from UploadWi_Excel.cs
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "WaveCont";
                DataColumn column = new DataColumn("Machine", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("ProgName", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_BdWid", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_BdWid_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_BdWid_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Flux_ConvSpd", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_ConvSpd_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_ConvSpd_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Flux_NozSpd", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_NozSpd_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_NozSpd_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Flux_Volumn", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_Volumn_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_Volumn_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Flux_NozSpray", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_NozSpray_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_NozSpray_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Flux_Power", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_Power_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_Power_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Flux_Pres", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_Pres_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Flux_Pres_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);


                column = new DataColumn("Flux_BiModel", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Low1", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Low1_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Low1_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Low2", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Low2_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Low2_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Low3", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Low3_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Low3_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Upp1", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Upp1_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Upp1_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Upp2", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Upp2_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Upp2_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Upp3", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Upp3_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Heat_Upp3_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("SP_Temp", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("SP_Temp_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("SP_Temp_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);


                column = new DataColumn("SP_ConWave", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("SP_ConWave_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("SP_ConWave_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("SP_LdClear", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("SP_LdClear_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("SP_LdClear_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Conv_Speed", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Conv_Speed_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Conv_Speed_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                column = new DataColumn("Conv_Width", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                //column = new DataColumn("Conv_Width_Max", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);
                //column = new DataColumn("Conv_Width_Min", System.Type.GetType("System.String"));
                //m_DT.Columns.Add(column);

                column = new DataColumn("Other_Ni", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Remark", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);


                column = new DataColumn("Check", System.Type.GetType("System.Boolean"));
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

    }
}