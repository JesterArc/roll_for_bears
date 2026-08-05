# 2\. wybor technologii backendowej

Date: 2026-08-05

## Status

Proposed

## Kontekst

Projekt opiera się o interakcje systemu z bazą danych więc kluczowym jest wybranie odpowiedniego sposobu komunikacji serwer <-> baza danych

## Decyzja

Backend aplikacji zostanie zaimplementowany w języku C# z wykorzystaniem ASP.NET Core oraz platformy .NET 9.0.

## Konsekwencje

Integracja z Entity Framework Core ułatwi mapowanie obiektów na tabele oraz wykonywanie operacji na bazie danych.

Silne typowanie języka C# może ograniczyć liczbę błędów.

Konieczność znajomości języka C#, platformy .NET oraz struktury projektów ASP.NET Core.

## Alternatywy

JavaScript / TypeScript – Node.js



Zaleta: możliwość używania jednego języka na frontendzie i backendzie.

Wada: słabsza kontrola typów w przypadku użycia czystego JavaScriptu.



Java – Spring Boot



Zaleta: dojrzały ekosystem oraz duża liczba bibliotek do tworzenia aplikacji backendowych.

Wada: większa ilość konfiguracji i bardziej rozbudowana struktura projektu.



Python – Django



Zaleta: szybkie tworzenie aplikacji dzięki wielu gotowym mechanizmom.

Wada: niższa wydajność i słabsze typowanie w porównaniu z C# lub Javą.

