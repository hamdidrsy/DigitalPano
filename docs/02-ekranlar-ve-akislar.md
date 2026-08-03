# DigitalPano Ekranları ve Kullanıcı Akışları

## 1. Bilgi mimarisi

```text
DigitalPano
├── /hesap/giris
├── /admin
│   ├── /duyurular
│   │   ├── /yeni
│   │   ├── /duzenle/{id}
│   │   └── /onizleme/{id}
│   ├── /medya
│   ├── /kayan-yazilar
│   ├── /ekranlar
│   ├── /acil-duyuru
│   ├── /ayarlar
│   └── /islem-kayitlari
└── /pano/{slug}
```

URL adları uygulama geliştirilirken ASP.NET yönlendirme kurallarına göre değişebilir; kullanıcı akışı korunacaktır.

## 2. Yönetim paneli menüsü

- Gösterge paneli
- Duyurular
- Medya
- Kayan yazılar
- Ekranlar
- Acil duyuru
- Kurum ayarları
- İşlem kayıtları
- Çıkış

## 3. Yönetim ekranları

### 3.1. Giriş

```text
┌─────────────────────────────────────────┐
│              DIGITALPANO                │
│                                         │
│  E-posta                                │
│  [___________________________________]  │
│                                         │
│  Parola                                 │
│  [___________________________________]  │
│                                         │
│               [ Giriş Yap ]             │
└─────────────────────────────────────────┘
```

### 3.2. Gösterge paneli

```text
┌─────────────┬──────────────────────────────────────────┐
│ Menü        │ DigitalPano                 Yönetici ▼   │
├─────────────┼──────────────────────────────────────────┤
│ Gösterge    │ [Yayında 4] [Planlı 2] [Ekran 3/3]      │
│ Duyurular   │                                          │
│ Medya       │ Yaklaşan yayınlar                        │
│ Kayan Yazı  │ ┌──────────────────────────────────────┐ │
│ Ekranlar    │ │ Başlık      Başlangıç      Ekran    │ │
│ Acil Duyuru │ └──────────────────────────────────────┘ │
│ Ayarlar     │                                          │
└─────────────┴──────────────────────────────────────────┘
```

### 3.3. Duyuru listesi

Filtreler:

- Arama
- Durum: yayında, planlanmış, süresi dolmuş, pasif
- Hedef ekran
- İçerik türü

Satır işlemleri:

- Ön izleme
- Düzenleme
- Aktif/pasif değiştirme
- Silme

### 3.4. Duyuru formu

Alanlar:

- Başlık
- Açıklama
- İçerik türü
- Medya seçme/yükleme
- Başlangıç ve bitiş tarihi
- Gösterim süresi
- Sıra
- Hedef ekranlar
- Aktiflik

Formun yanında veya ayrı sayfada 16:9 pano ön izlemesi gösterilir.

### 3.5. Ekran listesi

Her ekran için:

- Ad ve konum
- Pano adresi
- Aktiflik
- Çevrimiçi durum
- Son bağlantı zamanı
- Düzenle ve adresi kopyala işlemleri

### 3.6. Acil duyuru

```text
┌───────────────────────────────────────────────────────┐
│ ACİL DUYURU                                           │
│ Başlık       [____________________________________]   │
│ Açıklama     [____________________________________]   │
│               [____________________________________]   │
│ Ekranlar      ☑ Tümü  ☐ Giriş  ☐ Öğretmenler Odası   │
│                                                       │
│          [ Ön İzle ]  [ Acil Yayını Başlat ]          │
└───────────────────────────────────────────────────────┘
```

Başlatma işlemi ikinci bir onay penceresi göstermelidir. Aktif yayın varsa aynı ekran “Acil yayını sonlandır” işlevi sunmalıdır.

## 4. Pano yerleşimi

İlk hedef, 1920×1080 yatay ekrandır.

```text
┌──────────────────────────────────────────────────────────────┐
│ LOGO  KURUM ADI                              TARİH — SAAT    │
├───────────────────────────────────────────┬──────────────────┤
│                                           │ HAVA DURUMU      │
│                                           ├──────────────────┤
│        DUYURU / FOTOĞRAF / VİDEO           │ ETKİNLİKLER      │
│                                           ├──────────────────┤
│                                           │ YEMEK / NÖBET    │
├───────────────────────────────────────────┴──────────────────┤
│                    KAYAN DUYURU YAZISI                       │
└──────────────────────────────────────────────────────────────┘
```

İlk uygulamada sağ taraftaki etkinlik, yemek ve nöbet alanları ayrı veri modülü yerine kategorili duyurulardan beslenebilir. Veri yoksa ilgili kutu gizlenebilir ve ana alan genişletilebilir.

## 5. Temel kullanıcı akışları

### 5.1. Duyuru yayımlama

```text
Giriş → Duyurular → Yeni duyuru → İçeriği gir
→ Tarihleri seç → Ekranları seç → Ön izle → Kaydet
→ SignalR bildirimi → Hedef panonun yayını yenilemesi
```

### 5.2. Medyalı duyuru

```text
Yeni duyuru → Görsel/Video seç → Dosyayı doğrula ve yükle
→ Ön izle → Yayın ayarlarını gir → Kaydet → Panoda oynat
```

### 5.3. Acil yayın

```text
Acil duyuru → Mesajı ve ekranları seç → Ön izle
→ İkinci onay → Tam ekran yayın → İşlem kaydı
→ Sonlandır → Normal yayına dönüş
```

### 5.4. Pano cihazı başlatma

```text
Cihaz açılır → Kiosk tarayıcı otomatik başlar → /pano/{slug}
→ Ekran doğrulanır → Aktif yayın alınır → SignalR grubuna katılır
→ Yayın döngüsü başlar → Son bağlantı zamanı güncellenir
```

### 5.5. Bağlantı kesintisi

```text
Bağlantı kaybı → Son uygun yayın devam eder
→ SignalR otomatik yeniden bağlanmayı dener
→ Bağlantı gelir → Güncel veri alınır → Önbellek yenilenir
```

## 6. Arayüz ilkeleri

- Yönetim panelinin dili Türkçe olacaktır.
- Silme ve acil yayın gibi yüksek etkili işlemler onay isteyecektir.
- Başarı ve hata mesajları açık, kısa ve işlemle ilişkili olacaktır.
- Pano ekranında fareyle etkileşim gerekmeyecektir.
- Pano ekranında kaydırma çubuğu, menü veya yönetim bağlantısı bulunmayacaktır.
- Metinler uzadığında düzeni bozmadan sınırlandırılacak veya uygun ölçüde küçültülecektir.
- Video otomatik oynatma uyumluluğu için varsayılan olarak sessiz başlayacaktır.
- Renkler, özellikle acil duyuruda, okunabilir kontrasta sahip olacaktır.
