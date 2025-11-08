import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Client, UpdateUserRequest, UserDto } from '../../core/api/generated-api';
import { NotificationService } from '../../core/services/notification.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss']
})
export class EditProfileComponent implements OnInit {
  user: UserDto | null = null;
  firstName = '';
  lastName = '';
  username = '';
  email = '';
  isSubmitting = false;
  isLoading = true;

  constructor(
    private client: Client,
    private router: Router,
    private notification: NotificationService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadUser();
  }

  loadUser() {
    this.isLoading = true;
    this.client.me().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.user = res.data;
          this.firstName = this.user.firstName ?? '';
          this.lastName = this.user.lastName ?? '';
          this.username = this.user.username ?? '';
          this.email = this.user.email ?? '';
        } else {
          this.notification.error('Failed to load user data');
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading user data:', err);
        this.notification.error('Failed to load user data');
        this.isLoading = false;
      }
    });
  }

  onSubmit() {
    if (!this.validateForm()) return;

    this.isSubmitting = true;

    const request = new UpdateUserRequest({
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      username: this.username.trim(),
      email: this.email.trim()
    });

    this.client.profile(request).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success(res.message ?? 'Profile updated successfully');
          this.router.navigate(['/profile']);
        } else {
          this.notification.error(res.message ?? 'Failed to update profile');
        }
      },
      error: (err) => {
        console.error('Error updating profile:', err);
        this.notification.error('Failed to update profile');
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  private validateForm(): boolean {
    if (!this.firstName.trim()) {
      this.notification.error('First name is required');
      return false;
    }
    if (!this.username.trim()) {
      this.notification.error('Username is required');
      return false;
    }
    if (this.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email)) {
      this.notification.error('Invalid email format');
      return false;
    }
    return true;
  }

  cancel() {
    if (confirm('Discard changes and return to profile?')) {
      this.router.navigate(['/profile']);
    }
  }
}
