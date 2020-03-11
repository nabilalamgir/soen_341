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
    public class UserPostsController : Controller
    {
        private SOEN341Entities db = new SOEN341Entities();

        // GET: UserPosts
        public ActionResult Index()
        {
            var userPosts = db.UserPosts.Include(u => u.User);
            return View(userPosts.ToList());
        }

        // GET: UserPosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserPost userPost = db.UserPosts.Find(id);
            if (userPost == null)
            {
                return HttpNotFound();
            }
            return View(userPost);
        }

        // GET: UserPosts/Details/5
        public ActionResult Details2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserPost userPost = db.UserPosts.Find(id);
            PostAndComments postAndComments = new PostAndComments();
            postAndComments.post = userPost;

            postAndComments.comments = (from x in db.Comments where x.Post_ID == userPost.ID select x).ToList();
            User user = db.Users.Find(userPost.User_ID);
            postAndComments.postUserName = user.Username.ToString();
            if(Session["username"] != null)
            {
                int sessionId = (int)Session["id"];
                if ( db.LikeDislikeLists.Where(x => (x.PostId == userPost.ID) && x.UserId == sessionId).Select(x => x.LikeOrDislike).FirstOrDefault() != 0) {
                    postAndComments.likeStatus = db.LikeDislikeLists.Where(x => (x.PostId == userPost.ID) && x.UserId == sessionId).Select(x => x.LikeOrDislike).FirstOrDefault();
                }
                else
                {
                    postAndComments.likeStatus = 0;
                }
            }


            //int likeStatus = db.Where(x => (x.FolloweeID == id) && (x.FollowerID == sessionID)).Select(x => x.ID)).FirstOrDefault();
            if (userPost == null)
            {
                return HttpNotFound();
            }
            return View(postAndComments);
        }

        // GET: UserPosts/Create
        public ActionResult Create()
        {
            ViewBag.User_ID = new SelectList(db.Users, "ID", "First_Name");
            return View();
        }

        // POST: UserPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Description,POST,Likes,Dislikes,Rating,Date_Posted,User_ID")] UserPost userPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid && image != null &&
                (image.ContentType == "image/png" || image.ContentType == "image/jpeg"))
            {

                if (image != null)
                {
                    userPost.POST = new byte[image.ContentLength];
                    image.InputStream.Read(userPost.POST, 0, image.ContentLength);
                }
                userPost.User_ID = (int)Session["id"];
                userPost.Rating = userPost.Likes = userPost.Dislikes = 0;
                userPost.Date_Posted = DateTime.Now;
                db.UserPosts.Add(userPost);
                db.SaveChanges();
                return RedirectToAction("Details2", "Users", new { id = Session["id"] });
            }
            ModelState.AddModelError("POST", "PNG or JPEG");
            ViewBag.User_ID = new SelectList(db.Users, "ID", "First_Name", userPost.User_ID);
            return View(userPost);
        }

        // GET: UserPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserPost userPost = db.UserPosts.Find(id);
            if (userPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_ID = new SelectList(db.Users, "ID", "First_Name", userPost.User_ID);
            return View(userPost);
        }

        // POST: UserPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description")] UserPost userPost, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                UserPost newUserPostDetails = db.UserPosts.Find(userPost.ID);
                newUserPostDetails.Title = userPost.Title;
                newUserPostDetails.Description = userPost.Description;
                db.Entry(newUserPostDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_ID = new SelectList(db.Users, "ID", "First_Name", userPost.User_ID);
            return View(userPost);
        }

        // GET: UserPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserPost userPost = db.UserPosts.Find(id);
            if (userPost == null)
            {
                return HttpNotFound();
            }
            return View(userPost);
        }

        // POST: UserPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserPost userPost = db.UserPosts.Find(id);
            db.UserPosts.Remove(userPost);
            db.SaveChanges();
            return RedirectToAction("Details2", "Users", new { id = userPost.User_ID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        [HttpPost]
        public ActionResult LikeOrDislike(int newRatingStatus, int loggedInUser, int postID)
        {


            //Check if record already exists of user liking or disliking this post
            LikeDislikeList returnStatus = new LikeDislikeList();
            if (db.LikeDislikeLists.Where(x => (x.PostId == postID) && x.UserId == loggedInUser).Select(x => x.LikeOrDislike).FirstOrDefault() != 0)
            {
                int id = (int)db.LikeDislikeLists.Where(x => (x.PostId == postID) && x.UserId == loggedInUser).Select(x => x.ID).FirstOrDefault();

                LikeDislikeList currentRating = db.LikeDislikeLists.Find(id);

                //User already liked or disliked but wants to cancel
                if (currentRating.LikeOrDislike == newRatingStatus)
                {
                    db.LikeDislikeLists.Remove(currentRating);
                    db.SaveChanges();
                    currentRating.LikeOrDislike = 0;
                }
                else //They are changing their like status
                {

                    currentRating.LikeOrDislike = newRatingStatus;
                    db.Entry(currentRating).State = EntityState.Modified;
                    db.SaveChanges();
                    returnStatus = db.LikeDislikeLists.Find(id);
                    return Json(new { returnStatus, Status = "Ok", Error = "" });
                }

                returnStatus = currentRating;
            }
            else
            {
                LikeDislikeList newRating = new LikeDislikeList();
                newRating.LikeOrDislike = newRatingStatus;
                newRating.PostId = postID;
                newRating.UserId = loggedInUser;
                db.LikeDislikeLists.Add(newRating);
                db.SaveChanges();
                returnStatus = newRating;
            }
            return Json(new { returnStatus, Status = "Ok", Error = "" });
        }

    }
}

/*CODE BEING KEPT IN CASE
 WILL DELETE BEFORE END OF PROJECT

                //if (image != null)
                //{
                //    userPost.POST = new byte[image.ContentLength];
                //    image.InputStream.Read(userPost.POST, 0, image.ContentLength);
                //}
                //if (image == null)
                //{
                //    HttpPostedFileBase file = Request.Files["imgsrc"];
                //    if (file != null)
                //    {
                //        System.Diagnostics.Debug.WriteLine("NOT Null");
                //        userPost.POST = new byte[file.ContentLength];
                //        file.InputStream.Read(userPost.POST, 0, file.ContentLength);
                //    }
                //    System.Diagnostics.Debug.WriteLine("NULL");
                //}
 





*/
