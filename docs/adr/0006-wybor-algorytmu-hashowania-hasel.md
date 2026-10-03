# 6\. wybor algorytmu hashowania hasel

Date: 2026-10-03

## Status

Accepted

## Kontekst

W aplikacji RollForBears użytkownicy będą posiadali konta zabezpieczone hasłem.
Hasła nie powinny być przechowywane w bazie danych w postaci jawnej.
Potrzebny jest algorytm przeznaczony do bezpiecznego hashowania haseł.
Algorytm powinien być możliwy do użycia w aplikacji ASP.NET.

## Decyzja

Do hashowania haseł zostanie wykorzystany algorytm Argon2id.
Każde hasło będzie hashowane z wykorzystaniem losowo generowanej soli.
Dodatkowo aplikacja będzie wykorzystywała pepper przechowywany poza bazą danych.
przyjete parametry zgodne z zaleceniami z strony https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html :
- długość soli: 16 bajtów,
- długość hasha: 32 bajty,
- pamięć: 19 MiB,
- liczba iteracji: 2,
- parallelism: 1.

## Konsekwencje

Wykorzystanie pamięci oraz wielu iteracji zwiększa koszt przeprowadzania ataków brute force.
Losowa sól powoduje, że dwa identyczne hasła nie generują takich samych hashy.
Wykorzystanie peppera zapewnia dodatkową warstwę ochrony w przypadku wycieku bazy danych.
Koszt obliczeniowy hashowania jest większy niż w przypadku prostych funkcji, jednak zwiększa bezpieczeństwo przechowywanych haseł.
Parametry algorytmu mogą w przyszłości zostać dowolnie dostosowane do sprzętu.

## Alternatywy

bcrypt
Zaleta: popularny, dobrze sprawdzony i szeroko wspierany algorytm do hashowania haseł.
Wada: oferuje mniejsze możliwości konfiguracji wykorzystania pamięci i jest zalecany do legacy systemów.

PBKDF2
Zaleta: szeroko wspierany oraz dostępny bezpośrednio w platformie .NET.
Wada: jest podatny na ataki masowe przy użyciu GPU, mocne obciążenie serwera ze względu na liczbę iteracji.

SHA-256
Zaleta: szybki, popularny i szeroko wspierany algorytm haszujący.
Wada: posiada za dużą moc obliczeniową przez co nie nadaje się do przetrzymywania haseł.
