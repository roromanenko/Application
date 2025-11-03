import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { EventService, CreateEventRequest } from '../../../core/services/event.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './create-event.component.html',
  styleUrls: ['./create-event.component.scss']
})
export class CreateEventComponent {
  title = '';
  description = '';
  startDate = '';
  endDate = '';
  location = '';
  isPublic = true;
  isSubmitting = false;
  minDate: string;
  minEndDate: string;

  constructor(
    private eventService: EventService,
    private notification: NotificationService,
    private router: Router
  ) {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    this.minDate = now.toISOString().slice(0, 16);

    this.startDate = this.minDate;
    const endDateTime = new Date(now.getTime() + 60 * 60 * 1000);
    this.endDate = endDateTime.toISOString().slice(0, 16);
    this.minEndDate = this.startDate;
  }

  onStartDateChange() {
    this.minEndDate = this.startDate;

    if (this.endDate <= this.startDate) {
      const startDateTime = new Date(this.startDate);
      const endDateTime = new Date(startDateTime.getTime() + 60 * 60 * 1000);
      this.endDate = endDateTime.toISOString().slice(0, 16);
    }
  }

  onEndDateChange() {
    if (this.endDate <= this.startDate) {
      this.notification.warning('End date must be after start date');
      const startDateTime = new Date(this.startDate);
      const endDateTime = new Date(startDateTime.getTime() + 60 * 60 * 1000);
      this.endDate = endDateTime.toISOString().slice(0, 16);
    }
  }

  onSubmit() {
    if (!this.validateForm()) {
      return;
    }

    this.isSubmitting = true;

    if (new Date(this.endDate) <= new Date(this.startDate)) {
      this.notification.error('End date/time must be after start date/time');
      this.isSubmitting = false;
      return;
    }

    const event: CreateEventRequest = {
      title: this.title.trim(),
      description: this.description.trim() || null,
      startDate: new Date(this.startDate).toISOString(),
      endDate: new Date(this.endDate).toISOString(),
      location: this.location.trim() || null,
      isPublic: this.isPublic
    };

    this.eventService.create(event).subscribe({
      next: (res) => {
        this.notification.success(res.message || 'Event created successfully!');
        console.log('Created event:', res.data);
        setTimeout(() => {
          this.router.navigate(['/events']);
        }, 1000);
      },
      error: (err) => {
        console.error('Error creating event:', err);
        this.isSubmitting = false;

        if (err.status === 401) {
          this.notification.error('You must be logged in to create events');
          this.router.navigate(['/login']);
        } else if (err.error?.message) {
          this.notification.error(err.error.message);
        } else {
          this.notification.error('Failed to create event. Please try again.');
        }
      }
    });
  }

  private validateForm(): boolean {
    if (!this.title.trim()) {
      this.notification.error('Title is required');
      return false;
    }

    if (this.title.trim().length < 3) {
      this.notification.error('Title must be at least 3 characters');
      return false;
    }

    if (!this.startDate) {
      this.notification.error('Start date is required');
      return false;
    }

    if (!this.endDate) {
      this.notification.error('End date is required');
      return false;
    }

    return true;
  }

  cancel() {
    if (confirm('Are you sure you want to cancel? All changes will be lost.')) {
      this.router.navigate(['/events']);
    }
  }
}
