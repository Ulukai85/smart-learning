import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { AiCreateCardsDto } from '../../models/card.model';
import { AiService } from '../../services/ai-service';
import { ToastService } from '../../services/toast-service';
import { SelectModule } from 'primeng/select';
import { DeckDto } from '../../models/deck.model';
import { DeckService } from '../../services/deck-service';
import { ImageModule } from 'primeng/image';

@Component({
  selector: 'app-card-wizard',
  imports: [
    ButtonModule,
    ReactiveFormsModule,
    TextareaModule,
    FloatLabelModule,
    InputTextModule,
    InputNumberModule,
    SelectModule,
    ImageModule,
  ],
  templateUrl: './card-wizard.html',
  styles: ``,
})
export class CardWizard implements OnInit {
  private aiService = inject(AiService);
  private deckService = inject(DeckService);
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  decks = signal<DeckDto[]>([]);
  form: FormGroup;
  isLoading = false;

  constructor() {
    this.form = this.fb.group({
      count: [10, Validators.required],
      topic: ['', Validators.required],
      description: ['', Validators.required],
      deckId: [null],
    });
  }

  ngOnInit(): void {
    this.loadDecks();
  }

  loadDecks(): void {
    this.deckService.getDecksForUser().subscribe({
      next: (data) => this.decks.set(data),
    });
  }

  aiCardCreation(): void {
    const dto: AiCreateCardsDto = this.form.value;

    this.isLoading = true;
    this.aiService.createCards(dto).subscribe({
      next: (data) => {
        console.log('Data', data);
        this.toast.success('Cards created!', `Created ${dto.count} cards about '${dto.topic}'`);
        this.form.reset();
        this.isLoading = false;
      },
      error: (err) => {
        console.log('Error:', err);
        this.isLoading = false;
      },
    });
  }
}
