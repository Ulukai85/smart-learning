import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardReview } from './card-review';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { ReviewService } from '../../services/review-service';
import { DeckToReviewDto } from '../../models/deck.model';
import { CardToReviewDto } from '../../models/card.model';
import { ReviewResultDto } from '../../models/xpTransaction.model';

describe('CardReview', () => {
  let component: CardReview;
  let fixture: ComponentFixture<CardReview>;

  let mockReviewService: {
    fetchDeckToReview: ReturnType<typeof vi.fn>;
    saveCardReview: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    mockReviewService = {
      fetchDeckToReview: vi.fn().mockReturnValue(of(createMockDeck())),
      saveCardReview: vi.fn().mockReturnValue(of({})),
    };

    const mockActivatedRoute = {
      paramMap: of(convertToParamMap({ deckId: '123' })),
    };

    await TestBed.configureTestingModule({
      imports: [CardReview],
      providers: [
        { provide: ReviewService, useValue: mockReviewService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CardReview);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch deck on init when deckId exists', () => {
    mockReviewService.fetchDeckToReview.mockReturnValue(of(createMockDeck()));

    fixture.detectChanges();
    expect(mockReviewService.fetchDeckToReview).toHaveBeenCalledWith('123');
  });

  it('should return first card as current', () => {
    const mockDeck = createMockDeck();
    component.deckToReview.set(mockDeck);

    expect(component.current()).toEqual(mockDeck.cards[0]);
  });

  it('should reinsert card when result.reinsertCard is true', () => {
    const mockDeck = createMockDeck();
    component.deckToReview.set(mockDeck);

    component.applyReviewResult(createMockReviewResult());

    const updated = component.deckToReview();
    expect(updated?.cards.length).toBe(mockDeck.cards.length);
  });
});

function createMockDeck(): DeckToReviewDto {
  return {
    id: '123',
    name: 'Testdeck',
    newCards: 2,
    dueCards: 3,
    cards: [createMockCard()],
  };
}

function createMockCard(): CardToReviewDto {
  return {
    id: '456',
    front: 'Frage',
    back: 'Antwort',
    nextReviewAt: '2026-04-01',
    isNew: true,
    strategyType: 'Anki',
    strategyData: '{}',
  };
}

function createMockReviewResult(): ReviewResultDto {
  return {
    reviewedCardId: '456',
    reinsertCard: true,
    wasNew: true,
    xpTransactions: [],
    nextReviewAt: '2026-04-01',
    updatedDueCount: 3,
    updatedNewCount: 3,
  };
}
