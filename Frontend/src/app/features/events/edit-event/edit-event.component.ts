import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Client, UpdateEventRequest, EventDto } from '../../../core/api/generated-api';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-edit-event',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './edit-event.component.html',
  styleUrls: ['./edit-event.component.scss']
})
export class EditEventComponent implements OnInit {
  id: string | null = null;
  title = '';
  description = '';
  startDate = '';
  endDate = '';
  location = '';
  capacity: number | null = null;
  isPublic = true;
  isSubmitting = false;
  isLoading = true;
  minDate: string;
  minEndDate: string;

  constructor(
    private route: ActivatedRoute,
    private client: Client,
    private notification: NotificationService,
    private router: Router
  ) {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    this.minDate = now.toISOString().slice(0, 16);
    this.minEndDate = this.minDate;
  }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) this.loadEvent(this.id);
  }

  loadEvent(id: string) {
    this.isLoading = true;
    this.client.eventGET2(id).subscribe({
      next: res => {
        if (res.success && res.data) {
          const event = res.data as EventDto;
          this.title = event.title ?? '';
          this.description = event.description ?? '';
          this.startDate = new Date(event.startDate!).toISOString().slice(0, 16);
          this.endDate = new Date(event.endDate!).toISOString().slice(0, 16);
          this.location = event.location ?? '';
          this.capacity = event.capacity ?? null;
          this.isPublic = event.isPublic ?? true;
        } else {
          this.notification.error('Event not found');
          this.router.navigate(['/events']);
        }
        this.isLoading = false;
      },
      error: err => {
        console.error('Error loading event:', err);
        this.notification.error('Failed to load event');
        this.isLoading = false;
        this.router.navigate(['/events']);
      }
    });
  }

  onStartDateChange() {
    this.minEndDate = this.startDate;
    if (this.endDate <= this.startDate) {
      const startDateTime = new Date(this.startDate);
      const endDateTime = new Date(startDateTime.getTime() + 60 * 180 * 1000);
      this.endDate = endDateTime.toISOString().slice(0, 16);
    }
  }

  onEndDateChange() {
    if (this.endDate <= this.startDate) {
      this.notification.warning('End date must be after start date');
      const startDateTime = new Date(this.startDate);
      const endDateTime = new Date(startDateTime.getTime() + 60 * 180 * 1000);
      this.endDate = endDateTime.toISOString().slice(0, 16);
    }
  }

  onSubmit() {
    if (!this.validateForm() || !this.id) return;

    this.isSubmitting = true;

    const request = new UpdateEventRequest({
      title: this.title.trim(),
      description: this.description.trim() || undefined,
      startDate: new Date(this.startDate),
      endDate: new Date(this.endDate),
      location: this.location.trim() || undefined,
      capacity: this.capacity ?? 0,
      isPublic: this.isPublic
    });

    this.client.eventPUT(this.id, request).subscribe({
      next: res => {
        if (res.success) {
          this.notification.success(res.message ?? 'Event updated successfully!');
          this.router.navigate(['/events', this.id]);
        } else {
          this.notification.error(res.message ?? 'Failed to update event');
        }
      },
      error: err => {
        console.error('Error updating event:', err);
        const msg = err.error?.message ?? 'Failed to update event.';
        this.notification.error(msg);
      },
      complete: () => (this.isSubmitting = false)
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
    if (!this.startDate || !this.endDate) {
      this.notification.error('Dates are required');
      return false;
    }
    if (this.capacity !== null && this.capacity < 0) {
      this.notification.error('Capacity cannot be negative');
      return false;
    }
    return true;
  }

  cancel() {
    if (this.id)
      this.router.navigate(['/events', this.id]);
    else
      this.router.navigate(['/events']);
  }
}
