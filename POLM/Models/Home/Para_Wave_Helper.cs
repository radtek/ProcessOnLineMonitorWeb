using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace POLM.Models.Home
{
    public struct POLM_Para
    {
        public float Para_Cen;
        public float Para_Min;
        public float Para_Max;
        public string ErrMsg;
    }
    public static class Para_Wave_Helper
    {
        public static POLM_Para Get_Wave_Parameter(string sVaueAll)
        {
            string ErrMsg = "";
            POLM_Para polm_Para = new Home.POLM_Para();
            try
            {
                #region Get_Wave_Parameter
                string patten = @"^[0-9.]+\s*[±]*\s*[0-9.]+$";
                if (Regex.IsMatch(sVaueAll, patten))
                {
                    if(sVaueAll.IndexOf("±") > 0)
                    {
                        string[] Paras = sVaueAll.Split('±');
                        polm_Para.Para_Cen = Convert.ToSingle(Paras[0].Trim());

                        float fDev = Convert.ToSingle(Paras[1].Trim());
                        polm_Para.Para_Min = polm_Para.Para_Cen - fDev;
                        polm_Para.Para_Max = polm_Para.Para_Cen + fDev;
                    }
                    else
                    {
                        polm_Para.Para_Cen = Convert.ToSingle(sVaueAll);
                        polm_Para.Para_Min = Convert.ToSingle(sVaueAll);
                        polm_Para.Para_Max = Convert.ToSingle(sVaueAll);
                    }
                    ErrMsg = "Success to parse";
                }
                else if (Regex.IsMatch(sVaueAll, @"^N[\/]*[A]+$"))
                {
                    polm_Para.Para_Cen = -1;
                    polm_Para.Para_Min = -1;
                    polm_Para.Para_Max = -1;
                    ErrMsg = "Success to NA";
                }
                else
                {
                    polm_Para.Para_Cen = -1;
                    polm_Para.Para_Min = -1;
                    polm_Para.Para_Max = -1;
                    ErrMsg = "Fail to parse->" + sVaueAll;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message; // + Environment.NewLine + ex.StackTrace;
            }
            finally
            {
                polm_Para.ErrMsg = ErrMsg;
            }
            return polm_Para;
        }

    }
}