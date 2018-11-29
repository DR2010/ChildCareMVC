﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.IO;

namespace FCMBusinessLibrary
{
    public class FCMEmail
    {
        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="iFrom"></param>
        /// <param name="iRecipient"></param>
        /// <param name="iSubject"></param>
        /// <param name="iBody"></param>
        public static ResponseStatus SendEmail(
                    string iFrom, 
                    string iPassword,
                    string iRecipient, 
                    string iSubject, 
                    string iBody,
                    string iAttachmentLocation)
        {

            ResponseStatus resp = new ResponseStatus();
            resp.Message = "Email has been sent.";

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(iFrom);
                mail.To.Add(iRecipient);
                mail.Subject = iSubject;
                mail.Body = iBody;

                if (!File.Exists(iAttachmentLocation))
                {
                    // MessageBox.Show("File not found. " + iAttachmentLocation);
                    resp.Message = "File not found. " + iAttachmentLocation;
                    resp.ReturnCode = -0020;
                    resp.ReasonCode = 0001;
                    return resp;
                }


                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(iAttachmentLocation);

                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(iFrom, iPassword);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                // MessageBox.Show("Email has been sent");
            
            }
            catch (Exception e1)
            {
                // MessageBox.Show(e1.ToString());

                resp.Message = "Exception in FCMEmail.cs >>>  " + iAttachmentLocation;
                resp.ReturnCode = -0030;
                resp.ReasonCode = 0001;
                return resp;

            }

            return resp;
        }

        /// <summary>
        /// Send email to group
        /// </summary>
        /// <param name="clientList"></param>
        public static ResponseStatus SendEmailToGroup(
                        List<Client.Client> clientList,
                        string iSubject,
                        string iBody,
                        string Attachment)
        {
            ResponseStatus resp = new ResponseStatus();
            
            string from = "fcmnoreply@gmail.com";
            string password = "grahamc1";

            foreach (var client in clientList)
            {
                resp = FCMEmail.SendEmail(
                    iPassword: password,
                    iFrom: from, 
                    iRecipient: client.EmailAddress, 
                    iSubject: iSubject, 
                    iBody: iBody,
                    iAttachmentLocation: Attachment );

                if (resp.ReturnCode < 0001)
                {
                    break;
                }
            }

            return resp;
        }
    }
}