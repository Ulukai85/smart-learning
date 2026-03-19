# Smart Learning Web App "Cache 'Em All"

## Flashcards mit Spaced Repetition und Gamification

# Benutzerdokumentation

## Übersicht

Diese Applikation besteht aus einem Frontend, das mit `Angular v21` entwickelt wurde, und einem Backend, das auf `ASP.NET Core v10` basiert. Die Anwendung ermöglicht es Benutzern, Flashcards zu erstellen und zu lernen, wobei ein Spaced Repetition Algorithmus verwendet wird, um die Lernzeit zu optimieren. Zusätzlich bietet die App Gamification-Elemente, um die Motivation der Benutzer zu steigern. Kartenstapel können veröffentlicht und dann geforkt werden. Außerdem kann ein KI-Agent genuzt werden, der automatisch Lernkarten zu einem Thema generiert.

---

## Installation

1. Klonen Sie das Repository von GitHub:
   ```bash
   git clone https://github.com/Ulukai85/smart-learning
   cd smart-learning
   ```
2. Um die KI-Features nutzen zu können, muss in der Datei `appsettings.json` ein gültiger OpenAI API Token hinterlegt werden.
3. Für die Demo-Applikation können Sie die Docker-Container verwenden. Stellen Sie sicher, dass Docker auf Ihrem System installiert ist, und führen Sie dann den folgenden Befehl aus:
   ```bash
   docker compose -f docker-compose.demo.yml --env-file .env.demo up --build
   ```
4. Wird die Anwendung zum ersten Mal gestartet, werden die Tabellen durch die automatische Migration erstellt und mit Testdaten gefüllt, damit Sie die Funktionen der App sofort ausprobieren können.

---

## Nutzung

### Registrierung/Anmeldung

Öffnen Sie Ihren Webbrowser und navigieren Sie zu `http://localhost:3000`, um die Anwendung zu nutzen. Die Backend-API ist unter `http://localhost:5000` erreichbar.
**Am besten registrieren Sie sich mit einem eigenen Konto** und melden sich dann mit diesem an. Oder nutzen Sie den Testaccount:

- Email: `demo@mail.de`
- Passwort: `Password1!`

### Dashboard

Hier sehen Sie **Statistiken** zu Ihrem aktuellen "Lernlauf" (Streak) und ihrer Aktivität, sowie ein globales Leaderboard.

### Karten verwalten

- Der Menüpunkt **Cards** öffnet eine Ansicht aller eigenen Karten.
- Durchsuchen Sie hier Ihre Lernkarten, filtern Sie nach Kartenstapel, dem Fragen- oder Antworttext.
- Nutzen Sie die Buttons in der rechten Spalte zum Bearbeiten oder Löschen einer Karte.
- Über den Button **Create Card** können Sie schnell neue Karte erstellen und einem Deck zuordnen.

### Decks erstellen und bearbeiten

- Das **Dialogfenster** zum Erstellen und Bearbeiten von Karten, ermöglicht es auch schnell mit den beiden Buttons oben rechts ein neues Deck zu erstellen oder ein bestehendes zu bearbeiten.

### Decks veröffentlichen und löschen

- Der Menüpunkt **Learn** öffnet eine Ansicht der eigenen Kartenstapel.
- Rechts kann hier in der Spalte **Public?** der öffentliche Status des Decks getoggelt werden. Öffentliche Decks sind für ander User sichtbar und können geforkt werden.
- Ganz rechts kann ein Stapel mitsamt Karten gelöscht werden.

### Lernen

- Der Menüpunkt **Learn** bietet außerdem eine Übersicht über den Lernstatus eines Decks.
- Die Spalten **New Cards** und **Due Cards** geben an, wie viele noch komplett ungelernte Karten bzw. wie viele zu wiederholen sind ("Review").
- Links kann ein Stapel über den Button **Learn** gelernt werden.
- Über diesen Button öffnet sich sofort die erste anstehende Karte dieses Stapels.
- Der Button **Show Answer** zeigt die Antwort und ermöglicht eine Eigenbewertung des Lernstands dieser Karte. Wird **Again** gewählt wird die Karte noch einmal in die aktuelle Warteschlange eingereiht.
- Oben rechts befinden sich zwei Counter, die die neuen (**New**) und zu wiederholenden (**Due**) Karten dynamisch anzeigen.

