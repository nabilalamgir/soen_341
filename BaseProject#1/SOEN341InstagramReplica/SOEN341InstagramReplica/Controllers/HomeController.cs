﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOEN341InstagramReplica.Models;

namespace SOEN341InstagramReplica.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /*
         * 
         * Login
         * 
         */
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            SOEN341Entities db = new SOEN341Entities();

            /*
             * This will check if the username is in the database while also retrieving the password asociated with the user.
             * If the user isn't in the database, then the query returns null which is handled by the ??. If a result is null
             * The the var password is assigned whatever is put on the right side of ??
             */
            string password = db.Users.Where(x => x.Username == model.Username.ToString()).Select(x => x.Password).FirstOrDefault() ?? "NoUser";

            if (password != model.Password.ToString() || password == "NoUser"){
                ModelState.AddModelError(string.Empty, "Incorrect user name or password");
                return View(model);
            }
            else{
                int userId = (int)(from x in db.Users where x.Username == model.Username.ToString() select x.ID).SingleOrDefault();
                Session["username"] = model.Username.ToString();
                Session["id"] = userId;
                return RedirectToAction("Index");
             }
        }


        /*
         * SignUp
         * 
         * 
         */
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User model,string  returnUrl)
        {
            SOEN341Entities db = new SOEN341Entities();
            db.Users.Add(model);
            db.SaveChanges();
            ModelState.Clear();
            ViewBag.SuccessMessage = "Registration Successful";
            Session["id"] = model.ID;
            Session["username"] = model.First_Name;
            return RedirectToAction("Index");

        }

        public ActionResult SignOut()
        {
            //Session.Remove("username");
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}