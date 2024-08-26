using AzureStorageAccountDemo.Services;

namespace AzureStorageAccountDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfigurationRoot config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .AddEnvironmentVariables() // This will automatically translate "__" to ":"
             .Build();



    //        builder.Configuration
    //.SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //.AddEnvironmentVariables(); // This will automatically translate "__" to ":"

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Configuration.AddEnvironmentVariables();
            //builder.Configuration.AddJsonFile("appsettings.json");

            // Register BlobService for dependency injection
            builder.Services.AddSingleton<BlobService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}