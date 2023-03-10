using DAL_SHOP.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BO_SHOP.Controllers
{
    public class LoginController : Controller
    {
        private ContextDB db = new ContextDB();
        // GET: Register

        [HttpGet]
        public ActionResult Register()
        {
            return View();

        }
        [HttpPost]
        public ActionResult RegisterPost(FormCollection f)
        {
            User _u = new User();
            _u.Id = db.Users.Max(u => u.Id) + 1;
            _u.Username = Request["username"];

            string pattern = "[^a-zA-Z0-9]"; // biểu thức chính quy: không phải ký tự a-z, A-Z, 0-9
            bool containsSpecialChars = Regex.IsMatch(_u.Username, pattern); // kiểm tra chuỗi có chứa ký tự đặc biệt hay không
            if (containsSpecialChars)
            {
                ViewBag.ErrorUserVal = "Chuỗi không được chứa ký tự đặc biệt";
                return View("Register");
            }
            string pass = f["password"];
            if (pass.Length < 8 || pass.Length > 20)
            {
                ViewBag.ErrorPasswordVal = "Password chỉ từ 8 - 20 kí tự";
                return View("Register");

            }

            string passre = f["passwordre"];
            if (pass != passre)
            {
                ViewBag.ErrorMessage = "Mật khẩu không trùng!!!";
                return View("Register");
            }
            _u.Email = Request["email"];
            if (db.Users.FirstOrDefault(u => u.Email == _u.Email) != null)
            {
                ViewBag.ErrorMessage = "Email đã tồn tại, xin mời nhập email khác!!!";
                return View("Register");
            }
            //SHA256 sha = SHA256.Create();
            //byte[] rs = sha.ComputeHash(Encoding.UTF8.GetBytes(pass));
            //_u.Password = BitConverter.ToString(rs).Replace("-", string.Empty);
            _u.Password = pass;
            _u.Role = 0;
            _u.CreatedAt = DateTime.Now;
            try
            {
                db.Users.Add(_u);
                db.SaveChanges();
                User user = db.Users.FirstOrDefault(u => u.Id == _u.Id);
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại";
                return View("Register");
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public ActionResult LoginPost()
        {
            string namelogin = Request["namelogin"];
            string pass = Request["password"];
            //SHA256 sha = SHA256.Create();
            //byte[] rs = sha.ComputeHash(Encoding.UTF8.GetBytes(pass));
            //pass = BitConverter.ToString(rs).Replace("-", string.Empty);

            string pattern = "[^a-zA-Z0-9]"; // biểu thức chính quy: không phải ký tự a-z, A-Z, 0-9
            bool containsSpecialChars = Regex.IsMatch(namelogin, pattern); // kiểm tra chuỗi có chứa ký tự đặc biệt hay không
            // nếu là email
            if (containsSpecialChars)
            {
                var user = db.Users.FirstOrDefault(_u => _u.Email == namelogin);
                if (user != null)
                {
                    if (pass == user.Password)
                    {
                        Session["user"] = user;
                        return RedirectToAction("MyProfile", "Users");
                    }
                    else
                    {
                        ViewBag.Message = "Thông tin đăng nhập không chính xác!!! Mật khẩu sai";
                        return View("Login");
                    }
                }
            }
            // nếu là user name
            else
            {
                var user = db.Users.FirstOrDefault(_u => _u.Username == namelogin);
                if (user != null)
                {
                    if (pass == user.Password)
                    {
                        Session["user"] = user;
                        return RedirectToAction("MyProfile", "Users");
                    }
                    else
                    {
                        ViewBag.Message = "Thông tin đăng nhập không chính xác!!!Mật khẩu sai";
                        return View("Login");
                    }
                }
            }
            ViewBag.Message = "Thông tin đăng nhập không chính xác!!!Không tìm thấy Email hay tên đăng nhập!!!";
            return View("Login");
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("user");
            return RedirectToAction("Login", "Login");

        }
    }
}