# DB Plan — Tempo MVP (SQLite, .NET 10)

## 1. Tabele i kolumny

### Users (Identity)
- `Id` TEXT PRIMARY KEY (GUID, IdentityUser)
- `Email` TEXT NOT NULL UNIQUE
- `PasswordHash` TEXT NOT NULL

### Companies
- `Id` TEXT PRIMARY KEY (GUID)
- `Name` TEXT NOT NULL

### UserCompanies
- `Id` INTEGER PRIMARY KEY AUTOINCREMENT
- `UserId` TEXT NOT NULL (FK -> Users.Id)
- `CompanyId` TEXT NOT NULL (FK -> Companies.Id)
- `Role` TEXT NOT NULL CHECK (Role IN ('Admin','Employee'))
- UNIQUE (`UserId`, `CompanyId`)

### Reports
- `Id` INTEGER PRIMARY KEY AUTOINCREMENT
- `UserId` TEXT NOT NULL (FK -> Users.Id)
- `CompanyId` TEXT NOT NULL (FK -> Companies.Id)
- `Hours` DECIMAL(18,2) NOT NULL
- `CreatedOn` TEXT NOT NULL
- `UpdatedOn` TEXT NOT NULL
- `IsApproved` INTEGER NOT NULL DEFAULT 0

## 2. Relacje
- Users 1:N UserCompanies
- Companies 1:N UserCompanies
- Users 1:N Reports
- Companies 1:N Reports

## 3. Indeksy
- Users: UNIQUE index na `Email`.
- UserCompanies: UNIQUE index na (`UserId`, `CompanyId`), dodatkowy index na `CompanyId`.
- Reports: index na `CompanyId`, index na `UserId`.

## 4. Zasady biznesowe w bazie
- Dolaczanie do firmy po GUID: GUID firmy to `Companies.Id`.
- `IsApproved` domyslnie `0` (false).
- `CreatedOn` / `UpdatedOn` ustawiane automatycznie w aplikacji (EF Core).

## 5. Uwagi implementacyjne (SQLite)
- SQLite nie ma natywnego typu GUID ani DECIMAL; EF Core mapuje GUID do TEXT, a decimal do NUMERIC.
- Wlaczyc `PRAGMA foreign_keys = ON`.
- Migracje EF Core utrzymuja schemat (Code First).
