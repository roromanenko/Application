import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Client, RegisterRequest } from '../../../core/api/generated-api';
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
    private client: Client,
    private router: Router,
    private notification: NotificationService
  ) { }

  onSubmit() {
    if (this.password !== this.confirmPassword) {
      this.notification.error('Passwords do not match!');
      return;
    }

    const request = new RegisterRequest({
      username: this.username,
      email: this.email,
      firstName: this.firstName,
      lastName: this.lastName,
      password: this.password,
      confirmPassword: this.confirmPassword
    });

    this.client.register(request).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Registration successful! Please log in.');
          this.router.navigate(['/login']);
        } else {
          this.notification.error(res.message ?? 'Registration failed. Please try again.');
        }
      },
      error: (err) => {
        console.error('Registration error:', err);

        const msg =
          (err.error?.message ?? err.response?.message) ??
          'Registration failed. Please try again.';

        this.notification.error(msg);
      }
    });
  }
}
