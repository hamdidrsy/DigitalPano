# Aşama 10 — Kullanıcı eğitimi ve kabul senaryoları

**Hazırlanma tarihi:** 6 Ağustos 2026  
**Hedef kullanıcı:** DigitalPano yönetiminden sorumlu kurum personeli  
**Önerilen süre:** 45 dakika eğitim + 30 dakika kabul testi

## 1. Eğitim öncesi hazırlık

- Canlı yönetim ve pano adresleri hazır olmalıdır.
- Eğitim alacak her kullanıcı kendi hesabıyla giriş yapmalıdır; parola paylaşılmamalıdır.
- En az bir aktif ekran ve bu ekrana ait cihaz anahtarlı pano adresi bulunmalıdır.
- Telif ve kişisel veri açısından kullanılmasına izin verilen bir örnek görsel hazırlanmalıdır.
- Acil yayın denemesi yalnız önceden belirlenen test ekranında yapılmalıdır.

## 2. Hızlı kullanım eğitimi

### 2.1. Giriş ve gösterge paneli — 5 dakika

1. `/hesap/giris` adresini açın.
2. Kurum hesabıyla giriş yapın.
3. Gösterge panelindeki yayın ve ekran özetlerini kontrol edin.
4. İş bitiminde sağ üstteki **Çıkış yap** düğmesini kullanın.

Yönetim adresi, kullanıcı parolası ve `?key=...` içeren pano adresi herkese açık kanallarda paylaşılmamalıdır.

### 2.2. Medya yönetimi — 5 dakika

1. **Medya > Dosya yükle** yolunu açın.
2. Görsel veya MP4 video seçip yükleyin.
3. Dosyanın kütüphanede ön izlemesini ve türünü kontrol edin.

Sınırlar: görsel en fazla **10 MB**, video en fazla **200 MB**. Yalnız gerçekten kullanılan dosyalar tutulmalı; bir duyuruya veya kurum logosuna bağlı medya önce bağlantısı kaldırılmadan silinmemelidir.

### 2.3. Duyuru oluşturma ve planlama — 10 dakika

1. **Duyurular > Yeni duyuru** seçeneğini açın.
2. Başlık, açıklama ve içerik türünü belirleyin.
3. Görsel/video seçildiyse daha önce yüklenen doğru medyayı seçin.
4. Başlangıç ve bitiş tarihlerini kontrol edin; bitiş başlangıçtan sonra olmalıdır.
5. Gösterim süresi, yayın sırası ve hedef ekranları belirleyin.
6. **Yayına açık** seçeneğini kontrol edip kaydedin.
7. Ön izlemeyi ve hedef panoyu kontrol edin.

Yanlış içerik yayınlanırsa duyuru pasife alınmalı veya düzeltilmelidir. Tarih/saat planlamasında sunucu ve kurum saatinin doğruluğu kontrol edilmelidir.

### 2.4. Kayan yazı, ekran ve kurum ayarları — 10 dakika

- **Kayan yazılar:** Metni, tarih aralığını ve aktiflik durumunu ayarlayın; panonun alt bandını kontrol edin.
- **Ekranlar:** Ekranı etkinleştirin/devre dışı bırakın, bağlantı durumunu kontrol edin ve **Kopyala** ile tam pano adresini alın.
- **Cihaz anahtarını yenile:** Yalnız adresin sızdığı düşünüldüğünde kullanın; eski pano adresi hemen geçersiz olur ve kiosk cihazına yeni adres girilmelidir.
- **Kurum ayarları:** Kurum adı, şehir, logo ve renkleri değiştirin; hedef panoda sonucu kontrol edin.

### 2.5. Acil duyuru — 10 dakika

1. Önceden belirlenmiş test ekranını açık tutun.
2. **Acil duyuru > Acil duyuru başlat** yolunu açın.
3. Başlık, açıklama, varsa görsel ve hedef ekranı dikkatle seçin.
4. Onay adımından sonra yayını başlatın.
5. Acil içeriğin normal yayını tamamen kapladığını doğrulayın.
6. **Yayını sonlandır** ile acil durumu kapatın.
7. Normal yayının geri geldiğini doğrulayın.

Acil yayın yalnız gerçek acil durumlarda yetkili personel tarafından kullanılmalıdır. Yanlış hedef seçimini önlemek için kaydetmeden önce ekran adı sesli olarak ikinci kez kontrol edilmelidir.

### 2.6. Sorun anında ilk kontroller — 5 dakika

