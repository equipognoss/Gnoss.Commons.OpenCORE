using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Util.Configuracion
{
    public enum VirtuosoConnectionType
    {
        ReadOnly,
        WriteOnly,
        ReadAndWrite
    }

    public class VirtuosoConnectionData
    {
        private static readonly char[] PARAMETERS_SEPARATOR = { ';' };
        private static readonly char[] KEY_VALUE_SEPARATOR = { '=' };
        private const string HOST_PARAMETER = "host";
        private const string UID_PARAMETER = "uid";
        private const string PWD_PARAMETER = "pwd";
        private static readonly object UPGRADE_CONNECTION_LOCK = new object();

        public string Name { get; }
        public string ConnectionString { get; private set; }
        public string Ip { get; private set; }
        public int SparqlEndpointPort { get; private set; }
        public string WriteUser { get; private set; }
        public string WriteUserPassword { get; private set; }
        public string ReadUser { get; private set; }
        public string ReadUserPassword { get; private set; }

        public string SparqlEndpoint
        {
            get
            {
                return $"http://{Ip}:{SparqlEndpointPort}/sparql";
            }
        }

        public string AuthSparqlEndpoint
        {
            get
            {
                return $"http://{Ip}:{SparqlEndpointPort}/sparql-auth";
            }
        }
        public VirtuosoConnectionType VirtuosoConnectionType { get; private set; }

        public VirtuosoConnectionData(string pConnectionName, string pConnectionString, int pSparqlEndpointPort, VirtuosoConnectionType pVirtuosoConnectionType = VirtuosoConnectionType.ReadAndWrite)
        {
            ConnectionString = pConnectionString;
            Name = pConnectionName;
            VirtuosoConnectionType = pVirtuosoConnectionType;
            GetAttributesFromConnectionString(pConnectionString, pSparqlEndpointPort, pVirtuosoConnectionType);
        }

        private void GetAttributesFromConnectionString(string pConnectionString, int pSparqlEndpointPort, VirtuosoConnectionType pVirtuosoConnectionType)
        {
            string ip = null;

            Dictionary<string, string> parameters = GetConnectionParameters(pConnectionString);

            string host;

            if (parameters.TryGetValue(HOST_PARAMETER, out host))
            {
                ip = host;
                // Remove port
                if (host.Contains(':'))
                {
                    ip = host.Substring(0, host.IndexOf(':'));
                }
            }

            string user, password;
            parameters.TryGetValue(UID_PARAMETER, out user);
            parameters.TryGetValue(PWD_PARAMETER, out password);

            this.Ip = ip;
            this.SparqlEndpointPort = pSparqlEndpointPort;

            if (pVirtuosoConnectionType.Equals(VirtuosoConnectionType.ReadOnly) || pVirtuosoConnectionType.Equals(VirtuosoConnectionType.ReadAndWrite))
            {
                this.ReadUser = user;
                this.ReadUserPassword = password;
            }
            if (pVirtuosoConnectionType.Equals(VirtuosoConnectionType.WriteOnly) || pVirtuosoConnectionType.Equals(VirtuosoConnectionType.ReadAndWrite))
            {
                this.WriteUser = user;
                this.WriteUserPassword = password;
            }
        }

        private Dictionary<string, string> GetConnectionParameters(string pConnectionString)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            string[] parametersFromConnectionString = pConnectionString.Split(PARAMETERS_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            foreach(string parameter in parametersFromConnectionString)
            {
                string[] parameterKeyValue = parameter.Split(KEY_VALUE_SEPARATOR, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if(parameterKeyValue.Length > 1)
                {
                    parameters.Add(parameterKeyValue[0].ToLower(), parameterKeyValue[1]);
                }
            }

            return parameters;
        }

        public void UpgradeToReadAndWriteConnection()
        {
            lock (UPGRADE_CONNECTION_LOCK)
            {
                if (VirtuosoConnectionType.Equals(VirtuosoConnectionType.ReadOnly))
                {
                    WriteUser = ReadUser;
                    WriteUserPassword = ReadUserPassword;
                    VirtuosoConnectionType = VirtuosoConnectionType.ReadAndWrite;
                }
            }
        }

        public void SetReadUserFromConnection(VirtuosoConnectionData virtuosoReadOnlyConnectionData)
        {
            ReadUser = virtuosoReadOnlyConnectionData.ReadUser;
            ReadUserPassword = virtuosoReadOnlyConnectionData.ReadUserPassword;
            VirtuosoConnectionType = VirtuosoConnectionType.ReadAndWrite;
        }
    }
}
