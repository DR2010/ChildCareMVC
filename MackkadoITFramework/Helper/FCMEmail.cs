﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.IO;
﻿using MackkadoITFramework.ErrorHandling;


namespace FCMMySQLBusinessLibrary
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
        /// Send Email
        /// </summary>
        /// <param name="iFrom"></param>
        /// <param name="iRecipient"></param>
        /// <param name="iSubject"></param>
        /// <param name="iBody"></param>
        /// <param name="iAttachmentLocation"></param>
        public static ResponseStatus SendEmailSimple(
                    string iRecipient,
                    string iSubject,
                    string iBody,
                    string iAttachmentLocation = "",
                    string iAttachmentLocation2 = "",
                    string inlineAttachment = "")
        {


            string iFrom = "fcmnoreply@gmail.com";
            string iPassword = "grahamc1";

            ResponseStatus resp = new ResponseStatus();
            resp.Message = "Email has been sent.";

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient( "smtp.gmail.com" );
                mail.From = new MailAddress( iFrom );
                mail.To.Add( iRecipient );
                mail.Subject = iSubject;
                mail.Body = iBody;
                mail.IsBodyHtml = true;


                // Only if attachment is passed
                if (! string.IsNullOrEmpty(iAttachmentLocation))
                {
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
                }

                if (!string.IsNullOrEmpty(iAttachmentLocation2))
                {
                    if (!File.Exists(iAttachmentLocation2))
                    {
                        // MessageBox.Show("File not found. " + iAttachmentLocation);
                        resp.Message = "File not found. " + iAttachmentLocation2;
                        resp.ReturnCode = -0020;
                        resp.ReasonCode = 0001;
                        return resp;
                    }
                    System.Net.Mail.Attachment attachment2;
                    attachment2 = new System.Net.Mail.Attachment(iAttachmentLocation2);
                    mail.Attachments.Add(attachment2);
                }


                // Only if attachment is passed
                if (!string.IsNullOrEmpty(inlineAttachment))
                {
                    System.Net.Mail.Attachment inlineatc;
                    inlineatc = new System.Net.Mail.Attachment(inlineAttachment);
                    string contentID = "inlineattach@host";
                    inlineatc.ContentId = contentID;
                    inlineatc.ContentDisposition.Inline = true;
                    mail.Attachments.Add(inlineatc);

                    mail.Body += "<html><body><img src=\"cid:" + contentID + "\"></body></html>";

                }

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential( iFrom, iPassword );
                SmtpServer.EnableSsl = true;

                SmtpServer.Send( mail );

            }
            catch ( Exception e1 )
            {

                resp.Message = "Exception in FCMEmail.cs >>>  ";
                resp.ReturnCode = -0030;
                resp.ReasonCode = 0001;
                return resp;

            }

            return resp;
        }




    }
}