1. Başka bir cihazdan uygulamanın açılıp açılmadığını kontrol edin.
2. TV/mini PC'nin açık, doğru HDMI kaynağında ve ağa bağlı olduğunu kontrol edin.
3. Yönetimde **Ekranlar** sayfasındaki son bağlantı bilgisini inceleyin.
4. Kiosk cihazını kapatıp açmadan önce tarayıcının kendiliğinden yeniden bağlanması için kısa süre bekleyin.
5. Sorun sürerse tarih, ekran adı, görülen hata ve mümkünse ekran görüntüsüyle teknik sorumluya bildirin.

## 3. Kullanıcı kabul senaryoları

Her senaryo gerçek yetkili kullanıcı tarafından uygulanmalı; sonuç kabul formuna işlenmelidir.

| No | Senaryo | Uygulama | Beklenen sonuç |
|---:|---|---|---|
| KA-01 | Yetkili giriş/çıkış | Geçerli hesapla giriş yap, sonra çıkış yap | Panel açılır; çıkıştan sonra yönetim sayfası giriş ister |
| KA-02 | Yetkisiz erişim | Çıkış durumunda `/Admin` adresini aç | Yönetim içeriği gösterilmez, girişe yönlenir |
| KA-03 | Görsel yükleme | İzin verilen örnek görseli yükle | Dosya medya kütüphanesinde doğru ön izlenir |
| KA-04 | Anlık duyuru | Aktif tarih aralığında görsel duyuru oluştur ve test ekranını seç | Duyuru hedef panoda kısa süre içinde görünür |
| KA-05 | Ekran hedefleme | Duyuruyu yalnız bir test ekranına ata | İçerik seçilen ekranda görünür, diğerinde görünmez |
| KA-06 | Planlı yayın | Başlangıcı ileri saate ayarlı duyuru oluştur | Başlangıçtan önce görünmez; zamanı gelince yayına girer |
| KA-07 | Yayından kaldırma | Aktif duyuruyu pasife al | İçerik hedef panodan kısa süre içinde kalkar |
| KA-08 | Kayan yazı | Aktif tarih aralığında test mesajı oluştur | Metin pano alt bandında görünür |
| KA-09 | Kurum ayarı | Kurum adında kontrollü bir değişiklik yap, sonra geri al | Değişiklik panoya yansır ve geri alınabilir |
| KA-10 | Acil yayın | Yalnız test ekranında acil yayın başlat ve sonlandır | Acil ekran normal yayını kaplar; sonlandırınca normal yayın döner |
| KA-11 | Çevrimdışı davranış | Pano açıkken kiosk cihazının ağını kısa süre kesip geri getir | Pano siyah kalmaz; bağlantı gelince güncel yayın döner |
| KA-12 | Kiosk yeniden başlatma | Hedef cihazı yeniden başlat | Oturum sonrası pano insan müdahalesi olmadan tam ekran açılır |

## 4. Kabul koşulları

- KA-01–KA-10 senaryolarının tamamı başarılı olmalıdır.
- KA-11 ve KA-12 gerçek hedef cihazda başarılı olmalıdır.
- Yetkili kullanıcı yardım almadan medya yükleyebilmeli, duyuru oluşturabilmeli ve yayından kaldırabilmelidir.
- Acil yayın başlatma/sonlandırma işlemi yalnız test ekranında doğrulanmalıdır.
- Başarısız maddeler için hata kaydı açılmalı ve yeniden test tarihi yazılmalıdır.
- Kurum yetkilisi kabul formunu onaylamalıdır.

## 5. Eğitim sonrası teslim edilecek bilgiler

- Canlı yönetim adresi
- Ekran adları ve cihaz anahtarı gizlenmiş pano adresleri
- Teknik sorumlu ve iletişim yöntemi
- Yedekleme sorumlusu ve kontrol sıklığı
- Acil yayın kullanmaya yetkili kişiler
- Planlı bakım zamanı

## 6. Mevcut durum

Eğitim içeriği ve ölçülebilir kabul senaryoları hazırlanmıştır. Eğitim henüz gerçek kurum kullanıcısıyla uygulanmadığı ve kabul formu imzalanmadığı için görev operasyonel olarak **tamamlanmamıştır**. Uygulama sonrasında aşağıdaki form doldurulmalı ve görev listesi kapatılmalıdır.

Kabul kayıt formu: [`21-kullanici-kabul-formu.md`](21-kullanici-kabul-formu.md)
