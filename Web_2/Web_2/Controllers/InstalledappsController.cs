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
    public class InstalledappsController : Controller
    {
        private PC_TrackEntities db = new PC_TrackEntities();

        // GET: Installedapps
        public ActionResult Index()
        {
            var installedapp = db.Installedapp.Include(i => i.Applications).Include(i => i.PC);
            return View(installedapp.ToList());
        }

        // GET: Installedapps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installedapp installedapp = db.Installedapp.Find(id);
            if (installedapp == null)
            {
                return HttpNotFound();
            }
            return View(installedapp);
        }

        // GET: Installedapps/Create
        public ActionResult Create()
        {
            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name");
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name");
            return View();
        }

        // POST: Installedapps/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "App_ID,PC_ID,Date")] Installedapp installedapp)
        {
            if (ModelState.IsValid)
            {
                db.Installedapp.Add(installedapp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name", installedapp.App_ID);
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", installedapp.PC_ID);
            return View(installedapp);
        }

        // GET: Installedapps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installedapp installedapp = db.Installedapp.Find(id);
            if (installedapp == null)
            {
                return HttpNotFound();
            }
            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name", installedapp.App_ID);
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", installedapp.PC_ID);
            return View(installedapp);
        }

        // POST: Installedapps/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "App_ID,PC_ID,Date")] Installedapp installedapp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(installedapp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.App_ID = new SelectList(db.Applications, "App_ID", "App_name", installedapp.App_ID);
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", installedapp.PC_ID);
            return View(installedapp);
        }

        // GET: Installedapps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installedapp installedapp = db.Installedapp.Find(id);
            if (installedapp == null)
            {
                return HttpNotFound();
            }
            return View(installedapp);
        }

        // POST: Installedapps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Installedapp installedapp = db.Installedapp.Find(id);
            db.Installedapp.Remove(installedapp);
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
