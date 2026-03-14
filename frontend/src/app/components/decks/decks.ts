import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DeckSummaryDto } from '../../models/deck.model';
import { DeckService } from '../../services/deck-service';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../services/toast-service';
import { ConfirmationService } from 'primeng/api';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { TooltipModule } from 'primeng/tooltip';
import { BadgeModule } from 'primeng/badge';

@Component({
  selector: 'app-decks',
  imports: [
    TableModule,
    ButtonModule,
    ToggleSwitchModule,
    FormsModule,
    ConfirmPopupModule,
    TooltipModule,
    BadgeModule,
  ],
  providers: [ConfirmationService],
  templateUrl: './decks.html',
  styles: ``,
})
export class Decks implements OnInit {
  private deckService = inject(DeckService);
  private router = inject(Router);
  private toast = inject(ToastService);
  private confirmService = inject(ConfirmationService);

  deckSummaries = signal<DeckSummaryDto[]>([]);

  ngOnInit(): void {
    this.loadDeckSummary();
  }

  loadDeckSummary(): void {
    this.deckService.getDeckSummary().subscribe({
      next: (data) => {
        this.deckSummaries.set(data);
        console.log(data);
      },
    });
  }

  reviewDeck(deckId: string): void {
    this.router.navigate(['/review/deck', deckId]);
  }

  onTogglePublished(dto: DeckSummaryDto) {
    const originalValue = !dto.isPublished;

    this.deckService.toggleIsPublished(dto).subscribe({
      next: () => {
        this.toast.info('Information', `Deck is ${dto.isPublished ? 'now' : 'no longer'} public!`);
      },
      error: () => {
        console.log('err');
        this.deckSummaries.update((decks) =>
          decks.map((deck) =>
            deck.id === dto.id ? { ...deck, isPublished: originalValue } : deck,
          ),
        );
        this.toast.error('Something went wrong', 'Toggling of public status was unsuccessful!');
      },
    });
  }

  deleteDeck(id: string): void {
    this.deckService.deleteDeck(id).subscribe({
      next: () => {
        this.toast.info('Confirmed', 'Deck deleted!');
        this.loadDeckSummary();
      },
      error: (err) => {
        console.log('Err:', err);
        this.toast.error('Error', 'Deck not deleted');
      },
    });
  }

  confirmDelete(event: Event, deck: DeckSummaryDto) {
    this.confirmService.confirm({
      target: event.currentTarget as EventTarget,
      message: 'Do you really want to delete this card WITH ALL ITS CARDS?',
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
        this.deleteDeck(deck.id);
      },
    });
  }
}
