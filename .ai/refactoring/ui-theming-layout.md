# Refactoring: UI, theming i layout (Tempo)

## Zakres
- Dark theme z gradientowym tłem, ujednolicone karty (`card`) dla list i formularzy.
- Spójne przyciski (`button/primary/secondary/ghost`) z ikonami dla głównych akcji.
- Header: ukrywanie na landing dla niezalogowanych; prawy align akcji konta; responsywne ograniczenie szerokości (container min(1280px, 100% - 2.5rem)).
- Tabele: wrapper z overflow, paddingi komórek, lepszy kontrast.
- Footer przyklejony do dołu dzięki układowi flex (body + main flex).

## Efekt
- Landing i panele są spójne wizualnie, brak „rozjazdów”.
- Nawigacja i CTA są czytelne (ikony ➕/🔗/🕒, ghost buttons).
- Stopka nie wisi w połowie ekranu przy krótkiej treści.

## Testy/regresja
- `dotnet test` po każdym refaktorze UI (Razor Pages + integracyjny flow) — zielone.

## Do rozważenia w przyszłości
- SVG ikony zamiast emoji w przyciskach.
- Przełącznik trybu jasny/ciemny, jeśli będzie potrzeba.
