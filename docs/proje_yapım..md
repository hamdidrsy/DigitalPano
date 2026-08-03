# DigitalPano Proje Yapım Planı

## 1. Projenin amacı

DigitalPano; özel eğitim kursu içerisindeki televizyonlarda duyuru, fotoğraf, video, kayan yazı, tarih-saat ve gerektiğinde acil bildirim göstermek için geliştirilecek web tabanlı bir dijital pano sistemidir.

Sistem iki temel bölümden oluşacaktır:

1. **Yönetim paneli:** Yetkili personelin içerikleri ve ekranları yönettiği bölüm.
2. **Pano ekranı:** Televizyonlarda tam ekran çalışan, yönetim seçeneği içermeyen yayın sayfası.

İlk sürüm tek bir kurum için hazırlanacaktır. Mobil uygulama, öğrenci/veli hesapları, ücretli üyelik ve çok kiracılı yapı ilk sürüme dahil edilmeyecektir.

---

## 2. Proje başarı ölçütleri

Proje aşağıdaki koşullar sağlandığında tamamlanmış kabul edilecektir:

- Yetkili kullanıcı güvenli biçimde giriş yapabilmelidir.
- Duyuru ekleme, düzenleme, silme ve ön izleme işlemleri yapılabilmelidir.
- Fotoğraf ve desteklenen video dosyaları yüklenebilmelidir.
- Bir duyuru bir veya birden fazla ekrana atanabilmelidir.
- Duyurular başlangıç ve bitiş zamanlarına göre otomatik yayımlanmalı ve kaldırılmalıdır.
- Pano ekranı içerikleri otomatik sırayla göstermelidir.
- Kayan yazı, kurum adı, logo, tarih ve saat gösterilmelidir.
- Yönetim panelindeki yayın değişiklikleri açık ekranlara SignalR ile iletilmelidir.
- Acil duyuru ilgili ekranı tam ekran kaplayabilmelidir.
- Bağlantı kısa süreli kesildiğinde pano boş veya siyah kalmamalıdır.
- Sistem hedef televizyon veya TV Box üzerinde kararlı biçimde çalışmalıdır.
- Yetkisiz kullanıcılar yönetim paneline erişememelidir.

---

## 3. Kapsam ve öncelikler

### 3.1. İlk sürümde yapılacaklar

- Yönetici girişi ve çıkışı
- Duyuru yönetimi
- Görsel ve video yönetimi
- Yayın başlangıç ve bitiş tarihleri
- İçerik gösterim süresi ve sıralaması
- Ekran yönetimi
- Duyuruyu ekranlara atama
- Tam ekran pano sayfası
- Slider
- Kayan yazı
- Kurum adı, logo ve renk ayarları
- Canlı tarih ve saat
- SignalR ile anlık yenileme
- Acil duyuru
- Ekranın son bağlantı zamanını kaydetme
- Temel çevrimdışı/yedek ekran davranışı
- İşlem ve hata kayıtları

### 3.2. İlk sürümden sonra yapılabilecekler

- Gelişmiş kullanıcı rolleri
- Ayrıntılı yayın raporları
- Sürükle-bırak ekran tasarım editörü
- Ders programı entegrasyonu
- Mobil uygulama
- Push bildirimleri
- Öğrenci ve veli hesapları
- Çok kurumlu ve ücretli üyelik sistemi
- Yapay zekâ destekli duyuru üretimi

---

## 4. Teknoloji ve mimari

### 4.1. Kullanılacak teknolojiler

- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server
- SignalR
- HTML, CSS ve JavaScript
- Bootstrap
- Service Worker ve tarayıcı önbelleği

Geliştirme başlamadan önce destek süresi devam eden bir .NET LTS sürümü seçilecek ve proje boyunca aynı ana sürüm kullanılacaktır.

### 4.2. Önerilen katmanlar

Başlangıçta gereksiz karmaşıklık oluşturmadan aşağıdaki sorumluluk ayrımı uygulanacaktır:

