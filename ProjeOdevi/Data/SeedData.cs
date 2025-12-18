using Microsoft.AspNetCore.Identity;
using ProjeOdevi.Data;
using ProjeOdevi.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjeOdevi.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Member" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = "ogrencinumarasi@sakarya.edu.tr";
            var adminPassword = "sau"; // ödevde istenen :contentReference[oaicite:1]{index=1}

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception("Admin user oluşturulamadı: " + errors);
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                await userManager.AddToRoleAsync(adminUser, "Admin");


        }

        public static async Task SeedDemoDataAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // DB hazır mı?
            await context.Database.MigrateAsync();

            // Eğer zaten veri varsa tekrar ekleme
            if (await context.ServiceTypes.AnyAsync() || await context.Trainers.AnyAsync())
                return;

            // 1) Hizmetler
            var s1 = new ServiceType { Name = "Personal Training", DurationMinutes = 60, Price = 800m };
            var s2 = new ServiceType { Name = "Pilates", DurationMinutes = 45, Price = 600m };
            var s3 = new ServiceType { Name = "Fitness Danismanligi", DurationMinutes = 30, Price = 400m };

            context.ServiceTypes.AddRange(s1, s2, s3);

            // 2) Antrenörler
            var t1 = new Trainer { FullName = "Ahmet Yilmaz" };
            var t2 = new Trainer { FullName = "Elif Kaya"};

            context.Trainers.AddRange(t1, t2);

            await context.SaveChangesAsync();

            // 3) Trainer - Service eşleşmeleri (çoktan-çoğa)
            context.TrainerServiceTypes.AddRange(
                new TrainerServiceType { TrainerId = t1.Id, ServiceTypeId = s1.Id },
                new TrainerServiceType { TrainerId = t1.Id, ServiceTypeId = s3.Id },
                new TrainerServiceType { TrainerId = t2.Id, ServiceTypeId = s2.Id },
                new TrainerServiceType { TrainerId = t2.Id, ServiceTypeId = s3.Id }
            );

            // 4) Müsaitlikler (Pzt-Cuma 09-17)
            foreach (var day in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
            {
                context.TrainerAvailabilities.Add(new TrainerAvailability
                {
                    TrainerId = t1.Id,
                    DayOfWeek = day,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });

                context.TrainerAvailabilities.Add(new TrainerAvailability
                {
                    TrainerId = t2.Id,
                    DayOfWeek = day,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0)
                });
            }

            await context.SaveChangesAsync();
        }

    }
}


