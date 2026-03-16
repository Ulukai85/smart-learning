import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ChipModule } from 'primeng/chip';
import { MenubarModule } from 'primeng/menubar';
import { AuthService } from '../../services/auth-service';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-navbar',
  imports: [MenubarModule, ButtonModule, ChipModule, TooltipModule],
  templateUrl: './navbar.html',
  styles: ``,
})
export class Navbar implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthService);
  username = this.auth.username;

  isDark = signal(false);

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
      {
        label: 'Card Wizard',
        icon: 'pi pi-sparkles',
        routerLink: ['/wizard'],
      },
    ];
  }

  logout() {
    this.auth.deleteToken();
    this.router.navigateByUrl('/login');
  }

  toggleDarkMode() {
    const element = document.querySelector('html');
    element?.classList.toggle('dark');
    this.isDark.set(!this.isDark());
  }
}
