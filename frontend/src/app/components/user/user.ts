import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ImageModule } from 'primeng/image';

@Component({
  selector: 'app-user',
  imports: [RouterOutlet, ImageModule],
  templateUrl: './user.html',
  styles: ``,
})
export class User {}
