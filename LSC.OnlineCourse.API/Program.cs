
using LSC.OnlineCourse.Data.Entities;
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

            builder.Services.AddDbContextPool<OnlineCourseDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DbContext"),
                provideroptions => provideroptions.EnableRetryOnFailure());
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #endregion

            #region Middlewares
            var app = builder.Build();

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
