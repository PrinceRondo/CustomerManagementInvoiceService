using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmailNotificationService
{
    public class SendMail
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
        public static void writeErrorLog(Exception exx)
        {
            StreamWriter sm = null;
            //get list of people awaiting emails
            try
            {
                sm = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sm.WriteLine(DateTime.Now.ToString() + " : " + exx.Source.ToString().Trim() + " : " + exx.Message.ToString());
                sm.Flush();
                sm.Close();
            }
            catch (Exception ex)
            {

            }
        }
        public static void writeErrorLog(SmtpException exx)
        {
            StreamWriter sm = null;
            //get list of people awaiting emails
            try
            {
                sm = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sm.WriteLine(DateTime.Now.ToString() + " : " + exx.Source.ToString().Trim() + " : " + exx.Message.ToString() + " SMTP Code: " + exx.StatusCode);
                sm.Flush();
                sm.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public static void writeErrorLog(string exx)
        {
            StreamWriter sm = null;
            try
            {
                sm = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sm.WriteLine(DateTime.Now.ToString() + " : " + exx );
                sm.Flush();
                sm.Close();
            }
            catch (Exception ex)
            {

            }
        }
        public static void doSend(string username, string emailfrom, string host, string ppasword, bool sslval, int portno)
        {
            EmailCalls emailc = new EmailCalls();
            try
            {
                System.Collections.Generic.List<ClassModel> mylistfromdb = new List<ClassModel>();
                string cs = ConfigurationManager.ConnectionStrings["EmailNotificationService.Properties.Settings.CMdbConnectionString"].ConnectionString;
                //SqlConnection con = new SqlConnection(cs);
                using (SqlConnection con = new SqlConnection(cs))
                {
                    // con.ConnectionString = cs; 

                    SqlCommand command = new SqlCommand("Select TOP 100 Email,Url,Id from InvoiceUrl where IsSent = 0", con);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ClassModel cm = new ClassModel()
                            {
                                MailRecipient = reader.GetString(0),
                                MailSubject = "Monthly Invoice",
                                Body = reader.GetString(1),
                                ID = reader.GetInt64(2)
                            };
                            mylistfromdb.Add(cm);
                        }
                    }
                    con.Close();
                    reader.Close();
                    command.Clone();
                }
                if (mylistfromdb.Count > 0)
                {
                    using (SqlConnection con1 = new SqlConnection(cs))
                    {
                        SqlCommand command1 = new SqlCommand();
                        SqlCommand command2 = new SqlCommand();
                        foreach (ClassModel cm in mylistfromdb)
                        {
                            try
                            {
                                //Log the request going to the server
                                writeErrorLog("Sent to mail server. ID :"+ cm.ID +". Recipient:"+ cm.MailRecipient);

                                emailc.SendHtmlFormattedEmail(cm.MailSubject, cm.Body, cm.MailRecipient, username, emailfrom, host, ppasword, sslval, portno);
                                //update the sending status before submitting to the mail server
                                command2 = new SqlCommand("UPDATE InvoiceUrl set IsSent = 1, SentAt= getDate(), ExpiredAt = DateAdd(HH, 1, getDate()) Where Id=" + cm.ID, con1);
                                con1.Open();
                                command2.ExecuteNonQuery();
                                ////Log the request coming back from the server
                                writeErrorLog("Successul from the mail server. ID :" + cm.ID + ". Recipient:" + cm.MailRecipient);

                                con1.Close(); command1.Clone();

                                Thread.Sleep(1000);

                            }
                            catch (SmtpException exx)
                            {
                                con1.Close();
                                command1.Clone();
                                writeErrorLog(exx);
                                Thread.Sleep(30000);
                            }
                            catch (Exception exx)
                            {
                                con1.Close();
                                command1.Clone();
                                writeErrorLog(exx);
                                Thread.Sleep(30000);
                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                writeErrorLog(ex);
            }

        }

    }
}
