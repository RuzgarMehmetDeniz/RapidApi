# 🌍 Atlas: API-Driven Real-Time Dashboard

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core_8.0-black?style=for-the-badge&logo=dotnet)
![RapidAPI](https://img.shields.io/badge/Data_Source-RapidAPI-blue?style=for-the-badge)
![Status](https://img.shields.io/badge/Architecture-Modular_&_Dynamic-green?style=for-the-badge)

**Atlas**, modern web ekosistemindeki çeşitli veri servislerini tek bir noktada birleştiren, tamamen gerçek zamanlı verilerle beslenen dinamik bir yönetim panelidir. Proje, harici bir veritabanı (SQL) gereksinimi duymadan, veriyi doğrudan global RESTful servislerden tüketerek işleme ve görselleştirme yeteneğine odaklanır.

## 🛠 Teknik Yaklaşım ve Mimari

Proje, servis odaklı bir mantıkla modüler bir yapıda inşa edilmiştir:

* **Merkezi Veri Yönetimi:** Tüm dış veri akışları `IHttpClientFactory` üzerinden yönetilerek performanslı ve güvenli bağlantılar kurulmuştur.
* **Dinamik Veri İşleme:** API'lerden gelen farklı şemalardaki JSON çıktıları, projenin ihtiyaç duyduğu veri modellerine (DTO) asenkron olarak map edilmiştir.
* **Hata Yönetimi ve Stabilite:** API tarafında oluşabilecek veri eksiklikleri veya gecikmelere karşı `fallback` mekanizmaları kullanılarak arayüz bütünlüğü korunmuştur.

## 📊 Entegre Edilen Canlı Veri Servisleri

Atlas paneli, 4 farklı dikeyde anlık veri akışı sağlamaktadır:

* **Finansal Göstergeler (Currency API):** USD, EUR ve GBP birimlerinin Türk Lirası karşısındaki anlık değerleri ve piyasa takibi.
* **Hava Durumu Servisi (Weather API):** Lokasyon bazlı anlık sıcaklık, nem, rüzgar hızı ve durum ikonlarıyla zenginleştirilmiş hava tahmini.
* **Enerji Piyasası (Gas Price API):** Türkiye akaryakıt piyasasındaki anlık Benzin, Motorin ve LPG fiyatları. (Global Euro verileri, anlık kur üzerinden TL'ye normalize edilerek sunulmaktadır).
* **Kripto Varlık Paneli (Coinranking API):** Bitcoin ve Ethereum için anlık borsa fiyatları ve 24 saatlik değişim analizleri.

## 🎨 UI/UX Tasarım Detayları

* **Premium Dashboard Arayüzü:** SaaS tasarım trendlerine uygun, temiz ve kullanıcı odaklı widget yapıları.
* **Veri Görselleştirme:** Akaryakıt doluluk oranları için dinamik ilerleme çubukları (Progress Bars).
* **Akıllı Renklendirme:** Piyasa verilerindeki artış ve azalış durumlarının dinamik CSS sınıflarıyla anlık işaretlenmesi.
* **Modern Estetik:** Cam efekti (Glassmorphism) ve modern kart tasarımlarıyla zenginleştirilmiş görsel dil.

## 💻 Kullanılan Teknolojiler

* **Framework:** .NET 8.0
* **Dil:** C#
* **Arayüz Teknolojisi:** Razor Pages & ViewComponents
* **Veri Formatı:** JSON / REST Services
* **Kütüphaneler:** `Newtonsoft.Json`, `Microsoft.Extensions.Http`

---

# <img width="1361" height="2834" alt="List" src="https://github.com/user-attachments/assets/e12f0abc-72c6-43ae-9581-2fc882d2df66" />
# <img width="1361" height="2226" alt="Detail" src="https://github.com/user-attachments/assets/8d83243d-c5cd-442f-b111-7a91e3065c90" />
