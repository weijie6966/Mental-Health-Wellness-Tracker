🚀 Development Progress Report: Mental Health Wellness Tracker (Backend & Core Architecture)

**📅 Status:** Core features complete / Key architecture operational
**🏗️ Architecture Pattern:** MVVM + Repository Pattern + Offline-First
**☁️ Tech Stack:** .NET MAUI, SQLite (Local), Firebase Firestore (Cloud via REST API)

#### ✅ Key Achievements

**1. 🔐 Authentication System**
* **Features:** Fully functional Registration, Login, and Forgot Password (via email reset) flows.
* **Security:** User Token and ID are securely stored using `SecureStorage`.
* **Highlight:** Implemented custom exception handling to translate complex backend error codes (like `INVALID_EMAIL`) into user-friendly messages.

**2. 💾 Dual-Layer Data Architecture (Local + Cloud)**
* **Local-First:** All data (Assessments, Diaries, Profiles) is written to **SQLite** first, ensuring the app works perfectly without internet.
* **Cloud Sync:** Implemented a background silent synchronization mechanism that automatically uploads local data to **Firebase Firestore** when online.
* **Technical Breakthrough:** utilized native `HttpClient` (REST API) instead of the heavy SDK, successfully resolving complex API Key permission issues (`CONSUMER_INVALID`) and configured proper Firestore security rules.

**3. 🌍 Community & Diary Features**
* **Diary:** Users can write diaries, select mood Emojis, and data is automatically saved and synced.
* **Global Feed:** Implemented cloud data fetching (`GET`). The Community page now displays a **real-time global feed** of diaries from all users, not just local data.
* **Data Flow:** Fixed UserID and Username binding logic to ensure every post correctly displays the author's name.

**4. 👤 User Profile Management**
* **Independent Storage:** Upgraded user profile storage from temporary `Preferences` to a dedicated **SQLite database table (`UserProfile`)**.
* **Account Isolation:** Implemented data isolation based on UserID. When switching accounts, the Profile page automatically loads the correct avatar and bio for the logged-in user without conflict.

**5. 📝 Assessment & History**
* Implemented dynamic questionnaire loading (supporting reverse scoring logic for Rosenberg/PSS scales).
* The History page (`AnalyticPage`) now retrieves and displays the user's past assessment scores directly from the database.

**6. 🛠️ Engineering & Code Quality**
* **Dependency Injection (DI):** Fully adopted Constructor Injection to fix navigation crash risks associated with the Service Locator pattern.
* **Standardization:** Core logic code has been standardized with English comments for better team collaboration.
