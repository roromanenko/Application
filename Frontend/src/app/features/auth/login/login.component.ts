import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/api/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

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
    private auth: AuthService,
    private router: Router,
    private notification: NotificationService
  ) { }

  onSubmit() {
    console.log('Login attempt:', {
      usernameOrEmail: this.usernameOrEmail,
      password: this.password,
      rememberMe: this.rememberMe
    });

    this.auth.login(this.usernameOrEmail, this.password, this.rememberMe).subscribe({
      next: () => {
        this.notification.success('Welcome back! Login successful.');
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 1000);
      },
      error: (err) => {
        console.error(err);

        if (err.status === 401) {
          this.notification.error('Invalid username or password. Please try again.');
        } else if (err.status === 0) {
          this.notification.error('Unable to connect to server. Please check your connection.');
        } else {
          this.notification.error('Login failed. Please try again later.');
        }
      }
    });
  }
}
