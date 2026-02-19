import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DeckSummaryDto } from '../../models/deck.model';
import { DeckService } from '../../services/deck-service';

@Component({
  selector: 'app-decks',
  imports: [TableModule, ButtonModule],
  templateUrl: './decks.html',
  styles: ``,
})
export class Decks implements OnInit {
  private deckService = inject(DeckService);
  private router = inject(Router);

  deckSummaries = signal<DeckSummaryDto[]>([]);
  selectedDeck: DeckSummaryDto | null = null;

  ngOnInit(): void {
    this.loadDeckSummary();
  }

  loadDeckSummary(): void {
    this.deckService.getDeckSummary().subscribe({
      next: (data) => this.deckSummaries.set(data),
    });
  }

  reviewDeck(deckId: string): void {
    console.log('deck id in decks comp:', deckId);
    this.router.navigate(['/review/deck', deckId]);
  }

  onRowSelect() {
    const selectedId = this.selectedDeck?.id;
    if (selectedId != null) {
      this.reviewDeck(selectedId);
    }
  }
}
