using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsShop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            //Deklaracja listy kategori do wyswietlenia
            List<CategoryVM> categoryVMList;
            using (Db db = new Db())
            {
                categoryVMList = db.Categories
                                .ToArray()
                                .OrderBy(x => x.Sorting)
                                .Select(x => new CategoryVM(x))
                                .ToList();
            }

            return View(categoryVMList);
        }
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Deklaracja id
            string id;
            using (Db db = new Db())
            {
                // sprawdzenie czy nazwa kategorii jest unikalna
                if (db.Categories.Any(x=>x.Name==catName))
                    return "tytulzajety";

                //Inicjalizacja DTO

                CategoryDTO dto = new CategoryDTO
                {
                    Name = catName,
                    Slug = catName.Replace(" ", "-").ToLower(),
                    Sorting = 1000
                };

                //zapis do bazy
                db.Categories.Add(dto);
                db.SaveChanges();

                // pobieramy id
                id = dto.Id.ToString();

            }
            return id;
        }

        [HttpPost]
        public ActionResult ReorderCategories(int[] id)
        {
            using (Db db=new Db())
            {
                //inicjalizacja licznika
                int count = 1;

                //deklaracja kategori DTO
                CategoryDTO dto;

                //sortowanie kategorii
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    //zapis na bazie
                    db.SaveChanges();

                    count++;
                }

            }
            return View();
        }

        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db=new Db())
            {
                //pobieramy kategorie o podanym id
                CategoryDTO dto = db.Categories.Find(id);

                //usuwamy kategorie
                db.Categories.Remove(dto);

                //zapis na bazie
                db.SaveChanges();
            }
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                // sprawdzenie czy kategoria jest unikalna
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "tytulzajety";

                //pobieramy kategorie
                CategoryDTO dto = db.Categories.Find(id);
                //edycja kategorii
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                //zapis na bazie
                db.SaveChanges();
            }
            return "Ok";
        }

    }
}