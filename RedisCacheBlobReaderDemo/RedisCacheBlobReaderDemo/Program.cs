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


            // Configure Azure Blob Storage directly from configuration
            builder.Services.AddSingleton(new BlobService(
                builder.Configuration["AzureBlobStorage:ConnectionString"],
                builder.Configuration["AzureBlobStorage:ContainerName"],
                builder.Configuration["AzureBlobStorage:BlobName"]
            ));

            // Configure Redis Cache
            var redisCacheConfig = builder.Configuration.GetSection("RedisCache");
            builder.Services.AddSingleton(new RedisCacheService(redisCacheConfig["ConnectionString"]));

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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
