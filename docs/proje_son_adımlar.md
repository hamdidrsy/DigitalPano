# DigitalPano — Proje Son Adımları ve Canlıya Alma

**Hazırlanma tarihi:** 7 Ağustos 2026  
**Hedef:** DigitalPano'yu ekonomik bir paylaşımlı Windows Hosting üzerinde yayınlamak ve internet tarayıcısı bulunan TV'de tam ekran çalıştırmak.

## Seçilen ekonomik yöntem

Bu proje için başlangıçta Windows VPS alınmayacaktır. ASP.NET Core 8 ve MSSQL destekleyen paylaşımlı Windows Hosting kullanılacaktır.

Canlı yayında video kullanılmayacaktır; kapsam metin, görsel ve kayan yazıdır. Önerilen başlangıç paketi:

- ASP.NET Core 8 desteği
- MSSQL veritabanı
- En az 2 GB RAM
- En az 5 GB disk
- En az 25 GB aylık trafik
- Plesk panel
- Ücretsiz SSL
- Uygulama klasörüne yazma izni

Örnek paket: Turhost **Windows-Giriş**. Görsel veya ekran sayısı büyürse **Windows-Giriş Plus** paketine geçilebilir. Satın almadan önce güncel toplam ve yenileme fiyatı ödeme ekranından kontrol edilmelidir.

## 1. Satın alma öncesi son kontrol

Hosting sağlayıcısının satış desteğine şu metin gönderilir:

> ASP.NET Core 8 MVC ve MSSQL kullanan bir dijital pano uygulaması yayınlayacağım. Uygulama SignalR/WebSocket kullanıyor ve kendi klasörüne görsel yüklüyor; video kullanılmayacak. WebSocket desteği, uygulama klasörüne kalıcı yazma izni, ASP.NET Core uygulama havuzu çalışma süresi ve MSSQL üzerinde SQL/migration betiği çalıştırma yöntemini bildirir misiniz?

Kontrol tablosu:

- [x] ASP.NET Core 8 desteği resmî sayfada mevcut.
- [x] MSSQL desteği resmî sayfada mevcut.
- [x] Plesk üzerinden klasör izinleri düzenlenebiliyor.
- [ ] WebSocket desteği sağlayıcı tarafından teyit edildi.
- [ ] Uygulama havuzunun çalışma/uyku kuralı öğrenildi.
- [x] Büyük video yükleme ihtiyacı yok; bu kontrol kapsam dışı.
- [ ] MSSQL üzerinde idempotent migration SQL çalıştırma yöntemi teyit edildi.

> WebSocket bulunmazsa pano 30 saniyelik HTTP yenileme yedeğiyle çalışmaya devam eder. Görseller web için optimize edilerek yüklenmelidir.

## 2. Alan adını belirle

Kurumun mevcut alan adı varsa yeni alan adı alınmaz. Şu alt alan adı oluşturulur:

```text
pano.kurumadi.com
```

Mevcut alan adı yoksa `.com.tr` veya `.com` alan adı alınır. Alan adı ve hosting aynı firmadan alınmak zorunda değildir.

Kayıt edilecek bilgiler:

| Bilgi | Değer |
|---|---|
| Alan adı |  |
| Pano alt alan adı |  |
| Alan adı sağlayıcısı |  |
| Yenileme tarihi |  |
| Hesap sahibi |  |

## 3. Windows Hosting paketini satın al

- [ ] Paket aylık/yıllık toplamı ve KDV kontrol edildi.
- [ ] Kampanya sonrası yenileme fiyatı kaydedildi.
- [ ] Plesk erişimi teslim alındı.
- [ ] FTP/Web Deploy bilgileri teslim alındı.
- [ ] MSSQL veritabanı kotası öğrenildi.
- [ ] Yedekleme sıklığı ve saklama süresi öğrenildi.

Parolalar bu belgeye yazılmaz; güvenilir bir parola yöneticisinde tutulur.

## 4. Canlı MSSQL veritabanını oluştur

Plesk panelde:

1. **Veritabanları** bölümünü açın.
2. Yeni bir MSSQL veritabanı oluşturun.
3. Ayrı bir veritabanı kullanıcısı ve güçlü parola oluşturun.
4. Sunucu, veritabanı, kullanıcı adı ve bağlantı şablonunu güvenli yere kaydedin.
5. Sağlayıcının SQL yönetim aracı veya uzaktan SSMS erişimi üzerinden migration betiğini uygulayın.

