using Blazored.LocalStorage;
using FRONT.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FRONT
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5282/")
            });


            builder.Services.AddScoped<AuthServices>();
            builder.Services.AddScoped<PrescriptionServices>();
            builder.Services.AddScoped<PatientServices>();
            builder.Services.AddScoped<MedicationServices>();
            builder.Services.AddScoped<PharmacistServices>();
            builder.Services.AddScoped<PrescriptionMedicationServices>();
            builder.Services.AddScoped<DispensedMedicationServices>();

            // Add Blazored LocalStorage - MUST be after other services
            builder.Services.AddBlazoredLocalStorage();

            await builder.Build().RunAsync();
        }
    }
}