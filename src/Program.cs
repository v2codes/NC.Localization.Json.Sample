
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;

namespace NC.Localization.Json.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            ConfigureServcie(builder.Services);

            InitializeApplication(builder);

        }

        private static void ConfigureServcie(IServiceCollection services)
        {
            //var assembly = typeof(Program).Assembly;
            //var resourceNames = assembly.GetManifestResourceNames();
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            services.AddSingleton<IFileProvider>(embeddedProvider);
            services.AddLocalization();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IStringLocalizerFactory, JsonLocalizerFactory>();
            services.AddMvc()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                        {
                            return factory.Create(typeof(JsonLocalizerFactory));
                        };
                    });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportCultures = new[]
                {
                    new CultureInfo("zh-cn"),
                    new CultureInfo("en-us"),
                };
                options.DefaultRequestCulture = new RequestCulture(supportCultures[0]);
                options.SupportedCultures = supportCultures;
            });

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

        }

        private static void InitializeApplication(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            var supportedCultures = new[] { "zh-cn", "en-us", };
            app.UseRequestLocalization(options =>
            {
                options.SetDefaultCulture(supportedCultures[0]);
                options.AddSupportedCultures(supportedCultures);
            });

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}