import { Component, inject, OnInit, signal } from '@angular/core';
import { DeckService } from '../../services/deck-service';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DeckDto } from '../../models/deck.model';
import { ToastService } from '../../services/toast-service';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-deck-explorer',
  imports: [TableModule, ButtonModule, TooltipModule],
  templateUrl: './deck-explorer.html',
  styles: ``,
})
export class DeckExplorer implements OnInit {
  private deckService = inject(DeckService);
  private toast = inject(ToastService);

  decks = signal<DeckDto[]>([]);

  isForking = signal<boolean>(false);

  ngOnInit(): void {
    this.loadDecks();
  }

  loadDecks(): void {
    this.deckService.getPublicDecks().subscribe({
      next: (data) => {
        this.decks.set(data);
        console.log(data);
      },
    });
  }

  onFork(deck: DeckDto) {
    this.isForking.set(true);
    this.deckService.forkDeck(deck.id).subscribe({
      next: () => {
        this.loadDecks();
        this.isForking.set(false);
        this.toast.success('Success', `Successfully forked deck ${deck.name}`);
      },
      error: (err) => {
        this.isForking.set(false);
        this.toast.error('Error', 'Failed to fork the deck');
        console.log(err);
      },
    });
  }
}
