import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CreateUserDto, LoginUserDto } from '../models/user.model';
import { environment } from '../../environments/environment';
import { TOKEN_KEY } from '../constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  private apiUrl = environment.baseUrl + '/Auth';
  private sessionExpiredHandled = false;

  signup(dto: CreateUserDto) {
    return this.http.post(this.apiUrl + '/signup', dto);
  }

  signin(dto: LoginUserDto) {
    return this.http.post(this.apiUrl + '/signin', dto);
  }

  isLoggedIn() {
    return this.getToken() != null;
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

  handleSessionExpired(): boolean {
    if (this.sessionExpiredHandled) return false;

    this.sessionExpiredHandled = true;
    this.deleteToken();
    return true;
  }

  onLoginSuccess(token: string) {
    this.sessionExpiredHandled = false;
    this.saveToken(token);
  }
}
