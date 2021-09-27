using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;

namespace EmailNotificationService
{


    public partial class EmailScheduler : ServiceBase
    {
        private Timer timerinstance;
        private System.ComponentModel.IContainer component;
        private System.Diagnostics.EventLog eventLog1;

        private static string emailfrom = "";

        private static string host = "";
        private static string password = "";
        private static bool sslval = true;
        private static int portno = 0;
        private static string username = "";
        public EmailScheduler()
        {
            InitializeComponent();

        }

        private void setValues()
        {


        }

        protected override void OnStart(string[] args)
        {
            if (username == "")
            {
                try
                {
                    string cs = ConfigurationManager.ConnectionStrings["EmailNotificationService.Properties.Settings.CMdbConnectionString"].ConnectionString;
                    
                    username = ConfigurationManager.AppSettings["Username"].ToString();
                    password = ConfigurationManager.AppSettings["Password"].ToString();
                    emailfrom = ConfigurationManager.AppSettings["From"].ToString();
                    host = ConfigurationManager.AppSettings["SmtpServer"].ToString();
                    portno = Convert.ToInt16(ConfigurationManager.AppSettings["Port"]);
                    sslval = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);

                }
                catch (Exception ex)
                {
                    SendMail.writeErrorLog(ex);
                }

            }
            timerinstance = new Timer();
            this.timerinstance.Interval = 2592000;
            this.timerinstance.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timerinstance.Enabled = true;
            SendMail.writeErrorLog("Customer Management service has started");
        }

        protected override void OnStop()
        {
            timerinstance.Enabled = false;
            SendMail.writeErrorLog("Customer Management service has shutdown");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendMail.doSend(username, emailfrom, host, password, sslval, portno);
            }
            catch (Exception ex)
            {
                SendMail.writeErrorLog(ex);
            }
        }
    }
}
