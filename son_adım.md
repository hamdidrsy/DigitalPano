# DigitalPano — canlıya alma adımları

**Seçilen düşük maliyetli mimari:** Render Web Service + Neon PostgreSQL + Cloudflare R2.

Uygulama bu mimariye hazırlanmıştır. Veritabanı PostgreSQL'e, kalıcı görseller Cloudflare R2'ye taşınmıştır. Render dağıtımı için `Dockerfile` ve `render.yaml` hazırdır.

## 1. Hesapları ücretsiz paketlerle aç

- [ ] GitHub hesabı ve DigitalPano için özel (private) repository
- [ ] Neon Free projesi
- [ ] Cloudflare hesabı ve R2 bucket
- [ ] Render Free hesabı

Alan adı ilk yayın için zorunlu değildir. Render ücretsiz bir `*.onrender.com` adresi verir. Özel alan adı daha sonra bağlanabilir.

## 2. Neon veritabanını hazırla

1. Neon'da yeni bir Free proje oluşturun.
2. PostgreSQL bağlantı bilgisini açın ve .NET/Npgsql bağlantı metnini kopyalayın.
3. Bağlantının SSL kullandığını doğrulayın (`SSL Mode=Require`).
4. Bu değeri kimseyle paylaşmayın ve Git'e yazmayın.

Render'da kullanılacak değişken:

```text
ConnectionStrings__DefaultConnection=NEON_BAGLANTI_METNI
```

Uygulama ilk açılışta PostgreSQL migration'larını otomatik uygular.

## 3. Cloudflare R2 görsel alanını hazırla

1. Cloudflare panelinde R2'yi etkinleştirin.
2. `digitalpano-media` adında Standard bucket oluşturun.
3. Yalnız bu bucket için **Object Read & Write** yetkili API token üretin.
4. S3 API endpoint, Access Key ID ve Secret Access Key değerlerini güvenli yere kaydedin.
5. Bucket'ı herkese açık yapmayın; uygulama görselleri kendi denetimli adresinden sunar.

Render değişkenleri:

```text
R2Storage__Endpoint=https://HESAP_KIMLIGI.r2.cloudflarestorage.com
R2Storage__AccessKeyId=ACCESS_KEY
R2Storage__SecretAccessKey=SECRET_KEY
R2Storage__BucketName=digitalpano-media
```

## 4. Kodu GitHub'a gönder

Repository yoksa proje klasöründe:

```powershell
git add .
git commit -m "Render Neon R2 canli yayin hazirligi"
git branch -M main
git remote add origin GITHUB_REPOSITORY_ADRESI
git push -u origin main
```

Mevcut repository kullanılıyorsa yalnız değişiklikleri commit edip push edin. Gizli bağlantı ve API anahtarlarını hiçbir dosyaya eklemeyin.

## 5. Render Blueprint ile yayınla

1. Render panelinde **New > Blueprint** seçin.
2. GitHub'daki DigitalPano repository'sini bağlayın.
3. Render kökteki `render.yaml` dosyasını okuyacaktır.
4. Sorulan gizli ortam değişkenlerini doldurun:

```text
ConnectionStrings__DefaultConnection
R2Storage__Endpoint
R2Storage__AccessKeyId
R2Storage__SecretAccessKey
R2Storage__BucketName
SeedAdmin__Enabled=true
SeedAdmin__Email=YONETICI_EPOSTASI
SeedAdmin__Password=EN_AZ_12_KARAKTER_GUCLU_PAROLA
```

Parolada büyük harf, küçük harf, rakam ve özel karakter bulunmalıdır. Deploy tamamlandığında Render'ın verdiği `https://...onrender.com` adresini açın.

## 6. İlk açılışı doğrula

- [ ] `/health/live` yanıt veriyor.
- [ ] `/health/ready` veritabanını hazır gösteriyor.
- [ ] `/hesap/giris` üzerinden yönetici girişi yapılabiliyor.
- [ ] Kurum bilgileri admin panelinden girildi.
- [ ] Ekran oluşturuldu ve ekran bağlantısı kopyalandı.
- [ ] JPG/PNG/WebP görsel yüklendi ve pano sayfasında göründü.
- [ ] Duyuru değişikliği açık pano ekranına ulaştı.

İlk yönetici oluşunca Render ortam değişkenlerinde `SeedAdmin__Enabled=false` yapın ve `SeedAdmin__Password` değerini kaldırın. Normal yönetici hesabı veritabanında kalır.

## 7. TV'de aç

1. TV'yi internete bağlayın.
2. Tarayıcıda admin panelinden kopyalanan cihaz anahtarlı pano adresini açın.
3. Sayfayı tam ekran yapın ve tarayıcının bu adresi başlangıçta açmasını sağlayın.
4. TV tarayıcısı bunu desteklemiyorsa HDMI kablosu yerine küçük bir Android TV kutusu veya Chromecast with Google TV kullanılabilir.

IP adresi, aynı Wi-Fi ağı veya bilgisayarın sürekli açık kalması gerekmez; TV doğrudan canlı web adresine bağlanır.

## 8. Ücretsiz paket sınırları

- Render Free servis hareketsizlikte uyuyabilir; ilk açılış gecikebilir ve kesintisiz çalışma garantisi yoktur.
- Neon Free kullanılmadığında ölçeklenir; küçük pano verisi için uygundur.
- R2 ücretsiz kotası görsel ağırlıklı küçük bir pano için genellikle yeterlidir; panelden kullanım izlenmelidir.
- Gerçek 72 saat açık kalma testi TV üzerinde yapılmalıdır. Ücretsiz hizmetler kritik/garantili yayın için SLA sağlamaz.

## Bugün bitirmek için kalan kullanıcı işlemleri

- [ ] Neon bağlantı metnini oluşturmak
- [ ] R2 bucket ve anahtarlarını oluşturmak
- [ ] Kodu GitHub'a göndermek
- [ ] Render Blueprint ekranında gizli değerleri girmek
- [ ] İlk deploy ve TV testini yapmak

Bu hesap bilgileri olmadan uygulamanın gerçek internete dağıtımını tamamlamak mümkün değildir; kod tarafındaki hazırlık tamamlanmıştır.
