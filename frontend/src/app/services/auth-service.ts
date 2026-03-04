import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { CreateUserDto, LoginUserDto } from '../models/user.model';
import { environment } from '../../environments/environment';
import { TOKEN_KEY } from '../constants';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  nameid: string;
  email: string;
  unique_name: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  private apiUrl = environment.baseUrl + '/Auth';
  private sessionExpiredHandled = false;

  private tokenSignal = signal<string | null>(this.getToken());

  userId = computed(() => {
    const token = this.tokenSignal();
    if (!token) return null;

    const payload = jwtDecode<JwtPayload>(token);
    return payload.nameid;
  });

  username = computed(() => {
    const token = this.tokenSignal();
    if (!token) return null;

    const payload = jwtDecode<JwtPayload>(token);
    return payload.unique_name;
  });

  isLoggedInSignal = computed(() => !!this.tokenSignal());

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
    this.tokenSignal.set(token);
  }

  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  deleteToken() {
    localStorage.removeItem(TOKEN_KEY);
    this.tokenSignal.set(null);
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
