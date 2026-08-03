# Aşama 4 — Medya Yönetimi Özeti

## Durum

**Tamamlandı — 4 Ağustos 2026**

## Tamamlanan özellikler

- Yönetim panelinde medya kütüphanesi
- JPEG, PNG ve WebP görsel yükleme
- MP4 video yükleme
- Görsel için 10 MB, video için 200 MB varsayılan sınır
- Dosya uzantısı, istemci MIME türü ve gerçek dosya imzasını birlikte doğrulama
- Rastgele ve tahmin edilemez fiziksel dosya adı
- Yıl/ay klasörleriyle düzenli depolama
- `wwwroot` dışında fiziksel dosya saklama
- Yetkilendirilmiş kontrollü ön izleme endpoint'i
- Video için HTTP range desteği
- Medya kullanım sayısı gösterimi
- Bir duyuruda kullanılan medyayı silme koruması
- Yükleme ve silme işlem kayıtları
- Duyuru formunda Metin/Görsel/Video içerik türü
- Duyuruya uyumlu medya dosyası bağlama
- Görsel ve video duyurusu ön izleme

## Depolama

Varsayılan fiziksel konum:

```text
src/DigitalPano.Web/App_Data/media/{yıl}/{ay}/{rastgele-ad}.{uzantı}
```

Dosyalar doğrudan web köküne konulmaz. Veritabanında yalnızca güvenli göreli yol ve dosya bilgileri tutulur. Ön izleme sırasında yol tekrar çözülür ve depolama kökü dışına çıkılmasına izin verilmez.

## Dosya doğrulama

Sunucu aşağıdakileri kontrol eder:

- Dosya boş değil
- Orijinal dosya adı geçerli ve en fazla 255 karakter
- İzin verilen uzantı
- Dosyanın sihirli baytları/başlığı
- Tarayıcının gönderdiği MIME türü ile gerçek biçimin uyumu
- Görsel veya video boyut sınırı
- Duyuru içerik türü ile seçilen medya türünün uyumu

Sadece dosya uzantısını değiştirmek yükleme için yeterli değildir.

## Desteklenen biçimler

| Tür | Uzantılar | MIME |
|---|---|---|
| JPEG | `.jpg`, `.jpeg` | `image/jpeg` |
| PNG | `.png` | `image/png` |
| WebP | `.webp` | `image/webp` |
| MP4 | `.mp4` | `video/mp4` |

MP4 kapsayıcısı doğrulanır. Televizyon uyumluluğu için videoların H.264 ile kodlanması önerilir.

## Yapılandırma

Boyutlar ve depolama yolu `appsettings.json` içindeki `MediaStorage` bölümünden değiştirilebilir:

```json
"MediaStorage": {
  "RootPath": "App_Data/media",
  "MaxImageBytes": 10485760,
  "MaxVideoBytes": 209715200
}
```

## Test sonuçları

- JPEG, PNG, WebP ve MP4 imza tanıma
- Geçerli dosyayı saklama, okuma ve silme
- Rastgele fiziksel dosya adı
- Sahte PNG dosyasını reddetme
- Boyut sınırı aşımını reddetme
- Dizin geçişi (`../`) saldırısını reddetme
- Duyuru içerik türü–medya türü uyuşmazlığını reddetme
- Önceki veri, kimlik ve duyuru yaşam döngüsü testleri
- Toplam: **25/25 test başarılı**

## Veritabanı

Mevcut `Media`, `Announcement.MediaId` ve `Announcement.ContentType` alanları yeterli olduğu için yeni migration gerekmemiştir.

## Sonraki adım

**Aşama 5 — Ekranlar ve temel pano:** ekran CRUD işlemleri, güvenli cihaz anahtarı, `/pano/{slug}` yayını, metin/görsel/video slider'ı, canlı tarih-saat ve boş yayın görünümü.
