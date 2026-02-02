using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace coer91
{
    public class Email : EmailBuilder
    {
        public Email(IConfiguration configuration) : base(configuration) {}

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
    }
} 