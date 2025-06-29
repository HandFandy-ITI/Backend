
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.General;
using OstaFandy.PL.Hubs;

namespace OstaFandy.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.  

            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi  
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.UseNetTopologySuite()
                    );
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



            // Register your services here


            #region RegisterServices
            //system services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJWTService, JWTService>();
            //roles
            builder.Services.AddScoped<IHandyManService, HandyManService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IUserService,UserService>();
            builder.Services.AddScoped<IClientService, ClientService>();

            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IAnalyticsRepo, AnalyticsRepo>();

            builder.Services.AddScoped<IAutoBookingService, AutoBookingService>();

            builder.Services.AddScoped<IOrderFeedbackService, OrderFeedbackService>();

            builder.Services.AddScoped<IAutoBookingService, AutoBookingService>();


            builder.Services.AddScoped<IDashboardService, DashboardService>();



                        
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IClientPageService, ClientPageService>();

            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
 
            builder.Services.AddScoped<IHandymanJobsService, HandymanJobsService>();


            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();


            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();




            #endregion


            #region chat
            // chat services
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IHandymanJobsService, HandymanJobsService>();
            builder.Services.AddScoped<IAddressService, AddressService>();

 
            #endregion

            #region PaymentServices
            builder.Services.AddScoped<IPaymentService, PaymentService>();



            #endregion

            #region service catalog

            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            #endregion


            //JWT Authentication
            #region JWTAuth
            builder.Services.AddAuthentication(op => op.DefaultAuthenticateScheme = "myschema")
                .AddJwtBearer("myschema", option =>
                {
                    var key = builder.Configuration.GetSection("Jwt");
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key["Key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });
            builder.Services.AddAuthorization();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("UserType", General.UserType.Admin));
                options.AddPolicy("Customer", policy => policy.RequireClaim("UserType", General.UserType.Customer));
                options.AddPolicy("HandyMan", policy => policy.RequireClaim("UserType", General.UserType.Handyman));
            });
            #endregion


            //signaR
            builder.Services.AddSignalR();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.  
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));

            }

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}