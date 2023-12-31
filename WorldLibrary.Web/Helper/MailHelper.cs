﻿using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System;
using MimeKit;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace WorldLibrary.Web.Helper
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Response SendEmail(string to, string subject, string body)
        {
            var nameFrom = _configuration["Mail:NameFrom"];
            var from = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            if (to == null)
            {
                return new Response
                {
                    IsSuccess = false
                };
            }

            var emails = to.Split(","); 

            foreach (var mail in emails)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(nameFrom, from));
                message.To.Add(new MailboxAddress(mail, mail));
                message.Subject = subject;

                var bodybuilder = new BodyBuilder
                {
                    HtmlBody = body,
                };
                message.Body = bodybuilder.ToMessageBody();
                try
                {
                    using (var client = new SmtpClient())
                    {
                        client.Connect(smtp, int.Parse(port), false);
                        client.Authenticate(from, password);
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {

                    return new Response
                    {
                        IsSuccess = false,
                        Message = ex.ToString(),



                    };
                }

            }


            return new Response
            {
                IsSuccess = true
            };
        }
    }
    
}
