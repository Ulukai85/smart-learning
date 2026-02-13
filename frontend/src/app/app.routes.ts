import { Routes } from '@angular/router';
import { User } from './components/user/user';
import { Registration } from './components/registration/registration';
import { Login } from './components/login/login';
import { Dashboard } from './components/dashboard/dashboard';

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
    path: 'dashboard',
    component: Dashboard,
  },
];
