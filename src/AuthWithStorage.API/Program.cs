using AuthWithStorage.API.Extensions;

namespace AuthWithStorage.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication
                .CreateBuilder(args)
                .InitApp();

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.InitServices(builder.Configuration);

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
        }
    }
}