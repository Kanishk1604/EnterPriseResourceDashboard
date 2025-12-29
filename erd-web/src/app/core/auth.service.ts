import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private api = 'http://localhost:5083';
 private tokenKey = 'token';

  constructor(private http: HttpClient) {}

  login(email: string, password: string) {
    return this.http.post<{ token: string }>(`${this.api}/auth/login`, {
      email,
      password
    });
  }

  getToken(): string | null {
    if (typeof window === 'undefined') {
      return null;
    }
    return localStorage.getItem(this.tokenKey);
  }

  setToken(token: string) {
    if (typeof window === 'undefined') return;
    localStorage.setItem(this.tokenKey, token);
  }

  logout() {
    if (typeof window === 'undefined') return;
    localStorage.removeItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}