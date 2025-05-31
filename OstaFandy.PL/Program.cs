
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;

namespace OstaFandy.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string ForCore = "";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.  

            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi  
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Register your services here
            #region RegisterServices
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService,UserService>();
            builder.Services.AddScoped<IJWTService, JWTService>();

            #endregion

            //JWT Authentication
            #region JWTAuthentication
            builder.Services.AddAuthentication(op => op.DefaultAuthenticateScheme = "myschema")
                .AddJwtBearer("myschema", option => { 
                    var key = builder.Configuration.GetSection("Jwt");
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key["Key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.  
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));

            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(ForCore);

            app.MapControllers();

            app.Run();
        }
    }
}
