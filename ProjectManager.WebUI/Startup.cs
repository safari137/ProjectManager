using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Owin;
using ProjectManager.Services.XeroService;

namespace ProjectManager.WebUI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string key = ConfigurationManager.AppSettings["xeroKey"];
            string secret = ConfigurationManager.AppSettings["xeroSecret"];
            string certPass = ConfigurationManager.AppSettings["certPass"];

            XeroApiService.Init(key, secret, certPass);
        }
    }
}