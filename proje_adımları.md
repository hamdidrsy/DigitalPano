# Psychology Clinic Hasan Durusoy — Proje Adımları

Bu belge, psikoloji kliniği web sitesinin fikir aşamasından canlıya alınmasına ve canlı sonrası bakımına kadar izlenecek ana yol haritasıdır.

## 1. Proje hedefi ve başarı ölçütleri

### Amaç

- Hasan Durusoy'un uzmanlığını ve hizmetlerini güven veren bir dille tanıtmak.
- Ziyaretçilerin mobil ve masaüstünde kolayca randevu talebi oluşturmasını sağlamak.
- SEO uyumlu makalelerle organik görünürlüğü artırmak.
- Randevu, içerik ve site ayarlarının güvenli bir yönetim panelinden yönetilmesini sağlamak.
- Toplanan kişisel verileri veri minimizasyonu ve KVKK ilkelerine uygun şekilde işlemek.

### İlk sürüm başarı ölçütleri

- Temel sayfalar mobil ve masaüstünde sorunsuz çalışır.
- Randevu talebi kaydedilir ve ilgili kişiye e-posta bildirimi gider.
- Yönetici randevuları ve makaleleri yönetebilir.
- Lighthouse performans, erişilebilirlik, iyi uygulamalar ve SEO kontrollerinde hedeflenen seviyeye ulaşılır.
- Sitemap, robots.txt, metadata ve yapılandırılmış veriler doğrulanır.
- Üretim ortamında hata izleme, yedekleme ve geri dönüş planı hazırdır.

## 2. Kapsamı netleştirme

- [ ] Hedef kitleyi ve hizmet verilen şehir/bölgeleri belirle.
- [ ] Sunulacak hizmetleri ve çalışma yöntemlerini listele.
- [ ] Randevu sürecini belirle: talep, onay, iptal ve yeniden planlama.
- [ ] Online görüşme ve yüz yüze görüşme seçeneklerini netleştir.
- [ ] İlk sürümde ödeme alınıp alınmayacağına karar ver.
- [ ] Site dili veya dillerini belirle.
- [ ] Alan adı ve kurumsal e-posta adreslerini belirle.
- [ ] Logo, renkler, yazı tipleri, fotoğraflar ve iletişim bilgilerini temin et.
- [ ] KVKK aydınlatma metni, açık rıza gereksinimleri, çerez politikası ve mesleki tanıtım kurallarını uzman görüşüyle doğrula.

### İlk sürüm kapsamı

- Ana sayfa
- Hakkımda
- Hizmetler ve hizmet detayları
- Makaleler ve makale detayları
- Sık sorulan sorular
- İletişim ve randevu talep formu
- Gizlilik/KVKK ve çerez sayfaları
- Güvenli yönetici girişi
- Makale ve randevu yönetimi

### İlk sürüm sonrasına bırakılabilecekler

- Online ödeme
- Danışan hesabı
- Otomatik takvim entegrasyonu
- SMS veya WhatsApp bildirimi
- Çoklu uzman desteği
- Çoklu dil
- Gelişmiş içerik editörü ve medya kütüphanesi

## 3. Teknik kararlar ve mimari

### Önerilen teknoloji yığını

- Next.js App Router ve TypeScript
- Tailwind CSS
- PostgreSQL
- Prisma ORM
- Zod ile veri doğrulama
- Resend ile işlem e-postaları
- Güvenli yönetici kimlik doğrulaması ve rol kontrolü
- Bot koruması ve hız sınırlama
- Vercel veya eşdeğer bir Node.js barındırma ortamı
- Yönetilen PostgreSQL hizmeti

### Teknik karar kayıtları

- [ ] Node.js ve paket yöneticisi sürümünü sabitle.
- [ ] Barındırma ve PostgreSQL sağlayıcısını seç.
- [ ] Dosya/görsel depolama yöntemini seç.
- [ ] Kimlik doğrulama çözümünü seç.
- [ ] E-posta gönderen alan adını belirle.
- [ ] Analitik ve hata izleme araçlarını seç; çerez/onay gereksinimlerini değerlendir.
- [ ] Geliştirme, ön izleme ve üretim ortamlarını ayır.

