import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CardToReviewDto } from '../../models/card.model';
import { ReviewService } from '../../services/review-service';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-card-review',
  imports: [CardModule, ButtonModule],
  templateUrl: './card-review.html',
  styles: ``,
})
export class CardReview implements OnInit {
  private reviewService = inject(ReviewService);
  private activatedRoute = inject(ActivatedRoute);

  deckId = signal('');

  queue = signal<CardToReviewDto[]>([]);
  current = computed(() => this.queue()[0] ?? null);
  showFront = signal('')

  ngOnInit(): void {
    const deckId = this.activatedRoute.snapshot.paramMap.get('deckId');
    if (!deckId) return;
    this.deckId.set(deckId);
    this.fetchCardBatch();
  }

  getNextCard() {
    this.queue.update((queue) => queue.slice(1));
  }

  fetchCardBatch() {
    console.log('deckid when fetching:', this.deckId());
    this.reviewService.fetchCardBatch(this.deckId()).subscribe({
      next: (cards) => this.queue.set(cards),
    });
  }

  onReview(grade: number) {
    console.log('grade:', grade)
    this.getNextCard()
  }
}