```text
DigitalPano.Web
├── Areas/Admin              Yönetim paneli
├── Controllers              Pano ve ortak web işlemleri
├── Hubs                     SignalR bağlantıları
├── Services                 Yayın, dosya ve ayar işlemleri
├── ViewModels               Ekrana özel veri modelleri
├── Views                    MVC görünümleri
└── wwwroot                  CSS, JavaScript ve istemci dosyaları

DigitalPano.Data
├── AppDbContext
├── Entities
├── Configurations
└── Migrations
```

Proje küçük tutulacaksa veri katmanı ayrı proje yerine aynı web projesindeki `Data` klasöründe de bulunabilir.

---

## 5. Geliştirme öncesi kararlar

Kodlamaya başlamadan önce aşağıdaki değerler kesinleştirilecektir:

- Kullanılacak televizyon ve cihaz sayısı
- Ekranların adları ve fiziksel konumları
- Desteklenen görsel biçimleri: örneğin JPEG, PNG ve WebP
- Desteklenen video biçimi: tercihen MP4/H.264
- Görsel ve video için maksimum dosya boyutları
- Toplam medya depolama kotası
- Videoların sesli veya sessiz oynatılması
- Varsayılan içerik gösterim süresi
- Aktif içerik yokken gösterilecek yedek ekran
- Kurumun adı, logosu, renkleri ve hava durumu şehri
- Pano adreslerinin yerel ağda mı, internette mi çalışacağı
- Sunucu ve SQL Server'ın kurulacağı ortam
- Veritabanı ve medya yedekleme yöntemi
- Kurumun saat dilimi

Bu kararlar `appsettings` değerleri veya kurum ayarları olarak sisteme aktarılacaktır.

---

## 6. Veri tabanı tasarımı

### 6.1. Temel tablolar

#### AppUser

ASP.NET Core Identity kullanıcısıdır. İlk sürümde en az bir yönetici hesabı bulunacaktır.

#### InstitutionSetting

- `Id`
- `InstitutionName`
- `LogoPath`
- `PrimaryColor`
- `SecondaryColor`
- `City`
- `TimeZoneId`
- `UpdatedAt`

Tek kurum kullanılacağı için kurum ve tema ayarları tek tabloda tutulabilir.

#### Screen

- `Id`
- `Name`
- `Slug`
- `DeviceKey`
- `Location`
- `IsActive`
- `LastConnectionDate`
- `CreatedAt`

`Slug`, okunabilir pano adresini; `DeviceKey` ise gerekiyorsa cihaz doğrulamasını sağlar.

#### Announcement

- `Id`
- `Title`
- `Description`
- `ContentType`
- `StartDate`
- `EndDate`
- `DisplayDuration`
- `SortOrder`
- `IsActive`
- `IsEmergency`
- `CreatedAt`
- `UpdatedAt`
- `CreatedByUserId`

#### Media

- `Id`
- `OriginalFileName`
- `StoredFileName`
- `RelativePath`
- `MimeType`
- `FileSize`
- `MediaType`
- `CreatedAt`

Bir duyurunun bir veya birden fazla medya içerebilmesi gerekmiyorsa `Announcement` tablosunda nullable `MediaId` alanı yeterlidir.

#### AnnouncementScreen

- `AnnouncementId`
- `ScreenId`

Bu ara tablo duyuru ve ekran arasında çoktan çoğa ilişki kurar.

#### TickerMessage

- `Id`
- `Text`
- `StartDate`
- `EndDate`
- `SortOrder`
- `IsActive`

Gerekirse kayan yazılar da ekranlarla çoktan çoğa ilişkilendirilir.

#### ActivityLog

- `Id`
- `UserId`
- `ActionType`
- `EntityType`
- `EntityId`
- `Description`
- `IpAddress`
- `CreatedAt`

### 6.2. Tarih kuralları

- Başlangıç tarihi bitiş tarihinden önce olmalıdır.
- Sunucuda tarihler mümkünse UTC tutulmalıdır.
- Kullanıcıya kurumun yerel saat diliminde gösterilmelidir.
- Aktif yayın koşulu başlangıç zamanı gelmiş ve bitiş zamanı geçmemiş içeriktir.
- Süresi dolan duyuru veritabanından silinmez; yalnızca yayından kalkar.

---

## 7. Aşama 0 — Proje hazırlığı

> **Durum: Tamamlandı — 3 Ağustos 2026.** Hazırlık çıktıları için [`docs/00-asama-0-ozeti.md`](docs/00-asama-0-ozeti.md) belgesine bakınız.

