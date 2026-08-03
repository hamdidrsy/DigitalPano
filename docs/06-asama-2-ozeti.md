# Aşama 2 — Kimlik ve Yönetim Kabuğu Özeti

## Durum

**Tamamlandı — 3 Ağustos 2026**

## Tamamlanan özellikler

- Türkçe yönetici giriş sayfası
- Identity tabanlı parola doğrulama ve başarısız giriş kilitlemesi
- Güvenli yerel dönüş adresi kontrolü
- Yalnızca POST ve antiforgery korumalı çıkış işlemi
- `[Authorize]` ile korunan Admin alanı
- Responsive yönetim paneli yerleşimi ve menüsü
- Yayın, ekran ve acil duyuru özet kartları
- EF Core tabanlı dashboard sorgu servisi
- HTTP-only, Secure ve SameSite cookie ayarları
- HTTPS yönlendirmesi ve bir yıllık HSTS yapılandırması
- Üretime özel düşük ayrıntılı log yapılandırması
- Türkçe erişim reddedildi sayfası

## Adresler

- Giriş: `/hesap/giris`
- Çıkış: `POST /hesap/cikis`
- Erişim reddedildi: `/hesap/erisim-reddedildi`
- Yönetim paneli: `/Admin`

## Dashboard göstergeleri

- Yayındaki duyuru sayısı
- Planlanmış duyuru sayısı
- Süresi dolmuş duyuru sayısı
- Aktif ve çevrimiçi ekran sayısı
- Aktif acil yayın sayısı

Çevrimiçi ekran, son iki dakika içerisinde bağlantı bildiren aktif ekran olarak kabul edilmektedir.

## Güvenlik notları

- Giriş hataları e-posta adresinin sistemde bulunup bulunmadığını açıklamaz.
- Beş başarısız girişten sonra hesap 15 dakika kilitlenir.
- Harici dönüş adresleri kabul edilmez; açık yönlendirme engellenir.
- Cookie'ler `HttpOnly`, `Secure=Always` ve `SameSite=Strict` olarak ayarlanmıştır.
- Çıkış bağlantısı GET değildir; antiforgery token içeren POST formudur.
- Geliştirme yöneticisinin parolası kaynak kodda tutulmaz ve User Secrets ile tanımlanır.

## Test sonuçları

- Anonim Admin isteği giriş sayfasına yönlendirilir.
- Türkçe giriş sayfası başarıyla açılır.
- Kimliği doğrulanmış yönetici dashboard'u görüntüler.
- Çıkış işlemi GET isteğini kabul etmez.
- Anonim çıkış isteği giriş sayfasına yönlendirilir.
- Önceki veri modeli testleri korunmuştur.
- Toplam sonuç: **10/10 test başarılı**.

## Sonraki adım

**Aşama 3 — Duyuru yönetimi:** listeleme, filtreleme, ekleme, düzenleme, silme, ekran atama, yayın durumu hesaplama ve ön izleme.
