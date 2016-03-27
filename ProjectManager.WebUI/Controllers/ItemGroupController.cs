using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models.Xero;
using ProjectManager.Services.XeroService.Items;
using ProjectManager.WebUI.Infrastructure;

namespace ProjectManager.WebUI.Controllers
{
    [Authorize(Roles="manager, admin")]
    [SelectedTab("Item Manager")]
    public class ItemGroupController : Controller
    {
        private readonly XeroItemService _itemService = new XeroItemService();
        private readonly IRepository<ItemGroup> _itemGroupRepository = new ItemGroupRepository(new DataContext()); 
        private readonly IRepository<XeroItemCode> _xeroItemCodeRepository = new XeroItemCodeRepository(new DataContext()); 

        public ActionResult Index()
        {
            return View(_itemGroupRepository.GetAll());
        }

        [HttpGet]
        public ActionResult CreateGroupItem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGroupItem(ItemGroup itemGroup)
        {
            itemGroup.ItemGroupName = itemGroup.XeroGroupItemCode;

            _itemGroupRepository.Insert(itemGroup);
            _itemGroupRepository.Commit();

            return RedirectToAction("ItemGroupDetails", new { itemGroupId = itemGroup.ItemGroupId });
        }

        public ActionResult DeleteGroupItem(int id)
        {
            var groupItem = _itemGroupRepository.GetById(id);
            if (groupItem != null)
            {
                _itemGroupRepository.Delete(id);
                _itemGroupRepository.Commit();
                _itemService.DeleteItem(groupItem.XeroGroupItemCode);
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public ActionResult ItemGroupDetails(int itemGroupId)
        {
            var itemGroup = _itemGroupRepository.GetById(itemGroupId);

            ViewBag.ItemGroupItems = _itemService.GetAddedItems(itemGroup.XeroItemCodes);

            return View(itemGroup);
        }

        [HttpPost]
        public ActionResult ItemGroupDetails(XeroItem xeroItem, int itemGroupId)
        {
            var xeroItemCode = new XeroItemCode()
            {
                ItemCode = xeroItem.ItemCode,
                Quantity = xeroItem.Quantity,
                ItemGroupId = itemGroupId
            };

            _xeroItemCodeRepository.Insert(xeroItemCode);
            _xeroItemCodeRepository.Commit();

            return RedirectToAction("ItemGroupDetails", new { itemGroupId = itemGroupId });
        }

        public ActionResult AddXeroItem()
        {
            var baseItems = _itemService.GetBaseItems();
            ViewBag.XeroBaseItems = baseItems;
            ViewBag.XeroBaseItemCount = baseItems.Count;

            return PartialView();
        }

        public ActionResult XeroItemDelete(int id)
        {
            var item = _xeroItemCodeRepository.GetById(id);

            var itemGroupId = item.ItemGroupId;

            _xeroItemCodeRepository.Delete(id);
            _xeroItemCodeRepository.Commit();
            
            return RedirectToAction("ItemGroupDetails", new { itemGroupId = itemGroupId });
        }

        public ActionResult PublishGroupItemPartialView(ItemGroup itemGroup)
        {
            return PartialView("PublishGroupItem", itemGroup);
        }

        [HttpPost]
        public ActionResult PublishGroupItem(int id)
        {
            var itemGroup = _itemGroupRepository.GetById(id);

            _itemService.PublishItemGroup(itemGroup);

            return RedirectToAction("Index");
        }
    }
}