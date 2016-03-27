using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectManager.Services.XeroService
{
    public static class XeroTrackingCategoryConnection
    {
        private static Dictionary<Guid, string> _trackingCategoryDictionary;
        private static bool _hasBeenInitialized = false;

        public static string GetNameFromId(Guid id)
        {
            if (!_hasBeenInitialized)
                Initialize();

            string result;

            var found =_trackingCategoryDictionary.TryGetValue(id, out result);
            if (found) return result;

            Initialize();
            found = _trackingCategoryDictionary.TryGetValue(id, out result);
            if (found) return result;

            _trackingCategoryDictionary.Add(id, "DoesNotExist");
            result = "DoesnotExist";

            return result;
        }

        public static void Initialize()
        {
            _hasBeenInitialized = true;
            _trackingCategoryDictionary = new Dictionary<Guid, string>();

            var trackingCategory = XeroApiService.AccountingApi
                .TrackingCategories
                .Where("Name == \"Customers\"")
                .Find()
                .SingleOrDefault();

            if (trackingCategory == null)
                return;

            foreach (var customer in trackingCategory.Options)
            {
                _trackingCategoryDictionary.Add(customer.Id, customer.Name);
            }
        }

        public static Dictionary<Guid, string> GetAll()
        {
            if (!_hasBeenInitialized)
                Initialize();

            return _trackingCategoryDictionary;
        }
    }
}
