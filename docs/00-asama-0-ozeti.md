# Aşama 0 — Proje Hazırlığı Özeti

## Durum

**Tamamlandı — 3 Ağustos 2026**

Bu aşamada DigitalPano'nun ilk üretim sürümünün sınırları, kullanıcı akışları, teknik tabanı, kabul kriterleri ve geliştirme sırası tanımlanmıştır. Kurumdan daha sonra alınabilecek operasyonel bilgiler açık karar olarak bırakılmış, yazılım geliştirmesini engellemeyecek varsayılanlar belirlenmiştir.

## Hazırlanan çıktılar

- [Gereksinimler ve kabul kriterleri](01-gereksinimler-ve-kabul-kriterleri.md)
- [Ekranlar ve kullanıcı akışları](02-ekranlar-ve-akislar.md)
- [Teknik karar kaydı](03-teknik-kararlar.md)
- [Geliştirme görev listesi](04-gorev-listesi.md)

## Kesinleşen kapsam

- Sistem tek bir özel eğitim kursunda kullanılacaktır.
- İki arayüz bulunacaktır: yetkili yönetim paneli ve tam ekran pano yayını.
- İlk sürümde tek yönetici rolü yeterlidir.
- Birden fazla fiziksel ekran ayrı ayrı yönetilebilecektir.
- Duyurular tarih aralığına ve hedef ekranlara göre yayımlanacaktır.
- Metin, görsel, MP4 video, kayan yazı ve acil duyuru desteklenecektir.
- Açık pano sayfaları SignalR üzerinden değişiklik bildirimi alacaktır.
- Kısa bağlantı kesintilerinde son uygun yayın korunacaktır.
- Öğrenci/veli hesabı, mobil uygulama, abonelik ve çok kiracılı yapı kapsam dışıdır.

## Başlangıç teknik tabanı

| Konu | Karar |
|---|---|
| Uygulama | ASP.NET Core MVC |
| Çalışma çatısı | .NET 8 LTS |
| Kimlik | ASP.NET Core Identity |
| Veri erişimi | Entity Framework Core 8 |
| Veritabanı | SQL Server |
| Gerçek zamanlı iletişim | SignalR |
| Arayüz | Razor Views, Bootstrap, JavaScript |
| Çevrimdışı destek | Service Worker ve yerel yayın önbelleği |
| Saat saklama | UTC; arayüzde `Europe/Istanbul` |
| İlk hedef ekran | 1920×1080, 16:9 yatay ekran |

Geliştirme makinesinde .NET SDK 8.0.419 ve 9.0.312 mevcuttur. Aşama 1 sırasında `MSSQLLocalDB` doğrulanmış ve `DigitalPano` geliştirme veritabanı başarıyla oluşturulmuştur. Makinedeki eski `sqlcmd`/ODBC 17 istemcisi çalışmadığından veritabanı işlemleri EF Core CLI üzerinden yürütülmektedir.

## Açık kararlar

Bu maddeler geliştirmeyi başlatmaya engel değildir:

| Karar | Geçici varsayım | Kesinleşmesi gereken zaman |
|---|---|---|
| Fiziksel ekran sayısı ve adları | `Giriş Katı` adlı örnek ekran | Canlı veri girişi öncesi |
| Canlı sunucu türü | Windows + IIS + SQL Server | Canlı ortam hazırlığı öncesi |
| Pano erişimi | Kurum ağı veya HTTPS adresi | Cihaz kurulumu öncesi |
| Video üst sınırı | Dosya başına 200 MB | Medya modülü öncesi |
| Görsel üst sınırı | Dosya başına 10 MB | Medya modülü öncesi |
| Video sesi | Varsayılan sessiz | Cihaz kabul testi öncesi |
| Hava durumu sağlayıcısı | Henüz seçilmedi | Hava durumu modülü öncesi |
| Logo ve renkler | Geçici tema kullanılacak | Kullanıcı kabulü öncesi |
| Yedekleme hedefi | Ayrı disk veya ağ konumu | Canlıya geçiş öncesi |

## Aşama 0 çıkış kontrolü

- [x] Proje amacı ve sınırları yazıldı.
- [x] Fonksiyonel gereksinimler tanımlandı.
- [x] Fonksiyonel olmayan gereksinimler tanımlandı.
- [x] Kabul kriterleri test edilebilir biçimde yazıldı.
- [x] Yönetim ve pano ekran akışları çıkarıldı.
- [x] Temel pano yerleşimi çizildi.
- [x] Teknoloji ve mimari kararları kaydedildi.
- [x] Ortam ve güvenlik yaklaşımı belirlendi.
- [x] İlk geliştirme görevleri bağımlılık sırasına kondu.
- [x] Açık kararlar ve geçici varsayımlar kaydedildi.

## Sonraki adım

Sıradaki çalışma **Aşama 1 — Proje iskeleti ve veri tabanı**dır. İlk teslim; çözüm yapısı, MVC uygulaması, veri modeli, ilk migration, geliştirme ayarları ve örnek başlangıç verisinden oluşacaktır.
