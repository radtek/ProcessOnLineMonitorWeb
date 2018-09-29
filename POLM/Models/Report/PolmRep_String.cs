using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POLM.Models.Report
{
    public static class RepStr
    {
        public readonly static string Program = "Program";
        public readonly static string Machine = "Machine";
        public readonly static string LogTime = "LogTime";

        public readonly static string Flux_BdWid = "Flux_BdWid";
        public readonly static string Flux_ConvSpd = "Flux_ConvSpd";
        public readonly static string Flux_NozSpd = "Flux_NozSpd";
        public readonly static string Flux_Volumn = "Flux_Volumn";
        public readonly static string Flux_NozSpray = "Flux_NozSpray";
        public readonly static string Flux_Power = "Flux_Power";
        public readonly static string Flux_Pres = "Flux_Pres";
        public readonly static string Flux_BiModel = "Flux_BiModel";

        public readonly static string Heat_Low1 = "Heat_Low1";
        public readonly static string Heat_Low2 = "Heat_Low2";
        public readonly static string Heat_Low3 = "Heat_Low3";
        public readonly static string Heat_Upp1 = "Heat_Upp1";
        public readonly static string Heat_Upp2 = "Heat_Upp2";
        public readonly static string Heat_Upp3 = "Heat_Upp3";

        public readonly static string SP_Temp = "SP_Temp";
        public readonly static string SP_ConWave = "SP_ConWave";
        public readonly static string SP_LdClear = "SP_LdClear";

        public readonly static string Conv_Speed = "Conv_Speed";
        public readonly static string Conv_Width = "Conv_Width";

        public readonly static string Other_Ni = "Other_Ni";

    }

    public enum RepPara
    {
        Flux_BdWid = 1,
        Flux_ConvSpd = 2,
    }

    public class ST_RepRes
    {
        //public string Line { get; set; }
        //public string Machine { get; set; }
        //public string Project { get; set; }
        //public string Program { get; set; }
        public string Parameter { get; set; }

        public string Val_Cen { get; set; }
        public string Val_Min { get; set; }
        public string Val_Max { get; set; }

        public string Val_Act { get; set; }
        public string Time { get; set; }
        public string Comment { get; set; }
        /// <summary>
        /// string=>某个参数的状态 pass | fail
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 整个的行是 pass | fail
        /// </summary>
        public string All_PF { get; set; }

        public string TEMPDAYS { get; set; }

        public string DocRev { get; set; }
        public ST_RepRes()
        {
            //Line = "";
            //Machine = "";
            //Project = "";
            //Program = "";
            Parameter = "";
            Val_Act = "";
            Time = "";
            Comment = "";
            Status = "";
            All_PF = "";
            TEMPDAYS = "";
            DocRev = "";
        }

    }

}