## 4. Proje kurulumu

- [ ] Yeni Next.js uygulamasını TypeScript, App Router, ESLint ve Tailwind CSS ile oluştur.
- [ ] Klasör yapısını belirle: uygulama rotaları, bileşenler, servisler, doğrulama şemaları ve veri katmanı.
- [ ] Kod biçimlendirme, lint ve tip kontrolü kurallarını yapılandır.
- [ ] `.env.example` oluştur; gerçek anahtarları repoya ekleme.
- [ ] Git dallanma ve commit yaklaşımını belirle.
- [ ] CI iş akışına lint, tip kontrolü, test ve build adımlarını ekle.
- [ ] Geliştirme kurulumunu ve komutları README dosyasında belgele.

### Tamamlanma ölçütü

Temiz bir ortamda bağımlılıklar kurulabilmeli; geliştirme sunucusu, lint, test ve production build komutları çalışmalıdır.

## 5. Bilgi mimarisi ve içerik hazırlığı

- [ ] Site haritasını ve menü yapısını oluştur.
- [ ] Her sayfanın amacı ve ana çağrı-eylem metnini belirle.
- [ ] Anahtar kelime ve arama niyeti çalışması yap.
- [ ] Hizmet, hakkımda, iletişim ve SSS metinlerini hazırla.
- [ ] İlk makale listesini ve yayın takvimini oluştur.
- [ ] Kullanılacak görseller için izin, lisans ve alternatif metinleri kaydet.
- [ ] Sağlık konusunda yanıltıcı vaatlerden ve kesin sonuç ifadelerinden kaçınan içerik kontrolü yap.
- [ ] Danışan yorumlarının yayımlanmasının etik ve hukuki uygunluğunu doğrula; uygun değilse bu modülü kapsamdan çıkar.

## 6. Tasarım sistemi ve kullanıcı deneyimi

- [ ] Renk paleti, tipografi, boşluk sistemi ve bileşen durumlarını belirle.
- [ ] Mobil öncelikli ana sayfa ve temel sayfa taslaklarını hazırla.
- [ ] Başlık, navigasyon, alt bilgi, buton, form, kart ve bildirim bileşenlerini tasarla.
- [ ] Randevu formunu kısa, anlaşılır ve minimum veri toplayacak şekilde tasarla.
- [ ] Klavye kullanımı, odak göstergeleri, renk kontrastı ve ekran okuyucu etiketlerini doğrula.
- [ ] Yükleniyor, boş, başarılı ve hatalı durumları tasarla.
- [ ] 404 ve genel hata sayfalarını hazırla.

## 7. Veritabanı tasarımı

### Temel modeller

- `AdminUser`: yönetici hesabı ve rol bilgisi
- `AppointmentRequest`: randevu talebi ve durumu
- `Article`: taslak/yayında makale, slug ve SEO alanları
- `Service`: hizmet bilgileri
- `SiteSetting`: iletişim ve genel site ayarları
- `AuditLog`: kritik yönetim işlemlerinin kaydı

### Uygulama adımları

- [ ] Prisma şemasını oluştur.
- [ ] Kimlikleri, ilişkileri, indeksleri ve benzersiz alanları tanımla.
- [ ] Tarihleri UTC sakla; kullanıcıya uygun saat diliminde göster.
- [ ] Randevu durumlarını tanımla: yeni, iletişime geçildi, onaylandı, iptal edildi, tamamlandı.
- [ ] Gereksiz hassas sağlık verilerinin kaydedilmesini engelle.
- [ ] İlk migration'ı oluştur ve test verileri için seed mekanizması hazırla.
- [ ] Yedekleme, geri yükleme ve veri silme prosedürünü belirle.

## 8. Herkese açık site geliştirmesi

