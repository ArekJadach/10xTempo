## Plan implementacji: Auth (login, rejestracja, wylogowanie)

## Zakres
- Rejestracja i logowanie oparte na Identity (cookie auth).
- Brak potwierdzenia e-mail w MVP.
- Hasło: min. 6 znaków, brak wymogów znaków specjalnych/wielkich liter.

## Endpoints / Pages
- `/Identity/Account/Register` (Razor) — tworzy konto, loguje po sukcesie.
- `/Identity/Account/Login` — logowanie, cookie auth.
- `POST /Identity/Account/Logout` — wylogowanie, redirect na `/`.

## Reguły dostępu
- Foldery `/Companies` i `/Reports` wymagają zalogowania.
- Landing `/Index` dostępny anonimowo (CTA do logowania/rejestracji).

## Testy
- Rejestracja → dostęp do `/Companies/Index`.
- Nieautoryzowany dostęp do `/Reports/Index` przekierowuje na login.
- Wylogowanie unieważnia sesję.
