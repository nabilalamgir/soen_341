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
    public class LinkedAccountsController : Controller
    {
        private SOEN341Entities db = new SOEN341Entities();

        // GET: LinkedAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int loggedUser = (int)Session["id"];
            id = db.LinkedAccounts.Where(x => ((x.Account1ID == id && x.Account2ID == loggedUser) ||
            (x.Account1ID == loggedUser && x.Account2ID == id))).Select(x => x.ID).First();
            LinkedAccount linkedAccount = db.LinkedAccounts.Find(id);
            if (linkedAccount == null)
            {
                return HttpNotFound();
            }
            return View(linkedAccount);
        }

        // POST: LinkedAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int loggedUser = (int)Session["id"];
            id = db.LinkedAccounts.Where(x => ((x.Account1ID == id && x.Account2ID == loggedUser) ||
            (x.Account1ID == loggedUser && x.Account2ID == id))).Select(x => x.ID).First();
            LinkedAccount linkedAccount = db.LinkedAccounts.Find(id);
            db.LinkedAccounts.Remove(linkedAccount);
            db.SaveChanges();
            return RedirectToAction("Details2", "Users", new { id = (int) Session["id"] });
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
