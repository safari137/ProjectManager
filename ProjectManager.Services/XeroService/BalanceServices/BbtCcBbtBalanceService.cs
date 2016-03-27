using Xero.Api.Core;

namespace ProjectManager.Services.XeroService.BalanceServices
{
    public class BbtCcBbtBalanceService : XeroBalanceBase
    {
        public BbtCcBbtBalanceService(XeroCoreApi api) : base()
        {
            base.Initialize("BB&T Credit Card");
        }
    }
}
