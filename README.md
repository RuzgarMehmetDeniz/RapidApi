<img width="1493" height="734" alt="hero" src="https://github.com/user-attachments/assets/0c0658c6-df65-4d9c-b093-56e9110c0cf2" />
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
# <img width="1493" height="734" alt="hero" src="https://github.com/user-attachments/assets/42b4360b-ad68-47f8-8027-922b6eb4cd53" />
# <img width="1364" height="134" alt="Milano" src="https://github.com/user-attachments/assets/01269a0f-6945-4cf7-a2cd-3f4116dca02e" />
# <img width="1018" height="505" alt="Milano2" src="https://github.com/user-attachments/assets/4659c304-08b9-4924-b9e2-db4593bee4d3" />
# <img width="1364" height="505" alt="Milano3" src="https://github.com/user-attachments/assets/e45c65a5-c7f4-4f21-96cf-fd917acf2e3e" />
# <img width="1364" height="486" alt="Listeleme2" src="https://github.com/user-attachments/assets/b3c1f134-e3a9-4827-ae8a-dc4f6b897134" />
# <img width="1364" height="562" alt="Listeleme1" src="https://github.com/user-attachments/assets/820c43b8-fd07-405f-aa3b-6a13d54d88e2" />
# <img width="1364" height="366" alt="AltVeriler" src="https://github.com/user-attachments/assets/11ebab97-ae68-4aa3-8fd2-7ca8d1d4cef5" />
# <img width="1364" height="1516" alt="DetailSayfası" src="https://github.com/user-attachments/assets/321ab52e-0cc7-461d-9b6f-22dbe2e97ef0" />
