using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MyModels.Common;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

namespace POLM.Models.Home
{
    public class ReviewWiHelper
    {
        public List<string> List_Error = new List<string>();
        public List<string> List_Track = new List<string>();

        #region get WI parameter projects
        public List<string> WI_Para_Project(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            List<string> List_Project = new List<string>();
            try
            {
                string McType = viewBag.Para1.ToUpper();
                string SQL = "select distinct project from POLM_WI_OV where UPPER(McType)='" + McType + "' order by project";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                if (dt != null && dt.Rows.Count > 0)
                {
                    List_Project = dt.AsEnumerable().Select(o => o.Field<string>("Project")).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return List_Project;
        }
        public List<string> WI_Para_Line(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            List<string> List_Line = new List<string>();
            try
            {
                string McType = viewBag.Para1.ToUpper();
                string SQL = "select distinct Line from POLM_WI_OV where UPPER(McType)='" + McType + "' order by line";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                if (dt != null && dt.Rows.Count > 0)
                {
                    List_Line = dt.AsEnumerable().Select(o => o.Field<string>("Line")).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return List_Line;
        }

        public List<string> WI_Para_Line_BaseProject(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            List<string> List_Line = new List<string>();
            try
            {
                string McType = viewBag.Para1.ToUpper();
                string Project = viewBag.Para2.ToUpper().Trim();
                //string SQL = "select distinct Line from POLM_WI_OV where UPPER(McType)='" + McType + "' order by line";
                string SQL = "select distinct Line from POLM_WI_OV where UPPER(McType)='#McType#' and UPPER(Project)='#Project#' order by line";
                SQL = SQL.Replace("#McType#", McType);
                SQL = SQL.Replace("#Project#", Project);
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                if (dt != null && dt.Rows.Count > 0)
                {
                    List_Line = dt.AsEnumerable().Select(o => o.Field<string>("Line")).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return List_Line;
        }

        #endregion
        public DataTable GetReviewedData(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string Project = viewBag.Project;
                string McType = viewBag.Para1;
                string Line = viewBag.Para2;
                
                if(McType == "Wave")
                {
                    #region Wave
                    #region obsolete
                    //string SQL = "SELECT * FROM POLM_WI_OV where McType='Wave' and Project='#Project#' and Line='#Line#'";
                    //string SQL = @"
                    //    select WorkSheet,DocNum,DocRev,b.Model,b.Program,b.Para_Name,b.Val_Cen,b.Val_Max,b.Val_Min,b.OV_ID from
                    //    (
                    //        select * from POLM_WI_OV where McType='Wave' and Project='#Project#' and Line='#Line#'
                    //    ) a left join
                    //    (
                    //        select * from POLM_WI_PARA  
                    //    ) b on a.ID = b.OV_ID
                    //"; 
                    //////////////////////////////////增加了参数描述，使用Table=>POLM_Config
                    //string SQL = @"
                    //    select WorkSheet,DocNum,DocRev,Model,Program,Para_Name, Val_Cen, Val_Max, Val_Min, OV_ID, d.Data as Para_Desc from
                    //    (
                    //        select WorkSheet,DocNum,DocRev,b.Model,b.Program,b.Para_Name,b.Val_Cen,b.Val_Max,b.Val_Min,b.OV_ID from
                    //        (
                    //            select * from POLM_WI_OV where McType='Wave' and Project='#Project#' and Line='#Line#'
                    //        ) a left join
                    //        (
                    //            select * from POLM_WI_PARA  
                    //        ) b on a.ID = b.OV_ID
                    //    ) c left join
                    //    ( 
                    //        select ParValue,Data from POLM_Config WHERE Family='WorkI' and Type='Parameter' and ParKey='Desc'
                    //    ) d on Upper(c.Para_Name)=Upper(d.ParValue)
                    //";
                    #endregion
                    //////////////////////////////////增加了参数描述，使用Table=>POLM_Config
                    /* WorkI 的参数需要考虑Tempory WI, 如果TemporyWI没有到期 （3天），则使用TemporyWI,否则使用正式WorkI参数
                     * 
                     */ 
                    string SQL = @"
                        select WorkSheet,DocNum,DocRev,Model,Program,Para_Name,OV_ID,Para_Desc, Val_Cen,TEMPDAYS, 
                        (case when TEMPDAYS is null or TEMPDAYS='0' then Val_Max else Val_MaxT end ) as Val_Max,
                        (case when TEMPDAYS is null or TEMPDAYS='0' then Val_Min else Val_MinT end ) as Val_Min 
                        from
                        (
                            select WorkSheet,DocNum,DocRev,Model,Program,Para_Name,OV_ID,Para_Desc, Val_Cen,Val_Max,Val_Min,Val_MaxT, Val_MinT,
                                (case when difDay >= 3 then '0' else TO_CHAR(difDay,'9.99') end) TEMPDAYS from 
                            (
                                 select e.WorkSheet,e.DocNum,e.DocRev,e.Model,e.Program,e.Para_Name, e.Val_Cen, e.Val_Max, e.Val_Min, e.OV_ID, e.Para_Desc, f.day_diff as difDay, f.Val_Max as Val_MaxT, f.Val_Min as Val_MinT from
                                 (
                                    select WorkSheet,DocNum,DocRev,Model,Program,Para_Name, Val_Cen, Val_Max, Val_Min, OV_ID, d.Data as Para_Desc from
                                    (
                                        select WorkSheet,DocNum,DocRev,b.Model,b.Program,b.Para_Name,b.Val_Cen,b.Val_Max,b.Val_Min,b.OV_ID from
                                        (
                                            select * from POLM_WI_OV where McType='Wave' and Project='#Project#' and Line='#Line#'
                                        ) a left join
                                        (
                                            select * from POLM_WI_PARA  
                                        ) b on a.ID = b.OV_ID
                                    ) c left join
                                    ( 
                                        select ParValue,Data from POLM_Config WHERE Family='WorkI' and Type='Parameter' and ParKey='Desc'
                                    ) d on Upper(c.Para_Name)=Upper(d.ParValue)
                                ) e left join 
                                (
                                    select Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, num, (sysdate - UpdateTime) day_diff from
                                    (
                                        select  Project, Line, Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, Remark,
                                            row_number() over (partition by Model,Program, Para_Name order by UpdateTime desc) as num
                                        from POLM_WI_TEMPORY where Machine='Wave' and Project='#Project#' AND Line='#Line#'
                                    ) where num=1 and (INSTR(Remark, 'No')=0 or Remark is null)
                                ) f on e.Model=f.Model and e.Program=f.Program and e.Para_Name=f.Para_Name
                            )       
                        )
                    ";

                    SQL = SQL.Replace("#Project#", Project);
                    SQL = SQL.Replace("#Line#", Line);
                    DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                    if(dt !=null && dt.Rows.Count > 0)
                    {
                        #region slq
                        return dt;
                        #endregion
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        #region Tempory WorkI parameter update
        public void WI_Tempory_Para_Update(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region tracking
                List_Track.Add("WI_Tempory_Para_Update v1.1");
                #endregion

                string Project = viewBag.Para1;
                string Line = viewBag.Para2;
                string User = viewBag.User;

                #region tracking
                List_Track.Add("--get Project=" + Project + ", Line=" + Line + ", User=" + User);
                #endregion

                dynamic objJson = JsonConvert.DeserializeObject(viewBag.jsonData);
                string Model = objJson.MODEL;
                string Program = objJson.PROGRAM;
                string Para_name = objJson.PARA_NAME;
                string Para_max = objJson.VAL_MAX;
                string Para_min = objJson.VAL_MIN;
                string Para_VAL_Temp = objJson.VAL_Temp; //临时WI的天数，3,2,1或者No Temp
                string Remark = Para_VAL_Temp;
                #region tracking
                List_Track.Add("--get Model=" + Model + ", Program=" + Program);
                List_Track.Add("--get Para_name=" + Para_name + ", Para_max=" + Para_max + ", Para_min=" + Para_min);
                #endregion


                #region 仅仅插入数据， 不Update
                OracleParameter[] param = new OracleParameter[11];
                param[0] = new OracleParameter("sModel", "Wave_TempWI_Update");
                param[1] = new OracleParameter("p_Para1", "Wave");
                param[2] = new OracleParameter("p_Para2", Project);
                param[3] = new OracleParameter("p_Para3", Line);
                param[4] = new OracleParameter("p_Para4", Model);
                param[5] = new OracleParameter("p_Para5", Program);
                param[6] = new OracleParameter("p_Para6", Para_name);
                param[7] = new OracleParameter("p_Para7", Para_max);
                param[8] = new OracleParameter("p_Para8", Para_min);
                param[9] = new OracleParameter("p_Para9", Remark);
                param[10] = new OracleParameter("p_Para10", User);
                MyOracleSql.Oracle_Sql.StoreProcedure_Paras("P_POLM", param, ref ErrMsg);
                #endregion

                #region tracking
                List_Track.Add("--Update tempory worki result = " + ErrMsg);
                #endregion

            }
            catch (Exception ex)
            {
                #region track
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
                List_Error.Add(ErrMsg);
                #endregion
            }
        }
        #endregion

    }
}