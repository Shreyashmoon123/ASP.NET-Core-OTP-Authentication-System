# 🔐 ASP.NET Core OTP Authentication System

A practical and secure **OTP Authentication System** built with **ASP.NET Core MVC, Integrated Web API, ASP.NET Core Identity, Entity Framework Core, SQL Server, Bootstrap, and SMTP Email Service**.

The project demonstrates a complete OTP workflow including secure OTP generation, hashing with salt, email delivery, expiration, verification, cooldown, rate limiting, and purpose-based validation.

---

## 🚀 Project Overview

This application combines **ASP.NET Core MVC and Web API in a single project**.

The MVC layer provides the user interface, while the integrated Web API handles OTP generation, storage, email delivery, and verification.

**ASP.NET Core Identity** is used for user management, **Entity Framework Core** handles database operations, and **SQL Server** is used as the database.

### 🔄 OTP Workflow

```text
User
 │
 ▼
Bootstrap MVC UI
 │
 ▼
Enter Email + OTP Purpose
 │
 ▼
MVC Controller
 │
 ▼
OTP API
 │
 ├── Validate Email
 ├── Validate OTP Purpose
 ├── Check Cooldown
 ├── Check Rate Limit
 ├── Invalidate Previous OTP
 ├── Generate Secure 6-Digit OTP
 ├── Hash OTP + Salt
 ├── Store OTP
 └── Send OTP through Email
 │
 ▼
User Receives OTP
 │
 ▼
Enter 6-Digit OTP
 │
 ▼
OTP Verify API
 │
 ├── Find Active OTP
 ├── Check Expiration
 ├── Verify OTP Hash
 └── Mark OTP as Used
 │
 ▼
OTP Verified Successfully
