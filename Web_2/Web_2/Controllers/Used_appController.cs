using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_2.Models;

namespace Web_2.Controllers
{
    public class Used_appController : Controller
    {
        private PC_TrackEntities db = new PC_TrackEntities();

        // GET: Used_app
        public ActionResult Index()
        {
            var used_app = db.Used_app.Include(u => u.Applications).Include(u => u.PC);
            //ViewBag.PCname = db.PC.i
            return View(used_app.ToList());
        }

        public ActionResult SelectedPC(string SelectedPC)
        {
            var selected = db.PC.FirstOrDefault(a => a.PC_name == SelectedPC);
            return View("Index",db.Used_app.Where(m => m.PC_ID == selected.PC_ID));
        }

        // GET: Used_app/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Used_app used_app = db.Used_app.Find(id);
            if (used_app == null)
            {
                return HttpNotFound();
            }
            return View(used_app);
        }

        // GET: Used_app/Create
        public ActionResult Create()
        {
            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name");
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name");
            return View();
        }

        // POST: Used_app/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "App_ID,PC_ID,Time,Date")] Used_app used_app)
        {
            if (ModelState.IsValid)
            {
                db.Used_app.Add(used_app);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name", used_app.App_ID);
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", used_app.PC_ID);
            return View(used_app);
        }

        // GET: Used_app/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Used_app used_app = db.Used_app.Find(id);
            if (used_app == null)
            {
                return HttpNotFound();
            }
            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name", used_app.App_ID);
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", used_app.PC_ID);
            return View(used_app);
        }

        // POST: Used_app/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "App_ID,PC_ID,Time,Date")] Used_app used_app)
        {
            if (ModelState.IsValid)
            {
                db.Entry(used_app).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name", used_app.App_ID);
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", used_app.PC_ID);
            return View(used_app);
        }

        // GET: Used_app/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Used_app used_app = db.Used_app.Find(id);
            if (used_app == null)
            {
                return HttpNotFound();
            }
            return View(used_app);
        }

        // POST: Used_app/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Used_app used_app = db.Used_app.Find(id);
            db.Used_app.Remove(used_app);
            db.SaveChanges();
            return RedirectToAction("Index");
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
