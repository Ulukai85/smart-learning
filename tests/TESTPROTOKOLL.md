# Testprotokoll

## Übersicht

Tests wurden hauptsächlich für das Backend geschrieben, da dort der Großteil der Logik implementiert wurde.

## Frontend Tests

Für die Tests wird die Bibliothek `vitest`genutzt. Beispielhaft sind die Tests de `CardReview`-Komponente in der Datei [card-review.spec.ts](../frontend/src/app/components/card-review/card-review.spec.ts) zu finden. Hier der Output eines Test Runs:

![Frontend Tests](frontend_test_overview.png 'Frontend Tests')

## Backend Tests

Für das Backend wurde ein eigenes Testprojekt erstellt (`SmartLearning.Tests`), das die Bibliotheken `xunit`, `moq` und `fluent assertions` nutzt.

![Backend Tests](backend-test-overview.png 'Backend Tests')

### Komponententests

Zum Großteil handelt es sich um Komponententests, die die Serviceklassen testen. Dazu wurde Wert darauf gelegt, dass die Klassen entkoppelt werden, um das Testen zu vereinfachen. Hauptaugenmerk wurde auf den `ReviewService` und den `StatisticService` gelegt, die die kritische Geschäftslogik enthalten. Hierzu wurden gesonderte Testprotokolle erstellt:

- [Review Service Test Protocol](REVIEW_SERVICE_TEST_PROTOCOL.md)
- [Statistic Service Test Protocol](STATISTIC_SERVICE_TEST_PROTOCOL.md)

![Review Service Tests](review-service-tests.png 'Review Service Tests')

![Statistic Service Tests](statistic-service-tests.png 'Statistic Service Tests')

### Integrationstests

Für das `DeckRepository` wurden einige Integrationstests geschrieben, um die Datenbankanbindung zu testen. Dazu wurde **Entity Framework Core** genutzt, um eine In-Memory-Datenbank zu erzeugen.

Testdatei: [DeckRepositoryTests.cs](../backend/SmartLearning.Tests/Repositories/DeckRepositoryTests.cs)
