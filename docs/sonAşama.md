# DigitalPano — Son Aşama: Canlıya Alma ve İlk Yayın

Bu belge DigitalPano projesinin alan adı üzerinden yayınlanmasını, kurum yetkilisinin sistemi yalnız web yönetim panelinden yönetmesini ve TV ekranlarının otomatik güncellenmesini adım adım açıklar.

## Hedef çalışma şekli

- Yönetici içerikleri `https://pano.kurumadi.com/Admin` adresinden yönetir.
- TV veya kiosk cihazı kendisine ait cihaz anahtarlı pano adresini tam ekran açık tutar.
- Yönetici duyuru, medya, kayan yazı veya kurum ayarını değiştirdiğinde açık panolar otomatik güncellenir.
- Normal kullanımda yönetici sunucuya veya TV cihazına bağlanmak zorunda kalmaz.

## 1. Alan adını belirle

- Kuruma ait bir alan adı veya alt alan adı seçin.
- Önerilen adres: `pano.kurumadi.com`.
- Alan adı yönetim paneline erişiminizin olduğundan emin olun.
- DNS değişikliklerini yapabilecek yetkili kişiyi belirleyin.

> Alan adı tek başına yeterli değildir; uygulamanın çalışacağı bir sunucu da gereklidir.

## 2. Canlı sunucuyu hazırla

DigitalPano için önerilen canlı ortam:

- Windows Server
- IIS
- ASP.NET Core 8 Hosting Bundle
- SQL Server
- Sabit genel IP adresi
- Yeterli disk alanı ve düzenli yedekleme

Sunucuda uygulama ve kullanıcı medyaları için ayrı klasörler oluşturun:

```text
D:\Apps\DigitalPano
D:\DigitalPanoData\media
```

Sunucu sağlayıcısından güvenlik duvarında yalnız gerekli portların açılmasını isteyin:

- TCP 80 — yalnız HTTPS yönlendirmesi için
- TCP 443 — güvenli web erişimi için
- SQL Server portu internete açılmamalıdır.

## 3. DNS kaydını oluştur

Alan adı sağlayıcısında bir `A` kaydı tanımlayın:

```text
Ad/Host: pano
Değer: SUNUCUNUN-GENEL-IP-ADRESİ
```

DNS yayılımından sonra aşağıdaki ad sunucuyu göstermelidir:

```text
pano.kurumadi.com
```

## 4. HTTPS sertifikasını kur

- `pano.kurumadi.com` adına geçerli bir TLS/SSL sertifikası alın.
- Sertifikayı Windows Server sertifika deposuna kurun.
- IIS sitesinde HTTPS/443 binding oluşturun.
- HTTP isteklerinin HTTPS adresine yönlendirildiğini doğrulayın.
- Süresi dolmadan otomatik yenileme yöntemini yapılandırın.

Canlı pano ve yönetim paneli yalnız HTTPS üzerinden kullanılmalıdır.

## 5. SQL Server veritabanını oluştur

1. `DigitalPano` canlı veritabanını oluşturun.
2. Uygulama için yalnız gerekli yetkilere sahip ayrı SQL kullanıcısı oluşturun.
3. Güçlü parolayı kaynak koduna veya belgeye yazmayın.
4. Bağlantı bilgisini IIS ortam değişkeni ya da sunucudaki korumalı yapılandırma üzerinden verin.
5. Migration işlemini kontrollü biçimde uygulayın.

Hazır SQL betiği:

```text
deploy\sql\Create-DigitalPanoRuntimeUser.sql
```

## 6. Uygulamayı yayınla

Projeyi geliştirme bilgisayarında paketlemek için:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\New-DigitalPanoRelease.ps1 `
  -Version "1.0.0"
```

Oluşan yayın paketini sunucuya aktarın ve `D:\Apps\DigitalPano` klasörüne açın. Daha sonra:

1. IIS uygulama havuzunu oluşturun.
2. **No Managed Code** seçeneğini kullanın.
3. Idle Time-out değerini `0` yapın.
4. IIS sitesinin fiziksel yolunu yayın klasörüne bağlayın.
5. Uygulama ve medya klasörü izinlerini servis hesabına verin.
6. Üretim bağlantı bilgilerini, alan adını ve medya yolunu tanımlayın.

Ayrıntılı teknik kurulum: [`18-asama-10-iis-canli-kurulum.md`](18-asama-10-iis-canli-kurulum.md)

## 7. Canlı sistem sağlık kontrolünü yap

Aşağıdaki adreslerin HTTPS üzerinden başarılı yanıt verdiğini doğrulayın:

```text
https://pano.kurumadi.com/health/live
https://pano.kurumadi.com/health/ready
https://pano.kurumadi.com/hesap/giris
```

- `health/live`, uygulamanın çalıştığını gösterir.
- `health/ready`, uygulama ve veritabanının hizmet vermeye hazır olduğunu gösterir.
- Üretim ortamında ayrıntılı hata sayfaları ziyaretçilere gösterilmemelidir.

## 8. İlk yönetici hesabını hazırla

1. İlk yönetici hesabını güvenli biçimde oluşturun.
2. İlk girişte başlangıç parolasını değiştirin.
3. Yönetici adresini yetkili personele verin:

```text
https://pano.kurumadi.com/Admin
```

4. Her kullanıcıya mümkünse ayrı hesap verin; ortak parola paylaşmayın.
5. Yönetici parolasını e-posta, mesaj grubu veya pano URL'siyle birlikte paylaşmayın.

## 9. Kurum verilerini admin panelinden gir

Yönetici aşağıdaki işlemleri web panelinden yapar:

