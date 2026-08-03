# Aşama 5 — Ekranlar ve Temel Pano Özeti

## Durum

**Tamamlandı — 4 Ağustos 2026**

## Ekran yönetimi

- Ekran listeleme, ekleme, düzenleme ve kontrollü silme
- Türkçe karakterleri dönüştüren URL slug üretimi
- Benzersiz slug kontrolü
- Her ekran için kriptografik olarak rastgele 256 bit cihaz anahtarı
- Cihaz anahtarını yenileme
- Aktif/pasif ekran yönetimi
- Fiziksel konum bilgisi
- Son bağlantı zamanı ve çevrimiçi/çevrimdışı durumu
- Kopyalanabilir güvenli pano adresi
- Ekran işlemleri için aktivite kayıtları

Duyuru atanmış ekran geçmiş ilişkileri korumak amacıyla silinemez; pasif yapılabilir.

## Pano adresi ve cihaz güvenliği

Pano adresi aşağıdaki yapıdadır:

```text
/pano/{slug}?key={deviceKey}
```

- Slug ekranı tanımlar.
- 64 karakterlik cihaz anahtarı erişimi doğrular.
- Geçersiz anahtar ve pasif ekran `404` döndürür.
- Anahtar karşılaştırması sabit zamanlı kriptografik karşılaştırmayla yapılır.
- Anahtar yenilenince eski adres çalışmayı bırakır.
- İlk migration'daki geliştirme anahtarı uygulama başlarken otomatik olarak güvenli anahtarla değiştirilir.

## Yayın seçme kuralları

Pano yalnızca şu duyuruları gösterir:

- Ekrana atanmış
- Aktif
- Başlangıç zamanı gelmiş
- Bitiş zamanı geçmemiş
- Acil olmayan normal yayın
- Medya türündeyse geçerli medya kaydı bulunan

Sıralama önce yönetici tarafından verilen yayın sırası, ardından duyuru kimliğiyle yapılır.

## Pano görünümü

- Televizyon için tam ekran 16:9 yerleşim
- Kurum ve ekran adı
- İstanbul saat diliminde canlı tarih, gün ve saat
- Metin duyurusu
- Ölçeği korunarak gösterilen görsel
- Sessiz otomatik başlayan MP4 video
- Metin/görsel için ayarlanabilir gösterim süresi
- Video bittiğinde sonraki içeriğe geçiş
- İçerikler arasında otomatik döngü
- Aktif yayın yoksa kurumsal yedek ekran
- Menü, giriş bağlantısı veya yönetim düğmesi bulunmayan kiosk görünümü

## Bağlantı takibi

Pano açıldığında ve her 60 saniyede bir güvenli heartbeat isteği gönderir. Başarılı istek ekranın `LastConnectionDateUtc` değerini günceller. Son iki dakika içinde bağlantı bildiren aktif ekran yönetim panelinde çevrimiçi görünür.

## Medya güvenliği

Pano medya adresi de ekran slug ve cihaz anahtarını doğrular. Ayrıca istenen medya dosyasının o anda aktif olan ve ilgili ekrana atanmış bir duyuruda kullanıldığı kontrol edilir. Video yanıtları ileri/geri sarma için HTTP range desteği sunar.

## Test sonuçları

- Türkçe ekran adından slug üretimi
- Cihaz anahtarı uzunluğu, rastgeleliği ve doğrulaması
- Ekran oluşturma, düzenleme, anahtar yenileme ve silme yaşam döngüsü
- Ekrana özel aktif yayın sorgusu
- Süresi dolmuş içeriğin dışlanması
- Başka ekrana ait içeriğin dışlanması
- Geçersiz cihaz anahtarının reddedilmesi
- Heartbeat ile son bağlantı zamanının güncellenmesi
- Önceki kimlik, duyuru ve medya güvenliği testleri
- Toplam: **33/33 test başarılı**

## Veritabanı

Mevcut `Screen`, `AnnouncementScreen` ve `LastConnectionDateUtc` alanları yeterli olduğu için yeni migration gerekmemiştir.

## Çalıştırma notu

Geliştirme sırasında uygulama zaten çalışıyorsa yeni kodu görmek için terminalde `Ctrl+C` ile durdurup tekrar başlatmak gerekir:

```powershell
dotnet run --project src\DigitalPano.Web
```

Ardından `/Admin/Screens` sayfasındaki **Aç** düğmesiyle pano görüntülenebilir.

## Sonraki adım

**Aşama 6 — Kayan yazı ve kurum ayarları:** kayan yazı CRUD, tarih filtreleme, kurum adı/logo/tema ayarları ve pano görünümüne uygulanması.
