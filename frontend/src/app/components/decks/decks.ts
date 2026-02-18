import { Component, inject, OnInit, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { DeckService } from '../../services/deck-service';
import { DeckSummaryDto } from '../../models/deck.model';

@Component({
  selector: 'app-decks',
  imports: [TableModule],
  templateUrl: './decks.html',
  styles: ``,
})
export class Decks implements OnInit {
  private deckService = inject(DeckService);

  deckSummaries = signal<DeckSummaryDto[]>([]);

  ngOnInit(): void {
    this.loadDeckSummary();
  }

  loadDeckSummary(): void {
    this.deckService.getDeckSummary().subscribe({
      next: (data) => this.deckSummaries.set(data),
    });
  }
}
