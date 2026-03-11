import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-navbar',
  imports: [MenubarModule, ButtonModule],
  templateUrl: './navbar.html',
  styles: ``,
})
export class Navbar implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthService);
  username = this.auth.username;

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
        icon: 'pi pi-pen-to-square',
        routerLink: ['/cards'],
      },
      {
        label: 'Learn',
        icon: 'pi pi-lightbulb',
        routerLink: ['/decks'],
      },
      {
        label: 'Explore',
        icon: 'pi pi-search',
        routerLink: ['/explorer'],
      },
    ];
  }

  logout() {
    this.auth.deleteToken();
    this.router.navigateByUrl('/login');
  }
}