- [ ] Genel sayfa şablonunu, navigasyonu ve alt bilgiyi geliştir.
- [ ] Ana sayfayı geliştir.
- [ ] Hakkımda ve hizmet sayfalarını geliştir.
- [ ] Makale listeleme, kategori/etiket ihtiyacı ve makale detay sayfalarını geliştir.
- [ ] SSS sayfasını geliştir.
- [ ] İletişim bilgilerini ve harita kullanımını gizlilik açısından değerlendirerek ekle.
- [ ] Randevu talep formunu geliştir.
- [ ] KVKK, gizlilik ve çerez sayfalarını ekle.
- [ ] Responsive görünümü yaygın ekran boyutlarında doğrula.

## 9. Randevu ve e-posta akışı

- [ ] Form alanlarını Zod ile hem istemci hem sunucu tarafında doğrula.
- [ ] Sunucu tarafında bot koruması, hız sınırlama ve istenmeyen istek kontrolleri uygula.
- [ ] Talebi PostgreSQL'e güvenli biçimde kaydet.
- [ ] Yöneticiye yeni talep bildirimi gönder.
- [ ] Ziyaretçiye talebin alındığını bildiren, randevunun henüz kesinleşmediğini açıkça belirten e-posta gönder.
- [ ] Gönderim hatasında talebi kaybetmeden yeniden deneme veya yönetici uyarısı oluştur.
- [ ] E-posta gönderen alan adı için SPF, DKIM ve DMARC kayıtlarını yapılandır.
- [ ] Loglarda form içeriği ve kişisel verilerin gereksiz şekilde tutulmadığını doğrula.

## 10. Yönetim paneli

- [ ] Güvenli giriş ve çıkış akışını geliştir.
- [ ] Parola politikası, oturum süresi ve giriş denemesi sınırlaması uygula.
- [ ] Yönetim rotalarını sunucu tarafında yetkilendir.
- [ ] Randevu taleplerini listeleme, filtreleme, görüntüleme ve durum güncelleme ekranlarını geliştir.
- [ ] Makale oluşturma, taslak kaydetme, ön izleme, yayımlama ve arşivleme akışını geliştir.
- [ ] Hizmet ve temel site ayarlarını yönetme ekranlarını geliştir.
- [ ] Kritik değişiklikleri denetim kaydına yaz.
- [ ] Yönetici hesabı oluşturma ve kurtarma prosedürünü belgele.

## 11. SEO uygulaması

- [ ] Her sayfa için benzersiz title, description ve canonical adres oluştur.
- [ ] Open Graph ve sosyal paylaşım görsellerini ekle.
- [ ] `sitemap.xml` ve `robots.txt` üret.
- [ ] Organization/Person, WebSite, BreadcrumbList, Article ve uygun olduğu ölçüde hizmet yapılandırılmış verilerini ekle.
- [ ] URL ve slug yapısını kalıcı ve okunabilir tut.
- [ ] Başlık hiyerarşisini ve dahili bağlantıları düzenle.
- [ ] Görselleri optimize et; boyut, format ve alternatif metinleri tanımla.
- [ ] Taslak, yönetim ve ön izleme sayfalarını indekslemeye kapat.
- [ ] Kırık bağlantı, yönlendirme ve 404 kontrollerini yap.
- [ ] Search Console doğrulama ve sitemap gönderimi için canlıya çıkış görevi oluştur.

## 12. Güvenlik ve mahremiyet

- [ ] Tüm girdileri sunucuda doğrula ve çıktıları güvenli işle.
- [ ] Yetkilendirmeyi yalnızca arayüzde değil, her sunucu işleminde kontrol et.
- [ ] Güvenlik başlıkları ve içerik güvenlik politikasını yapılandır.
- [ ] Secret ve bağlantı bilgilerini yalnızca ortam değişkenlerinde sakla.
- [ ] Bağımlılık güvenlik taramasını CI sürecine ekle.
- [ ] Veritabanı erişimini en az yetki ilkesiyle sınırla.
- [ ] Kişisel veriler için saklama süresi, erişim, dışa aktarma ve silme süreçlerini tanımla.
- [ ] Üretim loglarında e-posta, telefon ve form metni gibi verileri maskele.
- [ ] Yedeklerin şifreli, erişimi sınırlı ve geri yüklenebilir olduğunu test et.
- [ ] Olay müdahale ve veri ihlali iletişim planı hazırla.

