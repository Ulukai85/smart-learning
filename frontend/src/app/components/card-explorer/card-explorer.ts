import { Component, computed, inject, OnInit, Signal, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { CardDto } from '../../models/card.model';
import { CardService } from '../../services/card-service';
import { DialogModule } from 'primeng/dialog';
import { CardDesigner, UpsertMode } from '../card-designer/card-designer';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { DeckService } from '../../services/deck-service';
import { DeckDto } from '../../models/deck.model';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-card-explorer',
  imports: [TableModule, DialogModule, CardDesigner, ToolbarModule, ButtonModule, SelectModule],
  templateUrl: './card-explorer.html',
  styles: ``,
})
export class CardExplorer implements OnInit {
  private cardService = inject(CardService);
  private deckService = inject(DeckService);

  cards = signal<CardDto[]>([]);
  decks = signal<DeckDto[]>([]);
  dialogVisible = false;
  selectedCard = signal<CardDto | null>(null);

  header: Signal<string> = computed(() =>
    this.selectedCard() == null ? 'Create new card' : 'Update card',
  );

  ngOnInit(): void {
    this.loadCards();
    this.loadDecks();
  }

  loadDecks(): void {
    this.deckService.getAllDecks().subscribe({
      next: (decks) => {
        this.decks.set(decks);
      },
    });
  }

  loadCards(): void {
    this.cardService.getAllCards().subscribe({
      next: (cards) => this.cards.set(cards),
    });
  }

  onUpsertSuccess(mode: UpsertMode): void {
    this.loadCards();
    this.selectedCard.set(null);
    if (mode == 'edit') this.dialogVisible = false;
  }

  showDialog(card: CardDto | null): void {
    this.selectedCard.set(card);
    this.dialogVisible = true;
  }
}
