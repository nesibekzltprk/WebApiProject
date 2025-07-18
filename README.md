# 👤 ASP.NET Core Web API - Kullanıcı Yönetim Sistemi | User Management API

#📌 Açıklama

Bu proje, ASP.NET Core Web API ile geliştirilmiş bir kullanıcı yönetim sistemidir. SQL Server'daki `User` tablosu ile bağlantılı çalışmakta ve aşağıdaki işlemleri sunmaktadır:

- Kullanıcı ekleme
- Kullanıcıları listeleme
- Kullanıcı güncelleme
- Kullanıcı silme
- Login işlemi ve JWT token üretimi

#🛠️ Teknolojiler

- ASP.NET Core 8 Web API
- SQL Server
- ADO.NET (`SqlConnection`, `SqlCommand`)
- JWT (JSON Web Token)
- Attribute Routing
- [Authorize] ile endpoint koruma

#🔐 Auth Endpoint

- `POST /api/User/login`  
  Kullanıcı adı ve şifre doğrulamasıyla JWT token döner.  

#📋 CRUD Endpoint'leri | CRUD Endpoints

- `GET /api/User/GetAllUsers`  
  Tüm kullanıcıları listeler (**JWT korumalı**)  

- `POST /api/User/AddUser`  
  Yeni kullanıcı ekler  

- `PUT /api/User/UserChanged/{id}`  
  Belirtilen ID’ye sahip kullanıcıyı günceller  

- `DELETE /api/User/DeleteUser/{id}`  
  Belirtilen ID’ye sahip kullanıcıyı siler  

# 🧾 Kullanım

1. `appsettings.json` dosyasına bağlantı cümlesini ve JWT ayarlarını ekleyin:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=YourDbName;Trusted_Connection=True;"
},
"Jwt": {
  "Key": "your_secret_key_here",
  "Issuer": "your_issuer",
  "Audience": "your_audience"
}
SQL Server'da aşağıdaki şemaya uygun bir tablo oluşturun:
CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Password NVARCHAR(100)
);
Uygulamayı başlatın ve Postman veya Swagger ile endpoint'leri test edin.
