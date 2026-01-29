# Widoki: Companies / Reports / Approvals

## Companies
- Lista w karcie, przyciski z ikonami: „Nowa firma” (➕), „Dołącz po GUID” (🔗).
- Tabela w wrapperze (overflow), kolumny: Nazwa, GUID, Rola.
- Formularze Create/Join w wąskiej karcie, spójne przyciski i akcje.

## Reports
- Lista raportów w karcie, przycisk „Dodaj raport” (🕒).
- Tabela: Firma, Godziny, Utworzono, Status; wrapper overflow.
- Create: walidacja hours, select firm użytkownika; komunikaty flash.

## Approvals
- Dostęp tylko dla Adminów firm (Razor auth).
- Lista oczekujących raportów w karcie, akcja „Zatwierdź” per wiersz.

## Styl
- Karty z jasnym tłem na dark theme, ujednolicone paddingi i nagłówki (`page-header`).
- Przyciski z ikonami, warianty primary/ghost.
