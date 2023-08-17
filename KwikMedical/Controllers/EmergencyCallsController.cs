using KwikMedical.Data;
using KwikMedical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KwikMedical.Controllers
{
    public class EmergencyCallsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmergencyCallsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CreateEmergencyCall(EmergencyCall emergencyCall)
        {
            _context.EmergencyCalls.Add(emergencyCall);
            _context.SaveChanges();

            var medicalRecords = _context.MedicalRecords.FirstOrDefault(m => m.PatientId == emergencyCall.PatientId);
            if (medicalRecords != null)
            {
                // Simulate sending medical records to the ambulance
                SendMedicalRecordsToAmbulance(medicalRecords);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        private void SendMedicalRecordsToAmbulance(MedicalRecord medicalRecords)
        {
            // Assuming the ambulance has an associated user or device ID
            var ambulance = _context.Ambulances.FirstOrDefault(a => a.CurrentEmergencyCallId == medicalRecords.PatientId);
            if (ambulance != null)
            {
                // Here, you'd typically send the medical records to the ambulance's associated device.
                // For this prototype, we'll store the medical record ID in the ambulance's record as a simulation.
                ambulance.MedicalRecordId = medicalRecords.Id;
                _context.SaveChanges();
            }
        }

        // GET: EmergencyCalls
        public async Task<IActionResult> Index()
        {
            var emergencyCalls = _context.EmergencyCalls.Include(e => e.Patient);
            return View(await emergencyCalls.ToListAsync());
        }

        // GET: EmergencyCalls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencyCall = await _context.EmergencyCalls
                .Include(e => e.Patient)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            return View(emergencyCall);
        }

        [HttpPost]
        public IActionResult Create(EmergencyCall emergencyCall)
        {
            // 1. Check the patient's information against the existing database.
            var patient = _context.Patients.Find(emergencyCall.PatientId);
            if (patient == null)
            {
                ModelState.AddModelError("", "Patient not found in the database.");
                ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName");
                return View(emergencyCall);
            }

            // 2. Determine the best way to help the patient.
            // This can be enhanced with more complex logic. For now, we'll just log the patient's medical condition.
            var medicalCondition = emergencyCall.MedicalCondition;

            // 3. Generate an ambulance rescue request to one of the regional hospitals.
            // For simplicity, we'll assign the nearest available ambulance from the patient's city.
            var availableAmbulance = _context.Ambulances
                .FirstOrDefault(a => a.IsAvailable && a.CurrentCity == patient.City);
            if (availableAmbulance == null)
            {
                ModelState.AddModelError("", "No available ambulances in the patient's city.");
                ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName");
                return View(emergencyCall);
            }
            availableAmbulance.IsAvailable = false; // Mark the ambulance as occupied
            _context.Update(availableAmbulance);

            emergencyCall.AmbulanceId = availableAmbulance.Id;

            // 4. Extract the patient’s medical records and simulate sending them to an ambulance's smartphone.
            var medicalRecords = _context.MedicalRecords.Where(m => m.PatientId == emergencyCall.PatientId).ToList();
            // For now, we'll just log that the records were "sent" to the ambulance. 
            // In a real-world scenario, this would involve some API call or other mechanism to send the data.

            _context.EmergencyCalls.Add(emergencyCall);
            _context.SaveChanges();

            // Check if there's any error with the database save operation
            if (_context.Entry(emergencyCall).State == EntityState.Unchanged)
            {
                ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName", emergencyCall.PatientId);
                return View(emergencyCall);
            }

            return RedirectToAction(nameof(Index));
        }

        private void NotifyHospital(string hospitalName, EmergencyCall emergencyCall)
        {
            // For this demo, we'll just log the notification and update the hospital's preparation status.
            var hospital = _context.Hospitals.FirstOrDefault(h => h.Name == hospitalName);
            if (hospital != null)
            {
                hospital.PreparationStatus = $"Preparing for patient: {emergencyCall.Patient.FirstName} {emergencyCall.Patient.LastName} with ETA: {emergencyCall.EstimatedArrivalTime}";
                _context.Update(hospital);
                _context.SaveChanges();
            }

            System.Diagnostics.Debug.WriteLine($"Notified {hospitalName} about incoming patient: {emergencyCall.Patient.FirstName} {emergencyCall.Patient.LastName} with ETA: {emergencyCall.EstimatedArrivalTime}");
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // For simplicity, we'll use the Pythagorean theorem to calculate the distance
            // This is not accurate for real-world scenarios, but it's sufficient for our simulation
            return Math.Sqrt(Math.Pow(lat2 - lat1, 2) + Math.Pow(lon2 - lon1, 2));
        }

        private void SimulateAmbulanceMovement(Ambulance ambulance, EmergencyCall emergencyCall, double emergencyLatitude, double emergencyLongitude)
        {
            while (Math.Abs(ambulance.Latitude - emergencyLatitude) > 0.01 ||
                   Math.Abs(ambulance.Longitude - emergencyLongitude) > 0.01)
            {
                // Update ambulance's latitude and longitude in small increments
                ambulance.Latitude += (emergencyLatitude - ambulance.Latitude) * 0.01;
                ambulance.Longitude += (emergencyLongitude - ambulance.Longitude) * 0.01;

                // Save changes to the database
                _context.Update(ambulance);
                _context.SaveChanges();

                // Wait for a short duration before the next update
                System.Threading.Thread.Sleep(1000); // Wait for 1 second
            }

            // Once the ambulance reaches the destination, update the emergency call status
            emergencyCall.EmergencyStatus = "Ambulance Arrived";
            _context.Update(emergencyCall);
            _context.SaveChanges();

            // Simulate ambulance return to base after a short delay
            System.Threading.Thread.Sleep(5000); // Wait for 5 seconds

            ambulance.IsAvailable = true; // Mark the ambulance as available again
            ambulance.CurrentEmergencyCallId = null; // Clear the current emergency call
            _context.Update(ambulance);
            _context.SaveChanges();
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var emergencyCall = await _context.EmergencyCalls.FindAsync(id);
            if (emergencyCall == null)
            {
                return NotFound();
            }

            emergencyCall.IsCompleted = true;
            _context.Update(emergencyCall);

            var ambulance = await _context.Ambulances.FindAsync(emergencyCall.AmbulanceId);
            if (ambulance != null)
            {
                ambulance.IsAvailable = true;
                ambulance.CurrentEmergencyCallId = null;
                _context.Update(ambulance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName");
            return View();
        }

        public IActionResult Edit(int id)
        {
            var emergencyCall = _context.EmergencyCalls.Find(id);
            if (emergencyCall == null)
            {
                return NotFound();
            }
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "FullName", emergencyCall.PatientId);
            return View(emergencyCall);
        }

        [HttpPost]
        public IActionResult Edit(int id, EmergencyCall emergencyCall)
        {
            if (id != emergencyCall.Id)
            {
                return NotFound();
            }

            _context.Update(emergencyCall);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var emergencyCall = _context.EmergencyCalls.Find(id);
            if (emergencyCall == null)
            {
                return NotFound();
            }
            return View(emergencyCall);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var emergencyCall = _context.EmergencyCalls.Find(id);
            _context.EmergencyCalls.Remove(emergencyCall);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
