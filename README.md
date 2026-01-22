# AlPaSa - İkinci El Eşya Satış Platformu (Al, Pazarla, Sat)

**Ders:** ISE309 - WEB PROGRAMLAMA  
**Öğrenci:** B221200054 - Yiğit Kalaycıoğlu  
**Dönem:** 2025-2026 Güz

AlPaSa, kullanıcıların kullanmadıkları eşyaları satabilecekleri, ihtiyaç duydukları ürünleri satın alabilecekleri ve satıcılarla iletişime geçebilecekleri modern bir ASP.NET Core MVC projesidir.

---

## Projenin Amacı ve Kapsamı
Bu proje, **Entity Framework Core (Code-First)** yaklaşımı kullanılarak, katmanlı mimariye (MVC) ve **Repository Design Pattern** prensiplerine uygun olarak geliştirilmiştir. Kullanıcı güvenliği için **ASP.NET Core Identity** kütüphanesi entegre edilmiştir.

## Kullanılan Teknolojiler ve Mimari
* **Platform:** ASP.NET Core 10.0
* **Mimari:** MVC (Model-View-Controller)
* **Veritabanı:** SQLite / Entity Framework Core (Code-First & Migrations)
* **Tasarım:** Bootstrap 5, Custom CSS, jQuery
* **Desenler:** Repository Pattern, Dependency Injection, ViewModels

---

## Özellikler

### 1. Temel Özellikler
- [x] **Kimlik Doğrulama (Identity):** Kullanıcı kayıt olma, giriş yapma ve çıkış işlemleri.
- [x] **Yetkilendirme (Authorization):** Rol bazlı yönetim (Admin ve User rolleri).
- [x] **Ürün Yönetimi:**
    - Ürün ekleme (Resim yükleme desteği ile).
    - Ürün düzenleme ve silme (Sadece ürün sahibi veya Admin yapabilir).
    - Vitrin sayfası ve ürün detayları.
- [x] **Kategori Yönetimi:** Admin tarafından kategori ekleme, silme ve düzenleme.

### 2. Bonus Özellikler (Ekstra Geliştirmeler)
Proje isterlerine ek olarak aşağıdaki özellikler geliştirilmiştir:

1.  **Mesajlaşma Sistemi (Messaging System):**
    - Kullanıcılar satıcılara ürün detay sayfasından mesaj gönderebilir.
    - Gelen ve Giden kutusu üzerinden mesajlaşma geçmişi görüntülenebilir.
2.  **Dinamik Favori Sistemi (AJAX):**
    - Kullanıcılar sayfayı yenilemeden (AJAX ile) ürünleri favorilerine ekleyip çıkarabilir.
    - Favori butonu anlık olarak durum değiştirir (Dolu/Boş kalp).
3.  **Recursive (İç İçe) Kategori Yapısı:**
    - Sınırsız derinlikte alt kategori desteği (Örn: Elektronik > Bilgisayar > Donanım > Ekran Kartı).
    - Ana kategori seçildiğinde, alt kategorilerdeki ürünlerin de listelenmesini sağlayan akıllı filtreleme algoritması.
    - Sidebar'da açılır/kapanır (Accordion) modern kategori ağacı görünümü.
4.  **Karanlık Mod (Dark Mode):**
    - Kullanıcı tercihine göre Aydınlık/Karanlık tema seçeneği.
    - Tercih tarayıcı hafızasında (LocalStorage) saklanır, sayfa yenilendiğinde kaybolmaz.
5.  **Modern UI/UX Tasarımı:**
    - Glassmorphism efektleri, yumuşak gölgeler ve animasyonlu geçişler.
    - Tamamen responsive (mobil uyumlu) tasarım.
6.  **Ana Sayfada Popüler Ürün Gösterimi**
    - Bir fonksiyon kullanılarak en çok favorilenen ürünün ana sayfada gösterimi.

---

## Kurulum ve Çalıştırma Talimatları

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları sırasıyla uygulayın:

1.  **Repoyu Klonlayın:**
    Terminali açın ve projeyi bilgisayarınıza indirin:
    ```bash
    git clone https://github.com/SauWebProgramming/web-programming-project-2025-yigitkalaycioglu.git
    ```

2.  **Proje Dizinine Girin:**
    İndirilen klasörün içine girmeniz gerekmektedir:
    ```bash
    cd web-programming-project-2025-yigitkalaycioglu
    ```
    *(Not: Klasör ismi repoya göre değişiklik gösterebilir, lütfen kontrol edin.)*

3.  **Gerekli Paketleri Yükleyin (NuGet Restore):**
    Projenin bağımlılıklarını (NuGet paketlerini) indirmek için şu komutu çalıştırın:
    ```powershell
    dotnet restore
    ```

4.  **Veritabanını Oluşturun:**
    Proje dizininde terminal üzerinden şu komutu çalıştırarak SQLite veritabanını oluşturun:
    ```powershell
    dotnet ef database update
    ```
    *Eğer Visual Studio kullanıyorsanız, `Package Manager Console` üzerinden `Update-Database` komutunu da kullanabilirsiniz.*

5.  **Projeyi Başlatın:**
    ```powershell
    dotnet run
    ```
    Komut çalıştıktan sonra terminalde verilen `http://localhost:xxxx` adresine tarayıcınızdan gidin.

6.  **Admin Girişi (Varsayılan):**
    Proje ilk açıldığında aşağıdaki bilgilerle giriş yapabilirsiniz:
    * **Email:** admin@sakarya.edu.tr
    * **Şifre:** Sau.123!

---

## Proje Yapısı (Özet)

* `Controllers/`: Uygulama mantığını yöneten sınıflar.
* `Models/`: Veritabanı varlıkları (Entity).
* `ViewModels/`: View'lara özgü veri taşıyıcı modeller.
* `Repositories/`: Veri erişim katmanı (IProductRepository vb.).
* `Views/`: Kullanıcı arayüzü dosyaları (.cshtml).
* `wwwroot/`: CSS, JS ve Ürün Resimleri.

---

**© 2025 AlPaSa - Tüm Hakları Saklıdır.**
