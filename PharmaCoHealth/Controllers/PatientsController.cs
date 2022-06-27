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
    public class PatientsController : Controller
    {
        private PharmaCoHealth_25Entities db = new PharmaCoHealth_25Entities();

        // GET: Patients
        public ActionResult Index()
        {
            return View(db.Patients.ToList());
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            if (Convert.ToString(Session["patient_email"]) == "")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult LogIn(PatientLogInViewModel patient)
        {
            if (ModelState.IsValid)
            {
                var pat = db.Patients.Where(a => a.Email.Equals(patient.Email) && a.Password.Equals(patient.Password)).FirstOrDefault();
                if (pat == null)
                {
                    ViewBag.NoPatient = "No Patients Found";
                    return View();
                }
                else
                {
                    Session["patient_email"] = patient.Email;
                    TempData["patient_id"] = pat.PatientId;
                    return RedirectToAction("patientHome");
                }
            }
            return View();
        }

        public ActionResult patientHome()
        {
            string patinet_email = Convert.ToString(Session["patient_email"]);
            if (patinet_email == "")
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
        public ActionResult ProvideInformation()
        {
            //db.PATIENTINFOes.SqlQuery("SELECT * FROM PATIENTINFO WHERE PATIENTINFO.PatientID = (SELECT PatientId FROM Patient WHERE Email = "+Convert.ToString(Session["patient_email"])+")");
            //PATIENTINFO p = db.PATIENTINFOes.Where(a => a.PatientId.Equals());
            string patient_email = Convert.ToString(Session["patient_email"]);
            var patient = db.Patients.Where(a => a.Email.Equals(patient_email)).FirstOrDefault();
            ViewBag.firstName = patient.PatientFirstName;
            ViewBag.lastName = patient.PatientLastName;
            var list = new List<String>() { "Male", "Female", "Other" };
            ViewBag.list = list;
            var listOfMedicalHistory = new List<String>() { "Diabetes", "Heart Attack", "Tumor" };
            ViewBag.listOfMedicalHistory = listOfMedicalHistory;
            var listOfBloodGroup = new List<String>() { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            ViewBag.listOfBloodGroup = listOfBloodGroup;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProvideInformation(PATIENTINFO p)
        {
            string patient_email = Convert.ToString(Session["patient_email"]);
            var patient = db.Patients.Where(a => a.Email.Equals(patient_email)).FirstOrDefault();
            p.PatientFirstName = patient.PatientFirstName;
            p.PatientLastName = patient.PatientLastName;
            p.PatientId = patient.PatientId;
            if (ModelState.IsValid)
            { 
                db.PATIENTINFOes.Add(p);
                db.SaveChanges();
            }
            //ViewBag.done = "Check";
            //return View();
            return RedirectToAction("patientHome");
        }

        [HttpGet]
        public ActionResult SetAppointment()
        {
            var listOfDoctorsAndSpecialize = new AppointmentSet()
            {
                doctors = db.Doctors.ToList(),
                specialize = db.Specializes.ToList()
            };
            return View(listOfDoctorsAndSpecialize);
        }

        [HttpPost]
        public ActionResult SetAppointment(string specialize)
        {
            var list = db.Doctors.Where(d =>d.Specialize.Equals(specialize));
            var listOfDoctorsAndSpecialize = new AppointmentSet()
            {
                doctors = list,
                specialize = db.Specializes.ToList()
            };
            return View(listOfDoctorsAndSpecialize);
        }

        [HttpGet]
        public ActionResult FinalAppointment(string doctorEmail)
        {
            var doctor = db.Doctors.Where(d => d.Email.Equals(doctorEmail)).FirstOrDefault();
            ViewBag.DoctorFirstName = doctor.DoctorFirstName;
            ViewBag.DoctorLastName = doctor.DoctorLastName;
            TempData["doctor_email"] = doctor.Email;
            string patient_email = Convert.ToString(Session["patient_email"]);
            var patient = db.Patients.Where(a => a.Email.Equals(patient_email)).FirstOrDefault();
            ViewBag.PatientfirstName = patient.PatientFirstName;
            ViewBag.PatientlastName = patient.PatientLastName;
            var schedule = new List<String>() {};
            for (int i = 0; i < 7; i++)
            {
                DateTime today = DateTime.Now;
                DateTime answer = today.AddDays(i);
                var date = answer.Date.ToString("yyyy/MM/dd");
                //schedule.Add(Convert.ToString(date));
                schedule.Add(date);
            }
            ViewBag.schedule = schedule;
            return View();
        }

        [HttpPost]
        public ActionResult FinalAppointment(Schedule schedule)
        {
            string patient_email = Convert.ToString(Session["patient_email"]);
            var patient = db.Patients.Where(a => a.Email.Equals(patient_email)).FirstOrDefault();
            schedule.PatientId = patient.PatientId;
            string doctor_email = Convert.ToString(TempData["doctor_email"]);
            var doctor = db.Doctors.Where(a => a.Email.Equals(doctor_email)).FirstOrDefault();
            schedule.DoctorId = doctor.DoctorId;
            if (ModelState.IsValid)
            {
                db.Schedules.Add(schedule);
                db.SaveChanges();
            }
            return RedirectToAction("patientHome");
        }

        public ActionResult Prescription()
        {
            int patient_id = Convert.ToInt32(TempData["patient_id"]);
            var patient = db.PATIENTREPORTs.Where(p => p.PatientId.Equals(patient_id)).FirstOrDefault();
            if (patient != null)
            {
                var doctor = db.Doctors.Where(d => d.DoctorId.Equals(patient.DoctorId)).FirstOrDefault();
                ViewBag.DoctorName = doctor.DoctorFirstName + " " + doctor.DoctorLastName;
                var patientName = db.Patients.Where(p => p.PatientId.Equals(patient_id)).FirstOrDefault();
                ViewBag.PatientName = patientName.PatientFirstName + " "+ patientName.PatientLastName;
                return View(patient);
            }
            else {
                return RedirectToAction("patientHome");
            }
            //return View();
        }

        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientId,PatientFirstName,PatientLastName,PatientPhoneNo,Email,Password,ConfirmPassword")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                var ptn = db.Patients.Where(a => a.Email.Equals(patient.Email)).FirstOrDefault();
                if (ptn != null)
                {
                    ViewBag.SameEmail = "This Email already exists ! \n Chose another email";
                }
                else
                {
                    db.Patients.Add(patient);
                    db.SaveChanges();
                    return RedirectToAction("LogIn");
                }
            }

            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientId,PatientFirstName,PatientLastName,PatientPhoneNo,Email,Password")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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
