﻿using System.Net.Mail;
using System.Net;

namespace eShopSolution.AdminApp.Models
{
    public class DoingMail
    {
        private static string password = "nqng dvxj bbpu vxrc";
        private static string Email = "diamondluxuryservice@gmail.com";
        public static bool SendMail(string name, string subject, string content, string toMail)
        {
            bool rs = false;
            try
            {
                MailMessage message = new MailMessage();
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com"; //host name
                    smtp.Port = 587; //port number
                    smtp.EnableSsl = true; //whether your smt server requires SSL smtp.DeliveryMethod = System.Net .Mail.SmtpDeliveryMethod.Network;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential()
                    {
                        UserName = Email,
                        Password = password
                    };

                }

                MailAddress fromAddress = new MailAddress(Email, name);
                message.From = fromAddress;
                message.To.Add(toMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                smtp.Send(message);
                rs = true;

            }
            catch (Exception e)
            {
                rs = false;
            }
            return rs;
        }
    }
}
