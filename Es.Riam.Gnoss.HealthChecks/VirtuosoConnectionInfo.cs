using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Riam.Gnoss.HealthChecks
{
    public sealed class VirtuosoConnectionInfo
    {
        public string Ip { get; }
        public string? User { get; }
        public string? Password { get; }

        public VirtuosoConnectionInfo(string ip, string? user, string? password)
        {
            Ip = ip;
            User = user;
            Password = password;
        }
    }
}
