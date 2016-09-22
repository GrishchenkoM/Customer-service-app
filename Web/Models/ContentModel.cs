using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Models
{
    public class ContentModel
    {
        public ContentModel()
        {
            AvailableSort = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailableSort { get; set; }
        public string SelectId { get; set; }

        public bool IsManager { get; set; }
        public bool IsAdmin { get; set; }
        public IList<OrderViewModel> UserOrders { get; set; }
        public IEnumerable<OrderViewModel> ServicingOrders { get; set; }
        public List<ApplicationUser> Managers { get; set; }

        public int OrderId { get; set; }
        public string Note { get; set; }
        public string Manager { get; set; }
        public string Response { get; set; }
        public PageInfo PageInfo { get; set; }
        public string UserName { get; set; }
    }

    public class OrderViewModel
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Номер")]
        public string Number { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreationDateTime { get; set; }

        [Display(Name = "Дата отгрузки")]
        public DateTime? ShippingDateTime { get; set; }

        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [Display(Name = "Пользователь")]
        public string User { get; set; }

        [Display(Name = "Менеджер")]
        public string Manager { get; set; }
    }

    public class PageInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
    }
}