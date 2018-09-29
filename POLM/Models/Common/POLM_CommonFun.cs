using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Data;
using System.IO;
using System.Collections;

namespace MyModels.Common
{
    public static class POLM_CommonFun
    {
        /// <summary>
        /// Backup log file and return destination file, if sFixFld="", then backup based on date
        /// </summary>
        /// <param name="sLogFile">Orginal log file</param>
        /// <param name="bFixFld">if fixed, then alway save to this folder</param>
        /// <returns></returns>
        public static string BackupLogFile(string sLogFile, string sFixFld)
        {
            try
            {
                sFixFld = sFixFld.Trim();
                string sOrgFile = sLogFile;
                string sDestFile = "";
                if (sFixFld != "")
                {
                    if (!Directory.Exists(sFixFld))
                        Directory.CreateDirectory(sFixFld);

                    string sLogFileName = Path.GetFileName(sLogFile);
                    sDestFile = sFixFld + sLogFileName;
                }
                else
                {
                    #region backup log file for not fixted folder
                    string sLogFld = Path.GetDirectoryName(sLogFile);

                    string sYear = DateTime.Now.Year.ToString();
                    string sMonth = DateTime.Now.Month.ToString();
                    string sDay = DateTime.Now.Day.ToString();
                    string sRepFld_logBack = sLogFld + "\\backupLog";
                    if (!Directory.Exists(sRepFld_logBack))
                    {
                        Directory.CreateDirectory(sRepFld_logBack);
                    }
                    string sRepFld_logBack_Year = sRepFld_logBack + "\\" + sYear; ;
                    if (!Directory.Exists(sRepFld_logBack_Year))
                    {
                        Directory.CreateDirectory(sRepFld_logBack_Year);
                    }

                    string sRepFld_logBack_Mon = sRepFld_logBack_Year + "\\" + sMonth; ;
                    if (!Directory.Exists(sRepFld_logBack_Mon))
                    {
                        Directory.CreateDirectory(sRepFld_logBack_Mon);
                    }

                    string sRepFld_logBack_Mon_Day = sRepFld_logBack_Mon + "\\" + sDay;
                    if (!Directory.Exists(sRepFld_logBack_Mon_Day))
                    {
                        Directory.CreateDirectory(sRepFld_logBack_Mon_Day);
                    }

                    string sLogFileName = Path.GetFileName(sLogFile);
                    sDestFile = sRepFld_logBack_Mon_Day + "\\" + sLogFileName;
                    #endregion
                }
                string sResult = BackupLogFile_Move(sLogFile, sDestFile);

                return sDestFile;
            }
            catch (Exception ex)
            {
                string sMsg = ex.Source + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        private static string BackupLogFile_Move(string sOrgFile, string sDesFile)
        {
            if (System.IO.File.Exists(sOrgFile))
            {
                if (System.IO.File.Exists(sDesFile))
                {
                    sDesFile = sDesFile + " copy";
                }
                try
                {
                    System.IO.File.Move(sOrgFile, sDesFile);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "Success";
        }


    }
}