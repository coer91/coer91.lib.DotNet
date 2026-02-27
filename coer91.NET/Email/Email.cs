using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace coer91.NET
{
    public class Email(IConfiguration _configuration)  
    {
        //From
        protected string _from = "";
        protected string _sender = "COER 91";
        protected IEnumerable<string> _to = [];
        protected IEnumerable<string> _cc = [];
        protected string _subject = "";
        protected string _body = "";
        protected string _user = "";
        protected string _password = "";

        //Settings
        protected string _host = "smtp.gmail.com";
        protected int _port = 587;
        protected bool _enableSsl = true;
        protected bool _useDefaultCredentials = false;
        protected SmtpDeliveryMethod _deliveryMethod = SmtpDeliveryMethod.Network;
        protected bool _isBodyHtml = true;

        public Email SetFrom(string from)
        {
            _from = from;
            return this;
        }

        public Email SetSender(string sender)
        {
            _sender = sender;
            return this;
        }

        public Email To(IEnumerable<string> to)
        {
            _to = to is not null ? to : [];
            return this;
        }

        public Email CC(IEnumerable<string> cc)
        {
            _cc = cc is not null ? cc : [];
            return this;
        }

        public Email SetUser(string user)
        {
            _user = user;
            return this;
        }

        public Email SetPassword(string password)
        {
            _password = password;
            return this;
        }

        public Email SetSubject(string subject)
        {
            _subject = subject;
            return this;
        }

        public Email SetBody(string body)
        {
            _body = body;
            return this;
        }

        public Email SetHost(string host)
        {
            _host = host;
            return this;
        }

        public Email SetPort(int port)
        {
            _port = port;
            return this;
        }

        public Email EnableSSL(bool enableSSL)
        {
            _enableSsl = enableSSL;
            return this;
        }

        public Email UseDefaultCredentials(bool useDefaultCredentials)
        {
            _useDefaultCredentials = useDefaultCredentials;
            return this;
        }

        public Email SetDeliveryMethod(SmtpDeliveryMethod deliveryMethod)
        {
            _deliveryMethod = deliveryMethod;
            return this;
        }

        public Email IsBodyHtml(bool isBodyHtml)
        {
            _isBodyHtml = isBodyHtml;
            return this;
        }


        public async Task<ResponseDTO> Send()
        {
            ResponseDTO response = new();

            try
            {
                _from = string.IsNullOrWhiteSpace(_from) 
                    ? (_configuration.GetSection("Email:From").Get<string>() ?? string.Empty) : _from;

                _user = string.IsNullOrWhiteSpace(_user) 
                    ? (_configuration.GetSection("Email:User").Get<string>() ?? string.Empty) : _user;
                
                _user = string.IsNullOrWhiteSpace(_user) 
                    ? _from : _user;

                _sender = string.IsNullOrWhiteSpace(_sender) 
                    ? (_configuration.GetSection("Email:Sender").Get<string>() ?? string.Empty) : _sender;

                _password = string.IsNullOrWhiteSpace(_password) 
                    ? (_configuration.GetSection("Email:Password").Get<string>() ?? string.Empty) : _password;

                SmtpClient smtp = GetSMTPClient();

                MailMessage mail = new()
                {
                    From = new MailAddress(_from, _sender),
                    Subject = _subject,
                    Body = BuildBodyBase(_body),
                    IsBodyHtml = _isBodyHtml
                };

                foreach (string to in _to) mail.To.Add(to);
                foreach (string cc in _cc) mail.CC.Add(cc);
                await smtp.SendMailAsync(mail);
            }

            catch (Exception ex)
            {
                return response.Exception(ex);
            }

            return response;
        }


        private SmtpClient GetSMTPClient() => new()
        {
            Host = _host,
            Port = _port,
            EnableSsl = _enableSsl,
            UseDefaultCredentials = _useDefaultCredentials,
            DeliveryMethod = _deliveryMethod,
            Credentials = new NetworkCredential(_user, _password)
        };


        protected static string BuildBodyBase(string body) => @$"
            <html>
                <head>
                    <style>
                        h1, h2, h3, h4, h5, h6, p, pre {{ margin: 0px; color: black; }} 
                        .color-sky    {{ color: #0d6efd !important; }}
                        .color-cyan   {{ color: #00ffff !important; }}
                        .color-green  {{ color: #198754 !important; }}
                        .color-yellow {{ color: #ffc107 !important; }}
                        .color-orange {{ color: #fd6031 !important; }}
                        .color-red    {{ color: #dc3545 !important; }}
                        .color-purple {{ color: #a615bc !important; }}
                        .color-black  {{ color: #000000 !important; }}
                        .color-dark   {{ color: #292828 !important; }}
                        .color-gray   {{ color: #6c757d !important; }}
                        .color-light  {{ color: #ffffff !important; }}
                        .color-smoke  {{ color: #f5f5f5 !important; }}
                        .color-ghost  {{ color: #f8f8ff !important; }}
                        .background-color-sky    {{ background-color: #0d6efd !important; }}
                        .background-color-cyan   {{ background-color: #00ffff !important; }}
                        .background-color-green  {{ background-color: #198754 !important; }}
                        .background-color-yellow {{ background-color: #ffc107 !important; }}
                        .background-color-orange {{ background-color: #fd6031 !important; }}
                        .background-color-red    {{ background-color: #dc3545 !important; }}
                        .background-color-purple {{ background-color: #a615bc !important; }}
                        .background-color-black  {{ background-color: #000000 !important; }}
                        .background-color-dark   {{ background-color: #292828 !important; }}
                        .background-color-gray   {{ background-color: #6c757d !important; }}
                        .background-color-light  {{ background-color: #ffffff !important; }}
                        .background-color-smoke  {{ background-color: #f5f5f5 !important; }}
                        .background-color-ghost  {{ background-color: #f8f8ff !important; }} 
                    </style>
                </head>                    
                <body>{body}</body>
            <html> 
        ".CleanUpBlanks(); 
    }
} 