# Test integracyjny: rejestracja → firma → raport

## Zakres
- xUnit + WebApplicationFactory (net10.0).
- Scenariusz: rejestracja użytkownika → utworzenie firmy → dodanie raportu → weryfikacja na liście.
- SQLite per test (EnsureCreated), kultura ustawiona na Invariant (parse hours 2.5).

## Kroki
1) POST Register (formularz + antiforgery).
2) POST Companies/Create (twórca = Admin).
3) POST Reports/Create (hours 2.5, członkostwo wymagane).
4) GET Reports/Index i asercje: raport zapisany, status „Oczekuje”.

## Uruchomienie
- `dotnet test` w `10xTempo/10xTempo.sln` (build pod net10.0).

## Status
- Zielony (ostatni bieg po zmianach UI i .NET 10).
