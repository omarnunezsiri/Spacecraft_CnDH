using Microsoft.OpenApi.Models;
using JAPI.Services;

namespace JAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddScoped<ITelemetryService, TelemetryService>();
            services.AddSwaggerGen(c =>
            {
#if (RELEASE)
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JAPI", Version = "v1" });
#elif DEBUG
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JAPI", Version = "v1" });
#endif
            });

            // Register any other services
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region AppConfiguration
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
#if RELEASE
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAPI v1.0");
#elif DEBUG
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAPI v1.0");
#endif
                });
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();
            #endregion

            #region Endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    WeatherForecast wf = new WeatherForecast();
                    wf.Date = DateOnly.FromDateTime(DateTime.Now);
                    wf.Summary = "a summary";
                    wf.TemperatureC = 32;

                    await context.Response.WriteAsJsonAsync(wf);
                });
            });
            #endregion
        }
    }

}
