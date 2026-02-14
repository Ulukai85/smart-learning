import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { MenubarModule } from 'primeng/menubar';
import { AuthService } from '../../services/auth-service';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-navbar',
  imports: [MenubarModule, ButtonModule],
  templateUrl: './navbar.html',
  styles: ``,
})
export class Navbar implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);

  items: MenuItem[] = [];

  ngOnInit() {
    this.items = [
      {
        label: 'Dashboard',
        icon: 'pi pi-home',
        routerLink: ['/dashboard'],
      },
      {
        label: 'Cards',
        icon: 'pi pi-list',
        routerLink: ['/cards'],
      },
    ];
  }

  logout() {
    this.authService.deleteToken();
    this.router.navigateByUrl('/login');
  }
}
