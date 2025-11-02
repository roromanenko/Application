import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/api/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  firstName = '';
  lastName = '';
  username = '';
  email = '';
  password = '';
  confirmPassword = '';

  constructor(
    private auth: AuthService,
    private router: Router,
    private notification: NotificationService
  ) { }

  onSubmit() {
    if (this.password !== this.confirmPassword) {
      this.notification.error('Passwords do not match!');
      return;
    }

    this.auth.register(
      this.username,
      this.email,
      this.firstName,
      this.lastName,
      this.password,
      this.confirmPassword
    ).subscribe({
      next: () => {
        this.notification.success('Registration successful! Redirecting to home...');
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 1500);
      },
      error: (err: any) => {
        console.error(err);
        this.notification.error('Registration failed. Please try again.');
      }
    });
  }
}
