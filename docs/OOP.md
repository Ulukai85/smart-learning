# Kapselung

## Definition

Daten und Verhalten werden in einer Klasse gebündelt, und der Zugriff wird kontrolliert.

## Beispiel

```c#
public class UserCardProgress
{
    public Guid CardId { get; set; }
    public string UserId { get; set; }

    public DateTime NextReviewAt { get; set; }
    public DateTime LastReviewedAt { get; set; }

    public int Interval { get; set; }
    public double EaseFactor { get; set; }
}
```

Diese Klasse kapselt:

- den Zustand des Lernfortschritts
- alle relevanten Daten für Spaced Repetition

## Verhalten im Service

Die Logik liegt nicht im Controller, sondern im Service:

`UpdateSpacedRepetition(progress, grade, utcNow);`

→ Der Controller hat keinen direkten Zugriff auf die Logik.

Kapselung wird im Projekt dadurch umgesetzt, dass Daten und zugehörige Logik in Klassen gebündelt werden. Beispielsweise kapselt die Klasse UserCardProgress alle Informationen über den Lernfortschritt einer Karte. Der Zugriff auf diese Daten erfolgt kontrolliert über Services, wodurch eine klare Trennung zwischen Darstellung und Geschäftslogik entsteht.

# Vererbung

## Definition

Eine Klasse kann Eigenschaften und Verhalten von einer anderen Klasse übernehmen.

## Beispiel

`public class ApplicationUser : IdentityUser`

- Erweitert den bestehenden IdentityUser
- Nutzt vorhandene Funktionalität (Login, Passwort etc.)
- Ergänzt eigene Felder (Handle)

Vererbung wird im Projekt durch die Klasse ApplicationUser verwendet, welche von IdentityUser erbt. Dadurch kann die bestehende Authentifizierungslogik von ASP.NET Identity genutzt und gleichzeitig um projektspezifische Eigenschaften erweitert werden.

# Polymorphie

## Definition

Ein Interface oder eine Basisklasse kann mehrere Implementierungen haben, die unterschiedlich reagieren.

## Beispiel

`ISpacedRepetitionStrategy`

Implementierungen:

- `AnkiStrategy`
- `AnkiV2Strategy`

Verwendung:

```c#
var strategy = factory.Create(type);
strategy.CalculateNextReview(...);
```

Bedeutung:

- Das Verhalten kann ausgetauscht werden
- Der ReviewService kennt die konkrete Imlementierung nicht

Polymorphie wird durch das Strategy Pattern umgesetzt. Über das Interface ISpacedRepetitionStrategy können verschiedene Algorithmen zur Lernplanung implementiert werden. Der ReviewService arbeitet dabei nur mit der abstrakten Schnittstelle, wodurch unterschiedliche Strategien austauschbar sind.

# Abstraktion

## Definition

Komplexität wird reduziert, indem nur relevante Eigenschaften sichtbar gemacht werden.

## Beispiele

Interfaces
→ Programmierung gegen gegen Schnittstellen, nicht Implementierungen

DTOs
→ Nur notwendige Daten werden nach außen gegeben

Services
→ Controller kennt nur `reviewService.HandleReviewTransactionAsync(...)`, nicht interne Funktionalität

Frontend kennt keine Datenbankstruktur

Controller kennt keine Businesslogik im Detail

Abstraktion wird im Projekt durch die Verwendung von Interfaces, Services und DTOs erreicht. Die Controller greifen ausschließlich auf abstrahierte Services zu, während die konkrete Implementierung der Geschäftslogik verborgen bleibt. Zudem werden über DTOs nur die für das Frontend relevanten Daten bereitgestellt, wodurch interne Strukturen gekapselt werden.

# Zusammenfassung

### Kapselung:

Daten und Logik werden in Klassen wie UserCardProgress und Services gebündelt.

### Vererbung:

ApplicationUser erweitert IdentityUser.

### Polymorphie:

Unterschiedliche Spaced-Repetition-Strategien implementieren ein gemeinsames Interface.

### Abstraktion:

Interfaces, DTOs und Services reduzieren Komplexität und trennen Verantwortlichkeiten.

Besonders die Kombination aus Abstraktion und Polymorphie ermöglicht es, das System flexibel zu erweitern, beispielsweise um neue Lernalgorithmen, ohne bestehende Komponenten ändern zu müssen.
