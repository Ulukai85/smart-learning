import { Component, inject, OnInit, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { CardDto } from '../../models/card.model';
import { CardService } from '../../services/card-service';

@Component({
  selector: 'app-card-explorer',
  imports: [TableModule],
  templateUrl: './card-explorer.html',
  styles: ``,
})
export class CardExplorer implements OnInit {
  private cardService = inject(CardService);

  cards = signal<CardDto[]>([]);

  ngOnInit(): void {
    this.loadCards();
  }

  loadCards(): void {
    this.cardService.getAllCards().subscribe({
      next: (cards) => this.cards.set(cards),
    });
  }
}
