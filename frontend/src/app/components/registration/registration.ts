import { Component, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { FirstKeyPipe } from '../../pipes/first-key-pipe';
import { Auth } from '../../services/auth';
import { Toast } from '../../services/toast';
import { CreateUserDto } from '../../models/user.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-registration',
  imports: [
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
    MessageModule,
    FirstKeyPipe,
    PasswordModule,
    RouterLink,
  ],
  templateUrl: './registration.html',
  styles: ``,
})
export class Registration {
  isSubmitted = false;
  form: FormGroup;

  private fb = inject(FormBuilder);
  private auth = inject(Auth);
  private toast = inject(Toast);

  constructor() {
    this.form = this.fb.group(
      {
        handle: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(6),
            Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/),
            Validators.pattern(/(?=.*[A-Z])/),
            Validators.pattern(/(?=.*[0-9])/),
            Validators.pattern(/(?=.*[a-z])/),
          ],
        ],
        confirmPassword: [''],
      },
      { validators: this.passwordMatchValidator },
    );
  }

  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null => {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (password && confirmPassword && password.value != confirmPassword.value) {
      confirmPassword?.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword?.setErrors(null);
    }

    return null;
  };

  submit() {
    this.isSubmitted = true;

    if (this.form.valid) {
      this.auth.signup(this.form.value as CreateUserDto).subscribe({
        next: (result) => {
          console.log(result);
          this.toast.success('Success', 'Registration successful!');
          this.form.reset();
          this.isSubmitted = false;
        },
        error: (err) => {
          if (err.error.errors) {
            err.error.errors.forEach((x: any) => {
              this.toast.error('Error', x.code);
            });
            console.log('Error:', err);
          }
        },
      });
    }
  }

  hasDisplayableError(controlName: string): boolean {
    const control = this.form.get(controlName);
    return !!control?.invalid && (this.isSubmitted || !!control?.touched || !!control?.dirty);
  }
}
