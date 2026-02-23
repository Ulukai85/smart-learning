import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DeckToReviewDto } from '../../models/deck.model';
import { CreateReviewTransactionDto } from '../../models/XpTransaction.model';
import { ReviewService } from '../../services/review-service';
import { SkeletonModule } from 'primeng/skeleton';

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
      error: (err) => console.log('Error:', err)
    });
  }

  onShowAnswer() {
    this.showSolution.set(true);
  }

  onReview(grade: number) {
    console.log('grade:', grade);
    this.getNextCard();
    const dto: CreateReviewTransactionDto = new {
      cardId: 
    }

    }
    this.reviewService.saveCardReview
    this.showSolution.set(false);
  }
}
