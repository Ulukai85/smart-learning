import { Component, inject, OnInit, signal } from '@angular/core';
import { DeckService } from '../../services/deck-service';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DeckDto } from '../../models/deck.model';

@Component({
  selector: 'app-deck-explorer',
  imports: [TableModule, ButtonModule],
  templateUrl: './deck-explorer.html',
  styles: ``,
})
export class DeckExplorer implements OnInit {
  private deckService = inject(DeckService)

  decks = signal<DeckDto[]>([]);

  ngOnInit(): void {
    this.loadDecks()
  }

  loadDecks(): void {
    this.deckService.getPublicDecks().subscribe({
      next: (data) => this.decks.set(data),
    });
  }

  onRowSelect() {

  }
}


