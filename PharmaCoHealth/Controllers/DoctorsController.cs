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
    public class DoctorsController : Controller
    {
        private PharmaCoHealth_25Entities db = new PharmaCoHealth_25Entities();

        // GET: Doctors
        public ActionResult Index()
        {
            return View(db.Doctors.ToList());
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            string doctor_email = Convert.ToString(Session["doctor_email"]);
            if (doctor_email == "")
            {
                return View();
            }
            else
            {
                return RedirectToAction("doctorHome");
            }
        }

        [HttpPost]
        public ActionResult LogIn(DoctorLogInViewModel doctor)
        {
            if (ModelState.IsValid)
            {
                var doc = db.Doctors.Where(a => a.Email.Equals(doctor.Email) && a.Password.Equals(doctor.Password)).FirstOrDefault();
                if (doc == null)
                {
                    ViewBag.NoDocotr = "No Doctor Found";
                    return View();
                }
                else
                {
                    Session["doctor_email"] = doctor.Email;
                    TempData["Doctor_id"] = doc.DoctorId;
                    return RedirectToAction("doctorHome");
                }
            }
            return View();
        }

        public ActionResult doctorHome()
        {
            string doctor_email = Convert.ToString(Session["doctor_email"]);
            if (doctor_email == "")
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                return View();
            }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("LogIn");
            //return View();
        }

        public ActionResult CheckAppointments()
        {
            string doctor_email = Convert.ToString(Session["doctor_email"]);
            if (doctor_email == "")
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                var doc = db.Doctors.Where(d => d.Email.Equals(doctor_email)).FirstOrDefault();
                ViewBag.DoctorName = "Dr."+doc.DoctorFirstName + " " + doc.DoctorLastName;
                DateTime today = DateTime.Now;
                var dates = today.Date.ToString("yyyy/MM/dd");
                var scheduleList = db.Schedules.SqlQuery("SELECT * FROM Schedule WHERE DoctorId = "+doc.DoctorId + " AND ScheduledTime >= '" + dates +"'");
                var l = new List<string>();
                var l1 = new List<string>();
                if (scheduleList.ToList() != null)
                {
                    foreach (var date in scheduleList.ToList())
                    {
                        string d = Convert.ToString(date.ScheduledTime);
                        l.Add(d);
                    }
                    foreach (var patient_id in scheduleList.ToList())
                    {
                        //var patient_name = db.Patients.SqlQuery("SELECT PatientFirstName,PatientLastName FROM Patient WHERE Patient.PatientId  = " + patient_id.PatientId).FirstOrDefault();
                        var patient_name = db.Patients.Where(p => p.PatientId == patient_id.PatientId).FirstOrDefault();
                        l1.Add(patient_name.PatientFirstName + " " + patient_name.PatientLastName);
                    }
                }
                else
                {
                    l.Add("No Appointment");
                    l1.Add("No Appointment");
                }
                //listOfPatient.Add(patient_name.ToList());
                var obj = new CheckingAppointment()
                {
                    patient = l1,
                    schedule = l
                };
                //return View(scheduleList.ToList());
                return View(obj);
            }
        }

        [HttpGet]
        public ActionResult SearchPatient()
        {
            var patientInfo = db.PATIENTINFOes.Where(p => p.PatientId.Equals(8)).FirstOrDefault();
            return View(patientInfo);
        }

        [HttpPost]
        public ActionResult SearchPatient(string FirstName, string LastName)
        {
            var patient = db.Patients.Where(p => p.PatientFirstName.Equals(FirstName) && p.PatientLastName.Equals(LastName)).FirstOrDefault();
            var patientInfo = db.PATIENTINFOes.Where(p => p.PatientId.Equals(patient.PatientId)).FirstOrDefault();
            return View(patientInfo);
        }

        [HttpGet]
        public ActionResult SearchPatientToPrescribe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchPatientToPrescribe(string FirstName, string LastName)
        {
            var patient = db.Patients.Where(p => p.PatientFirstName.Equals(FirstName) && p.PatientLastName.Equals(LastName)).FirstOrDefault();
            TempData["patientId"] = patient.PatientId;
            TempData["patientFirstName"] = patient.PatientFirstName;
            TempData["patientLastName"] = patient.PatientLastName;
            TempData["patient_email"] = patient.Email;
            return RedirectToAction("Prescribe");
        }

        //Just need to make prescribe action to store value in patient report

        [HttpGet]
        public ActionResult Prescribe()
        {
            ViewBag.FirstName = TempData["patientFirstName"];
            ViewBag.LastName = TempData["patientLastName"];
            ViewBag.Email = TempData["patient_email"];
            var Symptom = new List<String>() { "Allergies", "Colds and Flu", "Conjunctivitis", "Diarrhea", "Headaches", "Mononucleosis" };
            ViewBag.Symptom = Symptom;
            var Diagnosis = new List<String>() { "Allergies", "Colds and Flu", "Conjunctivitis", "Diarrhea", "Headaches", "Mononucleosis" };
            ViewBag.Diagnosis = Diagnosis;
            var PrescribedMedicine = db.Medicines.ToList();
            var newList = new List<string>();
            foreach (var item in PrescribedMedicine)
            {
                newList.Add(item.MedicineName);
            }
            ViewBag.PrescribedMedicine = newList;
            return View();
        }

        [HttpPost]
        public ActionResult Prescribe(PATIENTREPORT patientReport)
        {
            if (ModelState.IsValid)
            {
                patientReport.DoctorId = Convert.ToInt32(TempData["Doctor_id"]);
                patientReport.PatientId = Convert.ToInt32(TempData["patientId"]);
                db.PATIENTREPORTs.Add(patientReport);
                db.SaveChanges();
                //return RedirectToAction("doctorHome");
                return Content("Diagnosis Successful");
                
            }
            return View();
        }

        // GET: Doctors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // GET: Doctors/Create
        public ActionResult Create()
        {
            var specialize = db.Specializes.ToList();
            var newList = new List<string>();
            foreach (var item in specialize)
            {
                newList.Add(item.SpecializedField);
            }
            //ViewBag.PrescribedMedicine = newList;
            //var list = new List<String>() { "Diabetes", "Medicine", "Heart Surgeon" };
            ViewBag.list = newList;
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DoctorId,DoctorFirstName,DoctorLastName,Specialize,DoctorPhoneNo,Email,Password,ConfirmPassword")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                var doctr = db.Doctors.Where(a => a.Email.Equals(doctor.Email)).FirstOrDefault();
                if (doctr != null)
                {
                    ViewBag.SameEmail = "This Email already exists ! \n Chose another email";
                }
                else
                {
                    db.Doctors.Add(doctor);
                    db.SaveChanges();
                    return RedirectToAction("LogIn");
                }
            }
            var list = new List<String>() { "Diabetes", "Medicine", "Heart Surgeon" };
            ViewBag.list = list;
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DoctorId,DoctorFirstName,DoctorLastName,Specialize,DoctorPhoneNo,Email,Password")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
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
