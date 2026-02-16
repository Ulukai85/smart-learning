import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { ToastService } from '../../services/toast-service';
import { LoginUserDto } from '../../models/user.model';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    PasswordModule,
    InputTextModule,
    MessageModule,
    RouterLink,
  ],
  templateUrl: './login.html',
  styles: ``,
})
export class Login {
  isSubmitted = false;
  form: FormGroup;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private toast = inject(ToastService);

  constructor() {
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  submit() {
    this.isSubmitted = true;
    if (this.form.valid) {
      this.authService.signin(this.form.value as LoginUserDto).subscribe({
        next: (result: any) => {
          this.authService.onLoginSuccess(result.token);
          this.toast.success('Welcome', 'Login successful!');
          this.router.navigateByUrl('/dashboard');
          this.form.reset();
          this.isSubmitted = false;
        },
        error: (err) => {
          if (err.status == 400) {
            this.toast.error('Login fail', 'Incorrect email or password!');
          }
          console.log('Error:', err);
        },
      });
    }
  }

  hasDisplayableError(controlName: string): boolean {
    const control = this.form.get(controlName);
    return !!control?.invalid && (this.isSubmitted || !!control?.touched || !!control?.dirty);
  }
}
