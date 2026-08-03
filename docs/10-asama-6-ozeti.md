# Aşama 6 — Kayan Yazı ve Kurum Ayarları Özeti

## Durum

**Tamamlandı — 4 Ağustos 2026**

## Kayan yazı yönetimi

- Kayan yazı listeleme, ekleme, düzenleme ve silme
- En fazla 1000 karakterlik mesaj
- İstanbul saatine göre başlangıç ve bitiş planlama
- Yayın sırası
- Aktif/pasif kontrolü
- Yayında, planlanmış, süresi dolmuş ve pasif durumları
- Oluşturma, güncelleme ve silme işlem kayıtları

Pano yalnızca aktif, başlangıç zamanı gelmiş ve bitiş zamanı geçmemiş mesajları gösterir. Birden fazla mesaj sıra değerine göre birleştirilir.

## Pano alt bandı

- Alt bölümde sabit “DUYURU” etiketi
- Kesintisiz yatay kayan mesaj animasyonu
- Birden fazla mesaj arasında ayırıcı
- Kayan yazı bulunmadığında kurum adının gösterilmesi
- Tema ana renginin alt bant etiketine uygulanması

## Kurum ayarları

- Kurum adı
- Şehir
- Kurum logosu
- Ana tema rengi
- İkincil tema rengi
- Pano tema ön izlemesi

Renkler yalnızca `#RRGGBB` biçiminde kabul edilir ve veritabanına büyük harfle kaydedilir. Saat dilimi `Europe/Istanbul` olarak korunur.

## Logo güvenliği

- Logo yalnızca medya kütüphanesindeki görsellerden seçilebilir.
- Video dosyası logo olarak kullanılamaz.
- Logo dosyası cihaz anahtarlı pano medya endpoint'inden sunulur.
- Kurum logosu olarak kullanılan medya silinemez.
- Logo kaldırıldığında medya dosyası tekrar silinebilir hale gelir.

## Pano entegrasyonu

- Seçilen logo pano başlığında gösterilir.
- Logo yoksa `DP` yedek işareti kullanılır.
- Kurum adı başlık, sayfa adı ve alt bantta kullanılır.
- Ana ve ikincil renkler CSS değişkenleri üzerinden panoya uygulanır.
- Aktif kayan yazılar her pano açılışında tarih filtresiyle alınır.

## Test sonuçları

- Kayan yazı oluşturma, düzenleme ve silme yaşam döngüsü
- Yerel tarihlerin UTC saklanması
- Aktif kayan yazının panoya dahil edilmesi
- Süresi dolmuş mesajın dışlanması
- Kurum adı, şehir ve renk kaydı
- Görsel logo ilişkilendirme
- Video dosyasının logo olarak reddedilmesi
- Önceki kimlik, duyuru, medya, ekran ve pano testleri
- Toplam: **36/36 test başarılı**

## Veritabanı

Mevcut `TickerMessage`, `InstitutionSetting.LogoPath`, renk ve şehir alanları yeterli olduğu için yeni migration gerekmemiştir.

## Kullanım adresleri

```text
/Admin/TickerMessages
/Admin/Settings
```

Yeni değişiklikleri görmek için çalışan uygulamayı `Ctrl+C` ile durdurup yeniden başlatın:

```powershell
dotnet run --project src\DigitalPano.Web --launch-profile https
```

## Sonraki adım

**Aşama 7 — Gerçek zamanlı çalışma:** SignalR hub, ekran grupları, yönetim değişikliklerinin panoya anında iletilmesi, otomatik yeniden bağlantı ve periyodik HTTP yenileme yedeği.
