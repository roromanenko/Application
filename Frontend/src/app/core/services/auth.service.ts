import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { constants } from '../../shared/constants';
import { UserDto } from '../api/generated-api';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  login(token: string, storage: any, user: UserDto | null = null) {
    storage.setItem(constants.TOKEN_KEY, token);

    if (user)
    {
      storage.setItem(constants.USER_KEY, JSON.stringify(user));
    }

    this.isLoggedInSubject.next(true);
  }

  logout(storage: any) {
    storage.removeItem(constants.TOKEN_KEY);
    storage.removeItem(constants.USER_KEY);
    this.isLoggedInSubject.next(false);
  }

  hasToken(): boolean {
    return !!(localStorage.getItem(constants.TOKEN_KEY) || sessionStorage.getItem(constants.TOKEN_KEY));
  }

  getCurrentUser(): UserDto | null {
	const userString = (localStorage.getItem(constants.USER_KEY) || sessionStorage.getItem(constants.USER_KEY));
	if (userString) {
	  try {
		return JSON.parse(userString);
	  } catch (e) {
		console.error('Error parsing user data:', e);
	  }
	}
	return null;
  }
}