## 13. Test ve kalite kontrol

### Otomatik testler

- [ ] Doğrulama ve yardımcı fonksiyonlar için birim testleri yaz.
- [ ] Randevu oluşturma, yetkilendirme ve makale işlemleri için entegrasyon testleri yaz.
- [ ] Randevu talebi ve yönetici içerik akışı için uçtan uca testler yaz.
- [ ] CI üzerinde lint, type-check, test ve build çalıştır.

### Manuel kontroller

- [ ] Chrome, Firefox, Safari ve Edge'in güncel sürümlerinde kontrol et.
- [ ] Telefon, tablet ve masaüstü ekranlarında kontrol et.
- [ ] Klavye ve ekran okuyucu ile temel akışları test et.
- [ ] Yavaş bağlantı ve hata durumlarını test et.
- [ ] Form tekrar gönderimi ve eşzamanlı işlemleri test et.
- [ ] E-posta teslimatını ve spam durumunu test et.
- [ ] Gerçek kişisel veri kullanmadan ön izleme ortamında kabul testi yap.

### Yayın kapısı

- [ ] Kritik veya yüksek öncelikli hata kalmadı.
- [ ] Production build başarılı.
- [ ] Migration üretim benzeri bir ortamda denendi.
- [ ] Geri yükleme ve geri alma planı doğrulandı.
- [ ] İçerikler ve hukuki metinler onaylandı.

## 14. Performans ve gözlemlenebilirlik

- [ ] Core Web Vitals ölçümlerini kontrol et.
- [ ] Fontları ve kritik görselleri optimize et.
- [ ] Gereksiz istemci JavaScript'ini azalt.
- [ ] Uygun önbellekleme ve yeniden doğrulama stratejilerini belirle.
- [ ] Hata izleme ve kritik işlem alarmlarını yapılandır.
- [ ] Sağlık kontrolü ve çalışma süresi izleme ekle.
- [ ] Analitik kullanılacaksa mahremiyet ve çerez tercihlerine uygun yapılandır.

## 15. Ön izleme ortamı ve kullanıcı kabul testi

- [ ] Ayrı ön izleme veritabanı ve test e-posta ayarları oluştur.
- [ ] Ortam değişkenlerini ön izleme ortamına ekle.
- [ ] Migration'ları uygula ve test yöneticisini oluştur.
- [ ] Site sahibiyle içerik, tasarım ve randevu akışını uçtan uca kontrol et.
- [ ] Geri bildirimleri önceliklendir ve kritik olanları düzelt.
- [ ] Canlıya çıkış onayı al.

## 16. Canlı ortam hazırlığı

- [ ] Alan adını satın al veya erişimini doğrula.
- [ ] Üretim barındırma projesini ve PostgreSQL veritabanını oluştur.
- [ ] Üretim ortam değişkenlerini güvenli biçimde tanımla.
- [ ] Resend alan adı doğrulamasını ve DNS kayıtlarını tamamla.
- [ ] Alan adı DNS kayıtlarını barındırma ortamına yönlendir.
- [ ] HTTPS ve otomatik sertifika yenilemeyi doğrula.
- [ ] Üretim veritabanı yedekleme politikasını etkinleştir.
- [ ] İlk yönetici hesabını güvenli yöntemle oluştur.
- [ ] İzleme, alarm ve hata bildirim alıcılarını doğrula.
- [ ] Bakım sorumlusu ve acil iletişim yöntemini belirle.

## 17. Canlıya alma sırası

1. Son kod sürümünü etiketle ve production build'i doğrula.
2. Üretim veritabanının başlangıç yedeğini al.
3. Onaylanmış migration'ları üretim veritabanına uygula.
4. Uygulamayı üretim ortamına dağıt.
5. Alan adı, HTTPS ve yönlendirmeleri doğrula.
6. Ana sayfa, hizmet, makale, iletişim, KVKK ve 404 sayfalarını kontrol et.
7. Gerçek akışa uygun bir test randevusu oluştur; veritabanı ve iki yönlü e-posta bildirimlerini doğrula.
8. Yönetici girişi, randevu durumu ve makale yayımlama akışlarını kontrol et.
9. robots.txt, sitemap, canonical, yapılandırılmış veri ve sosyal paylaşım ön izlemesini doğrula.
10. Hata izleme ve alarm sistemine kontrollü bir test olayı gönder.
11. Test kayıtlarını ve test yöneticilerini güvenli biçimde temizle.
12. Siteyi yayınlandı olarak ilan et ve ilk 24–48 saat yakından izle.

