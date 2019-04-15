using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Deklaration of list PageVM
            List<PageVM> pagesList;


            using (Db db = new Db())
            {
                //initialization List
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            // return pages to view
            return View(pagesList);
        }
        [HttpGet]
        public ActionResult AddPage()
        {

            return View();
        }

        //POST : Admin/Pages/AdPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //check model state

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                string slug;
                // initialization PageDTO
                PageDTO dto = new PageDTO();
                

                // if we have not page address the add title
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", " -").ToLower();
                }

                //preventing against add the same page
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Ten tytuł lub adres strony już istnieje.");
                    return View(model);
                }
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 1000;
                //Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "Dodałeś nową stronę";
            return RedirectToAction("AddPage");
        }

    }
}