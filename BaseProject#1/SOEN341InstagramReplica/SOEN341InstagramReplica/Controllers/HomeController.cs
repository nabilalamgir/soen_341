using System; //Commenting on the project
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SOEN341InstagramReplica.Models;

namespace SOEN341InstagramReplica.Controllers
{
    public class HomeController : Controller
    {
        private SOEN341Entities db = new SOEN341Entities();

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
                Session["role"] = (from x in db.Users where x.Username == model.Username.ToString() select x.Role).SingleOrDefault(); ToString();
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
        public ActionResult SignUp([Bind(Include = "First_Name,Last_Name,Username,Password,Email")] User model,string  returnUrl)
        {
            if (model.First_Name != null && model.Last_Name != null && model.Password != null && model.Email != null)
            {
                SOEN341Entities db = new SOEN341Entities();
                int test = (db.Users.Where(x => x.Username == model.Username).Count());
                if ((db.Users.Where(x => x.Username == model.Username).Count()) == 0){
                    model.Date_Joined = DateTime.Now;
                    model.Role = "REGULAR";
                    db.Users.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    ViewBag.SuccessMessage = "Registration Successful";
                    Session["id"] = model.ID;
                    Session["username"] = model.First_Name;
                    Session["role"] = model.Role.ToString();
                    return RedirectToAction("Index");                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Username already being used");
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Missing information, fill in all fields");
                return View(model);
            }


        }

        public ActionResult SignOut()
        {
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

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page, string postAndOrUsername)
        {

            ViewBag.CurrentSort = sortOrder;
            List<string> list = new List<string>();
            list.Add("Post Title");
            list.Add("Username");
            list.Add("Both");
            ViewBag.PostAndOrUsername = new SelectList(list);
            ViewBag.SelectedPostAndOrUsername = postAndOrUsername;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";
            ViewBag.LikesSortParm = sortOrder == "Likes" ? "likes_desc" : "Likes";
            ViewBag.DislikesSortParm = sortOrder == "Dislikes" ? "dislikes_desc" : "Dislikes";
            ViewBag.UserNameSortParm = sortOrder == "UserName" ? "userName_desc" : "UserName";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var posts = from x in db.UserPosts select x;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (postAndOrUsername == "Both")
                {
                    posts = posts.Where(x => x.Title.Contains(searchString) || x.User.Username.Contains(searchString));
                }
                else if(postAndOrUsername == "Username")
                {
                    posts = posts.Where(x => x.User.Username.Contains(searchString));
                }
                else
                {
                    posts = posts.Where(x => x.Title.Contains(searchString));
                }
            }
            switch (sortOrder)
            {
                case "date_asc":
                    posts = posts.OrderBy(p => p.Date_Posted);
                    break;
                case "Title":
                    posts = posts.OrderBy(p => p.Title);
                    break;
                case "title_desc":
                    posts = posts.OrderByDescending(p => p.Title);
                    break;
                case "Likes":
                    posts = posts.OrderBy(p => p.Likes);
                    break;
                case "likes_desc":
                    posts = posts.OrderByDescending(p => p.Likes);
                    break;
                case "Dislikes":
                    posts = posts.OrderBy(p => p.Dislikes);
                    break;
                case "dislikes_desc":
                    posts = posts.OrderByDescending(p => p.Dislikes);
                    break;
                case "UserName":
                    posts = posts.OrderBy(p => p.User.Username);
                    break;
                case "userName_desc":
                    posts = posts.OrderByDescending(p => p.User.Username);
                    break;
                default:
                    posts = posts.OrderByDescending(p => p.Date_Posted);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }
<<<<<<< HEAD

        public ActionResult GlobalSearch(string searchString, string postAndOrUsername)
        {
=======
        //public ActionResult GlobalSearch(string searchString, string postAndOrUsername)
        //{
>>>>>>> refs/remotes/origin/master

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        if (postAndOrUsername == "Username")
        //        {
        //            List<User> lst_users = new List<User>();
        //            var user = from x in db.Users select x;
        //            user = user.Where(x => x.Username.Contains(searchString));
        //            lst_users = user.ToList();
        //            return View("~/Views/Users/Index.cshtml",lst_users);
        //        }
        //        else
        //        {
        //            List<UserPost> lst_posts = new List<UserPost>();
        //            var posts = from x in db.UserPosts select x;
        //            posts = posts.Where(x => x.Title.Contains(searchString));
        //            lst_posts = posts.ToList();
        //            return View("~/Views/UserPosts/Index.cshtml",lst_posts);
        //        }
        //    }
        //    else
        //    {
        //        if (postAndOrUsername == "Username")
        //        {
        //            List<User> lst_users = new List<User>();
        //            var user = from x in db.Users select x;
        //            lst_users = user.ToList();
        //            return View("~/Views/Users/Index.cshtml", lst_users);
        //        }
        //        else
        //        {
        //            List<UserPost> lst_posts = new List<UserPost>();
        //            var posts = from x in db.UserPosts select x;
        //            lst_posts = posts.ToList();
        //            return View("~/Views/UserPosts/Index.cshtml", lst_posts);
        //        }
        //    }
            
        //}

        public ActionResult Trending()
        {
            DateTime this_Week = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
            DateTime startWeek = this_Week.AddDays(0);
            DateTime endWeek = this_Week.AddDays(7);

//System.Diagnostics.Debug.WriteLine(result);
            System.Diagnostics.Debug.WriteLine("Start of the Week: "+ startWeek);
            System.Diagnostics.Debug.WriteLine("End of the Week: " + endWeek);
            List<UserPost> lst_posts = new List<UserPost>();
            var posts = (from x in db.UserPosts select x).OrderByDescending(p => p.Likes).Where(p => p.Date_Posted >= startWeek && p.Date_Posted < endWeek).Take(5);
            lst_posts = posts.ToList();
            return View(lst_posts);
        }

    }
}