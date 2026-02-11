import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { Toast } from '../../services/toast';
import { LoginUserDto } from '../../models/user.model';
import { Auth } from '../../services/auth';

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
  private auth = inject(Auth);
  private router = inject(Router);
  private toast = inject(Toast);

  constructor() {
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  submit() {
    this.isSubmitted = true;
    if (this.form.valid) {
      this.auth.signin(this.form.value as LoginUserDto).subscribe({
        next: (result: any) => {
          localStorage.setItem('token', result.token);
          this.toast.success('Welcome', 'Login successful!');
          this.router.navigateByUrl('/dashboard');
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
