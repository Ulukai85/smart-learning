import { Routes } from '@angular/router';
import { User } from './components/user/user';
import { Registration } from './components/registration/registration';
import { Login } from './components/login/login';
import { Dashboard } from './components/dashboard/dashboard';
import { Layout } from './components/layout/layout';
import { CardDesigner } from './components/card-designer/card-designer';
import { CardExplorer } from './components/card-explorer/card-explorer';

export const routes: Routes = [
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
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
      },
      {
        path: 'card',
        component: CardDesigner,
      },
      {
        path: 'cards',
        component: CardExplorer,
      },
    ],
  },
];
