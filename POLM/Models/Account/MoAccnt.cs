using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Data.SqlClient;
using MyModels.Common;
using Oracle.ManagedDataAccess.Client;

namespace MyModels.Account
{
    public class MoAccnt
    {
        public string GetLoginProject(ViewBagAcnt viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region Get user
                string SQL = "select Project from FAI_ERECORD_USER order by Project,EN";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg); 
                List<string> List_Project = new List<string>();
                var resQ = dt.AsEnumerable().Select(p => p.Field<string>("Project")).Distinct();
                List_Project = resQ.ToList();
                if (List_Project.Count > 0)
                {
                    viewBag.Message = "Success to get project list from ERecord_User";
                    List_Project.Insert(0, "");

                    viewBag.jsonProject = JsonConvert.SerializeObject(List_Project);

                }
                else
                {
                    viewBag.Message = ErrMsg;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        public string GetLoginUsers(ViewBagAcnt viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region Get user
                string SQL = "select * from FAI_ERECORD_USER where Project='" + viewBag.Project + "' order by Project,Name";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);   // m_AISQL.SQLFunction_DataTable_Oracle(SQL, ref SQL);
                List<string> List_User = new List<string>();
                var resQ = (dt.AsEnumerable().Select(p => p.Field<string>("Name")).Distinct()).OrderBy(g=>g);
                List_User = resQ.ToList();
                if (List_User.Count > 0)
                {
                    viewBag.Message = "Success to get user list for project=" + viewBag.Project + " qty=" + List_User.Count;

                    viewBag.jsonUser = JsonConvert.SerializeObject(List_User);
                }
                else
                {
                    viewBag.Message = ErrMsg;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        public void ConfirmUserPw(ViewBagAcnt viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region Get pw based on user
                string User = viewBag.UserPw.Split('|')[0];
                string Pw = viewBag.UserPw.Split('|')[1];

                string SQL = "select * from FAI_ERECORD_USER where Project='" + viewBag.Project + "' and Name='" + User + "' order by Project,EN";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);  
                var resQ = dt.AsEnumerable().Where(p => p.Field<string>("Name") == User).Select(o => o.Field<string>("PW"));
                string passwordSTD = resQ.ToList()[0];
                if (passwordSTD == Pw)
                {
                    string UserEN = "", userRemark = "";
                    var resEN = dt.AsEnumerable().Where(p => p.Field<string>("Name") == User).Select(o => o.Field<string>("EN"));
                    if (resEN != null && resEN.ToList().Count > 0)
                    {
                        UserEN = resEN.ToList()[0].ToString();
                    }
                    var resRemark = dt.AsEnumerable().Where(p => p.Field<string>("Name") == User).Select(o => o.Field<string>("Remark"));
                    if (resRemark != null && resRemark.ToList().Count > 0)
                    {
                        userRemark = resRemark.ToList()[0].ToString();
                    }


                    ErrMsg = "Success to login 成功登陆用户名 @" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " for user=" + User + "|" + UserEN + "|" + userRemark;
                }
                else
                    ErrMsg = "Fail to validate password 验证用户名密码失败";
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        public string GetLoginUsersAll(ViewBagAcnt viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region Get user
                string SQL = "select Project,EN,Name,PW from FAI_ERecord_User where Project='" + viewBag.Project + "' order by Project, Name";
                DataTable dt = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg); 

                if (dt != null && dt.Rows.Count > 0)
                {
                    viewBag.jsonUser = JsonConvert.SerializeObject(dt);
                    viewBag.Message = "Success to get user.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "";
        }

        public void UserOperation_Action(ViewBagAcnt viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                dynamic objJson = JsonConvert.DeserializeObject(viewBag.jsonDt);
                CfgCls m_CfgCls = new CfgCls();
                m_CfgCls.id = objJson.id;
                m_CfgCls.Project = objJson.PROJECT;
                m_CfgCls.EN = objJson.EN;
                m_CfgCls.Name = objJson.NAME;
                m_CfgCls.PW = objJson.PW;
                m_CfgCls.Remark = objJson.REMARK;

                string sModel = "";
                if (viewBag.MntModel == "update")
                    sModel = "login_Update_eRecord"; //nModel = 5;
                else if (viewBag.MntModel == "delete")
                    sModel = "login_Delete_eRecord"; //nModel = 6;

                OracleParameter[] param = new OracleParameter[7];
                param[0] = new OracleParameter("sModel", sModel);
                param[1] = new OracleParameter("p_Para1", ""); // m_CfgCls.id);
                param[2] = new OracleParameter("p_Para2", m_CfgCls.Project);
                param[3] = new OracleParameter("p_Para3", m_CfgCls.EN);
                param[4] = new OracleParameter("p_Para4", m_CfgCls.Name);
                param[5] = new OracleParameter("p_Para5", m_CfgCls.PW);
                param[6] = new OracleParameter("p_Para6", m_CfgCls.Remark);


                MyOracleSql.Oracle_Sql.StoreProcedure_Paras("FAI_PROCEDURE", param, ref ErrMsg);

                if (ErrMsg.IndexOf("Success") == 0 && ErrMsg.IndexOf("=") > 0)
                {
                    viewBag.Para2 = ErrMsg.Split('=')[1].Trim();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        public class CfgCls
        {
            public string id { get; set; }
            public string Project { get; set; }
            public string EN { get; set; }
            public string Name { get; set; }
            public string PW { get; set; }
            public string Remark { get; set; }
        }

    }
}