## 18. Geri alma planı

- [ ] Bir önceki kararlı dağıtımın yeniden yayınlanma yöntemini belgele.
- [ ] Geriye uyumsuz migration'ları ayrı ve kontrollü aşamalara böl.
- [ ] Dağıtım öncesi yedeğin konumunu ve geri yükleme yetkisini doğrula.
- [ ] Hangi hata seviyesinde geri alma yapılacağını belirle.
- [ ] Geri alma sonrasında randevu taleplerinin kaybolmadığını kontrol edecek prosedürü hazırla.

## 19. Canlı sonrası işler

### İlk 48 saat

- [ ] Hata oranı, sayfa yanıt süreleri ve e-posta teslimatını izle.
- [ ] Randevu taleplerinin eksiksiz kaydedildiğini kontrol et.
- [ ] Mobil kullanım sorunlarını ve 404 kayıtlarını incele.

### İlk hafta

- [ ] Google Search Console mülkünü doğrula ve sitemap'i gönder.
- [ ] İndeksleme, Core Web Vitals ve yapılandırılmış veri raporlarını kontrol et.
- [ ] İlk yedek geri yükleme testini gerçekleştir.
- [ ] Kullanıcı geri bildirimlerini değerlendir.

### Düzenli bakım

- [ ] Haftalık hata, form ve teslimat kontrolü yap.
- [ ] Aylık bağımlılık ve güvenlik güncellemelerini değerlendir.
- [ ] Aylık SEO ve içerik performansını incele.
- [ ] Düzenli içerik yayın takvimini uygula.
- [ ] Üç aylık erişilebilirlik, performans ve güvenlik kontrolü yap.
- [ ] Veri saklama süresi dolan kayıtları tanımlı prosedürle sil veya anonimleştir.
- [ ] Yedekten geri yükleme testini düzenli aralıklarla tekrarla.

## 20. Önerilen uygulama fazları

| Faz | Çıktı | Tahmini süre |
| --- | --- | --- |
| 1. Keşif ve kapsam | Gereksinimler, içerik listesi, hukuki ihtiyaçlar | 2–4 gün |
| 2. Tasarım | Sayfa taslakları ve tasarım sistemi | 4–7 gün |
| 3. Teknik temel | Next.js, veritabanı, CI ve ortamlar | 2–4 gün |
| 4. Herkese açık site | Temel sayfalar, responsive arayüz ve SEO altyapısı | 5–8 gün |
| 5. Randevu ve yönetim | Form, e-posta, admin ve içerik yönetimi | 5–8 gün |
| 6. Test ve içerik | Testler, güvenlik, performans ve son içerikler | 3–6 gün |
| 7. Canlıya çıkış | Üretim kurulumu, DNS, doğrulama ve izleme | 1–2 gün |

Süreler kapsam, içeriklerin hazır olma durumu ve geri bildirim hızına göre değişir. İlk sürüm için toplam hedef yaklaşık 4–6 haftadır.

## 21. Projenin tamamlanmış sayılma koşulları

- [ ] Onaylı kapsam üretimde çalışıyor.
- [ ] Randevu talebi, bildirim ve yönetim akışları doğrulandı.
- [ ] Yetkisiz erişim ve temel güvenlik senaryoları test edildi.
- [ ] SEO ve erişilebilirlik kontrolleri tamamlandı.
- [ ] KVKK/gizlilik metinleri ve veri süreçleri onaylandı.
- [ ] Yedekleme, geri yükleme, izleme ve geri alma prosedürleri hazır.
- [ ] Teknik kurulum ve yönetici kullanım dokümanları teslim edildi.
- [ ] Canlı sonrası bakım sorumlulukları ve periyodu belirlendi.

