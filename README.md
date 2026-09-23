# ASP.NET Core OTP Authentication System

A secure and structured OTP Authentication System built using **ASP.NET Core MVC, Web API, ASP.NET Core Identity, Entity Framework Core, SQL Server, Bootstrap, and SMTP Email Service**.

The project demonstrates how OTP-based authentication workflows can be implemented securely, including cryptographically secure OTP generation, OTP hashing with salt, expiration handling, previous OTP invalidation, cooldown and rate limiting, OTP verification, email validation, and MVC-to-API integration.

---

## 🚀 Features

* ASP.NET Core MVC
* ASP.NET Core Web API
* ASP.NET Core Identity
* User registration and Identity-based user management
* Secure 6-digit OTP generation
* Cryptographically secure random OTP generation
* OTP hashing using PBKDF2
* Unique salt generation for OTP hashing
* OTP expiration handling
* Previous active OTP invalidation
* Email-based OTP delivery using SMTP
* Email format validation
* OTP purpose validation
* OTP request cooldown
* OTP request rate limiting
* OTP verification
* Expired OTP validation
* Used OTP validation
* Standardized API responses
* Bootstrap-based user interface
* MVC to Web API integration
* Entity Framework Core Code First
* SQL Server database

---

## 🛠️ Technologies Used

* **C#**
* **.NET 10**
* **ASP.NET Core MVC**
* **ASP.NET Core Web API**
* **ASP.NET Core Identity**
* **Entity Framework Core**
* **SQL Server**
* **Bootstrap**
* **SMTP / System.Net.Mail**
* **Swagger / OpenAPI**

---

## 🔐 OTP Security Flow

The OTP workflow follows these steps:

```text
User
 │
 │ Request OTP
 ▼
Email Validation
 │
 ▼
Identity User Check
 │
 ▼
Purpose Eligibility Check
 │
 ▼
Cooldown / Rate Limit Check
 │
 ▼
Invalidate Previous OTP
 │
 ▼
Generate Secure 6-Digit OTP
 │
 ▼
Generate Random Salt
 │
 ▼
Hash OTP using PBKDF2
 │
 ▼
Store Hash + Salt + Expiry
 │
 ▼
Send OTP via Email
 │
 ▼
User Enters OTP
 │
 ▼
Verify OTP
 │
 ├── Expired → Reject
 ├── Already Used → Reject
 ├── Invalid → Reject
 └── Valid → Success
```

---

## 📂 Project Structure

```text
OtpAuthenticationSystem
│
├── Areas
│   └── Identity
│
├── Controllers
│   ├── HomeController.cs
│   └── OtpController.cs
│
├── Data
│   └── ApplicationDbContext.cs
│
├── Models
│   ├── Otp.cs
│   └── SendOtpRequest.cs
│
├── Services
│   ├── OtpService.cs
│   └── EmailService.cs
│
├── Views
│   ├── Home
│   └── Shared
│
├── appsettings.json
├── Program.cs
└── OtpAuthenticationSystem.csproj
```

---

## 🗄️ Database

The project uses **SQL Server** with **Entity Framework Core Code First**.

The OTP data contains information such as:

* User ID
* Email
* OTP Purpose
* OTP Hash
* Salt
* Created Date
* Expiration Date
* Used Status

The actual OTP is not stored directly in the database. Instead, the OTP is securely hashed before storage.

---

## 🔑 OTP Generation

A cryptographically secure random number generator is used to generate a 6-digit OTP.

```csharp
int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
```

This generates an OTP between:

```text
100000 - 999999
```

---

## 🔒 OTP Hashing

The generated OTP is never stored as plain text.

The system uses:

```text
PBKDF2
+
SHA-256
+
Random Salt
```

The database stores the resulting hash and salt instead of the original OTP.

---

## ⏱️ OTP Expiration

Each OTP has a limited validity period.

Current configuration:

```text
OTP Validity = 5 Minutes
```

After expiration, the OTP cannot be successfully verified.

---

## ♻️ Previous OTP Invalidation

When a new OTP is requested for the same user and purpose, previously active OTP records are invalidated.

This prevents multiple previously generated OTPs from remaining active simultaneously.

---

## 📧 Email Delivery

OTP emails are sent using an SMTP email service.

Example:

```text
SMTP Host: smtp.gmail.com
Port: 587
SSL: Enabled
```

The receiver's email address is associated with an existing ASP.NET Identity user.

> **Security Note:** Email credentials and application passwords should never be committed to GitHub. Use secure configuration or environment variables for production applications.

---

## 🌐 API Endpoint

### Send OTP

```http
POST /api/Otp/Send
```

Request:

```json
{
  "email": "user@example.com",
  "purpose": "PasswordReset"
}
```

Successful response:

```json
{
  "message": "OTP Sent successfully"
}
```

---

## 🧪 Testing

The API can be tested using **Swagger / OpenAPI**.

### Send OTP

1. Register a user using ASP.NET Core Identity.
2. Open Swagger.
3. Select:

```text
POST /api/Otp/Send
```

4. Click **Try it out**.
5. Enter the registered user's email.
6. Provide the OTP purpose.
7. Execute the request.
8. Check the registered email inbox for the OTP.

---

## 🎯 Learning Objectives

This project demonstrates practical implementation of:

* ASP.NET Core MVC
* Web API development
* Dependency Injection
* ASP.NET Core Identity
* Entity Framework Core
* SQL Server
* Secure OTP generation
* Password/OTP hashing concepts
* Salt generation
* Email integration
* API validation
* Authentication workflows
* API security concepts
* MVC and API integration

---

## 🔮 Future Improvements

Possible improvements for production-level applications include:

* Secure configuration using environment variables or secret management
* More detailed authentication workflows
* Enhanced logging and monitoring
* Improved email templates
* Additional authentication methods

---

## 👨‍💻 Author

**Shreyash Katiyar**

.NET Developer | ASP.NET Core | C# | Web API | MVC | EF Core | SQL Server

---

## 📜 License

This project is created for learning, practice, and demonstration of ASP.NET Core authentication concepts.
