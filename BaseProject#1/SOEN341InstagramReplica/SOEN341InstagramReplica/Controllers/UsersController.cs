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
    public class UsersController : Controller
    {
        private SOEN341Entities db = new SOEN341Entities();

        /*
         * Here are the actions that only admins should have access to:
         * - Index
         * - Create
         * - Edit
         * This is one of the pages that only the dev should be able to access
         * so it can only be accessed by someone who's role is ADMIN.
         * If it a regular user, then they will be redirected to their
         * profile. Otherwise they get sent to the login page.
         */


        // GET: Users
        public ActionResult Index()
        {
            //To check for admin or regular user
            if(Session["username"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if(Session["role"].ToString() == "ADMIN")
            {
                return View(db.Users.ToList());
            }
            else
            {
                return RedirectToAction("Details2", "Users", new { id = Session["id"] });
            }
            
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
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

        // GET: Users/Details/5
        public ActionResult Details2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAndPosts user = new UserAndPosts();
            user.user = db.Users.Find(id);
            user.posts = (from x in db.UserPosts where x.User_ID == id select x).ToList();
            
            /*
             * There are several situations when it comes to being able to follow
             * and see if you are already following. We also have to consider whether
             * a user is viewing their own page in which case the option to follow
             * or unfollow shouldn't appear as a user can't follow themseves
             */

            //No one is logged in or user is viewing themselves
            if(Session["username"] == null || 
               Session["username"].ToString() == user.user.Username)
            {
                user.following = "invalid";
                if (Session["username"].ToString() == user.user.Username)
                {
                    if (db.LinkedAccounts.Where(x => x.Account1ID == id || x.Account2ID == id).Count() > 0)
                    {
                        LinkedAccount link = db.LinkedAccounts.Where(x => x.Account1ID == id || x.Account2ID == id).Single();
                        if (link.Account1ID == id)
                        {
                            user.otherAccount = db.Users.Find(link.Account2ID);
                        }
                        else
                        {
                            user.otherAccount = db.Users.Find(link.Account1ID);
                        }
                    }
                }
            }
            //User is viewing another profile and is logged in
            else
            {
                int sessionID = (int) Session["id"];
                //int followingint = db.FollowLists.Where(x => (x.FolloweeID == id) && (x.FollowerID == sessionID)).Select(x => x.ID);

                if ((db.FollowLists.Where(x => (x.FolloweeID == id) && (x.FollowerID == sessionID)).Select(x => x.ID)).FirstOrDefault() != 0)
                {
                    //There is a following between current user and profile of the user they are on,
                    //meaning they should get the option to unfollow
                    user.following = "Unfollow";
                }
                else
                {
                    //They are not following the user profile they are currently on.
                    //meaning they should get the option to follow
                    user.following = "Follow";
                }
                user.otherAccount = null;
            }

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            //To check for admin or regular user
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (Session["role"].ToString() == "ADMIN")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Details2", "Users", new { id = Session["id"] });
            }
            
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,First_Name,Last_Name,Username,Password,Email,Age,DOB,Date_Joined")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            //To check for admin or regular user
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (Session["role"].ToString() != "ADMIN")
            {
                return RedirectToAction("Details2", "Users", new { id = Session["id"] });
            }

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

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,First_Name,Last_Name,Username,Password,Email,Age,DOB,Date_Joined")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            int userId = id;
            db.Users.Remove(user);
            db.SaveChanges();
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult MySubscriptions(int id)
        {
            List<SubNamesAndLatestPost> list = new List<SubNamesAndLatestPost>();
            List<FollowList> subscriptions = (db.FollowLists.Where(x => x.FollowerID == id)).ToList();
            foreach(FollowList x in subscriptions)
            {
                int followeeId = x.FolloweeID;
                SubNamesAndLatestPost z = new SubNamesAndLatestPost();
                if ((db.UserPosts.Where(y => y.User_ID == followeeId).Count()) != 0)
                {
                    UserPost latestPost = db.UserPosts.Where(y => y.User_ID == followeeId).OrderByDescending(y => y.Date_Posted).First();
                    z.latestPost = latestPost;
                }
                
                z.subID = followeeId;
                User user = db.Users.Find(followeeId);
                z.subUsername = user.Username.ToString();
                list.Add(z);
            }
            return View(list);
        }

        public ActionResult MyFavourites(int id)
        {
            if (Session["id"] != null) { 
                List<UserPost> list = new List<UserPost>();
                List<int> idList = new List<int>();
                idList = db.FavouritesLists.Where(x => x.UserId == id).Select(x => x.PostId).ToList();
                foreach (int temp in idList)
                {
                    list.Add(db.UserPosts.Find(temp));
                }
                return View(list);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult FollowOrUnfollowUser(int newFollowStatus, int loggedInUser, int userProfile)
        {
            //They clicked that they want to follow
            if(newFollowStatus == 1)
            {
                FollowList newEntry = new FollowList();
                newEntry.FollowerID = loggedInUser;
                newEntry.FolloweeID = userProfile;
                db.FollowLists.Add(newEntry);
                db.SaveChanges();
            }
            else //They want to unfollow
            {
                int id = (db.FollowLists.Where(x => (x.FolloweeID == userProfile) && (x.FollowerID == loggedInUser)).Select(x => x.ID)).FirstOrDefault();
                db.FollowLists.Remove(db.FollowLists.Find(id));
                db.SaveChanges();
            }
            UserAndPosts user = new UserAndPosts();
            user.following = "unfollowing";
            return Json(new { user, Status = "Ok", Error = "" });
        }

        public ActionResult LinkAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LinkAccount(User model)
        {

            /*
             * This will check if the username is in the database while also retrieving the password asociated with the user.
             * If the user isn't in the database, then the query returns null which is handled by the ??. If a result is null
             * The the var password is assigned whatever is put on the right side of ??
             */
            string password = db.Users.Where(x => x.Username == model.Username.ToString()).Select(x => x.Password).FirstOrDefault() ?? "NoUser";

            if (password != model.Password.ToString() || password == "NoUser")
            {
                ModelState.AddModelError(string.Empty, "Incorrect user name or password");
                return View(model);
            }
            else
            {
                
                int userId = (int)(from x in db.Users where x.Username == model.Username.ToString() select x.ID).SingleOrDefault();
                User user = db.Users.Find(userId);
                //Check if trying to link to yourself
                int loggedInId = (int)Session["id"];
                if (user.ID == loggedInId)
                {
                    ModelState.AddModelError(string.Empty, "Cannot link to yourself");
                    return View(model);
                }

                //Check if this other account is already linked to be sure
                if (db.LinkedAccounts.Where(x=>x.Account1ID == user.ID || x.Account2ID == user.ID).Count() == 0)
                {
                    LinkedAccount newLink = new LinkedAccount();
                    newLink.Account1ID = (int)Session["id"];
                    newLink.Account2ID = userId;
                    db.LinkedAccounts.Add(newLink);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "That Account is already linked");
                    return View(model);
                }

                return RedirectToAction("Details2", "Users", new { id = (int) Session["id"] });
            }

        }

        public ActionResult SwitchAccount(int otherAccountId)
        {
            Session.RemoveAll();
            User otherUser = db.Users.Find(otherAccountId);
            Session["id"] = otherAccountId;
            Session["username"] = otherUser.Username.ToString();
            Session["role"] = otherUser.Role.ToString();
            return RedirectToAction("Details2", "Users", new { id = otherAccountId });
        }

    }
}
