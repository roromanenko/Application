import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { Client, UserDto } from '../../core/api/generated-api';
import { BehaviorSubject, Observable } from 'rxjs';
import { constants } from '../constants';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  user: UserDto | null = null;
  isLoggedIn = false;
  constructor(
    private client: Client,
    private router: Router,
    private authService: AuthService,
  )
  {

  }

  ngOnInit()
  {
    this.authService.isLoggedIn$.subscribe(isLoggedIn =>
    {
      this.isLoggedIn = isLoggedIn;
      if (isLoggedIn)
      {
        this.loadUserProfile();
      }
      else
      {
        this.user = null;
      }
    });
  }

  loadUserProfile()
  {
    this.user = this.authService.getCurrentUser();
  }

  getInitials(): string
  {
    if (!this.user) {
      const userInfoStr = localStorage.getItem(constants.USER_KEY) || sessionStorage.getItem(constants.USER_KEY);
      if (userInfoStr) {
        try {
          const userInfo = JSON.parse(userInfoStr);
          const firstInitial = userInfo.firstName?.charAt(0)?.toUpperCase() || '';
          const lastInitial = userInfo.lastName?.charAt(0)?.toUpperCase() || '';
          return firstInitial + lastInitial || 'U';
        } catch {
          return 'U';
        }
      }
      return 'U';
    }

    const firstInitial = this.user.firstName?.charAt(0)?.toUpperCase() || '';
    const lastInitial = this.user.lastName?.charAt(0)?.toUpperCase() || '';
    return firstInitial + lastInitial || 'U';
  }
}
