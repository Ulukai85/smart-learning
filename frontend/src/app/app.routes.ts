import { Routes } from '@angular/router';
import { User } from './components/user/user';
import { Registration } from './components/registration/registration';
import { Login } from './components/login/login';
import { Dashboard } from './components/dashboard/dashboard';
import { Layout } from './components/layout/layout';
import { CardExplorer } from './components/card-explorer/card-explorer';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: '',
    component: User,
    children: [
      { path: 'register', component: Registration },
      { path: 'login', component: Login },
    ],
  },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
      },
      {
        path: 'cards',
        component: CardExplorer,
      },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },

  { path: '**', redirectTo: 'login' },
];
