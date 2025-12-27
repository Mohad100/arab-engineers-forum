# Fun Forum - ASP.NET Razor Pages Web Application

A playful and friendly entertainment web application built with ASP.NET Core Razor Pages. This application allows users to register, login, and fill out a fun entertainment form to share their personality, favorites, and vibes with the community!

## 🎉 Features

- **User Registration** - Create an account with username, email (optional), and password
- **User Login** - Secure authentication using ASP.NET Cookie Authentication
- **Fun Entertainment Form** - Share your:
  - Name/Nickname
  - Favorite Movie/Song/Game
  - Current Mood (with fun emoji options)
  - Fun Facts About Yourself
  - "Would You Rather" choices
- **Success Page** - Cheerful confirmation after form submission
- **Responsive Design** - Built with Bootstrap 5 for a modern, mobile-friendly experience

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or higher
- Visual Studio 2022, VS Code, or any .NET-compatible IDE

### Installation & Running

1. **Navigate to the project directory:**
   ```powershell
   cd "c:\Users\mash9\OneDrive\المستندات\Fourm"
   ```

2. **Restore dependencies:**
   ```powershell
   dotnet restore
   ```

3. **Run the application:**
   ```powershell
   dotnet run
   ```

4. **Open your browser:**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - The application will automatically open in your default browser

## 📁 Project Structure

```
Fourm/
├── Models/                  # Data models
│   ├── User.cs
│   ├── RegisterModel.cs
│   ├── LoginModel.cs
│   └── FunFormModel.cs
├── Services/                # Business logic services
│   ├── IUserService.cs
│   ├── InMemoryUserService.cs
│   ├── IFunFormService.cs
│   └── InMemoryFunFormService.cs
├── Pages/                   # Razor Pages
│   ├── Index.cshtml         # Home page
│   ├── Register.cshtml      # Registration page
│   ├── Login.cshtml         # Login page
│   ├── FunForm.cshtml       # Main entertainment form
│   ├── Success.cshtml       # Success confirmation
│   ├── Error.cshtml         # Error page
│   └── Shared/
│       └── _Layout.cshtml   # Shared layout
├── wwwroot/                 # Static files
│   └── css/
│       └── site.css         # Custom styles
├── Program.cs               # Application entry point
└── Fourm.csproj            # Project file
```

## 🔐 Authentication

The application uses:
- **Cookie-based authentication** for secure session management
- **In-memory user storage** (for demonstration purposes)
- **Simple password hashing** using SHA256

> **Note:** For production use, replace the in-memory storage with a proper database and use stronger password hashing (e.g., BCrypt, Argon2, or PBKDF2).

## 🎨 Design & Styling

- **Bootstrap 5** for responsive layout
- **Custom CSS** with soft colors and rounded buttons
- **Emoji icons** for a fun, playful interface
- **Clean, modern design** with smooth transitions

## 📝 Pages Overview

### Home (`/Index`)
- Welcome page with cheerful message
- Different content for logged-in vs. guest users
- Call-to-action buttons to register or access the fun form

### Register (`/Register`)
- User registration with validation
- Username (required, 3-50 characters)
- Email (optional, valid email format)
- Password (required, minimum 6 characters)
- Confirm Password (must match)

### Login (`/Login`)
- User authentication
- "Remember Me" option
- Redirects to Fun Form after successful login

### Fun Form (`/FunForm`)
- **Protected page** - requires authentication
- Collects:
  - Nickname
  - Favorite entertainment
  - Current mood (dropdown with emoji options)
  - Fun fact (textarea)
  - Would You Rather choices
- Form validation with friendly error messages

### Success (`/Success`)
- Confirmation page after form submission
- Personalized message with user's nickname
- Options to fill another form or return home

## 💡 Usage Tips

1. **First Time?** Start by registering a new account
2. **Login** with your credentials
3. **Fill the Fun Form** with your personality and favorites
4. **Submit** and see your cheerful confirmation
5. **Fill it again!** You can submit multiple times with different moods

## 🛠️ Technical Notes

- Built with **ASP.NET Core 8.0** Razor Pages
- Uses **Model Binding** for form handling
- **TempData** for passing data between redirects
- **Data Annotations** for validation
- **Dependency Injection** for services
- **Authorization Attributes** to protect pages

## 🌟 Fun Elements

- Playful, friendly language throughout
- Emoji-enhanced UI elements
- Light-hearted form questions
- Cheerful success messages
- Entertainment-focused theme

## 📄 License

This project is for educational and entertainment purposes only.

## 🤝 Contributing

Feel free to fork, modify, and enhance this project! Add more fun features, improve the design, or integrate a real database.

---

**Remember:** This is all for fun! Answer honestly or hilariously — it's your choice! 🎉✨
