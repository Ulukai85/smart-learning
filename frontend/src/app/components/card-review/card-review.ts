import { Component, computed, inject, input, OnInit, signal, Signal } from '@angular/core';
import { ReviewService } from '../../services/review-service';
import { CardToReviewDto } from '../../models/card.model';

@Component({
  selector: 'app-card-review',
  imports: [ReviewService],
  templateUrl: './card-review.html',
  styles: ``,
})
export class CardReview implements OnInit {
  private reviewService = inject(ReviewService)

  queue = signal<CardToReviewDto[]>([])
  current = computed(() => this.queue()[0] ?? null);

  ngOnInit(): void {
    this.getCardBatch()
  }

  getNextCard() {
    this.queue.update(queue => queue.slice(1));
  }

  getCardBatch() {
    this.reviewService.getCardBatch().subscribe({
      next: cards => this.queue.set(cards)
    })
  }
}
