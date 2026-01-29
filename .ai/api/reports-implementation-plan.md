## Plan implementacji: Reports (tworzenie, lista, zatwierdzanie)

## Zakres
- Dodawanie raportów godzin do firm, do których użytkownik należy.
- Lista własnych raportów.
- Zatwierdzanie raportów przez Admina firmy.

## Endpoints (propozycja API, spójne z Razor Pages)
- `GET /api/reports`
  - Auth: cookie
  - Query opcjonalne: `companyId` (tylko gdy user jest Admin tej firmy – wtedy widzi raporty firmy; bez parametru widzi własne raporty).
  - Response: `{ reports: [ { id, companyId, companyName, hours, createdOn, isApproved } ] }`
- `POST /api/reports`
  - Body: `{ "companyId": "guid", "hours": number }`
  - Walidacja: user należy do firmy; `hours` w zakresie 0.25–24 (decimal 18,2).
  - Response 201: dane raportu (`isApproved=false`, timestamps = UtcNow).
- `POST /api/reports/{id}/approve`
  - Auth: cookie, rola Admin w danej firmie.
  - Response 204; ustawia `IsApproved=true`, `UpdatedOn=UtcNow`.

## Reguły biznesowe
- Raport należy do jednego usera i jednej firmy.
- Tylko Admin firmy może zatwierdzać raporty w tej firmie.
- User może tworzyć raport tylko w firmach, do których należy.

## Implementacja (backend)
- Dodawanie: sprawdź członkostwo (UserCompanies), set timestamps, save.
- Lista: join z Companies na potrzeby nazwy; dla admina opcjonalne filtrowanie po companyId.
- Approve: sprawdź rolę Admin w firmie raportu; update IsApproved + UpdatedOn.

## Testy (API/Razor)
- POST /reports odrzuca brak członkostwa lub hours poza zakresem.
- GET /reports zwraca własne; z param companyId zwraca dla admina raporty firmy.
- Approve działa tylko dla admina właściwej firmy; owner/employee nie może zatwierdzać.
