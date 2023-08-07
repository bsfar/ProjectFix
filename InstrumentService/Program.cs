using InstrumentService.Data;
using Microsoft.EntityFrameworkCore;

namespace InstrumentService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation(); 

            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));
            //AddHttpContextAccessor - добавление HttpContextAccessor является необходимым, потому что 
            //приложение работает с текущим контекстом HTTP
            //В предоставленном коде он используется вместе с AddSession для конфигурации параметров сессии, что может
            //потребовать доступа к HttpContext для обработки операций, связанных с сессией.
            builder.Services.AddHttpContextAccessor();
            //добавление поддержки сессий
            builder.Services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                //устанавливаем HttpOnly в значение true, что бы cookie былидоступны только для сервера
                //и не могли быть доступны через клиентский скрипт (например, JavaScript)
                //Это обеспечивает дополнительную защиту от определенных уязвимостей, таких как атаки CSRF (межсайтовая подделка запросов)
                Options.Cookie.HttpOnly = true;
                //IsEssential = true это означает, что сессионные cookies должны быть всегда отправлены с запросами,
                //даже если пользователь запретил использование cookies в своем браузере. Это важно, потому что сессионные cookies используются
                //для авторизации или других критических функций
                Options.Cookie.IsEssential = true;
            });
            builder.Services.AddControllersWithViews();
            

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
            //добавление middleware сессий. активируем сессии, позволяя приложению использовать механизм сессий для хранения данных,
            //связанных с определенным пользователем, на протяжении нескольких HTTP-запросов.
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}