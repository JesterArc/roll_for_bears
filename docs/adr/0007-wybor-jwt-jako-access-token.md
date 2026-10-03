# 7\. wybor JWT jako access token

Date: 2026-10-03

## Status

Accepted

## Kontekst

Potrzeba mechanizmu uwierzytelniania użytkowników.
Po zalogowaniu użytkownik powinien otrzymywać dane umożliwiające potwierdzenie swojej tożsamości przy kolejnych zapytaniach do bazy.
W celach wydajnościowych nie może być przechowywana informacja o każdej sesji użytkownika w bazie.


## Decyzja

Jako access token zostanie wykorzystany JWT (JSON Web Token).
Token będzie generowany przez backend po poprawnym uwierzytelnieniu użytkownika.
Access token będzie zawierał informacje potrzebne do identyfikacji użytkownika i będzie miał ograniczony czas ważności = 30 minut.
Access token nie będzie przechowywany w bazie danych.
Żeby użytkownik mógł pozostać zalogowany w aplikacji bez przymusu logowania co 30 minut zostanie zaimplementowany refresh token z czasem ważności 48h, umożliwiający odnowienie tokenów.

## Konsekwencje

JWT umożliwia weryfikację użytkownika.
Token może zawierać podstawowe informacje o użytkowniku oraz jego uprawnieniach.
Brak przechowywania access tokenów w bazie danych upraszcza obsługę uwierzytelniania i pozwala na zmniejszenie ilości danych przechowywanych w bazie.
Potrzebna jest zastosowanie refresh tokenów w celu utrzymania dłuższej sesji użytkownika.

## Alternatywy

Opaque token
Zaleta: łatwy w implementacji i pełna kontrola nad uwierzytelnieniem.
Wada: Obciążenie bazy danych ze względu na dużą ilość operacji in/out oraz konieczność trzymania dodatkowych danych w bazie.

Session based authentication
Zaleta: łatwe unieważnienie sesji użytkownika.
Wada: konieczność przechowywania każdej aktywnej sesji.

PASETO
Zaleta: nowoczesne rozwiązanie i bezpieczniejsze od JWT.
Wada: brak wsparcia dla Oauth oraz mniej rozbudowany ekosystem.
