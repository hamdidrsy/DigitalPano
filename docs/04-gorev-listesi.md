# DigitalPano Geliştirme Görev Listesi

## Çalışma yöntemi

- Görevler bağımlılık sırasıyla yapılır.
- Her görev kod, doğrulama ve gerekli belge güncellemesini birlikte içerir.
- Bir özellik, kabul kriteri doğrulanmadan tamamlanmış sayılmaz.
- Gizli bilgi veya gerçek parola Git'e eklenmez.
- Her aşamanın sonunda uygulama derlenir ve ilgili testler çalıştırılır.

## Aşama 1 — Proje iskeleti ve veri tabanı

- [x] `DigitalPano.sln` çözümünü oluştur.
- [x] ASP.NET Core MVC web projesini `net8.0` hedefiyle oluştur.
- [x] Test projesini oluştur ve çözüme ekle.
- [x] `.gitignore`, `.editorconfig`, `global.json` ve temel README dosyasını ekle.
- [x] Gerekli EF Core SQL Server, Identity ve geliştirme paketlerini ekle.
- [x] Development/Test/Production yapılandırma yaklaşımını kur.
- [x] Yerel SQL Server bağlantısını doğrula veya geliştirme alternatifi belirle.
- [x] `AppDbContext` ve Identity kullanıcı modelini oluştur.
- [x] `InstitutionSetting`, `Screen`, `Announcement`, `Media`, `AnnouncementScreen`, `TickerMessage` ve `ActivityLog` entity'lerini oluştur.
- [x] İlişkileri, alan sınırlarını ve indeksleri Fluent API ile tanımla.
- [x] İlk migration'ı oluştur ve boş veritabanına uygula.
- [x] Geliştirme için kurum, `Giriş Katı` ekranı ve kontrollü yönetici seed yöntemi ekle.
- [x] Temel veri modeli ve yapılandırma testlerini yaz.

## Aşama 2 — Kimlik ve yönetim kabuğu

- [x] Türkçe giriş ve çıkış akışını oluştur.
- [x] Admin alanını yetkilendirme ile koru.
- [x] Yönetim paneli ortak yerleşimini ve menüsünü oluştur.
- [x] Gösterge paneli özet kartlarını ekle.
- [x] Güvenli cookie, HTTPS ve üretim hata ayarlarını yapılandır.
- [x] Yetkili/yetkisiz erişim entegrasyon testlerini yaz.

## Aşama 3 — Duyuru yönetimi

- [x] Duyuru listeleme ve filtreleme sayfasını oluştur.
- [x] Ekleme, düzenleme, ön izleme ve silme işlemlerini oluştur.
- [x] Tarih, süre ve zorunlu alan doğrulamalarını ekle.
- [x] Duyuru-ekran atama arayüzünü oluştur.
- [x] Yayın durumunu hesaplayan servis yaz.
- [x] Ön izleme görünümünü oluştur.
- [x] İşlem kayıtlarını ekle.
- [x] CRUD ve yayın kuralı testlerini yaz.

## Aşama 4 — Medya

- [x] Dosya depolama servisi ve arayüzünü oluştur.
- [x] Güvenli görsel yükleme ve doğrulama ekle.
- [x] Güvenli MP4 yükleme ve doğrulama ekle.
- [x] Medya ön izleme ve silme kurallarını oluştur.
- [x] Boyut ve tür doğrulama testlerini yaz.

## Aşama 5 — Ekranlar ve temel pano

- [x] Ekran CRUD ve benzersiz slug üretimini oluştur.
- [x] DeviceKey üretimini ekle.
- [x] `/pano/{slug}` rotasını ve yayın sorgusunu oluştur.
- [x] Temel 16:9 pano yerleşimini uygula.
- [x] Metin, görsel ve video göstericisini oluştur.
- [x] Slider zamanlamasını ve video bitiş geçişini uygula.
- [x] Canlı tarih ve saat ekle.
- [x] Boş yayın yedek görünümünü ekle.
- [x] Ekrana özel yayın testlerini yaz.

## Aşama 6 — Kayan yazı ve kurum ayarları

- [x] Kayan yazı CRUD ve tarih filtrelemesini oluştur.
- [x] Alt bant animasyonunu uygula.
- [x] Kurum adı, logo ve renk ayarlarını oluştur.
- [x] Tema değerlerini pano CSS değişkenlerine bağla.
- [x] Ayar ve kayan yazı testlerini yaz.

## Aşama 7 — Gerçek zamanlı çalışma

- [x] Pano SignalR Hub'ını oluştur.
- [x] Ekran gruplarına katılma kuralını uygula.
- [x] Yönetim değişikliklerinden bildirim gönder.
- [x] Pano otomatik yeniden bağlantısını uygula.
- [x] Periyodik HTTP yenileme yedeğini ekle.
- [x] Son bağlantı ve çevrimiçi durum hesabını ekle.
- [x] SignalR entegrasyon testlerini yaz.

## Aşama 8 — Acil duyuru

- [x] Acil duyuru oluşturma ve onay akışını ekle.
- [x] Ekran başına tek aktif acil yayın kuralını uygula.
- [x] Tam ekran acil yayın görünümünü oluştur.
- [x] Başlatma/sonlandırma SignalR bildirimlerini ekle.
- [x] Normal yayına güvenli dönüşü uygula.
- [x] Acil yayın işlem kayıtlarını ve testlerini ekle.

## Aşama 9 — Yardımcı alanlar ve çevrimdışı çalışma

- [x] Hava durumu sağlayıcısını seç ve sunucu önbelleğini uygula.
- [x] Sağ panel kategori gösterimini oluştur.
- [x] Uygulama kabuğu Service Worker önbelleğini ekle.
- [x] Son yayın verisini istemcide sakla.
- [x] Bağlantı geri geldiğinde eşitleme davranışını uygula.
- [x] Çevrimdışı ve servis kesintisi testlerini yap.

## Aşama 10 — Kalite ve canlıya geçiş

- [x] Birim, entegrasyon ve uçtan uca test paketini tamamla.
- [x] XSS, CSRF, dosya yükleme ve yetki kontrollerini doğrula.
- [x] 1920×1080 ve varsa 4K görünüm testini yap.
- [ ] Hedef cihazda 72 saat açık kalma testi yap.
- [x] Render/HTTPS/Neon PostgreSQL/Cloudflare R2 canlı kurulumunu hazırla.
- [ ] Veritabanı ve medya yedekleme/geri yükleme testini yap.
- [x] Kiosk otomatik başlangıcını yapılandır.
- [ ] Gerçek kurum verilerini gir.
- [ ] Kullanıcı eğitimini ve kabul senaryolarını tamamla.
- [ ] İlk canlı yayını başlat ve ilk hafta izleme yap.

## Aşama 0 kapanış kaydı

- [x] Ürün kapsamı netleştirildi.
- [x] Gereksinim kimlikleri oluşturuldu.
- [x] Kabul senaryoları yazıldı.
- [x] Ekranlar ve kullanıcı akışları çıkarıldı.
- [x] Teknik kararlar kaydedildi.
- [x] Geliştirme görevleri sıralandı.
- [x] Açık kararlar ve varsayımlar belgelendi.
