# Smart Learning Web App "Cache 'Em All"
## Flashcards mit Spaced Repetition und Gamification

# Benutzerdokumentation

## Übersicht
Diese Applikation besteht aus einem Frontend, das mit Angular 21 entwickelt wurde, und einem Backend, das auf ASP.NET Core v.10 basiert. Die Anwendung ermöglicht es Benutzern, Flashcards zu erstellen und zu lernen, wobei ein Spaced Repetition Algorithmus verwendet wird, um die Lernzeit zu optimieren. Zusätzlich bietet die App Gamification-Elemente, um die Motivation der Benutzer zu steigern. Kartenstapel können veröffentlicht und dann geforkt werden.

## Installation
1. Klonen Sie das Repository von GitHub:
   ```bash
   git clone https://github.com/Ulukai85/smart-learning
   cd smart-learning
    ```
   
2. Für die Demo-Applikation können Sie die Docker-Container verwenden. Stellen Sie sicher, dass Docker auf Ihrem System installiert ist, und führen Sie dann den folgenden Befehl aus:
   ```bash
   docker compose -f docker-compose.demo.yml --env-file .env.demo up --build
   ```
3. Wird die Anwendung zum ersten Mal gestartet, werden die Tabellen durch die automatische Migration erstellt und mit Testdaten gefüllt, damit Sie die Funktionen der App sofort ausprobieren können.

## Nutzung
1. Öffnen Sie Ihren Webbrowser und navigieren Sie zu `http://localhost:3000`, um die Anwendung zu nutzen. Die Backend-API ist unter `http://localhost:5000` erreichbar.

2. Am besten registrieren Sie sich mit einem eigenen Konto und melden sich dann mit diesem an. Oder nutzen Sie den Testaccount:
   - Benutzername: `demouser`
   - Passwort: `Password1!`

## Wartung

### Frontend
#### Übersicht
Das Frontend ist mit Angular 21 und PrimeNG 21 entwickelt und bietet eine benutzerfreundliche Oberfläche für die Interaktion mit der Flashcard-Anwendung. Es kommuniziert mit dem Backend über RESTful API-End

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
Das Backend ist in ASP.NET Core v.10 und C# entwickelt und bietet eine RESTful API. Es verwendet Entity Framework Core für die Datenbankinteraktion und Identity Core und JWT für die Authentifizierung. Die API ermöglicht es, Flashcards und Stapel zu erstellen, zu verwalten und zu lernen, sowie Benutzerdaten zu speichern.

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
