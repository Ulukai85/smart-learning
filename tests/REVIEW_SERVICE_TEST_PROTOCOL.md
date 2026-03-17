# ReviewService Test Coverage Summary

This document outlines all test cases for the `ReviewService` class, organized by functionality area.

## Test Statistics

- **Total Test Cases**: 29
- **GetDeckToReviewAsync Tests**: 6
- **HandleReviewTransactionAsync Tests**: 23

---

## GetDeckToReviewAsync Tests (6 tests)

### Authorization & Validation Tests

1. **GetDeckToReviewAsync_ShouldThrow_WhenDeckNotFound**
   - Verifies the method throws `UnauthorizedAccessException` when deck doesn't exist
2. **GetDeckToReviewAsync_ShouldThrow_WhenDeckNotOwnedByUser**
   - Ensures users can only access their own decks

### Input Clamping Tests

3. **GetDeckToReviewAsync_ShouldClampLimits**
   - Verifies limits are clamped to maximum values (50 due, 20 new)
   - Tests that limits > 50/20 are reduced appropriately
4. **GetDeckToReviewAsync_ShouldClampNegativeLimits**
   - Ensures negative limits are clamped to 0
   - Prevents invalid query parameters

### Response Data Tests

5. **GetDeckToReviewAsync_ShouldReturnCorrectDeckData**
   - Validates the returned DeckToReviewDto contains correct deck information
   - Checks ID, Name, DueCards, and NewCards counts
6. **GetDeckToReviewAsync_ShouldCombineDueAndNewCards**
   - Verifies both due and new cards are properly combined
   - Ensures correct order (due cards first, then new cards)

---

## HandleReviewTransactionAsync Tests (23 tests)

### New Card Scenario Tests (3 tests)

These tests verify behavior when a card is reviewed for the first time.

7. **HandleReviewTransactionAsync_Should_CreateProgressAndReturnResult**
   - Verifies a new UserCardProgress is created
   - Confirms XP transaction is recorded
   - Validates review log entry is created
   - Ensures changes are persisted
8. **HandleReviewTransactionAsync_ForNewCard_ShouldUseDefaultStrategyType**
   - When no strategy type is provided, uses "Anki" as default
   - Verifies factory is called with correct default strategy
9. **HandleReviewTransactionAsync_ForNewCard_ShouldUseProvidedStrategyType**
   - When custom strategy type is provided, uses that instead of default
   - Allows flexibility for different spaced repetition algorithms

10. **HandleReviewTransactionAsync_ForNewCard_ShouldSetCreatedAtAndLastReviewedAt**
    - Ensures timestamps are set correctly for new progress records
    - Both CreatedAt and LastReviewedAt should equal review time

### Existing Card Scenario Tests (2 tests)

These tests verify behavior when a card that has been reviewed before is reviewed again.

11. **HandleReviewTransactionAsync_WhenProgressExists_ShouldNotAddProgress**
    - Existing progress should not be recreated
    - WasNew flag should be false
    - Progress record should be updated, not duplicated
12. **HandleReviewTransactionAsync_ShouldUpdateProgressDates**
    - LastReviewedAt should be updated to current time
    - UpdatedAt timestamp should be set
    - NextReviewAt should be recalculated by strategy

### XP and Reinsertion Tests (2 tests)

These tests verify XP reward logic and card reinsertion behavior.

13. **HandleReviewTransactionAsync_WhenNotReinserted_ShouldAwardXp**
    - When card is not reinserted, user gets 5 XP for "CardReview"
    - XP transaction is properly recorded
14. **HandleReviewTransactionAsync_WhenReinserted_ShouldNotAwardXp**
    - When card is reinserted, no XP is awarded
    - XpTransactions list is empty
    - Transaction is not persisted

### Deck Completion Bonus Tests (4 tests)

These tests verify special XP rewards for completing entire decks.

15. **HandleReviewTransactionAsync_WhenDeckCompleted_ShouldAwardDeckBonus**
    - When all cards in deck are reviewed, 20 bonus XP is awarded
    - User gets both "CardReview" (5 XP) and "DeckCompleted" (20 XP)
    - Total: 25 XP
16. **HandleReviewTransactionAsync_WhenDeckNotCompleted_ShouldNotAwardDeckBonus**
    - If cards remain due or new, no deck bonus is awarded
    - Only "CardReview" XP is given
