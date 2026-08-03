# Aşama 3 — Duyuru Yönetimi Özeti

## Durum

**Tamamlandı — 4 Ağustos 2026**

## Tamamlanan özellikler

- Başlık ve açıklamada arama
- Duruma göre filtreleme
- Hedef ekrana göre filtreleme
- Metin duyurusu oluşturma
- Duyuru düzenleme
- Silme onayı ve kalıcı silme
- 16:9 pano görünümüne yakın ön izleme
- Bir duyuruyu birden fazla ekrana atama
- Gösterim süresi ve yayın sırası yönetimi
- Aktif/pasif yayın kontrolü
- Oluşturma, güncelleme ve silme işlem kayıtları
- Responsive yönetim arayüzü

## Yayın durumları

Durumlar birbirini dışlayacak şekilde hesaplanır:

1. `IsActive = false`: **Pasif**
2. Aktif ve bitiş zamanı geçmiş: **Süresi dolmuş**
3. Aktif ve başlangıç zamanı gelmemiş: **Planlanmış**
4. Aktif ve yayın aralığında: **Yayında**

Liste filtreleri ve dashboard sayaçları aynı kuralları kullanmaktadır.

## Tarih ve saat

- Yönetici formlarında tarihler `Europe/Istanbul` saatinde gösterilir.
- Veritabanına UTC olarak kaydedilir.
- Liste ve ön izleme ekranında yeniden kurum saatine çevrilir.
- Bitiş tarihi başlangıç tarihinden sonra olmak zorundadır.

## Doğrulama ve güvenlik

- Başlık: zorunlu, en fazla 200 karakter
- Açıklama: zorunlu, en fazla 4000 karakter
- Gösterim süresi: 1–3600 saniye
- En az bir geçerli ekran seçimi zorunlu
- Kullanıcı tarafından gönderilen bilinmeyen ekran kimliği reddedilir
- Tüm değişiklik eylemleri yetkilendirme ve antiforgery koruması altındadır
- Arama ve veri işlemleri EF Core parametreli sorgularıyla yürütülür
- Çıktılar Razor tarafından HTML kodlamasından geçirilir

## İşlem kayıtları

Her oluşturma, güncelleme ve silme işleminde aşağıdaki bilgiler kaydedilir:

- Kullanıcı kimliği
- İşlem türü
- Entity türü ve kimliği
- Açıklama
- IP adresi
- UTC işlem zamanı

Silinen duyurunun kendisi kaldırılır fakat işlem kaydı korunur.

## Test sonuçları

- Yayında, planlanmış, süresi dolmuş ve pasif durum testleri
- Oluşturma–düzenleme–silme yaşam döngüsü
- Ekran ilişkisinin kaydedilmesi
- İstanbul saatinin UTC'ye çevrilmesi
- İşlem kayıtlarının oluşması
- Bilinmeyen ekran kimliğinin reddedilmesi
- Önceki kimlik ve veri modeli testleri
- Toplam: **16/16 test başarılı**

## Kapsam notu

Bu aşamada yalnızca metin duyuruları oluşturulur. Görsel ve MP4 video yükleme/bağlama işlemleri **Aşama 4 — Medya** kapsamında eklenecektir.

## Sonraki adım

**Aşama 4 — Medya:** güvenli dosya depolama servisi, görsel ve MP4 yükleme, içerik doğrulama, ön izleme ve kullanımda olmayan medya yönetimi.
