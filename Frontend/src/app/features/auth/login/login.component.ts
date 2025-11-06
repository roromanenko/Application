import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Client, LoginRequest } from '../../../core/api/generated-api';
import { NotificationService } from '../../../core/services/notification.service';
import { constants } from '../../../shared/constants';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  usernameOrEmail = '';
  password = '';
  rememberMe = false;

  constructor(
    private client: Client,
    private router: Router,
    private notification: NotificationService,
    private authService: AuthService
  ) { }

  onSubmit() {
    console.log('Login attempt:', {
      usernameOrEmail: this.usernameOrEmail,
      password: this.password,
      rememberMe: this.rememberMe
    });

    const request = new LoginRequest({
      usernameOrEmail: this.usernameOrEmail,
      password: this.password
    });

    this.client.login(request).subscribe({
      next: (response) => {
        const token = response.data?.accessToken;

        if (response.success && token) {
          const storage = this.rememberMe ? localStorage : sessionStorage;
          storage.setItem(constants.TOKEN_KEY, token);
          this.authService.login(token, storage, response.data?.user);

          this.notification.success('Welcome back! Login successful.');
          this.router.navigate(['/']);
        }
        else
        {
          this.notification.error(response.message || 'Login failed.');
        }
      },
      error: (err) => {
        console.error('Login error:', err);

        if (err.status === 401) {
          this.notification.error('Invalid username or password.');
        } else if (err.status === 0) {
          this.notification.error('Cannot connect to server.');
        } else {
          this.notification.error('Login failed. Please try again later.');
        }
      }
    });
  }
}
