using RedisCacheBlobReaderDemo.Services;

namespace RedisCacheBlobReaderDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Load configuration from environment variables
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddJsonFile("appsettings.json");

            //// Configure Azure Blob Storage directly from configuration
            //builder.Services.AddSingleton(new BlobService(
            //    builder.Configuration["AzureStorage:ConnectionString"],
            //    builder.Configuration["AzureStorage:ContainerName"],
            //    builder.Configuration["AzureStorage:BlobName"]
            //));
            builder.Services.AddSingleton(new BlobService());
            //// Configure Redis Cache
            //var redisCacheConfig = builder.Configuration["RedisCache:ConnectionString"];
            //builder.Services.AddSingleton(new RedisCacheService(redisCacheConfig));
            builder.Services.AddSingleton(new RedisCacheService());
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=DemoConfig}/{action=Index}/{id?}");

            app.Run();
        }
    }
}