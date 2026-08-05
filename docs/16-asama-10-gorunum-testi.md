# Aşama 10 — 1920×1080 ve 4K görünüm testi

**Test tarihi:** 5 Ağustos 2026  
**Tarayıcı motoru:** Chromium tabanlı Google Chrome / Microsoft Edge headless  
**Test edilen görünüm:** Gerçek geliştirme verisiyle normal pano yayını

## Üretilen test görüntüleri

- `artifacts/visual-tests/pano-1920x1080.png` — doğrulanan boyut: 1920×1080
- `artifacts/visual-tests/pano-3840x2160.png` — doğrulanan boyut: 3840×2160

## Kontrol sonuçları

| Kontrol | 1920×1080 | 3840×2160 |
|---|---|---|
| Pano ekranı tamamen dolduruyor | Başarılı | Başarılı |
| Yatay/dikey kaydırma veya taşma yok | Başarılı | Başarılı |
| Kurum adı, ekran adı, saat ve tarih okunuyor | Başarılı | Başarılı |
| Ana görsel oranı bozulmadan gösteriliyor | Başarılı | Başarılı |
| Duyuru başlığı ve açıklaması kesilmiyor | Başarılı | Başarılı |
| Gün, hava durumu ve yayın kartları hizalı | Başarılı | Başarılı |
| Kayan yazı alt bantta kalıyor | Başarılı | Başarılı |
| Kart kenarları veya metinler üst üste binmiyor | Başarılı | Başarılı |
| 4K ölçekleme oranı 1080p düzenini koruyor | Uygulanmaz | Başarılı |

## Teknik değerlendirme

Pano yerleşimi `100vh`, `100vw`, `vw` ve `vh` tabanlı ölçüler kullandığı için iki çözünürlükte aynı görsel oranı korudu. Ana medya `object-fit: contain` ile gösterildiğinden fotoğraf kırpılmadı veya esnetilmedi. Sağ panel üç eşit satırda kaldı; alt bant ekran dışına taşmadı.

Görsel incelemede CSS düzeltmesi gerektiren bir hata bulunmadı.

## Kapsam sınırı

Bu test Chromium tarayıcı motorunda piksel çözünürlüğünü ve yerleşimi doğrular. Gerçek Beko TV panelindeki overscan, renk profili, görüntü keskinliği ve izleme mesafesi fiziksel cihaz üzerinde ayrıca kontrol edilmelidir. TV Box kullanılacaksa cihazın görüntü çıkışı `1920×1080 60 Hz` veya destekleniyorsa `3840×2160 60 Hz` olarak ayarlanmalıdır.
