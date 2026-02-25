# Smart Learning Web App

## Flashcards mit Spaced Repetition und Gamification

## Woche 0 (09.02. - 13.02.2026)

### Dokumentation
- Ausfüllen des Projektantrags
- Erstellen von UML-Use-Case-Diagramm
- Auflisten und Bewerten der User Stories
- Strukturierung des Projekts und Aufsetzen von Git/Github

### Backend
- Aufsetzen der ASP.NET Web API App
- Installation und Einrichtung notwendiger Pakete (EF Core + Identity Core)
- Erstellen von custom Endpunkt zum Registrieren
- Erste Migration in DB (Code First)

### Frontend
- Aufsetzen des Angular Frontends
- Einrichten und Erweitern (PrimeNG)
- Registrierungs-Formular und testen der Verbindung zum Backend
- Testen von Angular/PrimeNG-Features (Pipes, Toast)

### Datenbank
- Erstellen von Docker-Compose-File für MySQL-Datenbank

## Woche 1 (16.02. - 20.02.2026)

### Dokumentation
- ERM-Diagramm (teilweise)

### Backend
- CRUD-Operationen für Cards und Decks
- Logik zum Abfragen der aktuell zu lernenden Karten (neue und fällige)
- Testbibliothek Xunit einrichten, probeweise Unit- und Komponententest
- Einrichten von Collection in Postman zum API-Testen

### Frontend
- Formular für Kartenerstellung und -update
- Layout mit Routing und Navbar
- Filter- und sortierbare Ansichten für Karten und Decks
- Konfigurieren der Testbibliothek
- Erstellen einer Komponente zur Starten von Lernsessions

### Datenbank
- Weitere Models für CardProgress, ReviewLogs und XPTransactions
- **Migration** der Models in DB
- Erstellen von **Indizes** für effizientere Queries (UserCardProgress, Card) mit FluentAPI

## Woche 2 (23.02. - 27.02.2026)

### Backend
- Klasse für **SpacedRepetiton** (vorerst einfache Implementierung mit Möglichkeit zur Erweiterung)
- Service für Abhandeln von Kartenreview mit Resultat (mehrere Entitäten gleichzeitig bearbeitet)
- **Refaktorisieren** dieses Services in kleinere Methoden und extra Repo, um Erweiterbarkeit und Testbarkeit zu gewährleisten
- Test für diese neue Methode

### Frontend
- **Logik für Lernsessions**: Auswahl von Kartenstapel und Bewerten der Karten
- Dynamische Anzeige der aktuell noch zu bearbeitenden Karten
- Styling mit PrimeIcons, Toast-Nachrichten und weiteren PrimeNG-Components
- Tests für Review-Prozess

## TODO NEXT

- Logik für Kartenreview und Lernsession vorerst abschließen
- Neues Feature: Decks können veröffentlicht werden; *öffentliche Decks* können durchsucht und geforkt werden
