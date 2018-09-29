using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyModels.Common;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mail;

namespace POLM.Models.eMail
{
    public class myEmail
    {
        public List<string> List_Track = new List<string>();
        public List<string> List_Error = new List<string>();

        public void SendMailToOwner(MoViewBag viewBag, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                #region tracking
                List_Track.Add("--SendMailToOwner V1.1");
                List_Track.Add("----Get email config");
                #endregion

                #region get config
                string SQL = "SELECT * FROM POLM_Config where Family='Category' and Type='eMail' and Data='Wave'";
                DataTable dtCfg = MyOracleSql.Oracle_Sql.DataTable_Oracle(SQL, ref ErrMsg);
                List_Track.Add("----Get email config result = " + ErrMsg + ", rows=" + (dtCfg == null ? "0" : dtCfg.Rows.Count.ToString()));
                #endregion

                string emailBody = GetEmailBody(viewBag, dtCfg, ref ErrMsg);
                if (emailBody != "")
                {
                    #region send email
                    string mailTitle = "Process On-Line Monitor for Wave";
                    string mailReceiver = "jmwang@Celestica.com"; //for test

                    bool bFormal = false;
                    List<string> List_mailReceiver = new List<string>();
                    List<string> List_mailReceiverCC = new List<string>();

                    #region test only
                    //using (Entities context = new Models.Entities())
                    //{
                    //    //foreach(var item in context.M7_CONFIG)
                    //    //{
                    //    //    ErrMsg = item.FAMILY;
                    //    //}

                    //    //foreach (var item in context.POLM_CONFIG)
                    //    //{
                    //    //    ErrMsg = item.FAMILY;
                    //    //}

                    //    var res = context.POLM_CONFIG.Where(p => p.FAMILY == "Category" && p.TYPE == "eMail" && p.PARKEY == "title" && p.DATA == "Wave")
                    //                .Select(o => o.PARVALUE);

                    //    List<string> List = res.ToList();
                    //    ErrMsg = List[0];

                    //    ErrMsg = res.FirstOrDefault();

                    //} 
                    #endregion

                    #region tracking
                    List_Track.Add("----Begin to get config from server");
                    #endregion
                    using (Entities context = new Entities())
                    {
                        //context.Database.Connection.Open();
                        #region get data from server
                        string sEnableSendMail = context.POLM_CONFIG.Where(p => p.FAMILY == "Category" && p.TYPE == "eMail" && p.PARKEY == "enable" && p.DATA == "Wave")
                                                           .Select(o => o.PARVALUE).FirstOrDefault();
                        #region tracking
                        List_Track.Add("------mail enable = " + sEnableSendMail);
                        if(sEnableSendMail.Trim().ToLower() != "true")
                        {
                            ErrMsg = "no need to send mail";
                            List_Track.Add("-----enale is not true, no need to send mail");
                            return;
                        }
                        #endregion

                        mailTitle = context.POLM_CONFIG.Where(p => p.FAMILY == "Category" && p.TYPE == "eMail" && p.PARKEY == "title" && p.DATA == "Wave")
                                                           .Select(o => o.PARVALUE).FirstOrDefault();
                        #region tracking
                        List_Track.Add("------mail title = " + mailTitle);
                        #endregion

                        var sFormal_db = context.POLM_CONFIG.Where(p => p.FAMILY == "Category" && p.TYPE == "eMail" && p.PARKEY == "model" && p.DATA == "Wave")
                                            .Select(o => o.PARVALUE).FirstOrDefault();
                        bFormal = Convert.ToBoolean(sFormal_db);

                        #region tracking
                        List_Track.Add("------mail offical = " + bFormal);
                        #endregion

                        var sReceiverList = context.POLM_CONFIG.Where(p => p.FAMILY == "Category" && p.TYPE == "eMail" && p.PARKEY == "received" && p.DATA == "Wave")
                                                .Select(o => o.PARVALUE).FirstOrDefault();
                        List_mailReceiver = sReceiverList.Split(',').ToList();

                        #region tracking
                        List_Track.Add("------mail receiver = " + sReceiverList);
                        #endregion

                        var sReceiverListCC = context.POLM_CONFIG.Where(p => p.FAMILY == "Category" && p.TYPE == "eMail" && p.PARKEY == "rec_cc" && p.DATA == "Wave")
                                                .Select(o => o.PARVALUE).FirstOrDefault();
                        List_mailReceiverCC = sReceiverListCC.Split(',').ToList();

                        #endregion
                    }
                    #region tracking
                    List_Track.Add("----Success to get config from server");

                    #endregion

                    string sResult = "";
                    if (!bFormal)
                    {
                        #region tracking
                        List_Track.Add("----Begin to sendSMforGMail");
                        #endregion

                        sResult = sendSMforGMail("jmwang@Celestica.com", "pw", "Jianming Wang", mailTitle, emailBody, mailReceiver, ref ErrMsg);
                    }
                    else
                    {
                        #region tracking
                        List_Track.Add("----Begin to sendSMforGMail2");
                        #endregion

                        sResult = sendSMforGMail2("jmwang@Celestica.com", "pw", "Jianming Wang", mailTitle, emailBody, List_mailReceiver, List_mailReceiverCC, ref ErrMsg);
                    }
                    if(ErrMsg.IndexOf("Success") == 0)
                        ErrMsg = "Success to Email result = " + sResult + "    " + ErrMsg;
                     

                    #region tracking
                    List_Track.Add("----" + ErrMsg);
                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region tracking
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
                List_Error.Add(ErrMsg); 
                #endregion
            }
        }

