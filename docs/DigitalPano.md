Bizim geliştireceğimiz sistem

Projeyi şimdilik Dijital Pano olarak adlandırabiliriz.

1. Yönetim paneli

Yetkili kullanıcı sisteme giriş yapacak ve şu işlemleri gerçekleştirecek:

Yeni duyuru ekleme
Duyuru düzenleme ve silme
Görsel veya video yükleme
Kayan yazı oluşturma
Yayın başlangıç ve bitiş tarihi seçme
Duyurunun hangi ekranlarda gösterileceğini belirleme
Kurum logosu ve renklerini değiştirme
Yayındaki duyuruları ön izleme
Acil duyuruyu tam ekran yayınlama

Örnek:

Başlık: 30 Ağustos Etkinliği

Açıklama:
30 Ağustos Cuma günü saat 10.00'da tören yapılacaktır.

Başlangıç:
27 Ağustos 2026 - 08.00

Bitiş:
30 Ağustos 2026 - 15.00

Gösterilecek ekranlar:
Giriş Katı, Öğretmenler Odası

Bitiş tarihi geldiğinde duyuru otomatik olarak yayından kalkacak.

2. Pano ekranı

Televizyonda aşağıdaki gibi özel bir adres açılacak:

dijitalpano.com/pano/giris-kati

Bu sayfada menü, giriş butonu veya yönetim seçenekleri olmayacak. Yalnızca tam ekran yayın gösterilecek.

Önerdiğim ekran düzeni:

┌───────────────────────────────────────────────────────┐
│ LOGO     KURUM ADI                  TARİH – SAAT       │
├───────────────────────────────────┬───────────────────┤
│                                   │ HAVA DURUMU       │
│                                   ├───────────────────┤
│     DUYURU / FOTOĞRAF / VİDEO     │ ETKİNLİK          │
│                                   ├───────────────────┤
│                                   │ YEMEK / NÖBET     │
├───────────────────────────────────┴───────────────────┤
│          KAYAN DUYURU YAZISI                          │
└───────────────────────────────────────────────────────┘

Ana bölümde içerikler örneğin 10 saniyede bir değişebilir.

İlk sürümde bulunması gereken modüller
Temel MVP

İlk çalışan sürüm için şunlar yeterli:

Modül	Görevi
Kullanıcı girişi	Yönetim panelini korur
Duyuru yönetimi	Yazılı duyuru ekler
Medya yönetimi	Fotoğraf ve video yükler
Slider	İçerikleri sırayla gösterir
Yayın tarihleri	Otomatik başlatır ve bitirir
Kayan yazı	Alt duyuru bandını oluşturur
Saat ve tarih	Canlı olarak gösterilir
Hava durumu	Seçilen şehri gösterir
Kurum ayarları	Logo, kurum adı ve renkler
Ekran yönetimi	Farklı TV’leri ayrı yönetir
Acil duyuru	Bütün ekranı kaplayan bildirim

Bu sürüm gerçekten kullanıma sunulabilecek bir dijital pano olur.

Teknik yapı

Senin için en uygun yapı şu olur:

ASP.NET Core MVC
Entity Framework Core
SQL Server
HTML + CSS + JavaScript
SignalR
Bootstrap

ASP.NET Core MVC

Yönetim paneli, kullanıcı işlemleri, duyuru ekleme ve veritabanı işlemlerini yönetecek.

Entity Framework Core

Duyuruları, ekranları, kullanıcıları ve kurum ayarlarını SQL Server veritabanına kaydedecek.

SignalR

Yönetici yeni bir duyuru yayınladığında televizyon sayfasının manuel yenilenmesine gerek kalmadan pano anında güncellenecek. SignalR sunucunun bağlı ekranlara gerçek zamanlı içerik göndermesi için kullanılabilir.

Service Worker

İnternet geçici olarak kesildiğinde pano tamamen siyah kalmamalı. Son indirilen duyurular ve görseller cihazda önbelleğe alınarak yayın devam ettirilebilir. Service Worker teknolojisi, uygulama dosyalarını ve içerikleri tarayıcı önbelleğinde tutarak çevrimdışı çalışmaya yardımcı olur.

Veritabanı tabloları

Başlangıçta şu entity’ler yeterli olur:

AppUser
Institution
Screen
Announcement
AnnouncementScreen
Media
TickerMessage
ThemeSetting
ActivityLog
Announcement
public class Announcement
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int DisplayDuration { get; set; }

    public bool IsActive { get; set; }

    public bool IsEmergency { get; set; }
}
Screen
public class Screen
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ScreenKey { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime? LastConnectionDate { get; set; }
}

ScreenKey televizyonun hangi panoya ait olduğunu belirleyecek.

Televizyonda nasıl çalışacak?

Üç farklı kurulum seçeneği var:

Akıllı televizyon

TV’nin tarayıcısında pano adresi açılır ve tam ekran yapılır.

Android TV Box

TV’ye Android TV Box bağlanır. Pano sayfası cihaz açıldığı anda otomatik çalıştırılır.

Mini bilgisayar

TV’ye HDMI ile mini bilgisayar veya Raspberry Pi bağlanır. Bilgisayar açılınca Chrome kiosk modunda pano adresi otomatik açılır.

En güvenilir seçenek genellikle Android TV Box veya mini bilgisayar olur. Çünkü bazı akıllı televizyonların tarayıcıları eski olabilir ve video, önbellek veya tam ekran konusunda sorun çıkarabilir.

Projeyi geliştirme sırası
Birinci aşama — Temel pano
Proje oluşturma
Veritabanı bağlantısı
Announcement entity
Yönetim paneli
Duyuru ekleme, listeleme, düzenleme ve silme
Basit pano ekranı
İkinci aşama — Görsel yayın
Fotoğraf yükleme
Video yükleme
Slider
Gösterim süresi
Başlangıç ve bitiş tarihleri
Kayan yazı

ASP.NET Core küçük dosyalar için standart yükleme, daha büyük dosyalar için akış tabanlı yükleme seçenekleri sunuyor; özellikle video yüklemesinde dosya boyutlarını ve depolama şeklini baştan sınırlandırmak gerekir.

Üçüncü aşama — Gerçek zamanlı sistem
SignalR bağlantısı
Pano ekranını anında güncelleme
Ekranın çevrimiçi durumunu izleme
Son bağlantı tarihini gösterme
Dördüncü aşama — Profesyonel kullanım
Çoklu ekran
Kullanıcı rolleri
Kuruma özel tema
Acil duyuru
Çevrimdışı çalışma
Yayın geçmişi
İşlem kayıtları

İlk sürümde yapmamamız gerekenler

Başlangıçta şunları eklememeliyiz:

Android ve iOS mobil uygulama
Push bildirim sistemi
Yapay zekâyla otomatik duyuru oluşturma
Öğrenci ve veli hesapları
Detaylı ders programı entegrasyonu
Çok kiracılı ücretli üyelik sistemi
Sürükle-bırak ekran tasarım editörü

Bunlar yanlış özellikler değil; yalnızca ilk sürüm için erken. Önce tek kurumda çalışan, yönetim panelinden kontrol edilen sağlam bir pano çıkarmalıyız.

lk kritik kararımız şu: Dijital Pano yalnızca okullar için mi olacak, yoksa belediye, hastane, işletme ve kurs merkezi gibi bütün kurumlara mı hitap edecek?
bu sorunun cevabı özel eğitim kursunda sadece ekranda gözükecek 
