using System.Reflection;
using System.Runtime.Loader;
using InstrumentService_DataAccess;
using InstrumentService_DataAccess.Repository;
using InstrumentService_DataAccess.Repository.IRepository;
using InstrumentService_Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
            var currentAssembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"Сборка текущего приложения (Assembly): {currentAssembly.FullName}");

            // Получаем сборку текущего приложения с помощью AssemblyLoadContext.Default.Assemblies
            var contextAssemblies = AssemblyLoadContext.Default.Assemblies;
            foreach (var assembly in contextAssemblies)
            {
                Console.WriteLine($"Сборка в контексте загрузки сборок (AssemblyLoadContext.Default.Assemblies): {assembly.FullName}");
            }
            // var r = Assembly.GetExecutingAssembly();
            // var a = AssemblyLoadContext.Default.Assemblies;
            // Type t = typeof(Program);
            // Assembly assemFromType = t.Assembly;
            // Console.WriteLine("Assembly that contains Example:");
            // Console.WriteLine("   {0}\n", assemFromType.FullName);
            //
            // // Get the currently executing assembly.
            // Assembly currentAssem = Assembly.GetExecutingAssembly();
            // Console.WriteLine("Currently executing assembly:");
            // Console.WriteLine("   {0}\n", currentAssem.FullName);
            //
            // Console.WriteLine("The two Assembly objects are equal: {0}",
            //     assemFromType.Equals(currentAssem));
            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));
            //AddDefaultIdentity устанавливает некоторую начальную конфигурацию, в качестве пользователя засовывем стандратный класс IdentityUser
            //Метод AddEntityFrameworkStores() устанавливает тип хранилища,
            //которое будет применяться в Identity для хранения данных. В качестве типа хранилища здесь указывается класс контекста данных.
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
            }).AddDefaultTokenProviders().AddDefaultUI().AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();
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
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();
            builder.Services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

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
            app.UseAuthentication();

            app.UseAuthorization();
            //добавление middleware сессий. активируем сессии, позволяя приложению использовать механизм сессий для хранения данных,
            //связанных с определенным пользователем, на протяжении нескольких HTTP-запросов.
            app.UseSession();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}