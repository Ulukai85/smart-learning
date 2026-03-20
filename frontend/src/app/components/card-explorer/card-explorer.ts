import { Component, computed, inject, OnInit, Signal, signal } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { CardDto } from '../../models/card.model';
import { DeckDto } from '../../models/deck.model';
import { CardService } from '../../services/card-service';
import { DeckService } from '../../services/deck-service';
import { ToastService } from '../../services/toast-service';
import { CardDesigner, UpsertMode } from '../card-designer/card-designer';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-card-explorer',
  imports: [
    TableModule,
    DialogModule,
    CardDesigner,
    ToolbarModule,
    ButtonModule,
    SelectModule,
    ConfirmPopupModule,
    TooltipModule,
  ],
  providers: [ConfirmationService],
  templateUrl: './card-explorer.html',
  styles: ``,
})
export class CardExplorer implements OnInit {
  private cardService = inject(CardService);
  private deckService = inject(DeckService);
  private confirmService = inject(ConfirmationService);
  private toast = inject(ToastService);

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
    this.deckService.getDecksForUser().subscribe({
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

  deleteCard(id: string): void {
    this.cardService.deleteCard(id).subscribe({
      next: () => {
        this.toast.info('Confirmed', 'Card deleted!');
        this.loadCards();
      },
    });
  }

  showCardDialog(card: CardDto | null): void {
    this.selectedCard.set(card);
    this.dialogVisible = true;
  }

  confirmDelete(event: Event, card: CardDto) {
    this.confirmService.confirm({
      target: event.currentTarget as EventTarget,
      message: 'Do you want to delete this card?',
      icon: 'pi pi-info-circle',
      rejectButtonProps: {
        label: 'Cancel',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Delete',
        severity: 'danger',
      },
      accept: () => {
        this.deleteCard(card.id);
      },
    });
  }
}
