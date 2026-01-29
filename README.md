# 10xTempo

Tempo – lekki system raportowania czasu pracy w ASP.NET Core Razor Pages (SQLite, EF Core Code First).

- Logowanie (cookie auth), firmy z GUID, role Admin/Employee, raporty godzin z akceptacją.
- Test integracyjny (xUnit + WebApplicationFactory) odwzorowujący przepływ: rejestracja → utworzenie firmy → dodanie raportu.
- CI/CD: GitHub Actions (restore → build → test).

## Szybki start
1. `cd 10xTempo/10xTempo`
2. `dotnet restore`
3. `dotnet run`

Pierwsze uruchomienie utworzy plik bazy `tempo.db`.

## Testy
`dotnet test 10xTempo/10xTempo.sln`

## CI/CD
Workflow: `.github/workflows/ci.yml` (push/PR na `main`).