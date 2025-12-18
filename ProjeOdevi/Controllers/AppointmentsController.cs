using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjeOdevi.Data;
using ProjeOdevi.Models;

namespace ProjeOdevi.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Member: randevu al
        public async Task<IActionResult> Create()
        {
            ViewBag.Trainers = new SelectList(
                await _context.Trainers.OrderBy(t => t.FullName).ToListAsync(),
                "Id", "FullName"
            );

            ViewBag.Services = new SelectList(
                await _context.ServiceTypes.OrderBy(s => s.Name).ToListAsync(),
                "Id", "Name"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int trainerId, int serviceTypeId, DateTime startDateTime)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var service = await _context.ServiceTypes.FindAsync(serviceTypeId);
            if (service == null) return NotFound();

            var endDateTime = startDateTime.AddMinutes(service.DurationMinutes);

            // 1) Müsaitlik kontrolü
            var day = startDateTime.DayOfWeek;
            var startT = startDateTime.TimeOfDay;
            var endT = endDateTime.TimeOfDay;

            var hasAvailability = await _context.TrainerAvailabilities.AnyAsync(a =>
                a.TrainerId == trainerId &&
                a.DayOfWeek == day &&
                startT >= a.StartTime &&
                endT <= a.EndTime);

            if (!hasAvailability)
                ModelState.AddModelError("", "Seçilen saat aralığı antrenörün müsaitliği dışında.");

            // 2) Çakışma kontrolü (Pending+Approved ile çakışmasın)
            var conflict = await _context.Appointments.AnyAsync(ap =>
                ap.TrainerId == trainerId &&
                ap.Status != AppointmentStatus.Rejected &&
                startDateTime < ap.EndDateTime &&
                endDateTime > ap.StartDateTime);

            if (conflict)
                ModelState.AddModelError("", "Bu saat aralığında antrenörün başka bir randevusu var.");

            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _context.Trainers.OrderBy(t => t.FullName).ToListAsync(), "Id", "FullName", trainerId);
                ViewBag.Services = new SelectList(await _context.ServiceTypes.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", serviceTypeId);
                return View();
            }

            var appt = new Appointment
            {
                MemberUserId = user.Id,
                TrainerId = trainerId,
                ServiceTypeId = serviceTypeId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                Status = AppointmentStatus.Pending,
                DurationSnapshot = service.DurationMinutes,
                PriceSnapshot = service.Price
            };

            _context.Appointments.Add(appt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAppointments));
        }

        // Member: benim randevularım
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var list = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.ServiceType)
                .Where(a => a.MemberUserId == user.Id)
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();

            return View(list);
        }

        // Admin: bekleyen randevular
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var list = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.ServiceType)
                .Where(a => a.Status == AppointmentStatus.Pending)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();

            return View(list);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return NotFound();

            appt.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return NotFound();

            appt.Status = AppointmentStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }

        [HttpGet]
        public async Task<IActionResult> ServicesByTrainer(int trainerId)
        {
            var services = await _context.TrainerServiceTypes
                .Where(x => x.TrainerId == trainerId)
                .Select(x => new { x.ServiceTypeId, x.ServiceType!.Name })
                .OrderBy(x => x.Name)
                .ToListAsync();

            return Json(services);
        }

    }
}
