using Microsoft.AspNetCore.Mvc;
using WorldDominion.Models;

namespace WorldDominion.Components.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var MenuItems = new List<MenuItem>
            {
                new MenuItem {Controller = "Home", Action = "Index", Label = "Home"},
                new MenuItem {Controller = "Departments", Action = "Index", Label = "Departments"},
                new MenuItem {Controller = "Home", Action = "Privacy", Label = "Privacy"},
                new MenuItem {Controller = "Home", Action = "Brief", Label = "Brief"},
                new MenuItem {Controller = "Products", Action = "Index", Label = "Products"},
            };
            return View(MenuItems);
        }
    }
}