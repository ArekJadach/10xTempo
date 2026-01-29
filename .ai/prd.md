# PRD — Tempo: System Raportowania Czasu Pracy (MVP)

## Cel produktu
Tempo to szybka aplikacja webowa do raportowania czasu pracy. Celem MVP jest umożliwienie logowania, zarządzania firmami oraz raportowania godzin z prostą obsługą ról i akceptacji raportów.

## Problem
Potrzebny jest lekki system, w ktorym uzytkownik moze raportowac czas do jednej lub wielu firm, a firma moze weryfikowac i zatwierdzac raporty.

## Zakres MVP
- Rejestracja i logowanie (cookie auth).
- Tworzenie firmy oraz dolaczanie do firmy po unikalnym GUID.
- Role w firmie: admin i pracownik.
- Zarzadzanie uzytkownikami w firmie przez admina.
- Raportowanie godzin do wybranej firmy (takze do swojej).
- Zatwierdzanie raportow innych uzytkownikow przez admina danej firmy.
- Test integracyjny z perspektywy uzytkownika.
- CI/CD: build + testy na push.

## Poza zakresem
- Rozbudowane role i polityki uprawnien.
- Eksporty, raporty zbiorcze, integracje zewnetrzne.
- Rozbudowana personalizacja UI/UX.

## Role i uprawnienia
- Uzytkownik: widzi firmy, do ktorych nalezy, tworzy raporty i przeglada swoje raporty.
- Pracownik firmy: raportuje godziny do firmy, widzi status akceptacji swoich raportow.
- Admin firmy: zarzadza uzytkownikami w firmie oraz zatwierdza raporty innych uzytkownikow dla tej firmy.

## User stories
- Jako uzytkownik chce sie zarejestrowac i zalogowac, aby uzyskac dostep do firm i raportow.
- Jako uzytkownik chce widziec liste firm, do ktorych naleze, oraz wybrac firme do raportu.
- Jako uzytkownik chce dolaczyc do firmy po GUID, aby moc raportowac czas dla tej firmy.
- Jako uzytkownik chce raportowac godziny do wybranej firmy (takze do swojej).
- Jako pracownik chce widziec status akceptacji swoich raportow.
- Jako admin chce zarzadzac uzytkownikami w swojej firmie (dodawac/odbierac role).
- Jako admin chce zatwierdzac raporty innych uzytkownikow dla swojej firmy.

## Zasady biznesowe
- Dolaczanie do firmy odbywa sie przez GUID.
- Raport zawsze nalezy do jednej firmy i jednego uzytkownika.
- Admin moze zatwierdzac raporty tylko dla firm, w ktorych jest adminem.
- Uzytkownik moze raportowac godziny do firm, do ktorych nalezy.

## Wymagania funkcjonalne
- Uwierzytelnianie oparte o cookie.
- CRUD dla:
  - Users
  - Companies
  - UserCompanies (wraz z rola uzytkownika w firmie)
  - Reports
- Automatyczne `CreatedOn` / `UpdatedOn` dla raportow.
- `Hours` jako `decimal(18,2)`.
- `IsApproved` domyslnie `false`.

## Wymagania niefunkcjonalne
- .NET 10, Razor Pages.
- EF Core Code First.
- SQLite (docelowo dla MVP).
- Responsywny, czytelny interfejs.

## Model danych (skrot)
- Company: `Id (Guid)`, `Name`
- User: `Id`, `Email`, `PasswordHash`
- UserCompany: `Id`, `UserId`, `CompanyId`, `Role` (Admin/Employee)
- Report: `Id`, `UserId`, `CompanyId`, `Hours`, `CreatedOn`, `UpdatedOn`, `IsApproved`

## Testy
- 1 test integracyjny (xUnit + Playwright/AngleSharp):
  - Logowanie -> dodanie raportu -> weryfikacja obecnosci raportu.

## CI/CD
- GitHub Actions:
  - restore NuGet
  - build
  - test

## Status wdrożenia (MVP)
- UI Razor Pages: logowanie, firmy z GUID, role Admin/Employee, raporty + zatwierdzanie.
- Baza: SQLite (`tempo.db`), EF Core Code First, kultura ustawiona na Invariant dla spójnego parsowania godzin.
- Test: xUnit + WebApplicationFactory (rejestracja → firma → raport → weryfikacja).
- CI: GitHub Actions (`.github/workflows/ci.yml`) – restore, build, test na push/PR do `main`, target .NET 10.

## Metryki sukcesu
- Uzytkownik moze dodac raport w < 2 min od zalogowania.
- 0 bledow w podstawowym przeplywie CRUD.
- Test integracyjny przechodzi w CI/CD.

## Ryzyka i zaleznosci
- Spojnosc konfiguracji bazy (SQLite vs inne).
- Autoryzacja dostepu do danych na poziomie Razor Pages.
