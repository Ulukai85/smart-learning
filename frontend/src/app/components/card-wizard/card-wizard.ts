import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ImageModule } from 'primeng/image';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TextareaModule } from 'primeng/textarea';
import { AiCreateCardsDto } from '../../models/card.model';
import { DeckDto } from '../../models/deck.model';
import { AiService } from '../../services/ai-service';
import { DeckService } from '../../services/deck-service';
import { ToastService } from '../../services/toast-service';

interface ModeOption {
  value: string;
  label: string;
}

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
    SelectButtonModule,
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

  modeOptions: ModeOption[];

  get useSource(): boolean {
    return this.form.get('mode')?.value === 'source';
  }

  constructor() {
    this.form = this.fb.group({
      mode: ['topic'],
      count: [10, Validators.required],
      topic: ['', Validators.required],
      description: ['', Validators.required],
      deckId: [null],
      sourceText: [null],
    });

    this.modeOptions = [
      { label: 'Generate from topic', value: 'topic' },
      { label: 'Generate from source text', value: 'source' },
    ];
  }

  ngOnInit(): void {
    this.loadDecks();

    this.form.get('mode')!.valueChanges.subscribe((mode) => {
      const control = this.form.get('sourceText');

      if (control === null) return;

      if (mode === 'topic') {
        control.setValue(null);
        control.clearValidators();
        control.disable();
      } else {
        control.enable();
        control.setValidators([Validators.required]);
      }

      control.updateValueAndValidity();
    });
  }

  loadDecks(): void {
    this.deckService.getDecksForUser().subscribe({
      next: (data) => this.decks.set(data),
    });
  }

  aiCardCreation(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const dto: AiCreateCardsDto = this.form.value;
    console.log(dto);

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

  isInvalid(controlName: string): boolean {
    const control = this.form.get(controlName);
    return !!(control && control.invalid && (control.touched || control.dirty));
  }

  error(controlName: string, errorName: string): boolean {
    const control = this.form.get(controlName);
    return !!(control?.hasError(errorName) && (control.touched || control.dirty));
  }
}
