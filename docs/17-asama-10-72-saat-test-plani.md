# Aşama 10 — 72 saat hedef cihaz dayanıklılık testi planı

**Hazırlık tarihi:** 5 Ağustos 2026  
**Durum:** Test altyapısı hazır; fiziksel hedef cihaz bekleniyor

## Neden görev henüz tamamlanmadı?

Bu test gerçek hedef cihazın pano sayfasını kesintisiz 72 saat açık tutmasını gerektirir. Beko TV'de yerleşik web tarayıcısı bulunmadığı ve kullanılacak TV Box, stick veya mini bilgisayar henüz belirlenmediği için süre başlatılmamıştır. Kısa bilgisayar testi fiziksel hedef cihaz kabulünün yerine geçmez.

## Ön koşullar

1. TV'ye bağlanacak Android TV Box, Google TV/Fire TV stick veya mini bilgisayar belirlenir.
2. Cihaz kablolu ağ ya da kararlı Wi-Fi bağlantısına alınır.
3. Uyku, ekran koruyucu ve otomatik kapanma kapatılır.
4. Kiosk tarayıcı pano adresini cihaz açılışında otomatik açar.
5. Sunucu bilgisayarın test boyunca kapanmaması sağlanır.
6. Cihaza atanmış tam pano URL'si yönetim panelindeki Ekranlar bölümünden alınır.

## Otomatik ölçüm

İzleme betiği dakikada bir pano URL'sini çağırır, HTTP durumunu, pano kök öğesini, yanıt süresini ve ardışık hata sayısını CSV dosyasına yazar.

```powershell
powershell -ExecutionPolicy Bypass -File scripts\Monitor-DigitalPano72h.ps1 `
  -PanoUrl "http://SUNUCU-IP:5230/pano/EKRAN?key=CIHAZ-ANAHTARI"
```

Çıktılar `artifacts/endurance` klasöründe CSV ve JSON özet olarak oluşur.

## Kabul ölçütleri

- Tam 72 saat tamamlanmalıdır.
- Yaklaşık 4.320 dakikalık örneğin en az %99,5'i başarılı olmalıdır.
- Beş dakikadan uzun ardışık erişim kesintisi olmamalıdır.
- Başarılı isteklerin p95 yanıt süresi 2.000 ms veya altında olmalıdır.
- Pano siyah/beyaz ekranda kalmamalıdır.
- Slider, görsel/video geçişi, saat, hava durumu ve kayan yazı çalışmaya devam etmelidir.
- SignalR değişikliği hedef ekranda en geç birkaç saniye içinde görünmelidir.
- Cihaz uykuya geçmemeli, tarayıcı kapanmamalı ve bellek yetersizliği uyarısı vermemelidir.

## Elle kontrol zamanları

| Saat | Kontrol |
|---|---|
| 0 | Başlangıç fotoğrafı, tam ekran, saat, medya ve ağ durumu |
| 24 | Görsel kontrol, bir duyuru değişikliği, cihaz sıcaklığı |
| 48 | Görsel kontrol, acil yayın aç/kapat, çevrimdışı/geri dönüş |
| 72 | Final fotoğrafı, slider ve video, log/CSV değerlendirmesi |

Her kontrolde saat, gözlemci, ekran fotoğrafı, yapılan işlem ve sonuç kayıt altına alınmalıdır.

## Tamamlanma kararı

Otomatik JSON özetinde `AcceptancePassed: true` bulunmalı ve dört fiziksel kontrol kaydı başarılı olmalıdır. Ancak bundan sonra görev listesindeki 72 saat maddesi tamamlandı olarak işaretlenebilir.
