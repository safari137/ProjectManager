using Xero.Api.Core;

namespace ProjectManager.Services.XeroService.BalanceServices
{
    public class LowesBalanceService : XeroBalanceBase
    {
        public LowesBalanceService(XeroCoreApi api) : base()
        {
            base.Initialize("Lowe's Credit Card");
        }
    }
}