17. **HandleReviewTransactionAsync_WhenDeckCompletedButCardReinserted_ShouldStillAwardDeckBonus**
    - Even if the current card is reinserted (no card review XP)
    - Deck completion bonus is still awarded if no other cards remain
    - Validates edge case where only reinserted card remains

### Review Log and Persistence Tests (2 tests)

These tests verify audit trail and data persistence.

18. **HandleReviewTransactionAsync_ShouldCreateReviewLog**
    - Review log is created with correct user, card, grade, and timestamp
    - Strategy type is recorded for audit purposes
19. **HandleReviewTransactionAsync_ShouldSaveChanges**
    - SaveChangesAsync is called exactly once per review
    - Ensures all changes are persisted to database

### Result Data Tests (2 tests)

These tests verify the response data is complete and accurate.

20. **HandleReviewTransactionAsync_ShouldReturnCorrectResultData**
    - ReviewedCardId matches the input card
    - UpdatedDueCount and UpdatedNewCount are correct
    - NextReviewAt is set by spaced repetition strategy
21. **HandleReviewTransactionAsync_ShouldPassGradeToSpacedRepetitionStrategy**
    - Verifies the grade is passed to the strategy
    - Different grades should result in different next review times
    - Strategy receives both timestamp and strategy data

### Authorization and Validation Tests (4 tests)

These tests ensure security and data integrity.

22. **HandleReviewTransactionAsync_ShouldThrow_WhenCardNotFound**
    - Throws KeyNotFoundException if card ID doesn't exist
    - Prevents reviewing non-existent cards
23. **HandleReviewTransactionAsync_ShouldThrow_WhenCardNotOwnedByUser**
    - Throws UnauthorizedAccessException if user doesn't own the deck
    - Ensures users can only review their own cards
24. **HandleReviewTransactionAsync_ShouldThrow_WhenCardDeckIsNull**
    - Throws NullReferenceException if card.Deck is null
    - Validates data integrity (all cards must have a deck)

### Grade Value Tests (1 parameterized test with 5 cases)

25. **HandleReviewTransactionAsync_ShouldWorkWithVariousGrades** (0-4)
    - Tests grades 0, 1, 2, 3, 4 (typical Anki grades)
    - Verifies strategy is called for all valid grades
    - Allows future algorithms with different grade scales

---

## Test Structure Best Practices

### Common Setup

The `SetupCommonReviewFlow` method reduces boilerplate by:

- Mocking card repository returns
- Setting up progress repository behavior
- Configuring time provider
- Setting up spaced repetition strategy behavior
- Establishing default counts for due/new cards

### Helper Methods

- `DefaultCard()` - Creates a standard test card
- `DefaultDto()` - Creates a standard review transaction DTO
- `CreateReviewService()` - Creates service with all mocked dependencies

### Test Organization

Tests are organized into logical sections with `#region` directives:

1. GetDeckToReviewAsync Tests
2. HandleReviewTransactionAsync - New Card Scenarios
3. HandleReviewTransactionAsync - Existing Card Scenarios
4. HandleReviewTransactionAsync - XP and Reinsertion
5. HandleReviewTransactionAsync - Deck Completion Bonus
6. HandleReviewTransactionAsync - Review Log and Persistence
7. HandleReviewTransactionAsync - Result Data
8. HandleReviewTransactionAsync - Authorization and Validation
9. HandleReviewTransactionAsync - Different Grade Values

---

## Coverage Analysis

### What's Tested ✓

- ✓ Authorization (user ownership validation)
- ✓ Input validation (clamping, null checks)
- ✓ Business logic (XP rewards, deck completion)
- ✓ Data persistence (SaveChangesAsync calls)
- ✓ Spaced repetition integration
- ✓ Edge cases (reinserted cards with deck completion)
- ✓ Timestamp handling
- ✓ Strategy pattern usage

### Critical Paths

All critical user flows are tested:

1. **First-time card review** → Progress creation → XP award
2. **Subsequent card review** → Progress update → XP award
3. **Deck completion** → Bonus XP award
4. **Card reinsertion** → No XP, but may complete deck
5. **Authorization checks** → Proper error handling

---

## Future Test Additions

Potential areas for additional testing:

1. **Integration tests** - Test with real database
2. **Strategy-specific tests** - Test different algorithm behaviors
3. **Performance tests** - Measure review transaction processing time
4. **Concurrency tests** - Test simultaneous reviews
5. **Streak calculation** - Test the streak calculation logic mentioned in comments
6. **XP chart data** - Test the new daily review data collection
