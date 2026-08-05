# Aşama 10 — Otomatik test paketi özeti

**Tamamlanma tarihi:** 4 Ağustos 2026

## Test katmanları

### Birim testleri

- Duyuru durum ve zaman kuralları
- Slug ve cihaz anahtarı davranışı
- Medya türü, imza, boyut ve güvenli dosya yolu doğrulaması
- Hava durumu WMO kod eşlemesi ve servis kesintisi toleransı
- Entity varsayılanları

### Entegrasyon testleri

- Identity yetkilendirme ve giriş/çıkış kuralları
- Duyuru oluşturma, düzenleme, silme ve ekran ilişkileri
- Ekran, kayan yazı ve kurum ayarları yaşam döngüleri
- Acil yayın başlatma, çakışma engelleme ve sonlandırma
- SignalR hedef ekran bildirimleri
- Pano zaman, hedef ekran, cihaz anahtarı ve acil yayın önceliği
- Ana sayfanın aktif panoya yönlendirilmesi

### HTTP uçtan uca testleri

- Yönetici gerçek antiforgery formuyla duyuru oluşturur ve hedef pano yayınında görür.
- Ana sayfa ilk aktif ekranın cihaz anahtarlı pano adresine yönlenir.
- Aktif duyuru, kayan yazı, kurum ve hava durumu aynı pano yanıtında görünür.
- Süresi dolmuş duyuru pano yanıtından çıkarılır.
- Yanlış cihaz anahtarı tüm HTTP hattında `404` ile reddedilir.
- Acil duyuru normal pano içeriğini tam ekran yanıtında bastırır.
- Service Worker ve ilk bağlantı çevrimdışı kabuğu yayımlanır.

Uçtan uca testler ayrı isimli bellek içi veritabanı, sabit saat, sahte hava durumu ve geçici veri koruma anahtarları kullanır. Geliştirme LocalDB verileri ve gerçek dış servisler değiştirilmez.

## Çalıştırma

```powershell
dotnet test DigitalPano.sln
```

Yalnızca uçtan uca paket:

```powershell
dotnet test DigitalPano.sln --filter FullyQualifiedName~EndToEnd
```

## Sonuç

- Toplam: 66 test
- Başarılı: 66
- Başarısız: 0
- Derleme: 0 uyarı, 0 hata
- Cobertura güncel kapsamı: satır %32,99 (1316/3988), dal %37,04 (333/899)

Kapsam ölçümü test edilmeyen Razor tarafından üretilen kodu ve uygulama başlangıç kodunu da içerir. Bu değer sonraki güvenlik ve kalite görevlerinde kritik servisler için artırılacak bir başlangıç ölçümüdür; yalnız başına canlıya geçiş onayı değildir.

## Kapsam sınırı

Otomatik paket tamamlanmıştır. Gerçek Beko TV/TV Box üzerinde 1920×1080 görünüm, kiosk başlangıcı ve 72 saat açık kalma testi donanım gerektirdiğinden Aşama 10'un sonraki görevlerinde yürütülmelidir.
