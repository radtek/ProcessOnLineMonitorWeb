using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Messaging;
using System.ServiceProcess;
using System.Collections;
using CsujwangRemoting.JmWsRemoteInterface;

namespace MyModels.Common
{
    public static class DataBaseOperation
    {
        public static void StoreTorqueIntoDB(TorRec pCommon, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                OracleParameter[] para = new OracleParameter[19];
                para[0] = new OracleParameter("sModel", "ER_Torque_Store");
                para[1] = new OracleParameter("p_Para1", pCommon.Project);
                para[2] = new OracleParameter("p_Para2", pCommon.Date + " " + pCommon.Time);
                para[3] = new OracleParameter("p_Para3", pCommon.Shift);
                para[4] = new OracleParameter("p_Para4", pCommon.Model);
                para[5] = new OracleParameter("p_Para5", pCommon.ScrewSN);
                para[6] = new OracleParameter("p_Para6", pCommon.Station);
                para[7] = new OracleParameter("p_Para7", pCommon.TorsionSpec);
                para[8] = new OracleParameter("p_Para8", pCommon.TestVal1);
                para[9] = new OracleParameter("p_Para9", pCommon.TestVal2);
                para[10] = new OracleParameter("p_Para10", pCommon.TestVal3);
                para[11] = new OracleParameter("p_Para11", pCommon.Result);
                para[12] = new OracleParameter("p_Para12", pCommon.CalibBy);
                para[13] = new OracleParameter("p_Para13", pCommon.CheckedBy);
                para[14] = new OracleParameter("p_Para14", pCommon.Meter_SN);
                para[15] = new OracleParameter("p_Para15", pCommon.Meter_Type);
                para[16] = new OracleParameter("p_Para16", pCommon.Meter_DateCalib);
                para[17] = new OracleParameter("p_Para17", pCommon.Meter_DateLose);
                para[18] = new OracleParameter("p_Para18", pCommon.Remark);


                MyOracleSql.Oracle_Sql.StoreProcedure_Paras("FAI_PROCEDURE", para, ref ErrMsg);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }


        public static void StoreStencilIntoDB(StencilRec pCommon, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string checkItems = "", reasonItems = "" ;
                checkItems = pCommon.Chk_A + "|" + pCommon.Chk_B + "|" + pCommon.Chk_C + "|" + pCommon.Chk_D + "|" + pCommon.Chk_E + "|" + pCommon.Chk_F;
                reasonItems = pCommon.Res_A + "|" + pCommon.Res_B + "|" + pCommon.Res_C + "|" + pCommon.Res_D;
                OracleParameter[] para = new OracleParameter[10];
                para[0] = new OracleParameter("sModel", "ER_Stencil_Store");
                para[1] = new OracleParameter("p_Para1", pCommon.Project);
                para[2] = new OracleParameter("p_Para2", pCommon.Date);
                para[3] = new OracleParameter("p_Para3", pCommon.Slot);
                para[4] = new OracleParameter("p_Para4", pCommon.EN);
                para[5] = new OracleParameter("p_Para5", pCommon.Time);
                para[6] = new OracleParameter("p_Para6", checkItems);
                para[7] = new OracleParameter("p_Para7", reasonItems);
                para[8] = new OracleParameter("p_Para8", pCommon.Remark);
                para[9] = new OracleParameter("p_Para9", "");


                MyOracleSql.Oracle_Sql.StoreProcedure_Paras("FAI_PROCEDURE", para, ref ErrMsg);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        /// <summary>
        /// 插入数据到10.198.9.166 StLogServiceData\logSrvTrack, 通过 LogServiceServer
        /// </summary>
        public static string PublishMSMQ_Tracking(string SN, string Project, string Model, string Line, string TestMachine, string Category, string Remark,
                           string TestTime, string TrackingAll)
        {
            string ErrMsg = "";
            ArrayList m_ArrayList = new ArrayList();
            try
            {
                string MsMqServer = @"10.198.9.166\Private$\";
                jmWangMsmq m_jmWangMsmq = new jmWangMsmq(MsMqServer);

                MsMqParas msmqPars = new MsMqParas();
                msmqPars.MsMqTitle = DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + "_Srvtrack";
                msmqPars.SN = SN;
                msmqPars.Para1 = Project;
                msmqPars.Para2 = Model;
                msmqPars.Para3 = Line;
                msmqPars.Para4 = TestMachine; // Environment.MachineName.ToUpper();
                msmqPars.Para5 = Category;
                msmqPars.Para6 = Remark;
                msmqPars.Para7 = TestTime;
                msmqPars.Para8 = TrackingAll;
                m_ArrayList.Add(msmqPars);
                m_jmWangMsmq.Publish_MsmqParas(m_ArrayList, "logSrvTrack", ref ErrMsg);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return ErrMsg;
        }

    }

    public class jmWangMsmq
    {
        #region Initilize MsMq
        private bool bMsmqRun { get; set; }
        private string MsMqServer { get; set; }

        public jmWangMsmq(string msmqServer)
        {
            bMsmqRun = false;
            MsMqServer = msmqServer;

            #region detect msmq if running
            if (Environment.MachineName == "CSUTL1000" || Environment.MachineName == "CSUAOI")
                bMsmqRun = true;
            else
            {
                #region check if running
                ServiceController[] svConArry = ServiceController.GetServices();
                for (int i = 0; i < svConArry.Length; i++)
                {
                    ServiceController msQue = svConArry[i];
                    if (msQue.ServiceName == "MSMQ")
                    {
                        if (msQue != null)
                        {
                            if (msQue.Status == ServiceControllerStatus.Running)
                            {
                                // It is running.
                                bMsmqRun = true;
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
        }

        #endregion


        public void Publish_MsmqParas(ArrayList m_ArryList, string MsMqName, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region Check if normal
                if (!bMsmqRun)
                {
                    ErrMsg = "此电脑 - " + Environment.MachineName + " MsMq没有Run.";
                    return;
                }
                if (m_ArryList.Count == 0)
                {
                    ErrMsg = "没有 MsMq list to transfer.";
                    return;
                }
                #endregion

                #region Send MsMq to server
                MsMqParas msmqParas = (MsMqParas)m_ArryList[0];

                using (MessageQueue queue = new MessageQueue())
                {
                    //queue.Path = "FormatName:DIRECT=OS:" + MsMqServer + "MsMqName"; //FormatName:DIRECT=TCP:<IP Address>\\PRIVATE$\\TestQueue
                    queue.Path = "FormatName:DIRECT=TCP:" + MsMqServer + MsMqName;
                    using (System.Messaging.Message message = new System.Messaging.Message())
                    {
                        message.Formatter = new BinaryMessageFormatter();
                        message.Label = msmqParas.MsMqTitle;
                        message.Body = m_ArryList;
                        message.Recoverable = true;
                        queue.Send(message);
                        ErrMsg = "Success to send MsMq - " + msmqParas.MsMqTitle;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }


    }

}