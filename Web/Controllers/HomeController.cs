﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            _pageSize = 5; // количество объектов на страницу (для постраничного вывода)
        }

        //public async Task<ActionResult> Index()
        //{
        //    var list = await GetUsers();

        //    ViewBag.UserList = list;

        //    return View();
        //}

        //public async Task<IList<ApplicationUser>> GetUsers()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        [Authorize(Roles = "admin, manager, user")]
        public ActionResult Index()
        {
            IList<string> roles = new List<string> {"Роль не определена"};
            var userManager = GetUserManager();
            var user = GetCurrentUser(userManager);

            if (user != null)
                roles = userManager.GetRoles(user.Id);
            
            ContentModel model;
            switch (roles[0])
            {
                case "admin": model = new ContentModel { IsAdmin = true }; break;
                case "manager": model = new ContentModel { IsManager = true }; break;
                default: model = new ContentModel(); break;
            }
            
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";



            return View();
        }
        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public JsonResult GetData()
        {
            var list = ServiceCenters.GetInstance().GetServiceCenterList;

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetUserContent(int page = 1)
        {
            ApplicationUser user;
            List<Order> orders;
            List<ApplicationUser> users;

            var model = CreateModel(out orders, out users, out user);

            CreateModelForUser(model, orders, users, user);

            try
            {
                var ordersPerPages = model.UserOrders.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();
                var pageInfo = new PageInfo { PageNumber = page, PageSize = _pageSize, TotalItems = model.UserOrders.Count() };
                model.PageInfo = pageInfo;
                model.UserOrders = ordersPerPages;
            }
            catch { }

            return View("_GetContent", model);
        }
        public ActionResult GetManagerContent(int page = 1)
        {
            ApplicationUser user;
            List<Order> orders;
            List<ApplicationUser> users;

            ContentModel model = CreateModel(out orders, out users, out user);

            CreateModelForManager(model, orders, users, user);

            model.IsManager = true;

            try
            {
                var ordersPerPages = model.ServicingOrders.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();
                var pageInfo = new PageInfo { PageNumber = page, PageSize = _pageSize, TotalItems = model.ServicingOrders.Count() };
                model.PageInfo = pageInfo;
                model.ServicingOrders = ordersPerPages;
            }
            catch { }

            return View("_GetContent", model);
        }
        public ActionResult GetAdminContent(string manager = null, int page = 1)
        {
            ApplicationUser user;
            List<Order> orders;
            List<ApplicationUser> users;

            ContentModel model = CreateModel(out orders, out users, out user);

            if (manager == null && Session["SelectedManager"] != null)
                manager = (string)Session["SelectedManager"];

            CreateModelForAdmin(model, orders, users, user, manager);

            model.IsAdmin = true;

            try
            {
                var ordersPerPages = model.ServicingOrders.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();
                var pageInfo = new PageInfo { PageNumber = page, PageSize = _pageSize, TotalItems = model.ServicingOrders.Count() };
                model.PageInfo = pageInfo;
                model.ServicingOrders = ordersPerPages;
            }
            catch { }

            Session["SelectedManager"] = manager;
            
            return View("_GetContent", model);
        }

        public ActionResult OrderView()
        {
            ApplicationUser user;
            List<Order> orders;
            List<ApplicationUser> users;

            var model = CreateModel(out orders, out users, out user);

            var currentuserIndex = model.Managers.FindIndex(x => x.Id.Equals(user.Id));
            if (currentuserIndex != -1)
                model.Managers.Remove(user);

            return View("_CreateOrderView", model);
        }
        public ActionResult UpdateOrderView(string orderId)
        {
            ApplicationUser currentUser;
            List<Order> allOrders;
            List<ApplicationUser> allUsers;
            OrderViewModel model = null;

            if (orderId == null)
                return null;

            CreateModel(out allOrders, out allUsers, out currentUser);

            var selectedOrder = allOrders.FirstOrDefault(x => x.Id == Convert.ToInt32(orderId));

            var userManager = GetUserManager();
            var user = userManager.Users.FirstOrDefault(x => x.Id.Equals(selectedOrder.UserId));

            if (selectedOrder != null && selectedOrder.ShippingDateTime == null)
            {
                model = new OrderViewModel
                {
                    Id = selectedOrder.Id,
                    Number = selectedOrder.Number,
                    CreationDateTime = selectedOrder.CreationDateTime,
                    Manager = selectedOrder.ManagerId,
                    User = user != null ? user.FirstName + " " + user.LastName 
                    : selectedOrder.UserId,
                    Note = selectedOrder.Note
                };

                return View("_UpdateOrderView", model);
            }

            return null;
        }
        public JsonResult CreateOrder(OrderViewModel model)
        {
            var result = true;
            var userManager = GetUserManager();

            var user = GetCurrentUser(userManager);

            var order = new Order
            {
                Number = GetRandomNumber().ToString(),
                UserId = user.Id,
                CreationDateTime = DateTime.Now,
                ManagerId = model.Manager,
                Note = model.Note
            };

            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            catch
            {
                result = false;
            }

            var responseModel = new ContentModel();
            switch (userManager.GetRoles(user.Id)[0])
            {
                case "manager":
                    responseModel.IsManager = true;
                    break;
                case "admin":
                    responseModel.IsAdmin = true;
                    break;
            }

            responseModel.Response = result
                ? "Запрос успешно добавлен!"
                : "Ошибка при добавлении запроса";

            return Json(responseModel);
        }
        public JsonResult UpdateOrder(int? id)
        {
            ApplicationUser currentUser;
            List<Order> allOrders;
            List<ApplicationUser> allUsers;

            var result = true;
            if (id == null)
                return null;

            CreateModel(out allOrders, out allUsers, out currentUser);

            var updatedOrder = allOrders.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
            if (updatedOrder != null)
            {
                updatedOrder.ShippingDateTime = DateTime.Now;

                try
                {
                    _context.Entry(updatedOrder).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                catch
                {
                    result = false;
                }
            }

            var responseModel = new ContentModel
            {
                IsManager = true,
                Response = result
                    ? "Запрос успешно добавлен!"
                    : "Ошибка при добавлении запроса"
            };
            var resp = Json(responseModel);
            return resp;
        }

        private ContentModel CreateModel(out List<Order> orders, out List<ApplicationUser> users, out ApplicationUser user)
        {
            IList<string> roles;
            var model = new ContentModel();
            
            SetCurrentUserInfo(out user, out roles);
            
            FillListsOfEntities(out orders, out users);
            
            FillListOfManagers(model, users);

            model.UserName = user.FirstName + " " + user.LastName;
            return model;
        }

        private void SetCurrentUserInfo(out ApplicationUser user, out IList<string> roles)
        {
            var userManager = GetUserManager();
            user = GetCurrentUser(userManager);

            roles = user != null ? userManager.GetRoles(user.Id) : null;
        }
        private void FillListsOfEntities(out List<Order> orders, out List<ApplicationUser> users)
        {
            var userManager = GetUserManager();

            orders = _context.Orders.ToList();
            users = userManager.Users.ToList();
        }
        private void CreateModelForUser(ContentModel model, List<Order> orders, List<ApplicationUser> users, ApplicationUser user)
        {
            if (orders.Count > 0 && users.Count > 0)
            {
                FillOrderListForUser(model, orders, users, user);
            }
        }
        private void CreateModelForManager(ContentModel model, List<Order> orders, List<ApplicationUser> users, ApplicationUser user)
        {
            if (orders.Count > 0 && users.Count > 0)
            {
                FillOrderListForManager(model, orders, users, user);
            }
        }
        private void CreateModelForAdmin(ContentModel model, List<Order> orders, List<ApplicationUser> users, ApplicationUser user, string selectManagerId)
        {
            if (orders.Count > 0 && users.Count > 0)
            {
                FillOrderListForAdmin(model, orders, users, user, selectManagerId);

                model.AvailableSort.Add(new SelectListItem
                {
                    Text = "-Please select-",
                    Value = "Select sort"
                });

                foreach (var manager in model.Managers)
                {
                    model.AvailableSort.Add(new SelectListItem
                    {
                        Text = manager.FirstName + " " + manager.LastName,
                        Value = manager.Id,
                        Selected = (manager.Id.Equals(selectManagerId))
                    });
                }

                var selectedManager = model.Managers.FirstOrDefault(x => x.Id.Equals(selectManagerId));
                if(selectedManager != null)
                    model.Manager = selectedManager.FirstName + " " + selectedManager.LastName;
            }
        }
        private void FillListOfManagers(ContentModel model, List<ApplicationUser> users)
        {
            try
            {
                model.Managers = new List<ApplicationUser>();
                
                var userManager = GetUserManager();

                foreach (var user in users)
                    if (userManager.IsInRole(user.Id, "manager"))
                        model.Managers.Add(user);
            }
            catch
            {}
        }
        private void FillOrderListForUser(ContentModel model, IList<Order> orders, IEnumerable<ApplicationUser> users, ApplicationUser user)
        {
            try
            {
                var ordersList = (from o in orders
                    join u in users
                        on o.ManagerId equals u.Id
                    where o.UserId.Equals(user.Id)
                    select new OrderViewModel()
                    {
                        Id = o.Id,
                        Number = o.Number,
                        CreationDateTime = o.CreationDateTime,
                        ShippingDateTime = o.ShippingDateTime,
                        Note = o.Note,
                        Manager = u.FirstName + " " + u.LastName
                    }).OrderByDescending(x => x.CreationDateTime).ToList();

                model.UserOrders = ordersList;
            }
            catch { }
        }
        private void FillOrderListForManager(ContentModel model, List<Order> orders, List<ApplicationUser> users, ApplicationUser user)
        {
            try
            {
                var ordersList = (from o in orders
                    join u in users
                        on o.UserId equals u.Id
                    where o.ManagerId.Equals(user.Id)
                    select new OrderViewModel()
                    {
                        Id = o.Id,
                        Number = o.Number,
                        CreationDateTime = o.CreationDateTime,
                        ShippingDateTime = o.ShippingDateTime,
                        Note = o.Note,
                        User = u.FirstName + " " + u.LastName
                    }).OrderByDescending(x => x.CreationDateTime).ToList();

                model.ServicingOrders = ordersList;
            }
            catch { }
        }
        private void FillOrderListForAdmin(ContentModel model, List<Order> orders, List<ApplicationUser> users, ApplicationUser user, string selectManagerId)
        {
            IEnumerable<OrderViewModel> allOrdersList;
            if (selectManagerId != null && !selectManagerId.Equals("All"))
            {
                allOrdersList = (from o in orders
                    join u in users
                        on o.ManagerId equals u.Id
                    join u1 in users
                        on o.UserId equals u1.Id
                    where u.Id.Equals(selectManagerId)
                    select new OrderViewModel()
                    {
                        Id = o.Id,
                        Number = o.Number,
                        CreationDateTime = o.CreationDateTime,
                        ShippingDateTime = o.ShippingDateTime,
                        Note = o.Note,
                        Manager = u.FirstName + " " + u.LastName,
                        User = u1.FirstName + " " + u1.LastName
                    }).OrderByDescending(x => x.CreationDateTime).ToList();
            }
            else
            {
                allOrdersList = (from o in orders
                    join u in users
                        on o.ManagerId equals u.Id
                    join u1 in users
                        on o.UserId equals u1.Id
                    select new OrderViewModel()
                    {
                        Id = o.Id,
                        Number = o.Number,
                        CreationDateTime = o.CreationDateTime,
                        ShippingDateTime = o.ShippingDateTime,
                        Note = o.Note,
                        Manager = u.FirstName + " " + u.LastName,
                        User = u1.FirstName + " " + u1.LastName
                    }).OrderByDescending(x => x.CreationDateTime).ToList();
            }

            model.ServicingOrders = allOrdersList;
        }
        private int GetRandomNumber()
        {
            var random = new Random();
            return Enumerable.Range(1, 99999).OrderBy(n => random.Next()).ToList()[0];
        }
        private ApplicationUserManager GetUserManager()
        {
            return HttpContext.GetOwinContext()
                .GetUserManager<ApplicationUserManager>();
        }
        private ApplicationUser GetCurrentUser(ApplicationUserManager userManager)
        {
            return userManager.FindByEmail(User.Identity.Name);
        }
        

        private readonly ApplicationDbContext _context;
        private readonly int _pageSize;
    }
}