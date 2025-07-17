using My_own_website;
using My_own_website.Components;
using My_own_website.Services;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<LoginManager>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new LoginManager(env.ContentRootPath);
});

var app = builder.Build();

var dataPath = Path.Combine(app.Environment.ContentRootPath, "Data");

if (!Directory.Exists(dataPath))
{
    Directory.CreateDirectory(dataPath);
}

var klassencodesPath = Path.Combine(dataPath, "klassencodes.txt");
if (!File.Exists(klassencodesPath))
{
    var defaultKlassencodes =
        "9c=Code123\n" +
        "8c=Code1234\n" +
        "7b=Code4567";

    File.WriteAllText(klassencodesPath, defaultKlassencodes);
}

var buchungenPath = Path.Combine(dataPath, "buchungen.txt");
if (!File.Exists(buchungenPath))
{
    File.WriteAllText(buchungenPath, "");
}

var workshopsPath = Path.Combine(dataPath, "workshops.txt");
if (!File.Exists(workshopsPath))
{
    File.WriteAllText(workshopsPath, "");
}

// Neu: workshopstatus.txt mit Defaultwerten anlegen
var workshopStatusPath = Path.Combine(dataPath, "workshopstatus.txt");
if (!File.Exists(workshopStatusPath))
{
    var defaultStatus =
     "Davinci=true;2025-07-30\n" +
     "VisualStudio=true;2025-08-05\n" +
     "Podcast=true;2025-08-10\n" +
     "3DPrinter=true;2025-07-25\n" +
     "Bildbearbeitung=true;2025-08-15";

    File.WriteAllText(workshopStatusPath, defaultStatus);
}

var accountsPath = Path.Combine(dataPath, "accounts");
if (!File.Exists(accountsPath))
{
    File.WriteAllText(accountsPath, "Admin=admin\n");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    Console.WriteLine("HTTPS-Redirect ist im Development-Modus deaktiviert.");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Urls.Add("http://0.0.0.0:80");

app.Run();
