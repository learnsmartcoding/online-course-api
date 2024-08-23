
using LSC.OnlineCourse.Data;
using LSC.OnlineCourse.Data.Entities;
using LSC.OnlineCourse.Service;
using Microsoft.EntityFrameworkCore;

namespace LSC.OnlineCourse.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //we have 2 parts here, one is service configuration for DI and 2nd one is Middlewares

            #region Service Configuration
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            //DB configuration goes here

            //Tips, if you want to see what are paramters we can enable here but it shows all sensitive data
            //so only used for development purpose should not go to PRODUCTION!
            builder.Services.AddDbContextPool<OnlineCourseDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DbContext"),
                provideroptions => provideroptions.EnableRetryOnFailure()
                
                );
                //options.EnableSensitiveDataLogging();
            });

            // In production, modify this with the actual domains you want to allow
            builder.Services.AddCors(o => o.AddPolicy("default", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //configure service DI here
            builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
            builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();

            #endregion

            #region Middlewares
            var app = builder.Build();

            app.UseCors("default");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            #endregion Middlewares
        }
    }
}
