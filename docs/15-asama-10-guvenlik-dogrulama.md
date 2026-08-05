# Aşama 10 — Güvenlik doğrulama raporu

**Doğrulama tarihi:** 5 Ağustos 2026

## Sonuç

XSS, CSRF, dosya yükleme ve yetkisiz erişim kontrolleri kaynak kod denetimi ve saldırı benzeri HTTP testleriyle doğrulandı.

## XSS

- Razor görünümlerinde `Html.Raw` veya kullanıcı girdisini kodlamadan yazdıran eşdeğer kullanım bulunmadı.
- Duyuru başlığına `<script>` ve açıklamasına olay işleyicili `<img>` yüklenerek pano HTTP çıktısı kontrol edildi.
- Zararlı etiketler çalıştırılabilir HTML olarak değil `&lt;...&gt;` biçiminde kodlanmış metin olarak üretildi.

## CSRF

- Yönetim controller'ları `AutoValidateAntiforgeryToken` kullanıyor.
- Giriş ve çıkış POST işlemleri `ValidateAntiForgeryToken` ile korunuyor.
- Geçerli yönetici kimliğiyle fakat antiforgery belirteci olmadan gönderilen değişiklik isteği `400 Bad Request` ile reddedildi.
- Reddedilen isteğin veritabanında değişiklik oluşturmadığı doğrulandı.
- Pano heartbeat işlemi yönetici cookie'si kullanmadığı, cihaz anahtarı gerektirdiği ve yalnız son bağlantı zamanını güncellediği için bilinçli olarak antiforgery kontrolünden muaftır.

## Dosya yükleme

- İzin verilen biçimler JPEG, PNG, WebP ve MP4 ile sınırlıdır.
- Uzantı, istemci MIME türü ve dosya imzası birlikte doğrulanıyor.
- Boş, fazla büyük, çift uzantılı, MIME türü uyuşmayan, SVG ve sahte PNG dosyaları reddedildi.
- İstemciden gelen dosya adı depolama adı olarak kullanılmıyor; kriptografik rastgele ad üretiliyor.
- Dosyalar `wwwroot` dışında saklanıyor ve yetkili controller üzerinden `nosniff` başlığıyla sunuluyor.
- Dizin dışına çıkma (`../`) denemesi reddediliyor.

## Yetki

- Tüm Admin alanı controller'ları kimlik doğrulama gerektiriyor.
- Gösterge paneli, duyuru, medya, acil duyuru ve ayar uçları anonim istekleri `401` veya giriş sayfası yönlendirmesiyle engelliyor.
- Reddedilen anonim istekler yönetim verisini değiştiremiyor.
- Pano yayın ve medya uçları doğru ekran cihaz anahtarını gerektiriyor; yanlış anahtar `404` döndürüyor.

## Otomatik test kanıtı

- Güvenlik odaklı paket: 25/25 başarılı
- Tüm otomatik paket: 64/64 başarılı
- Derleme: 0 uyarı, 0 hata

## Canlı ortam için kalan savunma katmanları

- Kurum dışından dosya kabul edilecekse antivirüs/malware taraması eklenmelidir.
- Cihaz anahtarlı pano URL'leri yalnız HTTPS üzerinden kullanılmalı ve log/ekran görüntülerinde paylaşılmamalıdır.
- Sisteme yeni kullanıcı türleri eklenecekse mevcut tek-yönetici kabulü rol/policy tabanlı yetkilendirmeye dönüştürülmelidir.
- Güvenlik kontrolleri canlıya geçiş öncesi bağımsız dinamik tarama ve gerçek sunucu yapılandırmasıyla tekrar doğrulanmalıdır.
