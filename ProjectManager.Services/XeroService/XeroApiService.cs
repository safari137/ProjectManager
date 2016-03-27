using System;
using System.IO;
using Xero.Api.Core;
using System.Security.Cryptography.X509Certificates;
using Xero.Api.Example.Applications.Private;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;
using AmericanPayroll = Xero.Api.Payroll.AmericanPayroll;

namespace ProjectManager.Services.XeroService
{
    public static class XeroApiService
    {
        private static string _key;
        private static string _secret;
        private static string _certPass;
        private static X509Certificate2 _cert;

        private static XeroCoreApi _accountingApi = null;
        private static AmericanPayroll _payrollApi = null;

        public static int PayrollCalls { get; set; }
        public static int AccountingCalls { get; set; }
        public static int ApiCalls => PayrollCalls + AccountingCalls;

        public static void Init(string xeroKey, string xeroSecret, string certPass)
        {
            _key = xeroKey;
            _secret = xeroSecret;
            _certPass = certPass;

            _cert = GetCert();
        }

        private static X509Certificate2 GetCert()
        {
            var rootDir = AppDomain.CurrentDomain.BaseDirectory;
            var certPath = rootDir + @"Resources\public_privatekey.pfx";
                                           
            if (!File.Exists(certPath))
                throw new FileNotFoundException(certPath);

            return new X509Certificate2(certPath, _certPass);
        }

        public static XeroCoreApi AccountingApi
        {
            get
            {
                if (_accountingApi == null)
                    GetAccountingApi();
                AccountingCalls++;
                return _accountingApi;
            }
        }

        public static AmericanPayroll PayrollApi
        {
            get
            {
                if (_payrollApi == null)
                    GetPayrollApi();
                PayrollCalls++;
                return _payrollApi;
            }
        }

        private static void GetAccountingApi()
        {
            _accountingApi = new XeroCoreApi(
                "https://api.xero.com",
                new PrivateAuthenticator(_cert),
                new Consumer(_key, _secret),
                null,
                new DefaultMapper(),
                new DefaultMapper()
                );
        }

        private static void GetPayrollApi()
        {
            _payrollApi = new AmericanPayroll("https://api.xero.com",
                new PrivateAuthenticator(_cert),
                new Consumer(_key, _secret),
                null,
                new DefaultMapper(),
                new DefaultMapper(),
                null
                );
        }
    }
}