### Yapılacak işler

1. Gereksinimler ve kabul ölçütleri kesinleştirilir.
2. Ekranların ve yönetim panelinin basit taslakları hazırlanır.
3. Git deposu ve dal çalışma yöntemi düzenlenir.
4. Kullanılacak .NET ve SQL Server sürümleri seçilir.
5. Geliştirme, test ve canlı ortam ayarları ayrılır.
6. Hassas bilgiler kaynak koda yazılmadan kullanıcı sırları veya ortam değişkenleriyle tutulur.
7. Temel görev listesi oluşturulur.

### Tamamlanma ölçütü

- Gereksinimler onaylanmıştır.
- Pano yerleşimi ve yönetim ekranlarının akışı bellidir.
- Teknoloji sürümleri ve kurulum ortamı kararlaştırılmıştır.

---

## 8. Aşama 1 — Proje iskeleti ve veri tabanı

> **Durum: Tamamlandı — 3 Ağustos 2026.** MVC çözümü, EF Core/Identity veri modeli, `InitialCreate` migration'ı, LocalDB geliştirme veritabanı, güvenli yönetici seed mekanizması ve temel testler oluşturuldu. Ayrıntılar için [`05-asama-1-ozeti.md`](05-asama-1-ozeti.md) belgesine bakınız.

### Yapılacak işler

1. ASP.NET Core MVC çözümü oluşturulur.
2. Gerekli NuGet paketleri eklenir.
3. SQL Server bağlantısı tanımlanır.
4. `AppDbContext` oluşturulur.
5. Entity sınıfları ve ilişkileri tanımlanır.
6. Alan uzunlukları, zorunluluklar ve indeksler yapılandırılır.
7. İlk migration oluşturulur ve geliştirme veritabanına uygulanır.
8. Örnek kurum, ekran ve yönetici verileri seed edilir.
9. Ortak hata sayfası ve loglama yapılandırılır.

### Tamamlanma ölçütü

- Uygulama hatasız açılır.
- Veritabanı migration ile sıfırdan oluşturulabilir.
- Örnek yönetici ve ekran kayıtları kullanılabilir.

---

## 9. Aşama 2 — Kimlik doğrulama ve yönetim paneli

> **Durum: Tamamlandı — 3 Ağustos 2026.** Türkçe Identity giriş/çıkış akışı, korumalı Admin alanı, ortak yönetim yerleşimi, veri tabanı destekli dashboard, güvenli cookie/HSTS ayarları ve HTTP yetkilendirme testleri oluşturuldu. Ayrıntılar için [`06-asama-2-ozeti.md`](06-asama-2-ozeti.md) belgesine bakınız.

### Yapılacak işler

1. ASP.NET Core Identity kurulumu tamamlanır.
2. Giriş ve güvenli çıkış ekranları hazırlanır.
3. Yönetim alanı `[Authorize]` ile korunur.
4. İlk yönetici parolasını güvenli şekilde değiştirme yöntemi eklenir.
5. Yönetim paneli ana sayfası oluşturulur.
6. Aktif duyuru, yaklaşan yayın, süresi dolan yayın ve ekran durumu özetleri gösterilir.
7. Başarısız giriş ve önemli yönetim işlemleri kaydedilir.

### Güvenlik kontrolleri

- Güçlü parola politikası
- CSRF koruması
- Güvenli cookie ayarları
- HTTPS yönlendirmesi
- Üretimde ayrıntılı hata bilgilerinin gizlenmesi

### Tamamlanma ölçütü

- Yetkisiz kullanıcı yönetim panelini açamaz.
- Yönetici giriş yapabilir ve güvenli biçimde çıkış yapabilir.

---

## 10. Aşama 3 — Duyuru yönetimi

> **Durum: Tamamlandı — 4 Ağustos 2026.** Duyuru listeleme/filtreleme, metin duyurusu CRUD işlemleri, ekran atama, İstanbul saati–UTC dönüşümü, yayın durumu servisi, ön izleme, silme onayı, işlem kayıtları ve testler oluşturuldu. Ayrıntılar için [`07-asama-3-ozeti.md`](07-asama-3-ozeti.md) belgesine bakınız.

