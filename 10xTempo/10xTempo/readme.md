Tempo – System Raportowania Czasu Pracy (MVP)
Tempo to ekspresowa aplikacja webowa zbudowana w technologii ASP.NET Core Razor Pages. Projekt demonstruje pełny cykl życia danych (CRUD), zaawansowaną logikę biznesową opartą na unikalnych identyfikatorach firm oraz zautomatyzowany proces CI/CD.

🛠️ Architektura Techniczna
Framework: .NET 8 (Razor Pages) – wybrany dla maksymalnej efektywności rozwoju UI i backendu.

Baza Danych: SQLite (lekka, plikowa baza danych, idealna do szybkich wdrożeń MVP).
Konfiguracja: `appsettings.json` uzywa `Data Source=tempo.db`.

ORM: Entity Framework Core (podejście Code First).

Bezpieczeństwo: Wbudowany system Cookie Authentication.

📋 Realizacja Wymogów Certyfikatu
1. Kontrola Dostępu (Access Control)
   System logowania oparty na sesji/cookie.

Użytkownik musi być zalogowany, aby raportować czas lub zarządzać firmą.

2. Zarządzanie Danymi (CRUD)
   Aplikacja zarządza czterema kluczowymi encjami:

Użytkownicy (Users): Rejestracja i autentykacja.

Firmy (Companies): Tworzenie firmy (generowanie GUID) lub dołączanie do istniejącej.

Relacje (User_Companies): Powiązanie użytkownika z wieloma firmami.

Raporty (Reports): Pełny CRUD wpisów czasu pracy (decimal 18,2, DateTimeOffset).

3. Logika Biznesowa
   GUID Onboarding: Dołączenie do firmy odbywa się wyłącznie przez unikalny klucz GUID, co eliminuje potrzebę ręcznego zapraszania pracowników przez administratora.

Walidacja raportów: Automatyczne ustawianie daty utworzenia/edycji oraz flaga is_approved.

4. Testy (User Perspective)
   Zaimplementowano test integracyjny (xUnit + Playwright/AngleSharp), który symuluje proces: Logowanie -> Dodanie raportu -> Weryfikacja obecności raportu na liście.

5. Pipeline CI/CD
   Skonfigurowany przepływ GitHub Actions, który przy każdym git push:

Przywraca pakiety NuGet.

Buduje aplikację.

Uruchamia testy automatyczne.

📋 Funkcje UI
- Strona startowa z CTA do rejestracji/logowania.
- Lista firm użytkownika + tworzenie firmy (twórca = Admin).
- Dołączanie do firmy po GUID (rola Employee).
- Raportowanie godzin do wybranej firmy + podgląd własnych raportów.
- Panel zatwierdzania raportów dla Adminów (firmy, w których użytkownik ma rolę Admin).

📐 Model danych
- `Company`: `Id (Guid)`, `Name`.
- `UserCompany`: `Id`, `UserId (string)`, `CompanyId (Guid)`, `Role (Admin/Employee)`.
- `Report`: `Id`, `UserId (string)`, `CompanyId (Guid)`, `Hours (decimal 18,2)`, `CreatedOn`, `UpdatedOn`, `IsApproved`.

▶️ Uruchomienie lokalne
1) `cd 10xTempo/10xTempo`
2) `dotnet restore`
3) `dotnet run` (baza SQLite `tempo.db` tworzy się automatycznie, kultura ustawiona na `Invariant` dla spójnego parsowania liczb).

🧪 Test
`dotnet test 10xTempo/10xTempo.sln` – integracyjny przebieg: rejestracja → utworzenie firmy → dodanie raportu → weryfikacja.

⚙️ CI/CD
Workflow: `.github/workflows/ci.yml` (restore → build → test na push/PR do `main`).
