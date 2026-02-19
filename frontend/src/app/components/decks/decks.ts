import { Component, inject, OnInit, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { DeckService } from '../../services/deck-service';
import { DeckSummaryDto } from '../../models/deck.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-decks',
  imports: [TableModule],
  templateUrl: './decks.html',
  styles: ``,
})
export class Decks implements OnInit {
  private deckService = inject(DeckService);
  private router = inject(Router);

  deckSummaries = signal<DeckSummaryDto[]>([]);

  ngOnInit(): void {
    this.loadDeckSummary();
  }

  loadDeckSummary(): void {
    this.deckService.getDeckSummary().subscribe({
      next: (data) => this.deckSummaries.set(data),
    });
  }

  reviewDeck(deckId: string): void {
    this.router.navigate(['/review/deck', deckId]);
  }
}