### Yapılacak işler

1. Duyuru listeleme sayfası hazırlanır.
2. Yeni duyuru formu oluşturulur.
3. Duyuru düzenleme ve silme işlemleri eklenir.
4. Başlık, açıklama, yayın tarihleri ve gösterim süresi doğrulanır.
5. Aktif/pasif durumu yönetilir.
6. Duyurunun gösterileceği ekranlar seçilir.
7. Manuel içerik sıralaması eklenir.
8. Duyuru ön izleme özelliği hazırlanır.
9. Süresi dolmuş ve planlanmış duyurular liste üzerinde ayırt edilir.
10. Oluşturma, düzenleme ve silme işlemleri `ActivityLog` tablosuna yazılır.

### Tamamlanma ölçütü

- Yönetici duyurunun tüm yaşam döngüsünü yönetebilir.
- Geçersiz tarih aralıkları kaydedilemez.
- Duyuru seçilen ekranlarla doğru ilişkilendirilir.

---

## 11. Aşama 4 — Medya yönetimi

> **Durum: Tamamlandı — 4 Ağustos 2026.** Güvenli yerel depolama, dosya imzası/MIME/uzantı/boyut doğrulaması, JPEG-PNG-WebP-MP4 yükleme, kontrollü ön izleme, kullanımda olan medyayı silme koruması, duyuruya medya bağlama ve testler oluşturuldu. Ayrıntılar için [`08-asama-4-ozeti.md`](08-asama-4-ozeti.md) belgesine bakınız.

### Yapılacak işler

1. Görsel yükleme desteği eklenir.
2. MP4 video yükleme desteği eklenir.
3. Dosya uzantısı tek başına güvenilir kabul edilmeden içerik türü kontrol edilir.
4. Maksimum dosya boyutları uygulanır.
5. Dosyalar rastgele üretilen güvenli adlarla saklanır.
6. Yüklenen dosya web kökünde çalıştırılabilir içerik olarak değerlendirilmez.
7. Görsel ve video ön izlemesi hazırlanır.
8. Kullanılmayan medya dosyalarını temizlemek için kontrollü yöntem oluşturulur.
9. Büyük videolar için akış tabanlı yükleme ve uygun sunucu sınırları değerlendirilir.

### Tamamlanma ölçütü

- Desteklenen dosyalar yüklenip duyuruya bağlanabilir.
- Desteklenmeyen veya sınırı aşan dosyalar anlaşılır hata mesajıyla reddedilir.
- Yüklenen medya pano ekranında doğru oynatılır.

---

## 12. Aşama 5 — Ekran yönetimi

### Yapılacak işler

1. Ekran ekleme, düzenleme, etkinleştirme ve devre dışı bırakma sayfaları hazırlanır.
2. Her ekran için benzersiz `Slug` üretilir.
3. Gerekirse tahmin edilmesi zor `DeviceKey` oluşturulur.
4. Ekranın pano adresi yönetim panelinde gösterilir.
5. Son bağlantı zamanı kaydedilir.
6. Çevrimiçi, yakın zamanda görülmüş ve çevrimdışı durumları tanımlanır.
7. Devre dışı ekranların yayın adresine erişim davranışı belirlenir.

Örnek pano adresi:

```text
/pano/giris-kati
```

### Tamamlanma ölçütü

- Her televizyon kendisine atanmış ekran adresini açabilir.
- Her ekran yalnızca kendisine atanmış içerikleri alır.
- Son bağlantı bilgisi yönetim panelinden görülebilir.

---

## 13. Aşama 6 — Pano ekranı ve slider

### Yapılacak işler

1. Menü ve yönetim bağlantısı içermeyen tam ekran pano görünümü hazırlanır.
2. Üst alanda logo, kurum adı, tarih ve saat gösterilir.
3. Ana içerik alanında metin, görsel ve video desteklenir.
4. İçerikler sıralama ve gösterim sürelerine göre döndürülür.
5. Video tamamlanınca sonraki içeriğe geçiş yapılır.
6. Görseller ekran oranı bozulmadan gösterilir.
7. Alt alanda kayan yazı gösterilir.
8. Sağ alan için hava durumu ve kategori bölümleri hazırlanır.
9. Aktif içerik yoksa kurumun yedek karşılama ekranı gösterilir.
10. Tarayıcı tam ekran/kiosk kullanımına uygun CSS uygulanır.
11. Uzun süre açık kalan sayfada bellek sızıntısı ve zamanlayıcı birikmesi önlenir.

