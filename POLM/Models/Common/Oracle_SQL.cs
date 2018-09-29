using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using OraClient = Oracle.ManagedDataAccess.Client;
using System.Reflection;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;


namespace MyOracleSql
{
    public static class Oracle_Sql
    {
        static string SQLConnectString_ODC = "User ID=extdev;Password=extuser;Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 10.198.9.17)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = orazhs)))";

        public static DataTable DataTable_Oracle(string SQL, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                OracleCommand myCommand = new OracleCommand(SQL, new OracleConnection(SQLConnectString_ODC));
                OracleDataReader sqlReader;

                try
                {
                    if (myCommand.Connection.State == ConnectionState.Closed)
                    {
                        myCommand.Connection.Open();
                    }
                    sqlReader = myCommand.ExecuteReader();// ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(sqlReader);

                    sqlReader.Close();

                    ErrMsg = "Success";
                    return dt;
                }
                catch (Exception ex)
                {
                    ErrMsg = ex.Message;
                    //MessageBox.Show(ErrMsg, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    myCommand.Connection.Close();
                    myCommand.Connection.Dispose();
                    myCommand = null;
                    sqlReader = null;
                    OracleConnection.ClearAllPools();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                //MessageBox.Show(ErrMsg, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return null;
        }

        public static void StoreProcedure_Paras(string ProcedureName, OracleParameter[] param, ref string MsgOut)
        {
            string ErrMsg = "";
            try
            {
                OracleConnection conn = new OracleConnection(SQLConnectString_ODC);
                OracleCommand pCmd = new OracleCommand();
                try
                {
                    #region procedure
                    conn.Open();
                    pCmd.CommandText = ProcedureName;
                    pCmd.BindByName = true;
                    pCmd.Connection = conn;

                    OracleParameter sp = new OracleParameter();
                    #region add parameter
                    pCmd.CommandType = CommandType.StoredProcedure;
                    pCmd.Parameters.AddRange(param);

                    #region add output parameter
                    OracleParameter parOutput = pCmd.Parameters.Add("p_ParamOut", OracleDbType.NVarchar2, 100);   //pCmd.Parameters.Add("returnPar", OracleType.NVarChar, 100);
                    parOutput.Direction = ParameterDirection.Output;
                    #endregion

                    pCmd.UpdatedRowSource = UpdateRowSource.None;
                    #endregion

                    pCmd.ExecuteNonQuery();

                    MsgOut = parOutput.Value.ToString();
                    #endregion
                }
                catch (Exception ex)
                {
                    MsgOut = ex.Message;
                }
                finally
                {
                    pCmd.Dispose();
                    conn.Close();
                    conn.Dispose(); //把连接放释放回连接池  
                    OracleConnection.ClearAllPools();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                //MessageBox.Show(ErrMsg, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static DataTable StoreProcedureDT_Paras(string ProcedureName, OracleParameter[] param, ref string MsgOut)
        {
            string ErrMsg = "";
            try
            {
                OracleConnection conn = new OracleConnection(SQLConnectString_ODC);
                OracleCommand pCmd = new OracleCommand();
                try
                {
                    #region procedure
                    conn.Open();
                    pCmd.CommandText = ProcedureName;
                    pCmd.BindByName = true;
                    pCmd.Connection = conn;

                    OracleParameter sp = new OracleParameter();
                    #region add parameter
                    pCmd.CommandType = CommandType.StoredProcedure;
                    pCmd.Parameters.AddRange(param);

                    #region add output parameter
                    OracleParameter parOutput = pCmd.Parameters.Add("o_cRefCursor", OracleDbType.RefCursor);   //pCmd.Parameters.Add("returnPar", OracleType.NVarChar, 100);
                    parOutput.Direction = ParameterDirection.Output;

                    OracleParameter parOutput1 = pCmd.Parameters.Add("p_ParamOut", OracleDbType.NVarchar2, 100);
                    parOutput1.Direction = ParameterDirection.Output;
                    #endregion

                    pCmd.UpdatedRowSource = UpdateRowSource.None;
                    #endregion

                    ////OracleDataReader reader = pCmd.ExecuteReader();
                    ////while (reader.Read())
                    ////{

                    ////}
                    ////reader.Close();   


                    //pCmd.ExecuteNonQuery();
                    //OracleDataAdapter da = new OracleDataAdapter(pCmd);
                    //da.Fill(dataset);    

                    OracleDataReader reader = pCmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();

                    MsgOut = parOutput1.Value.ToString();

                    return dt;
                    #endregion
                }
                catch (Exception ex)
                {
                    MsgOut = ex.Message;
                }
                finally
                {
                    pCmd.Dispose();
                    conn.Close();
                    conn.Dispose(); //把连接放释放回连接池  
                    OracleConnection.ClearAllPools();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                //MessageBox.Show(ErrMsg, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return null;
        }

    }

}
