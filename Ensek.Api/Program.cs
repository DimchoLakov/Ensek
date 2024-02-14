
namespace Ensek.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddControllers();

            builder
                .Services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app
                .UseHttpsRedirection()
                .UseAuthorization()
                .UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}
