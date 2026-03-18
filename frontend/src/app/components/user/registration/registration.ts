import { Component, inject, signal } from '@angular/core';
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
import { FirstKeyPipe } from '../../../pipes/first-key-pipe';
import { AuthService } from '../../../services/auth-service';
import { ToastService } from '../../../services/toast-service';
import { CreateUserDto } from '../../../models/user.model';
import { Router, RouterLink } from '@angular/router';

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
  isSubmitted = signal(false);
  isLoading = signal(false);
  form: FormGroup;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);

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
    this.isSubmitted.set(true);
    this.isLoading.set(true);

    if (this.form.valid) {
      this.authService.signup(this.form.value as CreateUserDto).subscribe({
        next: (result) => {
          console.log(result);
          this.toast.success('Success', 'Registration successful!');
          this.form.reset();
          this.isSubmitted.set(false);
          this.isLoading.set(false);
          this.router.navigateByUrl('/login');
        },
        error: (err) => {
          if (err.error.errors) {
            err.error.errors.forEach((x: any) => {
              this.toast.error('Error', x.code);
            });
            console.log('Error:', err);
            this.isLoading.set(false);
          }
        },
      });
    } else {
      this.isLoading.set(false);
    }
  }

  hasDisplayableError(controlName: string): boolean {
    const control = this.form.get(controlName);
    return !!control?.invalid && (this.isSubmitted() || !!control?.touched || !!control?.dirty);
  }
}
