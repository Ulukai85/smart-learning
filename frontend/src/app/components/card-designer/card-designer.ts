import {
  Component,
  computed,
  EventEmitter,
  inject,
  model,
  OnChanges,
  Output,
  Signal,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { Popover, PopoverModule } from 'primeng/popover';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { Observable, switchMap, tap } from 'rxjs';
import { CardDto, UpsertCardDto } from '../../models/card.model';
import { DeckDto, UpsertDeckDto } from '../../models/deck.model';
import { CardService } from '../../services/card-service';
import { DeckService } from '../../services/deck-service';
import { ToastService } from '../../services/toast-service';

export type UpsertMode = 'create' | 'edit';

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
export class CardDesigner implements OnChanges {
  selectedCard = model<CardDto | null>(null);
  mode: Signal<UpsertMode> = computed(() => (this.selectedCard() === null ? 'create' : 'edit'));
  decks = model<DeckDto[]>();
  @ViewChild('op') op!: Popover;
  @Output() upsertSuccess = new EventEmitter<UpsertMode>();

  cardService = inject(CardService);
  deckService = inject(DeckService);
  fb = inject(FormBuilder);
  toast = inject(ToastService);

  cardForm: FormGroup;
  deckForm: FormGroup;
  isSubmitted = false;
  isEditDeck = false;

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

  get currentDeck(): DeckDto | null {
    const deckId = this.cardForm.get('deckId');
    if (!deckId || this.decks()) return null;
    return this.decks()?.find((deck) => deck.id === deckId.value) ?? null;
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.patchCardForm();
  }

  onAddDeck(event: PointerEvent): void {
    this.op.toggle(event);
  }

  onEditDeck(event: PointerEvent): void {
    this.isEditDeck = true;
    this.op.toggle(event);
    this.patchDeckForm();
    console.log(this.currentDeck);
  }

  patchDeckForm(): void {
    const current = this.currentDeck;
    if (current != null) {
      this.deckForm.patchValue({
        name: current.name,
        description: current.description,
      });
    }
  }

  patchCardForm(): void {
    if (this.mode() == 'edit') {
      this.cardForm.patchValue({
        deckId: this.selectedCard()?.deckId,
        front: this.selectedCard()?.front,
        back: this.selectedCard()?.back,
      });
    }
  }

  reloadDecks(): Observable<DeckDto[]> {
    return this.deckService.getAllDecks().pipe(tap((decks) => this.decks.set(decks)));
  }

  createDeck(dto: UpsertDeckDto): void {
    this.deckService
      .createDeck(dto)
      .pipe(
        switchMap((deck) =>
          this.reloadDecks().pipe(tap(() => this.cardForm.patchValue({ deckId: deck.id }))),
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

  updateDeck(dto: UpsertDeckDto): void {
    this.deckService
      .updateDeck(this.currentDeck!.id, dto)
      .pipe(switchMap(() => this.reloadDecks()))
      .subscribe({
        next: () => {
          this.toast.success('Success', 'Deck updated!');
          this.op.hide();
          this.deckForm.reset();
          this.isEditDeck = false;
        },
      });
  }

  createCard(card: UpsertCardDto): void {
    this.cardService.createCard(card).subscribe({
      next: () => {
        this.handleUpsertSuccess(card);
      },
      error: (err) => {
        console.log('Error:', err);
        this.toast.error('Error', 'Card not created!');
      },
    });
  }

  updateCard(id: string, card: UpsertCardDto): void {
    this.cardService.updateCard(id, card).subscribe({
      next: () => {
        this.handleUpsertSuccess(card);
      },
      error: (err) => {
        console.log('Error:', err);
        this.toast.error('Error', 'Card not updated!');
      },
    });
  }

  handleUpsertSuccess(card: UpsertCardDto): void {
    this.upsertSuccess.emit(this.mode());
    console.log('submitted:', this.cardForm.value);
    const detail = this.mode() == 'create' ? 'Card created!' : 'Card updated!';
    this.toast.success('Success', detail);
    this.isSubmitted = false;
    this.cardForm.reset({ deckId: card.deckId });
  }

  submitCardForm(): void {
    this.isSubmitted = true;

    if (!this.cardForm.valid) return;

    if (this.mode() == 'create') {
      this.createCard(this.cardForm.value as UpsertCardDto);
    } else if (this.mode() == 'edit') {
      this.updateCard(this.selectedCard()!.id, this.cardForm.value as UpsertCardDto);
    }
  }

  submitDeckForm(): void {
    if (!this.deckForm.valid) return;

    if (this.isEditDeck) {
      this.updateDeck(this.deckForm.value as UpsertDeckDto);
    } else {
      this.createDeck(this.deckForm.value as UpsertDeckDto);
    }
  }

  hasDisplayableError(controlName: string): boolean {
    const control = this.cardForm.get(controlName);
    return !!control?.invalid && this.isSubmitted;
  }
}
