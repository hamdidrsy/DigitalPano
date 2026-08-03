# DigitalPano Gereksinimleri ve Kabul Kriterleri

## 1. Ürün tanımı

DigitalPano, özel eğitim kursundaki televizyonlarda planlanmış içerik yayımlayan ve yetkili personel tarafından web yönetim panelinden kontrol edilen bir sistemdir.

## 2. Kullanıcılar

### Yönetici

- Güvenli giriş yapar ve oturumu kapatır.
- Duyuru, medya, kayan yazı, ekran ve kurum ayarlarını yönetir.
- Yayını ön izler.
- Acil duyuru başlatır ve sonlandırır.
- Ekranların son bağlantı durumunu görür.

### Pano cihazı

- Kendisine verilen benzersiz ekran adresini açar.
- Yalnızca kendisine atanmış aktif içerikleri gösterir.
- Yönetim menüsü veya giriş bağlantısı göstermez.
- Değişiklikleri otomatik alır ve bağlantı kesilince son yayını korur.

## 3. Fonksiyonel gereksinimler

Kimlikler daha sonraki geliştirme ve testlerde kullanılacaktır.

### Kimlik ve yetkilendirme

- **FR-001:** Yönetici kullanıcı adı/e-posta ve parola ile giriş yapabilmelidir.
- **FR-002:** Kimliği doğrulanmamış kullanıcı yönetim sayfalarına erişememelidir.
- **FR-003:** Yönetici güvenli biçimde oturumu kapatabilmelidir.
- **FR-004:** İlk yönetici hesabı başlangıç verisi veya kontrollü kurulum işlemiyle oluşturulmalıdır.

### Duyuru yönetimi

- **FR-010:** Yönetici duyuru ekleyebilmeli, listeleyebilmeli, düzenleyebilmeli ve silebilmelidir.
- **FR-011:** Duyuru; başlık, açıklama, başlangıç, bitiş, gösterim süresi, sıra, aktiflik ve acil durum alanlarını desteklemelidir.
- **FR-012:** Başlangıç tarihi bitiş tarihinden önce olmalıdır.
- **FR-013:** Yönetici bir duyuruyu bir veya birden fazla ekrana atayabilmelidir.
- **FR-014:** Planlanmış, yayında, süresi dolmuş ve pasif duyurular yönetim listesinde ayırt edilmelidir.
- **FR-015:** Yönetici duyuruyu yayımlamadan önce ön izleyebilmelidir.
- **FR-016:** Süresi dolan duyuru otomatik olarak yayından kalkmalı, geçmiş kaydı korunmalıdır.

### Medya

- **FR-020:** Duyuru metin, görsel veya video içeriği gösterebilmelidir.
- **FR-021:** Sistem JPEG, PNG ve WebP görsellerini kabul etmelidir.
- **FR-022:** Sistem ilk sürümde MP4/H.264 video kabul etmelidir.
- **FR-023:** Dosya türü ve boyutu sunucu tarafında doğrulanmalıdır.
- **FR-024:** Dosyalar güvenli, benzersiz sunucu adlarıyla saklanmalıdır.
- **FR-025:** Yönetici yüklenen medyayı ön izleyebilmelidir.

### Ekran yönetimi

- **FR-030:** Yönetici ekran ekleyebilmeli, düzenleyebilmeli ve etkinliğini değiştirebilmelidir.
- **FR-031:** Her ekran benzersiz bir `Slug` ve cihaz anahtarına sahip olmalıdır.
- **FR-032:** Pano adresi `/pano/{slug}` biçiminde olmalıdır.
- **FR-033:** Devre dışı ekran normal yayın alamamalıdır.
- **FR-034:** Ekranın son başarılı bağlantı zamanı kaydedilmelidir.
- **FR-035:** Yönetim paneli ekranın çevrimiçi veya çevrimdışı durumunu gösterebilmelidir.

### Pano yayını

- **FR-040:** Pano yalnızca aktif, zamanı uygun ve kendisine atanmış içerikleri göstermelidir.
- **FR-041:** İçerikler belirlenen sıra ve süreye göre otomatik değişmelidir.
- **FR-042:** Video tamamlandığında sıradaki içerik açılmalıdır.
- **FR-043:** Pano logo, kurum adı, canlı tarih ve saat göstermelidir.
- **FR-044:** Pano aktif kayan yazıları alt bantta göstermelidir.
- **FR-045:** Aktif içerik yoksa tanımlı yedek kurum ekranı görünmelidir.
- **FR-046:** Pano 16:9 yatay ekranda tam ekran ve kaydırma çubuğu olmadan çalışmalıdır.

### Kayan yazı ve kurum ayarları

- **FR-050:** Yönetici zaman aralığı ve sıra içeren kayan yazıları yönetebilmelidir.
- **FR-051:** Yönetici kurum adını, logosunu ve tema renklerini değiştirebilmelidir.
- **FR-052:** Tema renkleri geçerli ve güvenli renk formatıyla sınırlandırılmalıdır.

### Gerçek zamanlı çalışma

- **FR-060:** Yayın veya kurum ayarı değişince etkilenen pano ekranına SignalR bildirimi gönderilmelidir.
- **FR-061:** Bildirimi alan pano güncel yayın verisini tekrar yüklemelidir.
- **FR-062:** SignalR bağlantısı kesilirse istemci otomatik yeniden bağlanmalıdır.
- **FR-063:** SignalR çalışmadığında periyodik veri yenileme yedek mekanizması bulunmalıdır.

### Acil duyuru

