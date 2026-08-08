# DigitalPano

Özel eğitim kurumları için ASP.NET Core 8 tabanlı dijital pano yönetim ve yayın uygulaması.

## Teknoloji

- ASP.NET Core MVC, Identity ve SignalR
- PostgreSQL / Entity Framework Core
- Yerelde dosya sistemi, canlıda Cloudflare R2 medya depolama
- Docker ile Render dağıtımı

## Yerel geliştirme

Gerekenler: .NET 8 SDK ve çalışan bir PostgreSQL sunucusu. Varsayılan geliştirme bağlantısı `appsettings.json` içindedir; gerekirse User Secrets ile değiştirin:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=digitalpano;Username=postgres;Password=PAROLANIZ" --project src/DigitalPano.Web
dotnet user-secrets set "SeedAdmin:Enabled" "true" --project src/DigitalPano.Web
dotnet user-secrets set "SeedAdmin:Email" "admin@example.local" --project src/DigitalPano.Web
dotnet user-secrets set "SeedAdmin:Password" "Guclu-Bir-Parola!123" --project src/DigitalPano.Web
```

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/DigitalPano.Web
```

Uygulama başlangıçta bekleyen migration'ları uygular. Elle uygulamak için:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/DigitalPano.Web --startup-project src/DigitalPano.Web
```

Canlı uygulama: https://digitalpano.onrender.com
