using Backend.App.PostgresIdentity;

using Microsoft.EntityFrameworkCore;

namespace Backend.App.Extensions;
public static class MigrationExtensions {
    public static void applMigrations(this IApplicationBuilder app) {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}