using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace eCertificateRest.Utils
{
    public class SendEmail
    {


        public static string serverEmail = "";
        public static string fromEmail = "";
        public static string toEmail = "";
        public static string ccEmail = "";
        public static string ccoEmail = "";
        public static ArrayList files = new ArrayList();
        public static string subject = "";
        public static string messageBody = "";
        public static string errorSend = "";
        public static int suscriptorId = 0;

        public static bool isBodyHtml = false;

        public static string send(string serverEmail, string fromEmail, string fromEmailPWD, string fromEmailPto, bool enableSsl)
        {

        string response ="";

        try {     

            SmtpClient client =new SmtpClient(serverEmail);
            MailAddress fromAddr =new MailAddress(fromEmail);
            MailMessage message=new MailMessage();

            message.From = fromAddr;
             
            foreach(string email in toEmail.Split(';')  ){
                MailAddress toAddr=new MailAddress(email);
                message.To.Add(toAddr);
            }

            if(! ccEmail.Trim().Equals("")) {
                 foreach(string email in ccEmail.Split(';')  ){
                        MailAddress toAddr=new MailAddress(email);
                        message.CC.Add(toAddr);
                    }
               
            }

            if(! ccoEmail.Trim().Equals("")) {
                 foreach(string email in ccoEmail.Split(';')  ){
                        MailAddress toAddr=new MailAddress(email);
                        message.Bcc.Add(toAddr);
                    }
               
            }

            Attachment itemFile  = null;

            client.Credentials = new System.Net.NetworkCredential(fromEmail, fromEmailPWD);
            
            //'Default port will be 25
            client.Port = Convert.ToInt32(fromEmailPto);
            client.Timeout = 1000 * 60 * 10;

            client.EnableSsl = enableSsl;

            
            message.Attachments.Clear();
             
            foreach(string  file in files ){
                itemFile = new Attachment(file);
                message.Attachments.Add(itemFile);
            }

            message.Subject = subject;
            message.Body = messageBody;

            message.IsBodyHtml = isBodyHtml;
            client.Send(message);

            response = "";
        }
        catch (Exception ex ){
            throw new Exception("MailServer. " + ex.Message);


        }

        errorSend = response;
        return response;

    }



    }
}