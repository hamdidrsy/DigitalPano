# Aşama 1 — Proje İskeleti ve Veri Tabanı Özeti

## Durum

**Tamamlandı — 3 Ağustos 2026**

## Oluşturulan yapı

- `DigitalPano.sln`
- `src/DigitalPano.Web`: ASP.NET Core MVC, .NET 8
- `tests/DigitalPano.Tests`: xUnit test projesi
- EF Core 8.0.25 ve SQL Server sağlayıcısı
- ASP.NET Core Identity veri katmanı
- Kod analizi, biçim ve Git dışlama kuralları
- User Secrets destekli güvenli başlangıç yöneticisi mekanizması

## Veri modeli

- `AppUser`
- `InstitutionSetting`
- `Screen`
- `Announcement`
- `AnnouncementScreen`
- `Media`
- `TickerMessage`
- `ActivityLog`

İlişkiler, benzersiz indeksler, alan uzunlukları ve tarih/dosya boyutu kontrol kısıtları Fluent API ile tanımlanmıştır.

## Migration ve veritabanı

- Migration: `InitialCreate`
- Sağlayıcı: SQL Server
- Geliştirme örneği: `(localdb)\\MSSQLLocalDB`
- Veritabanı: `DigitalPano`
- Migration başarıyla uygulanmıştır.
- Başlangıç kurumu: `Özel Eğitim Kursu`
- Başlangıç ekranı: `Giriş Katı` / `giris-kati`

## Güvenlik

- Parola kaynak koda veya `appsettings.json` dosyasına yazılmamıştır.
- Başlangıç yöneticisi varsayılan olarak kapalıdır.
- Yönetici bilgileri User Secrets veya ortam değişkenleriyle sağlanır.
- Identity parola, kilitleme ve benzersiz e-posta kuralları yapılandırılmıştır.
- Yerel araç/paket önbelleği ve medya dosyaları Git dışında bırakılmıştır.

## Doğrulama sonuçları

- `dotnet restore`: başarılı
- `dotnet build --no-restore`: başarılı, 0 uyarı, 0 hata
- `dotnet test --no-build --no-restore`: 5/5 başarılı
- `dotnet ef database update`: başarılı

Makinedeki eski `sqlcmd` istemcisi ODBC 17 sürücü kaydı nedeniyle kullanılamamıştır. EF Core'un SQL bağlantısı ve migration uygulaması başarılı olduğundan bu, uygulamayı engelleyen bir sorun değildir.

## Sonraki adım

**Aşama 2 — Kimlik ve yönetim kabuğu:** Türkçe giriş/çıkış akışı, yetkilendirilmiş Admin alanı, yönetim yerleşimi ve gösterge paneli.
