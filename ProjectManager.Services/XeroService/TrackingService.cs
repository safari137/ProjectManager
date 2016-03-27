using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManager.Models.Xero;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Status;

namespace ProjectManager.Services.XeroService
{
    public class TrackingService
    {
        private readonly List<TrackingItem> _trackingItems = new List<TrackingItem>();
        private TrackingCategory _trackingParent = null;

        public TrackingService()
        {
            this.Initialize();
        }

        public List<TrackingItem> GetTrackingItems()
        {
            return _trackingItems;
        }

        public void Insert(TrackingItem trackingItem)
        {

            _trackingParent.Options.Add(new Option
            {
                Name = trackingItem.Name,
                Status = TrackingOptionStatus.Active
            });

            XeroApiService.AccountingApi.TrackingCategories[_trackingParent.Id].Add(new Option
            {
                Name = trackingItem.Name,
                Status = TrackingOptionStatus.Active,
                Id = new Guid()
            });

            XeroTrackingCategoryConnection.Initialize();
        }

        private void Initialize()
        {
            this._trackingParent =
                XeroApiService.AccountingApi.TrackingCategories
                    .Where("Name == \"Customers\"")
                    .Find()
                    .FirstOrDefault();

            if (_trackingParent == null)
                return;

            foreach (var item in _trackingParent.Options)
            {
                this._trackingItems.Add(new TrackingItem
                {
                    Name = item.Name,
                    Id = item.Id
                });
            }
        }
    }
}
