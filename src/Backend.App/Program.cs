using Backend.App.Extensions;
using Backend.App.PostgresIdentity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication().AddJwtBearer(IdentityConstants.ApplicationScheme);

        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();
        builder.Services.AddDbContext<ApplicationDbContext>( options
            => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
        builder.Services.AddControllers();
        
        var app = builder.Build();
        if (app.Environment.IsDevelopment()) {
            app.applyMigrations();
        }
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapIdentityApi<User>();
        app.Run();
    }
}
