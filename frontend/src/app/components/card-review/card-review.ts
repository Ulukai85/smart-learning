import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DeckToReviewDto } from '../../models/deck.model';
import { CreateReviewTransactionDto, ReviewResultDto } from '../../models/xpTransaction.model';
import { ReviewService } from '../../services/review-service';
import { SkeletonModule } from 'primeng/skeleton';
import { CardToReviewDto } from '../../models/card.model';
import { OverlayBadgeModule } from 'primeng/overlaybadge';
import { ToastService } from '../../services/toast-service';

@Component({
  selector: 'app-card-review',
  imports: [CardModule, ButtonModule, SkeletonModule, OverlayBadgeModule],
  templateUrl: './card-review.html',
  styles: ``,
})
export class CardReview implements OnInit {
  private reviewService = inject(ReviewService);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);

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
    const cardReview: CreateReviewTransactionDto = {
      cardId: card.id,
      grade: grade,
      strategyType: card.strategyType,
      strategyData: card.strategyData,
    };
    this.reviewService.saveCardReview(cardReview).subscribe({
      next: (result) => {
        this.applyReviewResult(result);
        this.showSolution.set(false);
        this.toast.success(`${result.xpAmount} XP gained!`, `Reason: ${result.xpReason}`)
        console.log('result:', result);
      },
      error: (err) => console.log('Error:', err),
    });
  }

  applyReviewResult(result: ReviewResultDto) {
    this.deckToReview.update((data) => {
      if (!data) return data;

      const [currentCard, ...rest] = data.cards;

      const cards = result.reinsertCard
        ? [
            ...rest,
            {
              ...currentCard,
              isNew: false,
              nextReviewAt: result.nextReviewAt,
            },
          ]
        : rest;

      return {
        ...data,
        cards,
        dueCards: result.updatedDueCount,
        newCards: result.updatedNewCount,
      };
    });
  }
}
