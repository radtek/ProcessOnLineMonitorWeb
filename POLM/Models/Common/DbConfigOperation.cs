using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace MyModels.Common
{
    public enum CfgPara
    {
        Wave_Up_Path = 1,
    }
    public static class DbConfigHelper
    {
        public static string GetConfigPara(CfgPara emCfgPar)
        {
            string ErrMsg = "";
            try
            {
                string ThisMcName = Environment.MachineName.ToString().ToUpper().Trim();

                string SQL = "SELECT * FROM POLM_Config";
                DataTable DTcfg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                if(emCfgPar == CfgPara.Wave_Up_Path)
                {
                    ////var res = dtCfg.AsEnumerable().Where(p => p.Field<string>("Family") == "Wave" && p.Field<string>("Type") == "Upload" && p.Field<string>("ParKey") == "Path")
                    ////    .Select(o => o.Field<string>("ParValue")).FirstOrDefault();
                    var res = from b in DTcfg.AsEnumerable()
                              where b.Field<string>("Family") == "Wave" && b.Field<string>("Type") == "Upload" &&
                              b.Field<string>("ParKey") == "Path" && b.Field<string>("Machine").ToUpper() == ThisMcName
                              select b.Field<string>("ParValue");
                    if (res != null && res.Count() > 0)
                    {
                        string sRetVal = res.ToList()[0].Trim();
                        return sRetVal;
                    }

                    res = from b in DTcfg.AsEnumerable()
                          where b.Field<string>("Family") == "Wave" && b.Field<string>("Type") == "Upload" &&
                          b.Field<string>("ParKey") == "Path" && b.Field<string>("Machine").ToUpper() == "ALL"
                          select b.Field<string>("ParValue");
                    string sRetVal1 = res.ToList()[0].Trim();
                    if (sRetVal1.Length > 0)
                    {
                        return sRetVal1;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        public static List<string> GetProjectList()
        {
            string ErrMsg = "";
            List<string> List_Project = new List<string>();
            try
            {
                string SQL = "SELECT * FROM Common_Project order by Project";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);

                if(dt != null && dt.Rows.Count > 0)
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

    }
}