### Yayın sorgusu

Pano yalnızca aşağıdaki koşulları sağlayan duyuruları alır:

```text
IsActive = true
StartDate <= şu an
EndDate >= şu an
İlgili ekrana atanmış
```

### Tamamlanma ölçütü

- Pano hedef çözünürlüklerde taşma olmadan görünür.
- Metin, görsel ve video içerikleri kesintisiz sırayla gösterilir.
- Tarihi gelmeyen veya süresi dolan içerik görünmez.

---

## 14. Aşama 7 — Kayan yazı ve kurum ayarları

### Yapılacak işler

1. Kayan yazı ekleme, düzenleme ve silme ekranları hazırlanır.
2. Kayan yazılara başlangıç ve bitiş tarihi eklenir.
3. Birden fazla mesajın sıralı gösterimi sağlanır.
4. Kurum adı ve logo ayarları eklenir.
5. Ana ve ikincil tema renkleri yönetilebilir yapılır.
6. Renk değerleri güvenli formatta doğrulanır.
7. Ayarlar pano ön izlemesinde gösterilir.
8. Ayar değişiklikleri açık panolara anlık iletilir.

### Tamamlanma ölçütü

- Yönetici kod değiştirmeden pano markalamasını düzenleyebilir.
- Aktif kayan yazılar doğru zaman aralığında gösterilir.

---

## 15. Aşama 8 — SignalR ile gerçek zamanlı yayın

### Yapılacak işler

1. Bir SignalR Hub oluşturulur.
2. Her pano kendi ekran kimliğiyle uygun SignalR grubuna katılır.
3. Duyuru veya ayar değiştiğinde yalnızca etkilenen ekranlara yenileme mesajı gönderilir.
4. Pano mesaj aldığında yayın verisini tekrar yükler.
5. Bağlantı kesildiğinde otomatik yeniden bağlanma uygulanır.
6. SignalR çalışmasa bile belirli aralıklarla yedek veri yenilemesi yapılır.
7. Bağlantı durumu yönetim paneline aktarılır.

### Tamamlanma ölçütü

- Yönetici yayın değişikliği yaptığında televizyon sayfasını elle yenilemeye gerek kalmaz.
- Geçici ağ kesintisinden sonra bağlantı otomatik kurulur.

---

## 16. Aşama 9 — Acil duyuru

### Davranış kuralları

- Aynı anda tek bir aktif acil duyuru bulunması tercih edilir.
- Acil duyuru atandığı ekranı tamamen kaplar.
- Normal slider ve kayan yazı geçici olarak durur.
- Acil duyuru başlık, açıklama ve isteğe bağlı görsel içerir.
- Yönetici duyuruyu elle kapatabilir.
- İstenirse başlangıç ve bitiş zamanı kullanılır.
- Acil duyuru kapanınca normal yayın kaldığı yerden veya baştan devam eder.

### Yapılacak işler

1. Acil duyuru oluşturma ve sonlandırma ekranı hazırlanır.
2. Yanlışlıkla yayınlamayı önlemek için onay adımı eklenir.
3. SignalR ile hedef ekranlara anında bildirim gönderilir.
4. Pano üzerinde yüksek okunabilirlikte tam ekran tasarım hazırlanır.
5. Acil yayının kim tarafından ve ne zaman açılıp kapatıldığı kaydedilir.

### Tamamlanma ölçütü

- Acil duyuru hedef ekranlarda birkaç saniye içinde görünür.
- Kapatıldığında normal yayın güvenli biçimde geri gelir.

---

## 17. Aşama 10 — Hava durumu ve yardımcı alanlar

### Yapılacak işler

1. Hava durumu veri kaynağı seçilir.
2. API anahtarı kaynak kod dışında saklanır.
3. Kurum ayarındaki şehir için veri alınır.
4. Sonuç sunucu tarafında önbelleğe alınır; her ekran ayrı API isteği göndermez.
5. Servis çalışmazsa son başarılı veri veya sade bir yedek görünüm kullanılır.
6. Etkinlik, yemek ve nöbet alanlarının bağımsız modül mü yoksa duyuru kategorisi mi olacağı kesinleştirilir.

