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
        //GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage (int id)
        {
            //deklaracja PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //pobieramy strone z bazy o przekazanym id
                PageDTO dto = db.Pages.Find(id);
                //sprawdzamy czy taka strona istnieje
                if(dto == null)
                {
                    return Content("Strona nie istnieje");
                }

                model = new PageVM(dto);

            }                ;

            return View(model);
        }

        

        //POST: Admin/Pages/EditPage
        //[HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //pobranie id strony
                int id = model.Id;
                string slug = "home";
                //pobranie strony do edycji
                PageDTO dto = db.Pages.Find(id);



                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //sprawdzenie unikalnosci strony, adresu
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) || db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Strona lub adres strony juz istnieje");
                }
                //modyfikacja dto
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // zapis edytowanej strony do bazy
                
                db.SaveChanges();
            }
            //ustawienie komunikatu
            TempData["SM"] = "Wyedytowałeś stronę";
            //redirect
            return RedirectToAction("EditPage");
        }


        public ActionResult Details (int id)
        {
            PageVM model;

            using (Db db=new Db())
            {
                //pobranie strony o id
                PageDTO dto = db.Pages.Find(id);
                //sprawdzenie czy strona istnieje
                if (dto == null)
                {
                    return Content("Strona o podanym id nie istnieje");
                }
                //inicjalizacja PageVM
                model = new PageVM(dto);

            }
            return View(model);
        }

        public ActionResult Delete ( int id)
        {
            using (Db db = new Db())
            {
                //pobranie strony do usuniecia
                PageDTO dto = db.Pages.Find(id);
                //usuwanie wybranej strony z bazy
                db.Pages.Remove(dto);
                //zapis
                db.SaveChanges();
            }
            //Redirect
            return RedirectToAction("Index");
        }

        public ActionResult ReorderPages(int[] id)
        {
            using (Db db=new Db())
            {
                int count = 1;
                PageDTO dto;

                // sortowanie stron, zapis w bazie
                foreach (var pageID in id)
                {
                    dto = db.Pages.Find(pageID);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }

            return View();
        }

        //GET: Admin/Pages/
        public ActionResult EditSidebar()
        {
            //Deklaracja SidebarVM
            SidebarVM model;

            using (Db db = new Db())
            {
                //Pobieramy SidebarDTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //Inicjalizacja modelu
                model = new SidebarVM(dto);

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db=new Db())
            {
                //Pobieramy Sidebar DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //modyfikacja Sidebar
                dto.Body = model.Body;

                //zapis na bazie
                db.SaveChanges();
            }

            //Ustawiamy komunikat o modyfikacji
            TempData["SM"] = "Zmodyfikowałaś pasek boczny";

            return RedirectToAction("EditSidebar");
        }

    }
}