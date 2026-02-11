import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CreateUserDto, LoginUserDto } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  http = inject(HttpClient);

  baseUrl = 'http://localhost:5260/api';

  signup(dto: CreateUserDto) {
    return this.http.post(this.baseUrl + '/Auth/signup', dto);
  }

  signin(dto: LoginUserDto) {
    return this.http.post(this.baseUrl + '/Auth/signin', dto);
  }
}
