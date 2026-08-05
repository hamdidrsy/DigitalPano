# Aşama 10 — IIS, HTTPS ve SQL Server canlı kurulum rehberi

**Hazırlık tarihi:** 5 Ağustos 2026  
**Hedef:** Windows Server + IIS + .NET 8 Hosting Bundle + SQL Server

## 1. Sunucu ön koşulları

- Desteklenen ve güncel Windows Server kurulumu
- IIS Web Server rolü
- IIS Management Console
- WebSocket Protocol özelliği (SignalR için)
- Güncel .NET 8 Hosting Bundle
- SQL Server ve düzenli yedekleme hedefi
- Kuruma ait DNS adı ve güvenilir TLS sertifikası

Hosting Bundle IIS'ten önce kurulmuşsa bundle kurulumu onarılmalı veya yeniden çalıştırılmalıdır. Kurulumdan sonra IIS yeniden başlatılmalıdır.

```powershell
net stop was /y
net start w3svc
```

## 2. Önerilen klasörler

```text
C:\inetpub\DigitalPano\current     Uygulama dosyaları, yalnız okuma
D:\DigitalPanoData\media           Yüklenen medya, değiştirme izni
D:\DigitalPanoData\backup          Veritabanı/medya yedekleri
C:\inetpub\DigitalPano\releases    Geri alma için önceki paketler
```

Uygulama havuzu kimliği `IIS AppPool\DigitalPano` olarak kabul edilmiştir. İzin betiği yönetici PowerShell'de çalıştırılır:

```powershell
powershell -ExecutionPolicy Bypass -File deploy\iis\Set-DigitalPanoFolderPermissions.ps1 `
  -SitePath "C:\inetpub\DigitalPano" `
  -DataPath "D:\DigitalPanoData" `
  -AppPoolName "DigitalPano"
```

## 3. SQL Server hazırlığı

1. `DigitalPano` veritabanı DBA hesabıyla oluşturulur.
2. Yayın paketindeki `database/DigitalPano-migrate-idempotent.sql` DBA tarafından incelenir.
3. Veritabanının tam yedeği alınır.
4. İdempotent migration betiği DBA yetkisiyle uygulanır.
5. `Create-DigitalPanoRuntimeUser.sql` SQLCMD değişkenleri kontrol edilerek çalıştırılır.
6. Uygulama havuzu hesabına yalnız `db_datareader` ve `db_datawriter` verilir; şema değiştirme yetkisi verilmez.

Uygulama açılışında otomatik migration uygulanmaz. Şema değişiklikleri yayın öncesi ayrı ve geri alınabilir DBA adımıdır.

## 4. Yayın paketi

Geliştirme bilgisayarında:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\New-DigitalPanoRelease.ps1
```

Betik testleri çalıştırır, Release/win-x64 framework-dependent IIS çıktısını üretir, idempotent SQL betiğini ekler, SHA-256 manifesti oluşturur ve ZIP paketler.

ZIP sunucudaki yeni bir release klasörüne açılır. Mevcut çalışan klasörün üzerine doğrudan kopyalama yapılmaz.

## 5. Üretim yapılandırması

Yayınlanan `web.config`, Integrated Security kullanan SQL bağlantısı ve mutlak medya yolu ile yapılandırılır:

```powershell
powershell -ExecutionPolicy Bypass -File deploy\iis\Configure-DigitalPanoWebConfig.ps1 `
  -WebConfigPath "C:\inetpub\DigitalPano\releases\2026.08.05\web.config" `
  -SqlServer "SQL01.contoso.local" `
  -DatabaseName "DigitalPano" `
  -HostName "pano.contoso.local" `
  -MediaPath "D:\DigitalPanoData\media"
