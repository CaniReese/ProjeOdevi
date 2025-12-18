using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOdevi.Data;
using ProjeOdevi.Models;

namespace ProjeOdevi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalTrainers = await _context.Trainers.CountAsync();
            var totalServices = await _context.ServiceTypes.CountAsync();

            var pending = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Pending);
            var approved = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Approved);
            var rejected = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Rejected);

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalTrainers = totalTrainers;
            ViewBag.TotalServices = totalServices;

            ViewBag.Pending = pending;
            ViewBag.Approved = approved;
            ViewBag.Rejected = rejected;

            return View();
        }
    }
}
