# 8\. wybor JWT jako access token

Date: 2026-10-03

## Status

Accepted

## Kontekst

Aplikacja będzie rozwijana w technologii ASP.NET Core i będzie obsługiwała wiele niezależnych obszarów takich jak użytkownicy, sesje lub marketplace.
Potrzebna jest architektura, która umożliwi oddzielenie poszczególnych obszarów systemu i ograniczy zależności pomiędzy nimi.
Architektura powinna umożliwiać rozwijanie systemu jako jednej aplikacji, ale z wyraźnym podziałem na niezależne moduły.

## Decyzja

Backend aplikacji zostanie zaimplementowany jako modularny monolit.
Aplikacja będzie uruchamiana jako jeden proces i wdrażana jako jedna całość, jednak jej logika zostanie podzielona na niezależne moduły odpowiadające poszczególnym obszarom biznesowym.
Każdy moduł będzie posiadał własną implementację oraz publiczny interfejs umożliwiający komunikację z pozostałymi modułami.
Komunikacja pomiędzy modułami będzie realizowana za pomocą zdefiniowanych publicznych API i obiektów DTO.
Projekt RollForBears.Api będzie pełnił rolę hosta aplikacji i będzie odpowiedzialny za uruchamianie oraz konfigurację poszczególnych modułów.

## Konsekwencje

Modularny monolit pozwala zachować prostotę wdrażania jednej aplikacji przy jednoczesnym zachowaniu wyraźnych granic pomiędzy modułami.
Poszczególne moduły mogą być rozwijane niezależnie od siebie.
Publiczne API modułów ogranicza bezpośrednie zależności pomiędzy ich implementacjami.
Wymagane jest konsekwentne przestrzeganie granic pomiędzy modułami.

## Alternatywy

Monolit
Zaleta: prosta struktura projektu, łatwe uruchamianie, debugowanie i wdrażanie aplikacji.
Wada: przy rozwoju większego systemu łatwo doprowadzić do silnych zależności pomiędzy poszczególnymi częściami aplikacji i utrudnić ich niezależne rozwijanie.

Clean Architecture
Zaleta: wyraźne rozdzielenie logiki biznesowej od infrastruktury oraz zależność warstw skierowana do wewnątrz, co ułatwia testowanie i wymianę elementów infrastruktury.
Wada: wprowadza dodatkową liczbę warstw, interfejsów i projektów, co może zwiększyć złożoność aplikacji i ilość kodu potrzebnego do implementacji prostych funkcjonalności.

Onion Architecture
Zaleta: logika domenowa znajduje się w centrum aplikacji i nie zależy od warstw infrastrukturalnych, co zwiększa niezależność kodu biznesowego.
Wada: wporwadz dotakową liczbę warstw, interfejsów i projetków, co może zwiększyć złożoność aplikacji.