```

Gerçek DNS adı kullanılmalıdır. `appsettings.Production.json` içindeki `pano.example.local` örnek değeri canlıda bırakılmamalıdır. SQL Server sertifikası güvenilir olmalı; `TrustServerCertificate=True` canlı çözüm olarak kullanılmamalıdır.

## 6. IIS uygulama havuzu ve site

IIS Manager'da:

1. `DigitalPano` adında uygulama havuzu oluşturulur.
2. `.NET CLR version`: **No Managed Code**.
3. Managed pipeline: **Integrated**.
4. Identity: **ApplicationPoolIdentity**.
5. Start Mode: **AlwaysRunning**.
6. Idle Time-out: `0` (kiosk yayını için uykuya geçmesin).
7. Advanced Settings → Load User Profile: `True` (Data Protection anahtarları için).
8. Site fiziksel yolu yeni release'in `app` klasörüne verilir.
9. Preload Enabled: `True` yapılır.
10. WebSocket Protocol etkin olmalıdır.

`web.config` dosyasındaki `aspNetCore` modülü publish tarafından üretilir; elle `processPath` değiştirilmez.

## 7. HTTPS

1. `pano.contoso.local` için DNS kaydı IIS sunucusunu göstermelidir.
2. Güvenilir sertifika Local Computer → Personal deposuna yüklenir.
3. IIS site binding: `https`, port `443`, doğru hostname ve sertifika.
4. SNI, aynı IP'de başka HTTPS siteleri varsa etkinleştirilir.
5. Port 80 yalnız HTTPS'e yönlendirme için açık tutulur.
6. Güvenlik duvarında 443 izinli olmalıdır.

Uygulamada HTTPS redirection, Secure cookie ve HSTS zaten etkindir. Pano cihazlarına yalnız `https://pano.contoso.local/...` adresi verilmelidir.

## 8. İlk yönetici

İlk açılışta yönetici yoksa `SeedAdmin` değerleri yalnız ilk başlatma için güvenli sunucu ortamına geçici olarak eklenir. Güçlü parola kullanılır. Hesabın oluştuğu doğrulandıktan sonra `SeedAdmin__Password`, `SeedAdmin__Email` ve `SeedAdmin__Enabled` kaldırılır ve uygulama havuzu yeniden başlatılır. Parola release ZIP'ine, Git'e veya kalıcı web.config yedeğine yazılmaz.

## 9. Canlıya alma sırası

1. SQL ve medya yedeği alınır.
2. Yeni release klasöre açılır ve SHA-256 manifesti doğrulanır.
3. Migration SQL incelenip uygulanır.
4. `web.config` ve klasör izinleri hazırlanır.
5. IIS sitesi yeni release klasörüne yönlendirilir.
6. Uygulama havuzu yeniden başlatılır.
7. `https://HOST/health/live` → `200` beklenir.
8. `https://HOST/health/ready` → `200`, `database: ok` beklenir.
9. Yönetici girişi, medya erişimi, pano, SignalR ve acil duyuru kısa kabul testi yapılır.

## 10. Geri alma

1. Uygulama havuzu durdurulur.
2. Site fiziksel yolu bir önceki release klasörüne döndürülür.
3. Migration veri kaybı içeriyorsa otomatik aşağı migration yapılmaz; DBA onaylı geri dönüş SQL'i veya tam yedek kullanılır.
4. Medya klasörü release dışında tutulduğu için uygulama geri alışında korunur.
5. Uygulama havuzu başlatılır ve iki sağlık adresi yeniden doğrulanır.

## 11. Canlı kontrol listesi

- [ ] Hosting Bundle ve WebSocket kurulu
- [ ] SQL yedeği alındı ve migration incelendi
- [ ] Uygulama havuzu hesabı SQL'e en az yetkiyle tanımlandı
- [ ] Uygulama klasörü salt okunur, medya klasörü değiştirilebilir
- [ ] Production bağlantısı LocalDB içermiyor
- [ ] Gerçek AllowedHosts değeri yapılandırıldı
- [ ] Güvenilir HTTPS sertifikası bağlı
- [ ] HTTP, HTTPS'e yönleniyor
- [ ] `/health/live` ve `/health/ready` başarılı
- [ ] Giriş, pano, medya, SignalR ve acil duyuru kontrol edildi
- [ ] Eski release ve yedekle geri alma provası yapıldı