- **FR-070:** Yönetici hedef ekranlar için acil duyuru başlatabilmelidir.
- **FR-071:** Acil duyuru hedef panoda normal yayının önüne geçerek tam ekran görünmelidir.
- **FR-072:** Acil yayın başlatma işlemi onay istemelidir.
- **FR-073:** Aynı ekran için tek aktif acil duyuru kuralı uygulanmalıdır.
- **FR-074:** Acil duyuru sonlanınca normal yayın otomatik geri gelmelidir.

### Çevrimdışı davranış ve kayıtlar

- **FR-080:** Pano son başarılı yayın listesini yerel olarak saklamalıdır.
- **FR-081:** Kısa ağ kesintisinde mevcut uygun içerik gösterilmeye devam etmelidir.
- **FR-082:** Bağlantı geri geldiğinde pano kullanıcı müdahalesi olmadan güncellenmelidir.
- **FR-083:** Kritik yönetim işlemleri kullanıcı ve zaman bilgisiyle kaydedilmelidir.

## 4. Fonksiyonel olmayan gereksinimler

- **NFR-001 Güvenlik:** Canlı ortam yalnızca HTTPS üzerinden hizmet vermelidir.
- **NFR-002 Güvenlik:** Parolalar ASP.NET Core Identity ile saklanmalıdır.
- **NFR-003 Güvenlik:** Yönetim formları CSRF korumalı olmalıdır.
- **NFR-004 Güvenlik:** Kullanıcı metinleri çıktı kodlamasından geçmeli, XSS oluşturamamalıdır.
- **NFR-005 Performans:** Normal bir yönetim sayfası kurum ağı koşullarında yaklaşık 2 saniye içinde kullanılabilir olmalıdır.
- **NFR-006 Performans:** Yayın değişikliği sağlıklı bağlantıda hedef ekrana en geç 5 saniye içinde yansımalıdır.
- **NFR-007 Dayanıklılık:** Pano en az 72 saat açık kalma testini çökmeden tamamlamalıdır.
- **NFR-008 Uyumluluk:** İlk hedef 1920×1080 çözünürlük ve güncel Chromium tabanlı kiosk tarayıcıdır.
- **NFR-009 Kullanılabilirlik:** Temel yönetim işlemleri teknik eğitim gerektirmeyen Türkçe arayüzle yapılmalıdır.
- **NFR-010 Erişilebilirlik:** Yönetim formlarında etiket, klavye kullanımı ve görünür doğrulama mesajları bulunmalıdır.
- **NFR-011 Bakım:** Ortam ayarları kaynak koddan ayrılmalı ve gizli bilgiler depoya eklenmemelidir.
- **NFR-012 Veri:** Veritabanı ve yüklenen medya düzenli yedeklenebilir olmalıdır.
- **NFR-013 Gözlemlenebilirlik:** Uygulama hataları ve kritik işlemler yapılandırılmış loglarla izlenebilmelidir.
- **NFR-014 Yerelleştirme:** Kullanıcı arayüzü Türkçe, varsayılan kurum saat dilimi `Europe/Istanbul` olmalıdır.

## 5. Ana kabul senaryoları

### AC-01 — Planlı duyuru

**Verilen:** Yönetici oturum açmış ve `Giriş Katı` ekranı mevcut.  
**İşlem:** Başlangıcı gelecekte, bitişi başlangıçtan sonra olan duyuru kaydedilir.  
**Beklenen:** Duyuru başlangıçtan önce görünmez, başlangıç geldiğinde görünür, bitişten sonra otomatik kalkar.

### AC-02 — Ekrana özel yayın

**Verilen:** `Giriş Katı` ve `Öğretmenler Odası` ekranları mevcut.  
**İşlem:** Duyuru yalnızca `Giriş Katı` ekranına atanır.  
**Beklenen:** Duyuru giriş ekranında görünür, öğretmenler odası ekranında görünmez.

### AC-03 — Medya slider'ı

**Verilen:** Aynı ekrana atanmış metin, görsel ve video duyuruları mevcut.  
**İşlem:** Pano adresi açılır.  
**Beklenen:** Metin ve görsel tanımlı süre kadar gösterilir; video bitince sıradaki içeriğe geçilir.

### AC-04 — Anlık güncelleme

**Verilen:** Pano açık ve SignalR bağlantısı kurulmuş.  
**İşlem:** Yönetici aktif yayını değiştirir.  
**Beklenen:** Pano elle yenilenmeden en geç 5 saniye içinde güncellenir.

### AC-05 — Acil duyuru

**Verilen:** Normal yayın açık.  
**İşlem:** Yönetici onay vererek hedef ekranda acil duyuru başlatır.  
**Beklenen:** Acil duyuru tam ekran görünür; sonlandırılınca normal yayın geri gelir.

### AC-06 — Bağlantı kesintisi

**Verilen:** Pano daha önce başarılı yayın indirmiştir.  
**İşlem:** Ağ bağlantısı geçici olarak kesilir.  
**Beklenen:** Pano siyah kalmaz, son uygun yayın devam eder ve bağlantı gelince otomatik güncellenir.

### AC-07 — Yetkisiz erişim

**Verilen:** Kullanıcı giriş yapmamıştır.  
**İşlem:** Yönetim paneli adresi açılır.  
**Beklenen:** Kullanıcı giriş sayfasına yönlendirilir ve yönetim verisi gösterilmez.

## 6. Kapsam dışı

- Android veya iOS mobil uygulama
- Öğrenci ve veli hesapları
- Push bildirimleri
- Yapay zekâ ile içerik üretimi
- Ders programı entegrasyonu
- Çok kurumlu kiracı mimarisi
- Ücretli üyelik sistemi
- Sürükle-bırak ekran tasarım editörü