Yayın paketi üretildiğinde migration dosyası şurada bulunur:

```text
database\DigitalPano-migrate-idempotent.sql
```

Migration tamamlandıktan sonra tabloların oluştuğu doğrulanır.

## 5. Paylaşımlı hosting üretim ayarlarını belirle

Canlı uygulama aşağıdaki bilgilere ihtiyaç duyar:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection`
- `MediaStorage__RootPath`
- `AllowedHosts`
- İlk yönetici oluşturulacaksa geçici `SeedAdmin` değerleri

Örnek değer yapısı:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=...;Database=...;User Id=...;Password=...;Encrypt=True;TrustServerCertificate=False
MediaStorage__RootPath=C:\HostingSpaces\HESAP\alanadi.com\data\media
AllowedHosts=pano.kurumadi.com
```

Gerçek kullanıcı adı, parola ve fiziksel yol repoya yazılmamalıdır. Paylaşımlı hostingte fiziksel medya yolu Plesk'te görünen hesaba özel yol olmalıdır.

## 6. Canlı yayın paketini üret

Proje klasöründe PowerShell açın:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\New-DigitalPanoRelease.ps1 `
  -Version "1.0.0"
```

Betik testleri çalıştırır ve `artifacts\release` altında uygulama dosyalarını, migration SQL dosyasını ve ZIP paketini üretir.

> Mevcut betik IIS sunucusu için `win-x64` paket üretir. Hosting sağlayıcısının paylaşımlı Plesk ortamında framework-dependent `win-x64` yayını kabul ettiğini teyit edin.

## 7. Uygulamayı Plesk'e yükle

1. Plesk'te alan adı veya `pano` alt alan adını ekleyin.
2. Site kökünün yedeğini alın.
3. Varsayılan dosyaları temizlemeden önce geri dönüş kopyası oluşturun.
4. Yayın paketindeki `app` klasörünün **içeriğini** `httpdocs` klasörüne yükleyin.
5. `App_Data\media` veya sağlayıcının izin verdiği ayrı medya klasörünü oluşturun.
6. Uygulama havuzu kullanıcısına medya klasöründe okuma/yazma izni verin.
7. Üretim bağlantı ve ortam değerlerini Plesk üzerinden tanımlayın.
8. Uygulamayı yeniden başlatın.

## 8. Alan adı ve SSL'i etkinleştir

Alan adı başka firmadaysa sağlayıcının verdiği DNS hedefini kullanın. `pano` alt alan adı hosting hesabına yönlendirilir.

- [ ] DNS kaydı doğru hedefe yönlendirildi.
- [ ] Ücretsiz SSL sertifikası etkinleştirildi.
- [ ] HTTP istekleri HTTPS'e yönleniyor.
- [ ] Sertifika otomatik yenilemesi açık.

Canlı adres:

```text
https://pano.kurumadi.com
```

## 9. İlk açılış ve sağlık kontrolü

Şu adresleri kontrol edin:

```text
https://pano.kurumadi.com/health/live
https://pano.kurumadi.com/health/ready
https://pano.kurumadi.com/hesap/giris
```

Beklenen sonuçlar:

- `health/live`: uygulama çalışıyor.
- `health/ready`: veritabanı bağlantısı hazır.
- Giriş sayfası Türkçe ve HTTPS üzerinden açılıyor.
- Ziyaretçiye ayrıntılı geliştirme hatası gösterilmiyor.

## 10. İlk yönetici hesabını oluştur

İlk kurulum sırasında başlangıç yöneticisi geçici üretim ayarlarıyla oluşturulur. Hesap oluştuktan sonra:

1. Yönetim paneline giriş yapın.
2. Başlangıç parolasını değiştirin.
3. `SeedAdmin__Enabled` değerini kapatın veya kaldırın.
4. Uygulamayı yeniden başlatın.
5. Yetkisiz `/Admin` isteğinin giriş sayfasına yönlendiğini kontrol edin.

Yönetim adresi:

```text
https://pano.kurumadi.com/Admin
```

## 11. Gerçek kurum verilerini gir

Admin panelinde:

