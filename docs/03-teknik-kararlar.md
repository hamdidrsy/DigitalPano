# DigitalPano Teknik Karar Kaydı

Bu belge Aşama 0'da alınan teknik kararları ve gerekçelerini kaydeder. Bir karar değişirse eski kayıt silinmek yerine durum ve tarih eklenerek güncellenir.

## ADR-001 — Modüler tek uygulama

- **Durum:** Kabul edildi
- **Karar:** İlk sürüm modüler bir ASP.NET Core MVC uygulaması olarak geliştirilecektir.
- **Gerekçe:** Tek kurum ve sınırlı kullanıcı sayısında ayrı servisler operasyonel yük oluşturur. MVC, Identity, SignalR ve veri erişimi aynı dağıtım biriminde yeterlidir.
- **Sonuç:** Sorumluluklar klasörler ve servis sınıflarıyla ayrılacak, mikroservis kullanılmayacaktır.

## ADR-002 — .NET 8 LTS

- **Durum:** Kabul edildi
- **Karar:** Başlangıç hedef çatısı `net8.0` olacaktır.
- **Gerekçe:** Geliştirme makinesinde .NET SDK 8.0.419 kurulu ve .NET 8 LTS çizgisidir. Kurulum riski düşüktür.
- **Sonuç:** ASP.NET Core ve Entity Framework Core paketlerinin ana sürümü 8 olacaktır. Daha yeni ana sürüme geçiş ayrı çalışma olarak değerlendirilecektir.

## ADR-003 — PostgreSQL ve Entity Framework Core

- **Durum:** Kabul edildi
- **Karar:** İlişkisel veri PostgreSQL'de, erişim EF Core/Npgsql ile yönetilecektir.
- **Gerekçe:** Duyuru-ekran ilişkisi, Identity ve yayın sorguları ilişkisel modele uygundur.
- **Sonuç:** Şema değişiklikleri migration ile izlenecektir. Geliştirme veritabanı bağlantısı Aşama 1'de doğrulanacaktır.

## ADR-004 — ASP.NET Core Identity

- **Durum:** Kabul edildi
- **Karar:** Kullanıcı, parola ve oturum işlemlerinde Identity kullanılacaktır.
- **Gerekçe:** Parola saklama ve cookie güvenliğini özel çözümle geliştirmek gereksiz risk oluşturur.
- **Sonuç:** İlk sürümde `Admin` yetkisi yeterlidir; yeni roller gerektiğinde Identity üzerinden eklenebilir.

## ADR-005 — UTC tarih saklama

- **Durum:** Kabul edildi
- **Karar:** Kalıcı tarih/saat değerleri UTC saklanacak, arayüzde kurum saat dilimine çevrilecektir.
- **Gerekçe:** Sunucu konumu değiştiğinde yayın saatlerinin bozulmasını önler.
- **Sonuç:** Varsayılan kurum saat dilimi `Europe/Istanbul` olacaktır.

## ADR-006 — Ekran adresi ve cihaz anahtarı ayrımı

- **Durum:** Kabul edildi
- **Karar:** Okunabilir `Slug` ile cihazı doğrulamaya yarayan tahmin edilmesi zor `DeviceKey` farklı alanlar olacaktır.
- **Gerekçe:** Okunabilir URL parçası tek başına erişim sırrı olarak kullanılamaz.
- **Sonuç:** Kurum ağı dışında yayın yapılacaksa cihaz anahtarı zorunlu hale getirilecektir.

## ADR-007 — SignalR bildirimi, HTTP ile veri alma

- **Durum:** Kabul edildi
- **Karar:** SignalR değişiklik bildirimi gönderecek; pano güncel yayın modelini normal HTTP isteğiyle tekrar alacaktır.
- **Gerekçe:** Büyük yayın modellerini hub üzerinden taşımak yerine veri alma ve önbellek davranışını tek noktada tutar.
- **Sonuç:** SignalR kesintisinde periyodik HTTP yenilemesi yedek olarak çalışacaktır.

## ADR-008 — Ortama göre medya depolama

- **Durum:** Kabul edildi
- **Karar:** Geliştirmede yerel dosya sistemi, Render canlı ortamında Cloudflare R2 kullanılacaktır.
- **Gerekçe:** Render'ın geçici diski yüklenen görseller için kalıcı değildir; R2 düşük maliyetli ve S3 uyumludur.
- **Sonuç:** Dosya anahtarı veritabanında tutulur, depolama `IMediaStorageService` arkasından seçilir ve R2 bucket herkese kapalı kalır.

## ADR-009 — Desteklenen medya biçimleri

- **Durum:** Geçici kabul
- **Karar:** JPEG, PNG, WebP ve MP4/H.264 desteklenecektir. Video varsayılan olarak sessiz oynatılacaktır.
- **Gerekçe:** Hedef Chromium kiosk cihazlarında yaygın uyumluluk sağlar.
- **Sonuç:** Başlangıç sınırları görsel için 10 MB, video için 200 MB'dır; gerçek cihaz ve ağ testinden sonra değiştirilebilir.

## ADR-010 — Pano istemci önbelleği

- **Durum:** Kabul edildi
- **Karar:** Uygulama kabuğu Service Worker, son yayın verisi uygun tarayıcı depolaması ile saklanacaktır.
- **Gerekçe:** Ağ kesintisinde boş ekran oluşmasını engeller.
- **Sonuç:** Video önbelleği cihaz kapasitesine bağlı ayrı politika olacaktır; ilk öncelik metin ve görsellerdir.

## ADR-011 — Ortam ayrımı ve gizli bilgiler

- **Durum:** Kabul edildi
- **Karar:** Development, Test ve Production ayarları ayrılacaktır. Parola, bağlantı parolası ve API anahtarı Git'e eklenmeyecektir.
- **Gerekçe:** Güvenli ve tekrarlanabilir dağıtım gerekir.
- **Sonuç:** Geliştirmede User Secrets veya ortam değişkeni, canlıda sunucuya özel güvenli yapılandırma kullanılacaktır.

## ADR-012 — Test yaklaşımı

- **Durum:** Kabul edildi
- **Karar:** Yayın kuralları birim testi, veri ve yetki akışları entegrasyon testi, ana senaryolar uçtan uca ve gerçek cihaz testiyle doğrulanacaktır.
- **Gerekçe:** En yüksek risk zamanlama, ekran hedefleme, medya oynatma ve uzun süre açık kalan pano istemcisindedir.
- **Sonuç:** Aşama bazında test yazılacak; testler projenin sonuna bırakılmayacaktır.

## Açık teknik kararlar

- Render ücretsiz katmanının uyku davranışının gerçek TV kullanımındaki etkisi
- Open-Meteo ticari kullanım koşullarının canlıya geçiş öncesi kurum tarafından doğrulanması
- Neon ve R2 yedekleme/saklama süresi
- Pano cihazlarının marka, işletim sistemi ve tarayıcı sürümleri

## Canlı mimari güncellemesi

- **Tarih:** 7 Ağustos 2026
- **Karar:** Veritabanı PostgreSQL/Npgsql, canlı medya Cloudflare R2 ve barındırma Render Docker olarak değiştirilmiştir.
- **Durum:** PostgreSQL başlangıç migration'ı oluşturulmuş, R2 entegrasyonu ve otomatik testleri tamamlanmıştır.
- **Not:** Eski LocalDB verileri otomatik taşınmaz. Canlı Neon veritabanı ilk dağıtımda migration ile sıfırdan hazırlanır.
