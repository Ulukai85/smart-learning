# Smart Learning mit "Cache'Em All"

![Logo](/frontend/public/logo_1.png)

Cache'Em All ist eine Webanwendung zum Lernen mit digitalen Karteikarten.
Sie kombiniert ein **Spaced-Repetition-System** mit **Gamification-Elementen** wie XP und Streaks, um den Lernprozess effizienter und motivierender zu gestalten. Außerdem kann eine **KI** genutzt werden, um automatisch Lernkarten zu erstellen.

Die Anwendung wurde im Rahmen der Berufsschule in der Ausbildung für Fachinformatik der Anwendungsentwicklung erstellt.

---

## 🔐 Benutzerdokumentation

[Hier geht es zur detaillierten **Benutzerdokumentation** mit Installation, Nutzung und Wartung](/docs/BENUTZERDOKUMENTATION.md)

---

## 🧪 Tests

[Hier geht es zum erstellten **Testprotokoll**](/tests/TESTPROTOKOLL.md)

---

## 🧾 UML-Diagramme

[Hier geht es zu einer Übersicht der erstellten **UML-Diagramme**](/uml/UML.md)

## 🚀 Features

- Benutzerregistrierung und Authentifizierung (ASP.NET Identity)
- Erstellung und Verwaltung von Lernkarten und Stapeln (Decks)
- Spaced Repetition (z. B. Anki-Algorithmus, erweiterbar)
- Fortschrittsverfolgung pro Karte
- Gamification (XP-System)
- Veröffentlichung und Forking von Decks
- AI-Karten-Generierung

---

## 📁 Projektstruktur

Das Projekt besteht aus einem Backend (ASP.NET) und einem Frontend (Angular):

```text
smartlearning/
│
├── backend/
│   ├── SmartLearning/         # ASP.NET Backend (Web API, Business Logic, EF Core)
│   └── SmartLearning.Tests/   # Unit- und Integrationstests für das Backend
├── frontend/                  # Angular Frontend (UI, Routing, State Management)
├── db/                        # Datenbankmodell
├── tests/                     # Testprotokolle
├── uml/                       # UML-Diagramme
├── weeklies/                  # Zusammenfassung der wöchentlichen Stand-Ups
├── docs/                      # u.a. Benutzerdokumentation

```

---

## 🔧 Backend

Das Backend basiert auf **ASP.NET (Web API)** und verwendet:

- Entity Framework Core (Code-First, MySQL)
- ASP.NET Identity für Authentifizierung
- Layered Architecture:
  - Controller → Service → Repository → Database

- Design Patterns:
  - Strategy Pattern (Spaced Repetition)
  - Repository Pattern
  - Dependency Injection

---

## 💻 Frontend

Das Frontend basiert auf **Angular** und ist verantwortlich für:

- Darstellung der Lernkarten und Decks
- Steuerung von Lernsessions
- Kommunikation mit dem Backend über REST APIs
- State Management mit Signals

---

## 🧠 Lernsystem

Das Lernen basiert auf einem **Spaced-Repetition-System**, bei dem:

- Karten individuell geplant werden (`UserCardProgress`)
- Nutzer jede Karte bewerten (z. B. leicht, schwer, falsch)
- Algorithmen austauschbar sind (Strategy Pattern)

---

## 🗄 Datenbank

- Relationale Datenbank (MySQL)
- Modellierung mit EF Core (Code-First)
- Migrationen zur Versionierung des Schemas

![Datenbankmodell](/db/datenbankmodell.png)

---

## 📌 Ziel

Ziel des Projekts ist es, eine erweiterbare Lernplattform zu entwickeln, die sowohl didaktisch sinnvoll als auch technisch sauber aufgebaut ist.

---
