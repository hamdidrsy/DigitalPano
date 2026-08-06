# Aşama 10 — Kiosk otomatik başlangıcı

**Hazırlanma tarihi:** 6 Ağustos 2026  
**Hedef:** Windows 10/11 mini PC veya Windows tabanlı TV Box + Microsoft Edge

## Hazırlanan yapı

- Edge, ayrı bir kiosk profiliyle tam ekran açılır.
- `DigitalPano-Kiosk` zamanlanmış görevi kullanıcı oturum açtığında çalışır.
- Edge beklenmedik biçimde kapanırsa 10 saniye sonra yeniden açılır.
- İsteğe bağlı olarak AC güçte ekran kapanması ve uyku devre dışı bırakılır.
- Kurulum ve kaldırma işlemleri tekrarlanabilir betiklerle yapılır.

## Kurulum

PowerShell'i hedef cihazdaki kiosk kullanıcısıyla açın. Yönetim panelindeki **Ekranlar > Kopyala** düğmesinden alınan tam adresi kullanın:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\Install-DigitalPanoKiosk.ps1 `
  -PanoUrl "https://pano.kurum.local/pano/giris-kati?key=CIHAZ-ANAHTARI" `
  -DisableSleep
```

`-DisableSleep` yalnız prize bağlı çalışma için güç ayarlarını değiştirir. İstenmiyorsa parametre kaldırılabilir.

## Bakım ve kaldırma

Kiosk ekranından çıkmak için `Ctrl+Alt+Delete` ile oturumu kapatın. Otomatik başlangıcı kalıcı olarak kaldırmak için:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\Uninstall-DigitalPanoKiosk.ps1
```

Kurulumu değiştirmek için kaldırma gerekmez; kurulum komutu yeni URL ile yeniden çalıştırılabilir.

## Yeniden başlatma kabul testi

1. `Get-ScheduledTask -TaskName DigitalPano-Kiosk` sonucu `Ready` veya `Running` olmalıdır.
2. Cihaz yeniden başlatılır.
3. Kiosk kullanıcısında otomatik oturum açma işletim sistemi tarafından ayrıca yapılandırılmış olmalıdır.
4. Masaüstünde işlem yapılmadan Edge tam ekran pano açılmalıdır.
5. Ağ kapatılıp açıldığında çevrimdışı içerik devam etmeli ve bağlantı gelince yayın güncellenmelidir.
6. Edge Görev Yöneticisi'nden kapatıldığında en geç 20 saniye içinde yeniden açılmalıdır.

> Betik, güvenlik nedeniyle Windows parolasını kayıt defterine yazarak otomatik oturum açma oluşturmaz. Elektrik kesintisinden sonra tamamen müdahalesiz açılış için cihaza özel kiosk hesabı/Windows otomatik oturum açma politikası ve BIOS'ta **AC Power Recovery** ayarı ayrıca yapılmalıdır.

## Durum

Otomatik başlangıç paketi hazırlanmış ve betik sözdizimi doğrulanmıştır. Görevin gerçek hedef cihazda yeniden başlatma sonrasında açılması fiziksel cihaz kabul testinde doğrulanmalıdır.
