using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Models.Xero;
using Xero.Api.Core;
using Xero.Api.Core.Model;

namespace ProjectManager.Services.XeroService.Items
{
    public class XeroItemService
    {
        public XeroItemService()
        {
            
        }

        public List<XeroItem> GetBaseItems()
        {
            var items = XeroApiService.AccountingApi
                .Items
                .Find()
                .Where(i => i.Name == "BaseItem")
                .ToList();

            var xeroItems = items.Select(item => new XeroItem
            {
                ItemCode = item.Code, ItemName = item.Name, Description = item.Description
            })
            .OrderBy(i => i.ItemCode)
            .ToList();

            return xeroItems;
        }

        public List<XeroItem> GetAddedItems(ICollection<XeroItemCode> itemCodes)
        {
            var items = new List<XeroItem>();

            if (itemCodes == null)
                return items;
            
            foreach (var id in itemCodes)
            {
                if (id == null)
                    continue;

                var item = GetItem(id.ItemCode);

                if (item == null)
                    continue;

                items.Add(new XeroItem
                {
                    ItemName = item.Name,
                    Description = item.Description,
                    ItemCode = item.Code,
                    Quantity = id.Quantity,
                    XeroItemCodeId = id.XeroItemCodeId,
                    Cost = item.PurchaseDetails.UnitPrice,
                    SalePrice = item.SalesDetails.UnitPrice
                });
            }
            return items;
        }

        public void DeleteItem(string itemCode)
        {
            var xeroItem = GetItem(itemCode);

            if (xeroItem != null)
                XeroApiService.AccountingApi.Items.Delete(xeroItem);
        }

        public void PublishItemGroup(ItemGroup itemGroup)
        {
            var xeroItem = GetItem(itemGroup.XeroGroupItemCode);

            var itemGroupPrice = GetItemGroupPrice(itemGroup.XeroItemCodes);

            var newXeroItem = new Item
            {
                Code = itemGroup.XeroGroupItemCode,
                Name = "ItemGroup " + itemGroup.XeroGroupItemCode,
                PurchaseDetails = new PurchaseDetails { UnitPrice = itemGroupPrice.Cost },
                SalesDetails = new SalesDetails { UnitPrice = itemGroupPrice.SalePrice }
            };

            if (xeroItem == null)
                XeroApiService.AccountingApi.Create(newXeroItem);
            else
                XeroApiService.AccountingApi.Items.Update(newXeroItem);
        }

        private ItemGroupPrice GetItemGroupPrice(ICollection<XeroItemCode> itemCodes)
        {
            var itemGroupPrice = new ItemGroupPrice();

            foreach (var item in itemCodes)
            {
                var xeroItem = GetItem(item.ItemCode);

                itemGroupPrice.Cost += (xeroItem?.PurchaseDetails.UnitPrice * item.Quantity) ?? 0m;
                itemGroupPrice.SalePrice += (xeroItem?.SalesDetails.UnitPrice * item.Quantity) ?? 0m;
            }

            return itemGroupPrice;
        }

        private Item GetItem(string itemCode)
        {
            var whereString = "Code == \"" + itemCode + "\"";

            var item = XeroApiService.AccountingApi
                .Items
                .Where(whereString)
                .Find()
                .SingleOrDefault();

            return item;
        }

        private class ItemGroupPrice
        {
            public decimal Cost { get; set; }
            public decimal SalePrice { get; set; }

            public ItemGroupPrice()
            {
                this.Cost = 0;
                this.SalePrice = 0;
            }
        }
    }


}
