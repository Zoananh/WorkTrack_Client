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
    public class Worked_timeController : Controller
    {
        private PC_TrackEntities db = new PC_TrackEntities();

        // GET: Worked_time
        public ActionResult Index()
        {
            var worked_time = db.Worked_time.Include(w => w.PC);
            return View(worked_time.ToList());
        }

        // GET: Worked_time/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worked_time worked_time = db.Worked_time.Find(id);
            if (worked_time == null)
            {
                return HttpNotFound();
            }
            return View(worked_time);
        }

        // GET: Worked_time/Create
        public ActionResult Create()
        {
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name");
            return View();
        }

        // POST: Worked_time/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PC_ID,Time,Date")] Worked_time worked_time)
        {
            if (ModelState.IsValid)
            {
                db.Worked_time.Add(worked_time);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", worked_time.PC_ID);
            return View(worked_time);
        }

        // GET: Worked_time/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worked_time worked_time = db.Worked_time.Find(id);
            if (worked_time == null)
            {
                return HttpNotFound();
            }
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", worked_time.PC_ID);
            return View(worked_time);
        }

        // POST: Worked_time/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PC_ID,Time,Date")] Worked_time worked_time)
        {
            if (ModelState.IsValid)
            {
                db.Entry(worked_time).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PC_ID = new SelectList(db.PC, "PC_ID", "PC_name", worked_time.PC_ID);
            return View(worked_time);
        }

        // GET: Worked_time/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worked_time worked_time = db.Worked_time.Find(id);
            if (worked_time == null)
            {
                return HttpNotFound();
            }
            return View(worked_time);
        }

        // POST: Worked_time/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Worked_time worked_time = db.Worked_time.Find(id);
            db.Worked_time.Remove(worked_time);
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
