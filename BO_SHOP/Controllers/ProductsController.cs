using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BO_SHOP.Models;
using DAL_SHOP.Models;
using DAL_SHOP.UtilityModels;

namespace BO_SHOP.Controllers
{
    public class ProductsController : Controller
    {
        private ContextDB db = new ContextDB();
        static productspaging productpg = new productspaging();
        // GET: Products
        public ActionResult Listproduct(int pageNumber = 1, int? pageSize = 15, string keyword = "", int? trending = null, int? isview = null)
        {
            productpg.view = isview;
            productpg.trending = trending;
            productpg.pageCurrent = pageNumber;
            List<ProductRow> products = new List<ProductRow>();
            using (SqlConnection conn = new SqlConnection(ContextDB_ULT.connectionString))
            {
                conn.Open();
                using (var command = new SqlCommand("GetPagedProducts", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
                    command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                    command.Parameters.Add(new SqlParameter("@Trending", trending));
                    command.Parameters.Add(new SqlParameter("@Keyword", keyword));
                    command.Parameters.Add(new SqlParameter("@IsView", isview));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductRow pro = new ProductRow();
                            pro.Id = reader.GetInt32(0);
                            pro.CategoryName = reader.GetString(1);
                            pro.Shopname = reader.GetString(2);
                            pro.ProductName = reader.GetString(3);
                            if (!String.IsNullOrEmpty(reader["Slug"].ToString()))
                            {
                                pro.Slug = reader["Slug"].ToString();
                            }
                            else
                            {
                                pro.Slug = null;
                            }
                            if (!String.IsNullOrEmpty(reader["Views"].ToString()))
                            {
                                pro.Views = int.Parse(reader["Views"].ToString());
                            }
                            else
                            {
                                pro.Views = 0;
                            }
                            if (!String.IsNullOrEmpty(reader["IsPublished"].ToString()))
                            {
                                pro.IsPublished = int.Parse(reader["IsPublished"].ToString());
                            }
                            else
                            {
                                pro.IsPublished = 0;
                            }

                            if (!String.IsNullOrEmpty(reader["ProductDetail"].ToString()))
                            {
                                pro.ProductDetail = reader["ProductDetail"].ToString();
                            }
                            else
                            {
                                pro.ProductDetail = null;
                            }
                            pro.Price = decimal.Parse(reader["Price"].ToString());
                            pro.Trending = bool.Parse(reader["Trending"].ToString());
                            dynamic CreatedAt = reader["CreatedAt"].ToString();
                            if (!String.IsNullOrEmpty(CreatedAt))
                            {
                                pro.CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString());
                            }
                            else
                            {
                                pro.CreatedAt = null;
                            }
                            dynamic dayUpdate = reader["UpdateAt"].ToString();
                            if (!String.IsNullOrEmpty(dayUpdate))
                            {
                                pro.UpdateAt = DateTime.Parse(reader["UpdateAt"].ToString());
                            }
                            else
                            {
                                pro.UpdateAt = null;
                            }
                            dynamic dayDelete = reader["DeletedAt"].ToString();

                            if (!String.IsNullOrEmpty(dayDelete))
                            {
                                pro.DeletedAt = DateTime.Parse(reader["DeletedAt"].ToString());
                            }
                            else
                            {
                                pro.DeletedAt = null;
                            }
                            products.Add(pro);
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            productpg.total = reader.GetInt32(0);
                        }
                    }
                }
                for(int i = 0; i < products.Count(); i++)
                {
                    List<Media> medias = new List<Media>();
                    using (var command = new SqlCommand("MediasShowById", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@id", pageNumber));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Media media = new Media();
                                media.Id = reader.GetInt32(0);
                                media.MediasName = reader.GetString(1);
                                media.PathLink= reader.GetString(2);
                                media.MediaType = int.Parse(reader["MediaType"].ToString());
                                media.ProductId = reader.GetInt32(4);
                                dynamic CreatedAt = reader["CreatedAt"].ToString();
                                if (!String.IsNullOrEmpty(CreatedAt))
                                {
                                    media.CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString());
                                }
                                else
                                {
                                    media.CreatedAt = null;
                                }
                                dynamic dayUpdate = reader["UpdateAt"].ToString();
                                if (!String.IsNullOrEmpty(dayUpdate))
                                {
                                    media.UpdateAt = DateTime.Parse(reader["UpdateAt"].ToString());
                                }
                                else
                                {
                                    media.UpdateAt = null;
                                }
                                dynamic dayDelete = reader["DeletedAt"].ToString();

                                if (!String.IsNullOrEmpty(dayDelete))
                                {
                                    media.DeletedAt = DateTime.Parse(reader["DeletedAt"].ToString());
                                }
                                else
                                {
                                    media.DeletedAt = null;
                                }
                                medias.Add(media);
                            }
                        }
                    }
                    products[i].medias = medias;
                }
            }
            productpg.ds_p = products;
            return View(productpg);
        }

       
        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categorys, "Id", "CategoryName");
            ViewBag.Shopid = new SelectList(db.Shops, "Id", "Shopname");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(FormCollection f)
        {
            Product p = new Product();
            p.Id = db.Products.Max(u => u.Id) + 1;
            p.Productname = Request["Productname"].ToString();
            p.Slug = Request["Slug"].ToString();
            p.CategoryId = int.Parse(Request["CategoryId"].ToString());
            p.Shopid = int.Parse(Request["Shopid"].ToString());
            p.ProductDetail = Request["ProductDetail"].ToString();
            if (String.IsNullOrEmpty(Request["Price"].ToString()))
            {
                ViewBag.ErrorMessage = "Price không được để trống";
                return View("Create");
            }
            p.Price = decimal.Parse(Request["Price"].ToString());
            var tam = Request["Trending"];
            if (tam!=null)
            {
                p.Trending = true;
            }
            else
            {
                p.Trending= false;
            }
            if (String.IsNullOrEmpty(p.Productname))
            {
                ViewBag.ErrorMessage = "Productname không được để trống";
                return View("Create");
            }
            if(p.Productname.Length<2 || p.Productname.Length > 50)
            {
                ViewBag.ErrorMessage = "Productname chỉ từ 2-50 kí tự";
                return View("Create");
            }
            if (String.IsNullOrEmpty(p.Slug))
            {
                ViewBag.ErrorMessage = "Slug không được để trống";
                return View("Create");
            }
            if (p.Productname.Length < 2 || p.Productname.Length > 150)
            {
                ViewBag.ErrorMessage = "Slug chỉ từ 2-150 kí tự";
                return View("Create");
            }
            p.CreatedAt = DateTime.Now;
            try
            {
                db.Products.Add(p);
                db.SaveChanges();
                return RedirectToAction("Listproduct");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Tên sarn phẩm đã tồn tại";
                return View("Create");
            }
        }

        // GET: Products/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            ViewBag.CategoryId = new SelectList(db.Categorys, "Id", "CategoryName");
            ViewBag.Shopid = new SelectList(db.Shops, "Id", "Shopname");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        public ActionResult Edit()
        {

            int Id = int.Parse(Request["id"]);
            Product p = db.Products.FirstOrDefault(_u => _u.Id == Id);
            p.Productname = Request["Productname"].ToString();
            p.Slug = Request["Slug"].ToString();
            p.CategoryId = int.Parse(Request["CategoryId"].ToString());
            p.Shopid = int.Parse(Request["Shopid"].ToString());
            p.ProductDetail = Request["ProductDetail"].ToString();
            if (String.IsNullOrEmpty(Request["Price"].ToString()))
            {
                ViewBag.ErrorMessage = "Price không được để trống";
                return View("Edit");
            }
            p.Price = decimal.Parse(Request["Price"].ToString());
            var tam = Request["Trending"];
            if (tam != null)
            {
                p.Trending = true;
            }
            else
            {
                p.Trending = false;
            }
            if (String.IsNullOrEmpty(p.Productname))
            {
                ViewBag.ErrorMessage = "Productname không được để trống";
                return View("Edit");
            }
            if (p.Productname.Length < 2 || p.Productname.Length > 50)
            {
                ViewBag.ErrorMessage = "Productname chỉ từ 2-50 kí tự";
                return View("Edit");
            }
            if (String.IsNullOrEmpty(p.Slug))
            {
                ViewBag.ErrorMessage = "Slug không được để trống";
                return View("Edit");
            }
            if (p.Productname.Length < 2 || p.Productname.Length > 150)
            {
                ViewBag.ErrorMessage = "Slug chỉ từ 2-150 kí tự";
                return View("Edit");
            }
            p.UpdateAt = DateTime.Now;
            try
            {
                db.SaveChanges();
                ViewBag.Success = "Sửa thông tin thành công";
                return RedirectToAction("Listproduct", "Products");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Tên sarn phẩm đã tồn tại";
                return View("Edit");
            }
        }


        // GET: Admin/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.FirstOrDefault(u => u.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            productpg.ds_p.Find(u => u.Id == id).DeletedAt = DateTime.Now;
            db.Products.FirstOrDefault(u => u.Id == id).DeletedAt = DateTime.Now;
            db.SaveChanges();

            ViewBag.deletesuccess = 1;
            return View("Listproduct", productpg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
