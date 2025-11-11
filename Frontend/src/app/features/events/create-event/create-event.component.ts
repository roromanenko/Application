import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Client, CreateEventRequest, TagDto } from '../../../core/api/generated-api';
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
  capacity: number | null = null;
  isPublic = true;
  isSubmitting = false;
  minDate: string;
  minEndDate: string;
  allTags: TagDto[] = [];
  selectedTagIds: string[] = [];

  constructor(
    private client: Client,
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

    this.loadTags();
  }

  loadTags() {
    this.client.tagGET().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.allTags = res.data;
        }
      },
      error: (err) => {
        console.error('Failed to load tags:', err);
        this.notification.error('Failed to load tags');
      }
    });
  }

  toggleTag(tagId: string) {
    if (this.selectedTagIds.includes(tagId)) {
      this.selectedTagIds = this.selectedTagIds.filter(id => id !== tagId);
    } else {
      this.selectedTagIds.push(tagId);
    }
  }

  isTagSelected(tagId: string): boolean {
    return this.selectedTagIds.includes(tagId);
  }

  onStartDateChange() {
    const start = new Date(this.startDate);
    this.minEndDate = this.startDate;

    const end = new Date(this.endDate);
    if (end <= start) {
      const newEnd = new Date(start.getTime() + 60 * 180 * 1000);
      this.endDate = newEnd.toISOString().slice(0, 16);
    }
  }

  onEndDateChange() {
    const start = new Date(this.startDate);
    const end = new Date(this.endDate);

    if (end <= start) {
      this.notification.warning('End date must be after start date');
      const correctedEnd = new Date(start.getTime() + 60 * 180 * 1000);
      this.endDate = correctedEnd.toISOString().slice(0, 16);
    }
  }

  onSubmit() {
    if (!this.validateForm()) return;

    this.isSubmitting = true;

    const request = new CreateEventRequest({
      title: this.title.trim(),
      description: this.description.trim() || undefined,
      startDate: new Date(this.startDate),
      endDate: new Date(this.endDate),
      location: this.location.trim() || undefined,
      capacity: this.capacity ?? 0,
      isPublic: this.isPublic,
      tags: this.selectedTagIds
    });

    this.client.eventPOST(request).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success(res.message ?? 'Event created successfully!');
          this.router.navigate(['/events']);
        } else {
          this.notification.error(res.message ?? 'Failed to create event');
        }
      },
      error: (err) => {
        console.error('Error creating event:', err);

        if (err.status === 401) {
          this.notification.error('You must be logged in to create events');
          this.router.navigate(['/login']);
        } else {
          const msg =
            err.error?.message ??
            err.response?.message ??
            'Failed to create event. Please try again.';
          this.notification.error(msg);
        }
      },
      complete: () => {
        this.isSubmitting = false;
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

    const start = new Date(this.startDate);
    const end = new Date(this.endDate);

    if (end <= start) {
      this.notification.error('End date must be after start date');
      return false;
    }

    if (this.capacity !== null && this.capacity < 0) {
      this.notification.error('Capacity cannot be negative');
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
