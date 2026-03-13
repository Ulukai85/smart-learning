import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { AiService } from '../../services/ai-service';
import { AiCreateCardsDto } from '../../models/card.model';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-card-wizard',
  imports: [
    ButtonModule,
    ReactiveFormsModule,
    TextareaModule,
    FloatLabelModule,
    InputTextModule,
    InputNumberModule
  ],
  templateUrl: './card-wizard.html',
  styles: ``,
})
export class CardWizard {
  private aiService = inject(AiService)
  private fb = inject(FormBuilder);

  form: FormGroup;
  isSubmitted = false;
  isEditDeck = false;

  constructor() {
    this.form = this.fb.group({
      count: [10, Validators.required],
      topic: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  aiCardCreation(): void {
    this.aiService.createCards(this.form.value as AiCreateCardsDto).subscribe({
      next: data => console.log("Data", data)
    })
  }
}