İlk sürüm için etkinlik, yemek ve nöbet bilgilerinin duyuru kategorileri olarak yönetilmesi yeterlidir.

### Tamamlanma ölçütü

- Harici servis kesintisi ana pano yayınını durdurmaz.
- Hava durumu belirlenen şehir için kontrollü sıklıkta güncellenir.

---

## 18. Aşama 11 — Çevrimdışı çalışma

### Yapılacak işler

1. Uygulama kabuğu Service Worker ile önbelleğe alınır.
2. Son başarılı yayın verisi tarayıcıda saklanır.
3. Gerekli görseller kontrollü biçimde önbelleğe alınır.
4. Büyük videolar için cihaz kapasitesine uygun ayrı politika uygulanır.
5. Çevrimdışıyken tarih kuralları istemci tarafında uygulanmaya devam eder.
6. Bağlantı geri geldiğinde yeni yayın alınır ve eski önbellek temizlenir.
7. İlk kez açılan ve hiç önbelleği olmayan cihaz için anlaşılır bağlantı ekranı hazırlanır.

### Tamamlanma ölçütü

- Kısa süreli internet veya ağ kesintisinde ekran siyah kalmaz.
- Son indirilen uygun içerikler gösterilmeye devam eder.
- Bağlantı geldiğinde pano kullanıcı müdahalesi olmadan güncellenir.

---

## 19. Aşama 12 — Test süreci

### 19.1. Birim testleri

- Aktif yayın tarih filtresi
- Ekrana göre içerik seçimi
- Tarih doğrulaması
- Acil duyuru önceliği
- Dosya türü ve boyutu doğrulaması
- İçerik sıralama kuralları

### 19.2. Entegrasyon testleri

- Kullanıcı girişi ve yetkilendirme
- Duyuru CRUD işlemleri
- Entity Framework ilişkileri
- Dosya yükleme
- SignalR grup mesajları
- Ayarların pano ekranına yansıması

### 19.3. Uçtan uca testler

- Yönetici duyuru oluşturur ve ilgili televizyonda görür.
- Planlanmış duyuru başlangıç zamanında görünür.
- Duyuru bitiş zamanında yayından kalkar.
- Video bittikten sonra sonraki içerik açılır.
- Acil duyuru normal yayını kaplar ve sonra kaldırılır.
- Ağ kesilip geri geldiğinde yayın devam eder.

### 19.4. Cihaz testleri

- Hedef televizyon tarayıcısı
- Android TV Box veya mini bilgisayar
- 1920×1080 çözünürlük
- Varsa 4K çözünürlük
- Uzun süreli, en az 24–72 saat açık kalma testi
- Cihaz yeniden başladıktan sonra otomatik kiosk açılışı

### 19.5. Güvenlik testleri

- Yetkisiz yönetim erişimi
- Dosya yükleme saldırıları
- XSS girişleri
- CSRF koruması
- Tahmin edilebilir ekran anahtarları
- Hatalarda hassas bilgi sızıntısı

### Tamamlanma ölçütü

- Kritik ve yüksek öncelikli hata kalmamıştır.
- Ana kullanıcı senaryoları hedef cihazda başarıyla tamamlanmıştır.

---

## 20. Aşama 13 — Canlı ortam hazırlığı

### Yapılacak işler

1. Sunucu işletim sistemi ve barındırma yöntemi hazırlanır.
2. IIS veya seçilen ters proxy yapılandırılır.
3. SQL Server canlı veritabanı oluşturulur.
4. Canlı bağlantı bilgileri güvenli şekilde tanımlanır.
5. HTTPS sertifikası kurulur.
6. Uygulama yayın klasörü ve medya depolama izinleri ayarlanır.
7. Veritabanı migration işlemi kontrollü biçimde uygulanır.
8. İlk yönetici hesabı oluşturulur ve varsayılan parola değiştirilir.
9. Veritabanı ve medya yedekleme planı kurulur.
10. Log saklama ve disk doluluk takibi hazırlanır.
11. Uygulama sağlığı için basit health check eklenir.
12. Geri dönüş planı hazırlanır.

