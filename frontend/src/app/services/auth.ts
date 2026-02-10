import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CreateUserDto } from '../components/registration/registration';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  http = inject(HttpClient);

  baseUrl = 'http://localhost:5260/api';

  createUser(dto: CreateUserDto) {
    return this.http.post(this.baseUrl + '/Auth/signup', dto);
  }
}
