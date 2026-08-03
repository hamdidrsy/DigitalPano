# DigitalPano

Özel eğitim kursları için ASP.NET Core tabanlı dijital pano yönetim ve yayın uygulaması.

## Gereksinimler

- .NET SDK 8.0.419 veya uyumlu .NET 8 SDK
- SQL Server LocalDB (geliştirme) veya SQL Server

## Yerel geliştirme

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/DigitalPano.Web
```

Varsayılan geliştirme bağlantısı `MSSQLLocalDB` kullanır. İlk yönetici hesabı kaynak koda yazılmaz. Gerekli değerler User Secrets ile tanımlanır:

```powershell
dotnet user-secrets set "SeedAdmin:Enabled" "true" --project src/DigitalPano.Web
dotnet user-secrets set "SeedAdmin:Email" "admin@example.local" --project src/DigitalPano.Web
dotnet user-secrets set "SeedAdmin:Password" "güçlü-bir-parola" --project src/DigitalPano.Web
```

Migration uygulamak için:

```powershell
dotnet ef database update --project src/DigitalPano.Web --startup-project src/DigitalPano.Web
```

Proje kapsamı ve geliştirme planı `docs/` klasöründedir.
