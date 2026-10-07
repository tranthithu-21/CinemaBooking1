using Plugins.DataStore.SqlServer;
using UseCases;
using UseCases.DataStorePluginInterfaces;
using WebApp.Components;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // SQL Server Configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\MSSQLLocalDB;Database=CinemaBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
            builder.Services.AddSingleton(new SqlServerConfiguration(connectionString));

            // Dependency Injection (Composition Root) - Repositories (K1.2 & K1.4)
            builder.Services.AddTransient<IMovieRepository, MovieSqlServerRepository>();
            builder.Services.AddTransient<IAuditoriumRepository, AuditoriumSqlServerRepository>();
            builder.Services.AddTransient<ISeatRepository, SeatSqlServerRepository>();
            builder.Services.AddTransient<IShowtimeRepository, ShowtimeSqlServerRepository>();
            builder.Services.AddTransient<IBookingRepository, BookingSqlServerRepository>();

            // Use Cases (Application Business Rules & 2 Core Business Rules - K1.3)
            builder.Services.AddTransient<ViewMoviesUseCase>();
            builder.Services.AddTransient<ViewShowtimesByMovieIdUseCase>();
            builder.Services.AddTransient<GetShowtimeByIdUseCase>();
            builder.Services.AddTransient<GetAuditoriumSeatsUseCase>();
            builder.Services.AddTransient<CreateBookingUseCase>();   // Luật 1: Tranh chấp ghế + Transaction K2.2
            builder.Services.AddTransient<CreateShowtimeUseCase>();  // Luật 2: Tránh trùng lịch phòng chiếu
            builder.Services.AddTransient<ViewBookingsUseCase>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