        /// <summary>
        /// 此段代码测试OK @2017/06/12
        /// </summary>
        private string sendSMforGMail(string sender, string pwd, string name, string SMtitle, string SMContent, string receiver, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                //msg.To.Add(receiver);
                msg.From = new System.Net.Mail.MailAddress(sender, name, System.Text.Encoding.UTF8);

                msg.To.Add(new System.Net.Mail.MailAddress(receiver));

                msg.Subject = SMtitle;//邮件标题             
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码 
                msg.Body = SMContent;//邮件内容 
                msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码 
                msg.IsBodyHtml = true;//是否是HTML邮件 

                SmtpClient client = new SmtpClient();
                //client.Credentials = new System.Net.NetworkCredential(sender, pwd);
                //上述写你的邮箱和密码 
                client.Port = 25;//gmail 使用的端口 
                client.Host = "smtp.celestica.com"; //"smtp.gmail.com";
                //client.EnableSsl = true;//经过ssl加密   能与服务器不支持 ssl 连接有关。调试程序时监视下面的变量，是否为 false。this.sc.EnableSsl = this.ck_ssl.Checked
                //打开则会报错=>服务器不支持安全连接
                object userState = msg;

                List_Track.Add("------host = " + client.Host + "  port=" + client.Port);

                try
                {
                    //client.SendAsync(msg, userState);
                    client.Send(msg);
                    //简单一点儿可以client.Send(msg); 
                    // MessageBox.Show("发送成功");
                    //textBox2.Text = textBox2.Text + DateTime.Now.ToString() + " Have sent a message!" + "/r/n";
                    ErrMsg = "Success to send email";
                    return "1";
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    //MessageBox.Show(ex.Message, "发送邮件出错");
                    // textBox2.Text = textBox2.Text + DateTime.Now.ToString() + " " + ex.Message + "/r/n";
                    ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                    return "0";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "0";
        }

        private string sendSMforGMail2(string sender, string pwd, string name, string SMtitle, string SMContent, List<string> List_receiver, List<string> List_receiverCC, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                //msg.To.Add(receiver);
                msg.From = new System.Net.Mail.MailAddress(sender, name, System.Text.Encoding.UTF8);

                foreach (string receiver in List_receiver)
                {
                    if (receiver.Trim() != "")
                        msg.To.Add(new System.Net.Mail.MailAddress(receiver.Trim()));
                }

                foreach (string receiverCC in List_receiverCC)
                {
                    string receiverCC2 = receiverCC.Trim();
                    if (receiverCC2 != "")
                    {
                        msg.CC.Add(new System.Net.Mail.MailAddress(receiverCC2));
                    }
                }

                msg.Subject = SMtitle;//邮件标题             
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码 
                msg.Body = SMContent;//邮件内容 
                msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码 
                msg.IsBodyHtml = true;//是否是HTML邮件 

                SmtpClient client = new SmtpClient();
                //client.Credentials = new System.Net.NetworkCredential(sender, pwd);
                //上述写你的邮箱和密码 
                client.Port = 25;//gmail 使用的端口 
                client.Host = "smtp.celestica.com"; //"smtp.gmail.com";
                //client.EnableSsl = true;//经过ssl加密   能与服务器不支持 ssl 连接有关。调试程序时监视下面的变量，是否为 false。this.sc.EnableSsl = this.ck_ssl.Checked
                //打开则会报错=>服务器不支持安全连接
                object userState = msg;

                try
                {
                    //client.SendAsync(msg, userState);
                    client.Send(msg);
                    //简单一点儿可以client.Send(msg); 
                    // MessageBox.Show("发送成功");
                    //textBox2.Text = textBox2.Text + DateTime.Now.ToString() + " Have sent a message!" + "/r/n";
                    ErrMsg = "Success to send email";
                    return "1";
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    //MessageBox.Show(ex.Message, "发送邮件出错");
                    // textBox2.Text = textBox2.Text + DateTime.Now.ToString() + " " + ex.Message + "/r/n";
                    ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                    return "0";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return "0";
        }


        private string GetEmailBody(MoViewBag viewBag, DataTable dtCfg, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string firstWord = dtCfg.AsEnumerable().Where(p => p.Field<string>("ParKey") == "first").Select(o => o.Field<string>("ParValue")).FirstOrDefault();
                #region tracking
                List_Track.Add("-------- first word = " + firstWord);
                #endregion

                #region format
                string sBody = "";
                sBody = sBody + "<html>";
                sBody = sBody + "<head>";
                sBody = sBody + "<style type='text/css'>";
                sBody = sBody + "    table {";
                sBody = sBody + "        border-collapse: collapse;";
                sBody = sBody + "    }";
                sBody = sBody + "    table, td, th, tr {";
                sBody = sBody + "       border: 1px solid black; padding:5px;";
                sBody = sBody + "   }";
                sBody = sBody + "</style>";
                sBody = sBody + "</head>";
                sBody = sBody + "<p>Hi all,</p>";
                sBody = sBody + "<p>" + firstWord + "</p>";
                sBody = sBody + "<table id='tableData' >";
                sBody = sBody + "<caption style='font-size:16px'>Process On-Line Monitor -- Wave</caption>";
                sBody = sBody + "<thead style='color: black; background: white;'>";
                sBody = sBody + "    <tr>";
                sBody = sBody + "        <th style='text-align:center'>Line</th>";
                sBody = sBody + "        <th style='text-align:center'>Machine</th>";
                sBody = sBody + "        <th style='text-align:center'>Customer</th>";
                sBody = sBody + "        <th style='text-align:center'>Program</th>";
                sBody = sBody + "        <th style='text-align:center'>Parameter</th>";
                sBody = sBody + "        <th style='text-align:center'>Value</th>";
                sBody = sBody + "        <th style='text-align:center'>Min</th>";
                sBody = sBody + "        <th style='text-align:center'>Max</th>";
                sBody = sBody + "        <th style='text-align:center'>Actual</th>";
                sBody = sBody + "        <th style='text-align:center'>DateTime</th>";
                sBody = sBody + "        <th style='text-align:center'>Comments</th>";
                sBody = sBody + "        <th style='text-align:center'>Status</th>";
                sBody = sBody + "    </tr>";
                sBody = sBody + "</thead>";
                sBody = sBody + "<tbody style='background: white; text-align: center'>";
                #endregion

                dynamic objJson = JsonConvert.DeserializeObject(viewBag.Para1);
                var result = ((JArray)objJson).ToObject<List<model_PolmRep>>();
                if (result != null)
                {
                    List_Track.Add("------Success to get fail records = " + result.Count);

                    foreach (var item in result)
                    {
                        sBody = sBody + "    <tr>";
                        sBody = sBody + "        <td>" + item.Line + "</td>";
                        sBody = sBody + "        <td>" + item.Machine + "</td>";
                        sBody = sBody + "        <td>" + item.Project + "</td>";
                        sBody = sBody + "        <td>" + item.Program + "</td>";
                        sBody = sBody + "        <td>" + item.Parameter + "</td>";
                        sBody = sBody + "        <td>" + item.Val_Cen + "</td>";
                        sBody = sBody + "        <td>" + item.Val_Min + "</td>";
                        sBody = sBody + "        <td>" + item.Val_Max + "</td>";
                        sBody = sBody + "        <td>" + item.Val_Act + "</td>";
                        sBody = sBody + "        <td>" + item.Time + "</td>";
                        sBody = sBody + "        <td>" + item.Comment + "</td>";
                        sBody = sBody + "        <td>" + item.Status + "</td>";
                        sBody = sBody + "    </tr>";
                    }
                }
                else
                    List_Track.Add("------Fail to get fail records");

                sBody = sBody + "</tbody>";
                sBody = sBody + "</table>";

                sBody = sBody + "<p>Regards,</p>";
                sBody = sBody + "</html>";

                #region tracking
                List_Track.Add("------------------------Email body");
                List_Track.Add(sBody);
                List_Track.Add("  ");
                #endregion


                return sBody;

            }
            catch (Exception ex)
            {
                #region track
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
                List_Error.Add(ErrMsg);
                #endregion
            }
            return "";
        }

        private string GetConfigData(string Key, DataTable dtCfg)
        {
            string ErrMsg = "";
            try
            {
                //if(Key == "eM_Model")
                //{
                //    var res = dtCfg.AsEnumerable().Where(p=>p.Field<string>("") == "")
                //}
            }
            catch (Exception ex)
            {
                #region track
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
                List_Track.Add(ErrMsg);
                List_Error.Add(ErrMsg);
                #endregion
            }
            return "";
        }

    }

    public class model_PolmRep
    {
        public string Line { get; set; }
        public string Machine { get; set; }
        public string Project { get; set; }
        public string Program { get; set; }
        public string Parameter { get; set; }
        public string Val_Cen { get; set; }
        public string Val_Min { get; set; }
        public string Val_Max { get; set; }
        public string Val_Act { get; set; }
        public string Time { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
    }



}