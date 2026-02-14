import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CreateUserDto, LoginUserDto } from '../models/user.model';
import { environment } from '../../environments/environment';
import { TOKEN_KEY } from '../constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Auth';

  signup(dto: CreateUserDto) {
    return this.http.post(this.apiUrl + '/signup', dto);
  }

  signin(dto: LoginUserDto) {
    return this.http.post(this.apiUrl + '/signin', dto);
  }

  isLoggedIn() {
    return this.getToken != null;
  }

  saveToken(token: string) {
    localStorage.setItem(TOKEN_KEY, token);
  }

  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  deleteToken() {
    localStorage.removeItem(TOKEN_KEY);
  }
}
