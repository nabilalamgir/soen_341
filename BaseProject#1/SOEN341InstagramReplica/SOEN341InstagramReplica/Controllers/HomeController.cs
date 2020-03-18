using System; //Commenting on the project
using System.Collections.Generic;
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

        public ActionResult Trending(string sortOrder)
        {
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";
            ViewBag.LikesSortParm = sortOrder == "Likes" ? "likes_desc" : "Likes";
            ViewBag.DislikesSortParm = sortOrder == "Dislikes" ? "dislikes_desc" : "Dislikes";
            ViewBag.UserNameSortParm = sortOrder == "UserName" ? "userName_desc" : "UserName";

            var posts = from x in db.UserPosts select x;
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
            return View(posts.ToList());
        }

        public ActionResult Trending2(string sortOrder, string searchString)
        {
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";
            ViewBag.LikesSortParm = sortOrder == "Likes" ? "likes_desc" : "Likes";
            ViewBag.DislikesSortParm = sortOrder == "Dislikes" ? "dislikes_desc" : "Dislikes";
            ViewBag.UserNameSortParm = sortOrder == "UserName" ? "userName_desc" : "UserName";

            var posts = from x in db.UserPosts select x;

            if (!String.IsNullOrEmpty(searchString)){
                posts = posts.Where(x => x.Title.Contains(searchString) || x.User.Username.Contains(searchString));
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
            return View(posts.ToList());
        }

        public ActionResult Trending3(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";
            ViewBag.LikesSortParm = sortOrder == "Likes" ? "likes_desc" : "Likes";
            ViewBag.DislikesSortParm = sortOrder == "Dislikes" ? "dislikes_desc" : "Dislikes";
            ViewBag.UserNameSortParm = sortOrder == "UserName" ? "userName_desc" : "UserName";

            if(searchString != null)
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
                posts = posts.Where(x => x.Title.Contains(searchString) || x.User.Username.Contains(searchString));
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

        public ActionResult Trending4(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            //List<string> list = new List<string>();
            //list.Add("Post Title");
            //list.Add("Username");
            //list.Add("Both");
            //ViewBag.PostAndOrUsername = new SelectList(list);
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
                posts = posts.Where(x => x.Title.Contains(searchString) || x.User.Username.Contains(searchString));
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

    }
}