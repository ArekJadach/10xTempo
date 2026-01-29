# API Plan — Tempo MVP

## 1. Zalozenia
- REST API wspierajace Razor Pages (cookie auth).
- Wszystkie endpointy poza rejestracja/logowaniem wymagaja uwierzytelnienia.
- Format JSON, kodowanie UTF-8.
- Baza danych: SQLite (EF Core Code First).

## 2. Zasoby
- **auth**: rejestracja, logowanie, wylogowanie.
- **users**: dane zalogowanego uzytkownika.
- **companies**: firmy uzytkownika, tworzenie i dolaczanie przez GUID.
- **company-members**: zarzadzanie rolami w firmie (admin).
- **reports**: raportowanie godzin i akceptacja raportow.

## 3. Endpointy

### Auth
- **POST** `/api/auth/register`
  - Tworzy konto, zapisuje haslo jako hash.
  - Request:
    ```json
    { "email": "string", "password": "string" }
    ```
  - Response 201:
    ```json
    { "id": "guid", "email": "string" }
    ```

- **POST** `/api/auth/login`
  - Ustawia cookie auth.
  - Request:
    ```json
    { "email": "string", "password": "string" }
    ```
  - Response 200:
    ```json
    { "id": "guid", "email": "string" }
    ```

- **POST** `/api/auth/logout`
  - Czyści cookie auth.
  - Response 204.

### Users
- **GET** `/api/users/me`
  - Zwraca dane zalogowanego uzytkownika.
  - Response 200:
    ```json
    { "id": "guid", "email": "string" }
    ```

### Companies
- **GET** `/api/companies`
  - Lista firm, do ktorych nalezy uzytkownik.
  - Response 200:
    ```json
    { "companies": [ { "id": "guid", "name": "string" } ] }
    ```

- **POST** `/api/companies`
  - Tworzy firme; tworca otrzymuje role Admin.
  - Request:
    ```json
    { "name": "string" }
    ```
  - Response 201:
    ```json
    { "id": "guid", "name": "string" }
    ```

- **POST** `/api/companies/join`
  - Dolacza uzytkownika do firmy po GUID.
  - Request:
    ```json
    { "companyGuid": "guid" }
    ```
  - Response 204.

### Company Members (Admin)
- **GET** `/api/companies/{companyId}/members`
  - Lista uzytkownikow firmy z rolami.
  - Response 200:
    ```json
    { "members": [ { "userId": "guid", "email": "string", "role": "Admin|Employee" } ] }
    ```

- **PATCH** `/api/companies/{companyId}/members/{userId}`
  - Zmiana roli uzytkownika w firmie.
  - Request:
    ```json
    { "role": "Admin|Employee" }
    ```
  - Response 200.

### Reports
- **POST** `/api/reports`
  - Tworzy raport godzin.
  - Request:
    ```json
    { "companyId": "guid", "hours": 7.5 }
    ```
  - Response 201:
    ```json
    { "id": "guid", "companyId": "guid", "userId": "guid", "hours": 7.5, "isApproved": false }
    ```

- **GET** `/api/reports`
  - Lista raportow uzytkownika; admin moze pobrac raporty firmy.
  - Query: `companyId` (opcjonalnie, tylko admin firmy)
  - Response 200:
    ```json
    { "reports": [ { "id": "guid", "companyId": "guid", "userId": "guid", "hours": 7.5, "createdOn": "timestamp", "updatedOn": "timestamp", "isApproved": false } ] }
    ```

- **POST** `/api/reports/{id}/approve`
  - Akceptacja raportu przez admina firmy.
  - Response 204.

## 4. Autoryzacja i reguly dostepu
- Cookie auth; brak dostepu anonimowego poza rejestracja/logowaniem.
- Uzytkownik widzi tylko firmy, do ktorych nalezy.
- Raporty: pracownik widzi swoje; admin firmy widzi raporty firmy i moze je akceptowac.

## 5. Walidacje
- Email unikalny, format sprawdzany w aplikacji.
- Password min. 8 znakow (doprecyzowac w implementacji).
- Hours > 0 oraz format decimal(18,2).
- companyGuid musi istniec i uzytkownik nie moze byc przypisany podwojnie.
