using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SOEN341InstagramReplica.Models;

namespace SOEN341InstagramReplica.Controllers
{
    public class CommentsController : Controller
    {
        private SOEN341Entities db = new SOEN341Entities();

        /*
         * THE FOLLOWING ACTIONS ARE GOING TO BE
         * INACCESSIBLE TO REGULAR USERS. IN FACT
         * IT ISN'T PART OF THE PROJECT SINCE
         * MOST OF THESE ACTIONS AREN'T SENSIBLE.
         * THEY ARE ONLY GOOD FOR DEVELOPERS SO
         * WHILE THEY ARE GOING TO BE ACCESSIBLE,
         * IT WILL ONLY BE US AND ONLY IF WE ARE THE ONES
         * LOGGING IN. OTHERS IF THEY TRY TO BRUTE FORCE
         * TO THE LINK, THEY WILL BE REROUTED.
         * 
         */
        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.UserPost).Include(c => c.User);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create/1
        public ActionResult Create(int? id)
        {
            
            if (Session["username"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ID = id;
            Session["currentPost"] = id;
            ViewBag.Post_ID = new SelectList(db.UserPosts, "ID", "Title");
            ViewBag.User_ID = new SelectList(db.Users, "ID", "First_Name");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Comment1,User_ID,Username,Post_ID")] Comment comment)
        {
            comment.User_ID = (int) Session["id"];
            comment.Post_ID = (int)Session["currentPost"];
            comment.Username = Session["username"].ToString();
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details2", "UserPosts", new { id = comment.Post_ID });
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Post_ID = new SelectList(db.UserPosts, "ID", "Title", comment.Post_ID);
            ViewBag.User_ID = new SelectList(db.Users, "ID", "First_Name", comment.User_ID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Comment1,Date_Posted,User_ID,Post_ID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Post_ID = new SelectList(db.UserPosts, "ID", "Title", comment.Post_ID);
            ViewBag.User_ID = new SelectList(db.Users, "ID", "First_Name", comment.User_ID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            int postId = comment.Post_ID;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details2", "UserPosts", new { id = postId});
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
