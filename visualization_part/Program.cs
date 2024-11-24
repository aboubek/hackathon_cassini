namespace Cassini
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddControllers(); // Register controllers
            builder.Services.AddEndpointsApiExplorer(); // Enable endpoints for Swagger
            builder.Services.AddSwaggerGen(); // Add Swagger for API documentation
            builder.Services.AddHttpClient();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(); // Enable Swagger
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tile API v1");
                    options.RoutePrefix = "swagger"; // Swagger available at /swagger
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Ensure Razor Pages serve as the default content
            app.MapRazorPages(); // Razor Pages mapped to root by default

            app.MapControllers(); // Map API controller routes
            //app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}