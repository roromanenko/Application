import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, tap } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private http: HttpClient, private api: ApiService) { }

  login(usernameOrEmail: string, password: string, rememberMe: boolean) {
    return this.http
      .post<{ data: { token: string } }>(
        this.api.url('/user/login'),
        { usernameOrEmail, password }
      )
      .pipe(
        tap(response => {
          const token = response.data.token;
          const storage = rememberMe ? localStorage : sessionStorage;
          storage.setItem('jwtToken', token);
          this.isLoggedInSubject.next(true);
        })
      );
  }

  register(
    username: string,
    email: string,
    firstName: string,
    lastName: string,
    password: string,
    confirmPassword: string
  ) {
    return this.http.post<{ success: boolean; message: string }>(
      this.api.url('/user/register'),
      {
        username,
        email,
        firstName,
        lastName,
        password,
        confirmPassword
      }
    );
  }

  logout() {
    localStorage.removeItem('jwtToken');
    sessionStorage.removeItem('jwtToken');
    this.isLoggedInSubject.next(false);
  }

  hasToken(): boolean {
    return !!(localStorage.getItem('jwtToken') || sessionStorage.getItem('jwtToken'));
  }

  getToken(): string | null {
    return localStorage.getItem('jwtToken') || sessionStorage.getItem('jwtToken');
  }
}