1. **Kurum ayarları:** kurum adı, şehir, logo ve tema renkleri
2. **Medya:** web için optimize edilmiş gerçek görseller
3. **Ekranlar:** her TV için ayrı ekran adı ve fiziksel konum
4. **Duyurular:** gerçek içerik, başlangıç/bitiş zamanı ve hedef ekran
5. **Kayan yazılar:** alt bant mesajları ve tarih aralığı

İlk denemede küçük bir JPG/PNG görsel kullanılmalıdır. Canlı kullanımda video yüklenmez.

## 12. TV ekranını bağla

1. Admin panelinde **Ekranlar** bölümünü açın.
2. TV için örneğin `Salon TV` kaydı oluşturun.
3. **Kopyala** düğmesiyle cihaz anahtarlı tam adresi alın.
4. TV'nin internet tarayıcısını açın.
5. Bağlantıyı adres çubuğuna girin.
6. Tarayıcıyı tam ekran yapın.

Örnek:

```text
https://pano.kurumadi.com/pano/salon-tv?key=GIZLI-CIHAZ-ANAHTARI
```

TV'nin ve yönetim bilgisayarının aynı Wi-Fi ağında olması gerekmez. İkisinin de internete erişebilmesi yeterlidir.

## 13. İlk canlı yayını başlat

1. Yalnız test ekranını hedefleyen gerçek bir duyuru oluşturun.
2. Duyuruyu aktif olarak kaydedin.
3. İçeriğin TV'de otomatik göründüğünü kontrol edin.
4. Görsel, kayan yazı, saat, logo ve renkleri doğrulayın.
5. Duyuruyu düzenleyip değişikliğin TV'ye yansımasını kontrol edin.
6. Duyuruyu pasife alıp panodan kalktığını doğrulayın.
7. Acil yayın testini yalnız test ekranında başlatıp sonlandırın.

## 14. İlk hafta izleme

Yedi gün boyunca günlük kayıt tutulur:

| Gün | Pano açık | İçerik güncellendi | Medya sorunsuz | Hata | Trafik/disk | Açıklama |
|---:|---|---|---|---|---|---|
| 1 |  |  |  |  |  |  |
| 2 |  |  |  |  |  |  |
| 3 |  |  |  |  |  |  |
| 4 |  |  |  |  |  |  |
| 5 |  |  |  |  |  |  |
| 6 |  |  |  |  |  |  |
| 7 |  |  |  |  |  |  |

Plesk'ten disk ve aylık trafik tüketimi her gün kontrol edilir. Paket sınırına yaklaşılırsa videolar küçültülür veya üst pakete geçiş değerlendirilir.

## 15. Canlıya geçiş kapanış listesi

- [ ] Hosting teknik teyitleri alındı.
- [ ] Alan adı/alt alan adı hazırlandı.
- [ ] Hosting paketi satın alındı.
- [ ] MSSQL veritabanı ve kullanıcısı oluşturuldu.
- [ ] Migration SQL başarıyla uygulandı.
- [ ] Uygulama Plesk'e yüklendi.
- [ ] Üretim ayarları gizli biçimde tanımlandı.
- [ ] HTTPS ve sağlık adresleri çalışıyor.
- [ ] İlk yönetici hesabı güvenli hale getirildi.
- [ ] Kurum bilgileri ve gerçek ekran oluşturuldu.
- [ ] TV, cihaz anahtarlı adresi tam ekran gösteriyor.
- [ ] İlk gerçek duyuru yayınlandı.
- [ ] Kullanıcı kabul formu tamamlandı.
- [ ] Yedi günlük izleme tamamlandı.
- [ ] Kritik hata kalmadı.

Tüm maddeler tamamlandığında proje görev listesindeki **İlk canlı yayını başlat ve ilk hafta izleme yap** görevi kapatılabilir.

## İlgili belgeler

- [`18-asama-10-iis-canli-kurulum.md`](18-asama-10-iis-canli-kurulum.md) — ayrılmış IIS sunucusu kurulumu
- [`19-asama-10-kiosk-otomatik-baslangic.md`](19-asama-10-kiosk-otomatik-baslangic.md) — Windows kiosk otomatik başlangıcı
- [`20-asama-10-kullanici-egitimi-ve-kabul.md`](20-asama-10-kullanici-egitimi-ve-kabul.md) — kullanıcı eğitimi
- [`21-kullanici-kabul-formu.md`](21-kullanici-kabul-formu.md) — kabul kayıt formu
