import { Routes } from '@angular/router';
import { User } from './components/user/user';
import { Registration } from './components/user/registration/registration';
import { Login } from './components/user/login/login';
import { Dashboard } from './components/dashboard/dashboard';
import { Layout } from './components/layout/layout';
import { CardExplorer } from './components/card-explorer/card-explorer';
import { authGuard } from './guards/auth-guard';
import { Decks } from './components/decks/decks';
import { CardReview } from './components/card-review/card-review';
import { DeckExplorer } from './components/deck-explorer/deck-explorer';
import { CardWizard } from './components/card-wizard/card-wizard';

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
      {
        path: 'decks',
        component: Decks,
      },
      {
        path: 'explorer',
        component: DeckExplorer,
      },
      {
        path: 'wizard',
        component: CardWizard,
      },
      {
        path: 'review/deck/:deckId',
        component: CardReview,
      },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },

  { path: '**', redirectTo: 'login' },
];
