using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProjectManager.Contracts;
using ProjectManager.Services.XeroService.TransactionLoaders;
using ProjectManager.Services.XeroService.TransactionLoaders.StatementImporters;
using ProjectManger.WebUI.Models;

namespace ProjectManger.WebUI.Controllers
{
    [Authorize(Roles = "viewer, manager, admin")]
    public class CashflowController : ApiController
    {
        private readonly IXeroCashflowConnection _xeroConnection;

        public CashflowController(IXeroCashflowConnection xeroConnection)
        {
            this._xeroConnection = xeroConnection;
            this.RegisterTransactionLoaders();
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var cashflow = new Cashflow()
            {
                XeroTransactions = _xeroConnection.GetOrderedTransactions(),
                Balance = _xeroConnection.GetBalance()
            };
                

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, cashflow);

            return response;
        }

        private void RegisterTransactionLoaders()
        {
            _xeroConnection.RegisterTransactionLoader(new RepeatingInvoiceTransactionLoader(20, 4));
            _xeroConnection.RegisterTransactionLoader(new InvoiceAndBillsLoader());
            _xeroConnection.RegisterTransactionLoader(new BbtCcStatementImporter());
            _xeroConnection.RegisterTransactionLoader(new LowesStatementImporter());
            _xeroConnection.RegisterTransactionLoader(new PayrollBillTrasactionLoader(new Guid("3a58979f-5752-4ef2-b190-19518ed6908f"), "Internal Revenue Service"));
            _xeroConnection.RegisterTransactionLoader(new PayrollBillTrasactionLoader(new Guid("2904b4e2-2551-4c92-a6fc-8912117a3a60"), "Virginia Department Of Taxation"));
            _xeroConnection.RegisterTransactionLoader(new PayrollBillTrasactionLoader(new Guid("db835228-bdfb-48b7-98ac-e9a445b90be4"), "Virginia Employment Commission"));
        }
    }
}