### Forking

- Der Menüpunkt **Explore** zeigt eine Ansicht der globalen öffentlichen Kartenstapel.
- Es kann nach Name und Beschreibung gefiltert werden und es wird angezeigt, wie viele Karten dieses Deck enthält.
- Über die rechte Spalte **Fork** kann das Deck geforkt werden, d.h. der Stapel wird mitsamt Karten kopiert und fortan unter den eigenen Karten angezeigt.

### KI Feature

- Der Menüpunkt **Card Wizard** navigiert zu einem Formular, über das mithilfe eines KI-Agenten automatisch Karten erstellt werden können.
- Durch den **Select Button** kann gewählt werden, ob entweder Karten zu einem bestimmten Thema erstellt werden, oder ob aus einem eingefügten Text Informationen für die Lernkarten genutzt werden.

---

## Wartung

### Frontend

#### Übersicht

Das Frontend ist mit `Angular 21` und `PrimeNG 21` entwickelt und bietet eine benutzerfreundliche Oberfläche für die Interaktion mit der Flashcard-Anwendung. Es kommuniziert mit dem Backend über RESTful API-Endpunkte.

#### Architektur

- **Components**: Enthalten die UI-Elemente und Logik für die verschiedenen Seiten und Funktionen der App, z.B. die Startseite, die Lernseite, die Kartenerstellung usw.
- **Services**: Verwalten die Kommunikation mit dem Backend, z.B. Authentifizierung, Flashcard-Management, Benutzerdaten usw.

#### Entwicklung

Um das Frontend weiterzuentwickeln oder zu testen, führen Sie die folgenden Befehle aus:

```bash
cd frontend
npm install
npm run start
```

Das Frontend wird unter `http://localhost:4200` verfügbar sein.

#### Tests

Um die Tests auszuführen, verwenden Sie:

```bash
npm run test
```

---

### Backend

#### Übersicht

Das Backend ist in `ASP.NET Core v10` und `C#` entwickelt und bietet eine RESTful API. Es verwendet `Entity Framework Core` für die Datenbankinteraktion und `Identity Core` und JWT für die Authentifizierung. Die API ermöglicht es, Flashcards und Stapel zu erstellen, zu verwalten und zu lernen, sowie Benutzerdaten zu speichern. Um die KI-Features nutzen zu können, muss in der Datei `appsettings.json` ein gültiger OpenAI API Token hinterlegt werden.

#### Architektur

- **Controllers**: Enthalten die Endpunkte der API, die Anfragen von der Frontend empfangen und die entsprechenden Aktionen ausführen.
- **Services**: Implementieren die Geschäftslogik, z.B. die Berechnung der nächsten Lernzeit basierend auf dem Spaced Repetition Algorithmus.
- **Repositories**: Verwaltet die Interaktion mit der Datenbank über Entity Framework Core.
- **Models**: Definieren die Datenstrukturen, z.B. Flashcard, Stapel, Benutzer usw.

#### Entwicklung

Um das Backend weiterzuentwickeln, benötigen Sie eführen Sie die folgenden Befehle aus:

```bash
cd backend/SmartLearning
dotnet run
```

Das Backend wird unter `http://localhost:5260` verfügbar sein.

#### Tests

Um die Unit-Tests für das Backend auszuführen, verwenden Sie folgende Befehle:

```bash
cd backend/SmartLearning.Tests
dotnet test
```

### Datenbank

#### Übersicht

Die Anwendung verwendet eine MySQL Datenbank, die durch das ORM Entity Framework Core verwaltet wird.

#### Einrichtung

Nutzen Sie die bereitgestellte `docker-compose.yml` Datei, um die Development-MySQL-Datenbank zu starten.
´´´bash
docker compose -f docker-compose.yml --env-file .env.example up --build
´´´

#### Migrationen

Es wird konsequent mit Code-First Migrations gearbeitet, um die Datenbankstruktur zu verwalten. Bei Änderungen an den Modellen können neue Migrationen erstellt und angewendet werden, um die Datenbank auf dem neuesten Stand zu halten.
Um eine neue Migration zu erstellen, verwenden Sie den folgenden Befehl:

```bash
dotnet ef migrations add MigrationName
```

Um die Migrationen auf die Datenbank anzuwenden, verwenden Sie:

```bash
dotnet ef database update
```