### Tamamlanma ölçütü

- Canlı uygulama HTTPS üzerinden erişilebilir.
- Yönetim ve pano adresleri çalışır.
- Yedek alma ve geri yükleme yöntemi test edilmiştir.

---

## 21. Aşama 14 — Televizyon kurulumu

### Önerilen cihaz sırası

1. Android TV Box
2. Mini bilgisayar
3. Uyumluysa akıllı televizyon tarayıcısı

### Yapılacak işler

1. Her cihaz için ekran kaydı oluşturulur.
2. İlgili pano adresi cihazda açılır.
3. Tarayıcı kiosk veya tam ekran modunda yapılandırılır.
4. Cihaz açıldığında pano sayfasının otomatik başlaması sağlanır.
5. Ekran koruyucu, uyku modu ve otomatik kapanma devre dışı bırakılır.
6. Ağ kesintisi ve elektrik kesintisi sonrası otomatik geri dönüş test edilir.
7. Video kodek desteği kontrol edilir.
8. Cihaza erişim ve bakım bilgileri güvenli şekilde kayıt altına alınır.

### Tamamlanma ölçütü

- Cihaz yeniden başlatıldığında insan müdahalesi olmadan pano açılır.
- Ekran doğru içerikleri tam ekran gösterir.

---

## 22. Aşama 15 — Kullanıcı kabulü ve yayına geçiş

### Yapılacak işler

1. Gerçek kurum logosu ve renkleri yüklenir.
2. Gerçek ekranlar ve örnek duyurular tanımlanır.
3. Yetkili personele kısa kullanım eğitimi verilir.
4. Duyuru oluşturma, planlama ve acil yayın senaryoları birlikte denenir.
5. Kullanıcı kabul formu veya kontrol listesi tamamlanır.
6. Test içerikleri temizlenir.
7. İlk canlı yayın başlatılır.
8. İlk gün ve ilk hafta sistem yakından takip edilir.

### Kullanıcıya verilecek bilgiler

- Giriş adresi
- Pano adresleri
- Duyuru oluşturma adımları
- Desteklenen medya biçimleri ve boyutları
- Acil duyuru açma ve kapatma işlemi
- Sorun durumunda iletişim ve müdahale yöntemi

### Tamamlanma ölçütü

- Yetkili personel temel işlemleri yardım almadan yapabilir.
- Tüm ekranlar canlı yayını kararlı biçimde gösterir.
- Kurum proje kabulünü tamamlamıştır.

---

## 23. Bakım ve işletme planı

Proje canlıya alındıktan sonra aşağıdaki işler düzenli yapılacaktır:

### Günlük veya otomatik

- Uygulamanın erişilebilirliğini kontrol etme
- Hata kayıtlarını izleme
- Ekranların son bağlantı durumunu izleme
- Veritabanı yedeği alma

### Haftalık

- Yedeklerin başarılı olduğunu kontrol etme
- Kullanılmayan medya ve disk alanını inceleme
- Süresi dolmuş içerikleri gözden geçirme

### Aylık

- İşletim sistemi ve uygulama güncellemelerini değerlendirme
- Güvenlik kayıtlarını inceleme
- Geri yükleme denemesi yapma
- Yönetici hesaplarını ve yetkileri kontrol etme

### Sürüm güncellemelerinde

1. Değişiklik test ortamında doğrulanır.
2. Veritabanı yedeği alınır.
3. Migration etkileri kontrol edilir.
4. Canlı yayın düşük kullanım zamanında güncellenir.
5. Yönetim paneli ve pano için hızlı sağlık testi yapılır.
6. Sorun çıkarsa önceki sürüme dönüş planı uygulanır.

---

## 24. Riskler ve önlemler

