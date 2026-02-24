import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DeckToReviewDto } from '../../models/deck.model';
import { CreateReviewTransactionDto } from '../../models/xpTransaction.model';
import { ReviewService } from '../../services/review-service';
import { SkeletonModule } from 'primeng/skeleton';
import { CardToReviewDto } from '../../models/card.model';

@Component({
  selector: 'app-card-review',
  imports: [CardModule, ButtonModule, SkeletonModule],
  templateUrl: './card-review.html',
  styles: ``,
})
export class CardReview implements OnInit {
  private reviewService = inject(ReviewService);
  private route = inject(ActivatedRoute);

  deckToReview = signal<DeckToReviewDto | null>(null);

  current = computed(() => this.deckToReview()?.cards[0] ?? null);
  showSolution = signal<boolean>(false);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const deckId = params.get('deckId');
      if (!deckId) return;

      this.fetchDeckToReview(deckId);
    });
  }

  getNextCard() {
    this.deckToReview.update((data) => {
      if (!data) return data;

      return {
        ...data,
        cards: data.cards.slice(1),
      };
    });
  }

  fetchDeckToReview(deckId: string) {
    this.reviewService.fetchDeckToReview(deckId).subscribe({
      next: (data) => {
        this.deckToReview.set(data);
      },
      error: (err) => console.log('Error:', err),
    });
  }

  onShowAnswer() {
    this.showSolution.set(true);
  }

  onReview(grade: number, card: CardToReviewDto) {
    this.getNextCard();
    const dto: CreateReviewTransactionDto = {
      cardId: card.id,
      grade: grade,
      strategyType: card.strategyType,
      strategyData: card.strategyData,
      isNew: card.isNew
    };
    this.reviewService.saveCardReview(dto).subscribe({
      next: (result) => {
        console.log('received resultdto:', result);
        this.deckToReview.update((data) => {
          if (!data) return data;

          const cardInReview = data.cards[0];
          cardInReview.isNew = false;
          cardInReview.nextReviewAt = result.nextReviewAt;
          let updatedCards = data.cards.slice(1);
                    
          if (result.reinsertCard) {
            updatedCards = [...updatedCards, cardInReview]
          }

          const newCards = dto.isNew ? data.newCards - 1 : data.newCards

          const dueCards = dto.isNew && result.reinsertCard ? data.dueCards + 1 : !result.reinsertCard ? data.dueCards - 1 : data.dueCards

          return {
            ...data,
            cards: updatedCards,
            dueCards: dueCards, 
            newCards: newCards,
          };
        });
        this.showSolution.set(false);
      },
      error: (err) => console.log('Error:', err)
    });
  }
}
