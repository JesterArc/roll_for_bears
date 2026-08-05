# 4\. wybor bazy danych

Date: 2026-08-05

## Status

Proposed

## Kontekst

W Aplikacji będziemy przechowywać dane zarówno klientów jak i ich rozgrywek. Oprócz tego potrzebne będą wszystkie dane wspieranych systemów RPG. Potrzebna będzie sprawna komunikacja z

backendem ASP.NET oraz przetwarzanie i przechowywanie danych typu JSON.

## Decyzja

Baza danych zostanie zaimplementowana w technologii PostgreSQL.

## Konsekwencje

Dobre wsparcie dla typu danych jsonb oraz hstore.

Łączenie rozwiązań obiektowych i relacyjnych baz danych.

Nowoczesne rozwiązania wielu problemów.

Darmowa technologia rozwijana jako open source.

Łatwa integracja z ASP.NET.

## Alternatywy

MSSQL



Zaleta: bardzo dobra integracja z .NET oraz narzędziami Microsoftu.

Wada: gorsze wsparcie dla typów złożonych takich jak JSON.



MySQL



Zaleta: popularna, prosta w konfiguracji i szeroko wspierana relacyjna baza danych.

Wada: oferuje mniej zaawansowanych funkcji niż PostgreSQL.



MongoDB:



Zaleta: elastyczna struktura dokumentów i łatwe przechowywanie danych bez schematu

Wada: ograniczone wsparcie ACID oraz brak natywnego wsparcia dla zapytań join. Ograniczony rozmiar pliku z danymi.

