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
    public class PCsController : Controller
    {
        private PC_TrackEntities db = new PC_TrackEntities();

        // GET: PCs
        public ActionResult Index()
        {
            var pC = db.PC.Include(p => p.Worked_time);
            return View(pC.ToList());
        }

        // GET: PCs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PC pC = db.PC.Find(id);
            if (pC == null)
            {
                return HttpNotFound();
            }
            return View(pC);
        }

        // GET: PCs/Create
        public ActionResult Create()
        {
            ViewBag.PC_ID = new SelectList(db.Worked_time, "PC_ID", "Time");
            return View();
        }

        // POST: PCs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PC_ID,PC_name")] PC pC)
        {
            if (ModelState.IsValid)
            {
                db.PC.Add(pC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PC_ID = new SelectList(db.Worked_time, "PC_ID", "Time", pC.PC_ID);
            return View(pC);
        }

        // GET: PCs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PC pC = db.PC.Find(id);
            if (pC == null)
            {
                return HttpNotFound();
            }
            ViewBag.PC_ID = new SelectList(db.Worked_time, "PC_ID", "Time", pC.PC_ID);
            return View(pC);
        }

        // POST: PCs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PC_ID,PC_name")] PC pC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PC_ID = new SelectList(db.Worked_time, "PC_ID", "Time", pC.PC_ID);
            return View(pC);
        }

        // GET: PCs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PC pC = db.PC.Find(id);
            if (pC == null)
            {
                return HttpNotFound();
            }
            return View(pC);
        }

        // POST: PCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PC pC = db.PC.Find(id);
            db.PC.Remove(pC);
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