1. **Kurum ayarları:** Kurum adı, şehir, logo, ana ve ikincil renk
2. **Ekranlar:** TV adı, fiziksel konum ve aktiflik durumu
3. **Medya:** Gerçek logo, görsel ve videolar
4. **Duyurular:** Başlık, açıklama, yayın tarihleri ve hedef ekranlar
5. **Kayan yazılar:** Alt bant mesajları ve tarih aralığı

Test verilerini canlı verilerden ayırın ve ilk yayından önce gereksiz test içeriklerini temizleyin.

## 10. Her TV için pano adresi oluştur

1. Admin panelinde **Ekranlar** bölümünü açın.
2. Her fiziksel TV için ayrı ekran kaydı oluşturun.
3. **Kopyala** düğmesiyle cihaz anahtarlı tam adresi alın.

Örnek biçim:

```text
https://pano.kurumadi.com/pano/giris-kati?key=GIZLI-CIHAZ-ANAHTARI
```

Bu adres yönetim parolası değildir ancak gizli tutulmalıdır. Cihaz anahtarı yenilenirse eski adres çalışmaz ve kiosk cihazına yeni adres girilmelidir.

## 11. TV kiosk cihazını kur

Beko TV'de uygun web tarayıcısı bulunmuyorsa TV'ye bir Windows mini PC veya kiosk destekli TV Box bağlanmalıdır. HDMI kablosu yalnız TV ile yanındaki küçük cihaz arasında olur; yönetim uzaktan web sitesi üzerinden yapılır.

Windows kiosk cihazında:

```powershell
Set-ExecutionPolicy -Scope Process Bypass

.\scripts\Install-DigitalPanoKiosk.ps1 `
  -PanoUrl "https://pano.kurumadi.com/pano/giris-kati?key=GIZLI-CIHAZ-ANAHTARI" `
  -DisableSleep
```

Kiosk cihazında ayrıca şunları doğrulayın:

- Elektrik gelince cihazın otomatik açılması
- Kiosk kullanıcısının otomatik oturum açması
- Ekran koruyucu ve uyku modunun kapalı olması
- TV'nin doğru HDMI kaynağında kalması
- Cihaz yeniden başlayınca panonun otomatik tam ekran açılması

Ayrıntılar: [`19-asama-10-kiosk-otomatik-baslangic.md`](19-asama-10-kiosk-otomatik-baslangic.md)

## 12. İlk canlı yayını başlat

1. En az bir gerçek duyuru oluşturun.
2. Doğru başlangıç ve bitiş tarihlerini girin.
3. Yalnız ilgili ekranları seçin.
4. Duyuruyu aktif olarak kaydedin.
5. TV'de içeriğin kısa süre içinde göründüğünü doğrulayın.
6. Görsel/video, kayan yazı, kurum logosu, saat ve hava bilgisini kontrol edin.
7. Acil duyuruyu yalnız belirlenmiş test ekranında başlatıp sonlandırın.
8. Normal yayının geri geldiğini doğrulayın.
9. Kurum yetkilisinden ilk yayın onayı alın.

## 13. Kullanıcı eğitimini ve kabulü tamamla

Yetkili kullanıcıyla eğitim ve kabul senaryolarını uygulayın:

- [`20-asama-10-kullanici-egitimi-ve-kabul.md`](20-asama-10-kullanici-egitimi-ve-kabul.md)
- [`21-kullanici-kabul-formu.md`](21-kullanici-kabul-formu.md)

Kabul formu tamamlanmadan kullanıcı eğitimi görevi kapatılmamalıdır.

## 14. İlk hafta izleme yap

Yedi gün boyunca her gün aşağıdaki kontrolleri kaydedin:

| Kontrol | Beklenen sonuç |
|---|---|
| Uygulama sağlık adresleri | Başarılı yanıt verir |
| TV/kiosk bağlantısı | Pano tam ekran ve çevrimiçidir |
| Duyuru zamanlaması | İçerikler doğru zamanda girer ve çıkar |
| Medya oynatma | Görseller ve videolar hatasızdır |
| Anlık güncelleme | Panel değişikliği TV'ye otomatik yansır |
| Hata kayıtları | Tekrarlayan kritik hata yoktur |
| Disk alanı | Uygulama ve medya diski yeterlidir |
| Yedekleme | Veritabanı ve medya yedeği başarılıdır |
| Kesinti dönüşü | Ağ/elektrik sonrası kiosk geri açılır |

Sorun kaydı için:

| Tarih-saat | Ekran | Sorun | Etki | Yapılan işlem | Sonuç |
|---|---|---|---|---|---|
|  |  |  |  |  |  |

## 15. Son kabul ve kapanış

Aşağıdaki koşullar karşılandığında canlıya geçiş tamamlanır:

- [ ] Alan adı HTTPS üzerinden çalışıyor.
- [ ] Yönetim paneline yalnız yetkili hesapla giriliyor.
- [ ] Kurum bilgileri ve gerçek ekranlar tanımlandı.
- [ ] Her TV doğru cihaz anahtarlı pano adresini gösteriyor.
- [ ] Kiosk yeniden başlatma testi başarılı.
- [ ] İlk gerçek duyuru yayınlandı.
- [ ] Acil yayın kontrollü test ekranında doğrulandı.
- [ ] Kullanıcı eğitimi ve kabul formu tamamlandı.
- [ ] Veritabanı ve medya yedekleri doğrulandı.
- [ ] Yedi günlük izleme tamamlandı ve kritik hata kalmadı.

Bu listenin tamamı işaretlendiğinde proje görev listesindeki **İlk canlı yayını başlat ve ilk hafta izleme yap** maddesi `[x]` olarak kapatılabilir.
