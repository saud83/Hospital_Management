using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PharmaCoHealth.Models;
using PharmaCoHealth.ViewModel;

namespace PharmaCoHealth.Controllers
{
    public class AdministratorsController : Controller
    {
        private PharmaCoHealth_25Entities db = new PharmaCoHealth_25Entities();
        // GET: Administrators
        public ActionResult Index()
        {
            return View(db.Administrators.ToList());
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            if (Convert.ToString(Session["admin_email"]) == "")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        [HttpPost]
        public ActionResult LogIn(AdminLogInViewModel admin)
        {
            if (ModelState.IsValid)
            {
                var admn = db.Administrators.Where(a => a.Email.Equals(admin.Email) && a.Password.Equals(admin.Password)).FirstOrDefault();
                if (admn == null)
                {
                    ViewBag.NoAdmin = "No Admin Found";
                    return View();
                }
                else
                {
                    Session["admin_email"] = admin.Email;
                    Session["admin_id"] = admn.AdminId;
                    return RedirectToAction("adminHome");
                }
            }
            return View();
        }

        
        public ActionResult SearchBloodGrp()
        {
            string admin_email = Convert.ToString(Session["admin_email"]);
            if (admin_email == "")
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                return View(db.BloodBanks.ToList());
            }
            return View(db.BloodBanks.ToList());
        }
        
        public ActionResult adminHome()
        {
            string admin_email = Convert.ToString(Session["admin_email"]);
            if (admin_email == "")
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                return View();
            }
            //return View();
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("LogIn");
            //return View();
        }

        [HttpGet]
        public ActionResult Pharmacy()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Pharmacy(string medicineName, string medicineAmount)
        {
            int admin_id = Convert.ToInt32(Session["admin_id"]);
            if (medicineAmount != "" && medicineName != "")
            {
                var medicine = new Medicine();
                medicine.MedicineName = medicineName;
                medicine.MedicineAmount = medicineAmount;
                medicine.AdminID = admin_id;
                db.Medicines.Add(medicine);
                db.SaveChanges();
                return View();
            }
            else {
                ViewBag.FillUp = "Please Fill Up ALl The Box";
            }
            return View();
        }

        [HttpGet]
        public ActionResult NewSpcialize()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewSpcialize(string name)
        {
            int admin_id = Convert.ToInt32(Session["admin_id"]);
            if (name != "")
            {
                var specialize = new Specialize();
                specialize.SpecializedField = name;
                specialize.AdminID = admin_id;
                db.Specializes.Add(specialize);
                db.SaveChanges();
            }
            return View();
        }

        // GET: Administrators/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Administrator administrator = db.Administrators.Find(id);
            if (administrator == null)
            {
                return HttpNotFound();
            }
            return View(administrator);
        }

        // GET: Administrators/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdminId,AdminFirstName,AdminLastName,AdminPhoneNo,Email,Password,ConfirmPassword")] Administrator administrator)
        {
            if (ModelState.IsValid)
            {
                var admn = db.Administrators.Where(a => a.Email.Equals(administrator.Email)).FirstOrDefault();
                if (admn != null)
                {
                    ViewBag.SameEmail = "This Email already exists ! \n Chose another email";
                }
                else
                {
                    db.Administrators.Add(administrator);
                    db.SaveChanges();
                    return RedirectToAction("LogIn");
                }
            }

            return View(administrator);
        }

        

        // GET: Administrators/Edit/5
        public ActionResult Edit(string bloodGrp)
        {
            if (bloodGrp == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Administrator administrator = db.Administrators.Find(id);
            BloodBank blood = db.BloodBanks.Find(bloodGrp);
            ViewBag.BloodGrpName = bloodGrp;
            if (blood == null)
            {
                return HttpNotFound();
            }
            return View(blood);
        }

        // POST: Administrators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BloodGrp,Amount")] BloodBank administrator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(administrator).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SearchBloodGrp");
            }
            return View(administrator);
        }

        // GET: Administrators/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Administrator administrator = db.Administrators.Find(id);
            if (administrator == null)
            {
                return HttpNotFound();
            }
            return View(administrator);
        }

        // POST: Administrators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Administrator administrator = db.Administrators.Find(id);
            db.Administrators.Remove(administrator);
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
