import { Component, inject, Input, OnInit, signal, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { FloatLabelModule } from 'primeng/floatlabel';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { CreateDeckDto, DeckDto } from '../../models/deck.model';
import { CardService } from '../../services/card-service';
import { DeckService } from '../../services/deck-service';
import { MessageModule } from 'primeng/message';
import { ToastService } from '../../services/toast-service';
import { CardDto, CreateCardDto } from '../../models/card.model';
import { Observable, switchMap, tap } from 'rxjs';
import { PopoverModule, Popover } from 'primeng/popover';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-card-designer',
  imports: [
    SelectModule,
    ButtonModule,
    ReactiveFormsModule,
    TextareaModule,
    FloatLabelModule,
    MessageModule,
    PopoverModule,
    InputTextModule,
  ],
  templateUrl: './card-designer.html',
  styles: ``,
})
export class CardDesigner implements OnInit {
  @Input() mode: 'display' | 'edit' | 'create' = 'create';
  @Input() card: CardDto | null = null;
  @ViewChild('op') op!: Popover;

  cardService = inject(CardService);
  deckService = inject(DeckService);
  fb = inject(FormBuilder);
  toast = inject(ToastService);

  cardForm: FormGroup;
  deckForm: FormGroup;
  decks = signal<DeckDto[]>([]);
  isSubmitted = false;

  constructor() {
    this.cardForm = this.fb.group({
      deckId: ['', Validators.required],
      front: ['', Validators.required],
      back: ['', Validators.required],
    });

    this.deckForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadDecks().subscribe();
  }

  loadDecks(): Observable<DeckDto[]> {
    return this.deckService.getAllDecks().pipe(tap((decks) => this.decks.set(decks)));
  }

  createDeck(deck: CreateDeckDto): void {
    this.deckService
      .createDeck(deck)
      .pipe(
        switchMap((deck) =>
          this.loadDecks().pipe(tap(() => this.cardForm.patchValue({ deckId: deck.id }))),
        ),
      )
      .subscribe({
        next: () => {
          this.toast.success('Success', 'New deck created!');
          this.op.hide();
          this.deckForm.reset();
        },
      });
  }

  createCard(card: CreateCardDto): void {
    this.cardService.createCard(card).subscribe({
      next: () => {
        console.log('submitted:', this.cardForm.value);
        this.toast.success('Success', 'New card created!');
        this.isSubmitted = false;
        this.cardForm.reset({ deckId: card.deckId });
      },
      error: (err) => {
        console.log('Error:', err);
        this.toast.error('Error', 'Card not created!');
      },
    });
  }

  submitCard(): void {
    this.isSubmitted = true;
    if (this.cardForm.valid) {
      this.createCard(this.cardForm.value as CreateCardDto);
    }
  }

  submitDeck(): void {
    if (this.deckForm.valid) {
      this.createDeck(this.deckForm.value as CreateDeckDto);
    }
  }

  hasDisplayableError(controlName: string): boolean {
    const control = this.cardForm.get(controlName);
    return !!control?.invalid && (this.isSubmitted || !!control?.touched || !!control?.dirty);
  }
}
