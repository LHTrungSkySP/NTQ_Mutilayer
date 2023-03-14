using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using DAL_SHOP.Models;
using DAL_SHOP.UtilityModels;

namespace BO_SHOP.Controllers
{
    public class AdminController : Controller
    {
        private ContextDB db = new ContextDB();
          static userspaging upg = new userspaging();
        [HttpGet]
        // GET: Admin
        public ActionResult Listuser(int pageNumber = 1, int? pageSize = 10, int? isDeleted =null,string keyword = "", int? role =null)
        {

            upg.role = role;
            upg.isDeleted = isDeleted;
            upg.pageCurrent = pageNumber;
            List<User> users = new List<User>();
            using(SqlConnection conn = new SqlConnection(ContextDB_ULT.connectionString))
            {
                conn.Open();
                using (var command = new SqlCommand("GetPagedUsers",conn))
                {
                    command.CommandType= CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@PageNumber",pageNumber));
                    command.Parameters.Add(new SqlParameter("@PageSize",pageSize));
                    command.Parameters.Add(new SqlParameter("@IsDeleted",isDeleted));
                    command.Parameters.Add(new SqlParameter("@Keyword",keyword));
                    command.Parameters.Add(new SqlParameter("@Role",role));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            user.Id = reader.GetInt32(0);
                            user.Username = reader.GetString(1);
                            user.Password = reader.GetString(2);
                            user.Email = reader.GetString(3);
                            user.Role = int.Parse(reader["Role"].ToString());
                            user.CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString());
                            dynamic dayUpdate = reader["UpdatedAt"].ToString();
                            if (!String.IsNullOrEmpty(dayUpdate))
                            {
                                user.UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());
                            }
                            else
                            {
                                user.UpdatedAt = null;
                            }
                            dynamic dayDelete = reader["DeletedAt"].ToString();

                            if (!String.IsNullOrEmpty(dayDelete))
                            {
                                user.DeletedAt = DateTime.Parse(reader["DeletedAt"].ToString());
                            }
                            else
                            {
                                user.DeletedAt = null;
                            }
                            users.Add(user);
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            upg.total = reader.GetInt32(0);
                        }
                    }
                }
            }
            upg.ds_u = users;
            return View(upg);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult ListShop()
        {

            return View(db.Shops.ToList());
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(FormCollection f)
        {
            User _u = new User();
            _u.Id = db.Users.Max(u => u.Id) + 1;
            _u.Username = Request["username"];
            string role = Request["role"];
            if(role == "ADMIN")
            {
                _u.Role = 1;
            }
            else
            {
                _u.Role = 0;
            }
            string pattern = "[^a-zA-Z0-9]"; // biểu thức chính quy: không phải ký tự a-z, A-Z, 0-9
            bool containsSpecialChars = Regex.IsMatch(_u.Username, pattern); // kiểm tra chuỗi có chứa ký tự đặc biệt hay không
            if (containsSpecialChars)
            {
                ViewBag.ErrorUserVal = "Chuỗi không được chứa ký tự đặc biệt";
                return View("Create");
            }
            string pass = f["password"];
            if (pass.Length < 8 || pass.Length > 20)
            {
                ViewBag.ErrorPasswordVal = "Password chỉ từ 8 - 20 kí tự";
                return View("Create");

            }
            _u.Email = Request["email"];
            if (db.Users.FirstOrDefault(u => u.Email == _u.Email) != null)
            {
                ViewBag.ErrorMessage = "Email đã tồn tại, xin mời nhập email khác!!!";
                return View("Create");
            }
            _u.Password = pass;
            _u.CreatedAt = DateTime.Now;
            try
            {
                db.Users.Add(_u);
                db.SaveChanges();
                return RedirectToAction("Listuser");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại";
                return View("Create");
            }
        }

        // GET: Admin/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit()
        {
            int Id = int.Parse(Request["id"]);
            var u = db.Users.FirstOrDefault(_u => _u.Id == Id);
            string role = Request["role"];
            if (role == "ADMIN")
            {
                u.Role = 1;
            }
            else
            {
                u.Role = 0;
            }
            string name = Request["username"];
            if (name == null)
            {
                ViewBag.ErrorUserNameVal = "Username không được để trống";
                return View("Edit", u);
            }
            if (u.Username != name)
            {
                if (db.Users.FirstOrDefault(x => x.Username == name) == null)
                {
                    u.Username = name;
                }
                else
                {
                    ViewBag.ErrorUserNameVal = "Username dã tồn tại";
                    return View("Edit", u);
                }
            }

            string pattern = "[^a-zA-Z0-9]"; // biểu thức chính quy: không phải ký tự a-z, A-Z, 0-9
            bool containsSpecialChars = Regex.IsMatch(u.Username, pattern); // kiểm tra chuỗi có chứa ký tự đặc biệt hay không
            if (containsSpecialChars)
            {
                ViewBag.ErrorUserNameVal = "Chuỗi không được chứa ký tự đặc biệt";
                return View("Edit");
            }
            u.UpdatedAt = DateTime.Now;

            string password_new = Request["password_new"];
            if (password_new != null)
            {
                string password_new_comfirm = Request["password_new_comfirm"];
                string passwordPattern = @"^(?=.*[A-Z])(?=.*[0-9])(?=.*[^\w\s]).{8,20}$";
                if (!Regex.IsMatch(password_new, passwordPattern))
                {
                    ViewBag.Error = "Password phải từ 8-20 kí tự bao gồm ít nhất 1 kí tự số, 1 kí tự viết hoa và 1 kí tự đặc biệt";
                    return View("Edit");
                }
                //if (string.IsNullOrEmpty(password_new_comfirm))
                //{
                //    ViewBag.Error = "Confirm password không được để trống";
                //    return View("Edit");
                //}
                u.Password = password_new;
            }

            db.SaveChanges();
            ViewBag.Success = "Sửa thông tin thành công";
            return RedirectToAction("Listuser","Admin");
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.FirstOrDefault(u =>u.Id==id);
            if (user == null)
            {
                return HttpNotFound();
            }
            upg.ds_u.Find(u => u.Id == id).DeletedAt = DateTime.Now;
            db.Users.FirstOrDefault(u=>u.Id== id).DeletedAt = DateTime.Now;
            db.SaveChanges();

            ViewBag.deletesuccess = 1;
            return View("Listuser", upg);
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