| Risk | Etki | Önlem |
|---|---|---|
| Televizyon tarayıcısının eski olması | Video veya SignalR çalışmayabilir | Android TV Box ya da mini bilgisayar kullanmak |
| Büyük video dosyaları | Disk ve ağ tüketimi artar | Format, boyut ve toplam kota sınırı koymak |
| İnternet veya yerel ağ kesintisi | Pano güncellenemez | Son içeriği önbellekte tutmak ve otomatik yeniden bağlanmak |
| Elektrik kesintisi | Cihaz kapanır | Otomatik açılış ve kiosk başlangıcı yapılandırmak |
| Yanlış acil duyuru | Tüm yayını keser | Onay adımı ve işlem kaydı eklemek |
| Saat farkı | İçerik yanlış zamanda yayımlanır | UTC saklama ve kurum saat dilimini açıkça tanımlamak |
| Zararlı dosya yükleme | Güvenlik sorunu oluşur | Tür, içerik ve boyut doğrulaması yapmak |
| Veritabanı veya medya kaybı | Yayın ve geçmiş kaybolur | Düzenli yedek ve geri yükleme testi yapmak |
| SignalR bağlantısının kopması | Anlık yenileme çalışmaz | Otomatik yeniden bağlanma ve periyodik yedek sorgu kullanmak |
| Diskin dolması | Yükleme ve uygulama durabilir | Kota, izleme ve kontrollü medya temizliği uygulamak |

---

## 25. Önerilen geliştirme sırası

Bağımlılıklar dikkate alınarak uygulama şu sırayla geliştirilecektir:

1. Gereksinimleri ve ekran taslaklarını kesinleştirme
2. Proje iskeleti ve veritabanı
3. Kimlik doğrulama
4. Duyuru CRUD işlemleri
5. Ekran yönetimi ve duyuru-ekran ilişkisi
6. Temel pano ekranı ve tarih filtreleme
7. Görsel/video yükleme
8. Slider ve gösterim süreleri
9. Kayan yazı ve kurum ayarları
10. SignalR ile anlık güncelleme
11. Acil duyuru
12. Ekran bağlantı takibi
13. Hava durumu ve yardımcı bölümler
14. Çevrimdışı çalışma
15. Güvenlik, performans ve uzun süreli cihaz testleri
16. Canlı sunucu ve televizyon kurulumu
17. Kullanıcı eğitimi ve kabul
18. Yayına geçiş ve bakım

---

## 26. Proje bitiş kontrol listesi

### Fonksiyonlar

- [ ] Yönetici giriş ve çıkışı çalışıyor.
- [ ] Duyuru ekleme, düzenleme ve silme çalışıyor.
- [ ] Görsel ve video yükleme güvenli çalışıyor.
- [ ] Duyurular ekranlara atanabiliyor.
- [ ] Başlangıç ve bitiş zamanları doğru uygulanıyor.
- [ ] Slider metin, görsel ve videoyu oynatıyor.
- [ ] Kayan yazı çalışıyor.
- [ ] Kurum adı, logo ve renkler yönetilebiliyor.
- [ ] Tarih ve saat doğru gösteriliyor.
- [ ] SignalR değişiklikleri ekranlara iletiyor.
- [ ] Acil duyuru tam ekran çalışıyor.
- [ ] Ekran bağlantı durumu izlenebiliyor.
- [ ] Bağlantı kesildiğinde yedek içerik gösteriliyor.

### Kalite ve güvenlik

- [ ] Tüm yönetim sayfaları yetkilendirilmiş.
- [ ] Dosya türü ve boyut sınırlamaları uygulanmış.
- [ ] Kritik işlemler kayıt altına alınıyor.
- [ ] Hata mesajları hassas bilgi içermiyor.
- [ ] Otomatik ve manuel testler tamamlanmış.
- [ ] Hedef cihazda uzun süreli yayın testi yapılmış.
- [ ] HTTPS etkin.

### Canlı kullanım

- [ ] Canlı veritabanı hazırlanmış.
- [ ] Veritabanı ve medya yedekleri çalışıyor.
- [ ] Geri yükleme yöntemi test edilmiş.
- [ ] Televizyonlar otomatik kiosk modunda açılıyor.
- [ ] Yetkili kullanıcı eğitilmiş.
- [ ] Kullanım ve bakım bilgileri teslim edilmiş.
- [ ] İlk canlı yayın başarıyla tamamlanmış.

Bu kontrol listesindeki zorunlu maddeler tamamlandığında DigitalPano ilk üretim sürümü bitmiş ve kurumda kullanıma hazır kabul edilecektir.
