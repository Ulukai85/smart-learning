import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-dashboard',
  imports: [ButtonModule],
  templateUrl: './dashboard.html',
  styles: ``,
})
export class Dashboard {
  private router = inject(Router);

  logout() {
    localStorage.removeItem('token');
    this.router.navigateByUrl('/login');
  }
}
