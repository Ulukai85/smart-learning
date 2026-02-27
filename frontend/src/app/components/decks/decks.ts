import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DeckSummaryDto } from '../../models/deck.model';
import { DeckService } from '../../services/deck-service';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-decks',
  imports: [TableModule, ButtonModule, ToggleSwitchModule, FormsModule],
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
    this.router.navigate(['/review/deck', deckId]);
  }

  onRowSelect() {
    const selectedId = this.selectedDeck?.id;
    if (selectedId != null) {
      this.reviewDeck(selectedId);
    }
  }

  onTogglePublished(dto: DeckSummaryDto) {
    console.log('row:', dto)
    const originalValue = !dto.isPublished;

    this.deckService.toggleIsPublished(dto).subscribe({
    next: () => {
      console.log('Success!')
    },
    error: () => {
      console.log('err')
      this.deckSummaries.update(decks =>
        decks.map(deck =>
          deck.id === dto.id
            ? { ...deck, isPublished: originalValue }
            : deck
        )
      );
    }
  });
  }
}
