# Plan implementacji: Companies (lista, tworzenie, dołączanie)

## Zakres
- Lista firm użytkownika z rolą (Admin/Employee).
- Utworzenie nowej firmy z automatycznym nadaniem roli Admin twórcy.
- Dołączenie do firmy po GUID (rola Employee, brak duplikatów).

## Endpoints (propozycja API, spójne z Razor Pages)
- `GET /api/companies`
  - Auth: cookie
  - Response: `{ companies: [ { id, name, role } ] }`
- `POST /api/companies`
  - Body: `{ "name": "string" }` (2–100 znaków)
  - Response 201: `{ id, name, role: "Admin" }`
- `POST /api/companies/join`
  - Body: `{ "companyId": "guid" }`
  - Walidacje: firma istnieje, user nie jest już członkiem.
  - Response 204 lub `{ id, role: "Employee" }`

## Reguły biznesowe
- GUID firmy = `Company.Id` (tworzony przy create).
- Unikalność członkostwa: `(UserId, CompanyId)` unikalne.
- Rola domyślna przy join: Employee.
- Brak kasowania firm w MVP.

## Implementacja (backend)
- DbContext: `Companies`, `UserCompanies` (unikalny index na UserId+CompanyId).
- Tworzenie: add Company, add UserCompany(Admin), SaveChanges.
- Join: znajdź Company po Id, sprawdź istniejące UserCompany, dodaj Employee.

## Testy (API/Razor)
- GET zwraca tylko firmy użytkownika z prawidłową rolą.
- POST /companies tworzy firmę i przypisuje Admina.
- POST /companies/join odrzuca duplikat (409/400) i brakujący GUID (404).
