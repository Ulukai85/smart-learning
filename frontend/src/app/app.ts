import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { User } from './components/user/user';
import { ToastModule } from 'primeng/toast'

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, User, ToastModule],
  templateUrl: './app.html',
  styles: ``,
})
export class App {}
