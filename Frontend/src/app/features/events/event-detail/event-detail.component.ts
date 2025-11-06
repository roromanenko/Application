import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Client, EventDto, UserDto } from '../../../core/api/generated-api';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { constants } from '../../../shared/constants';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.scss']
})
export class EventDetailComponent implements OnInit {
  event?: EventDto;
  participantCount = 0;
  participants: UserDto[] = [];
  isLoading = true;
  isLoadingAction = false;
  isLoggedIn = false;
  currentUserId: string | null = null;
  isParticipant = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private client: Client,
    private authService: AuthService,
    private notification: NotificationService
  ) { }

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
      if (isLoggedIn) {
        this.loadCurrentUser();
      }
    });

    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadEvent(id);
  }

  loadCurrentUser()
  {
    const userString = (localStorage.getItem(constants.USER_KEY) || sessionStorage.getItem(constants.USER_KEY));
    if (userString)
    {
      try
      {
        const user = JSON.parse(userString);
        this.currentUserId = user.id || user.userId;
      }
      catch (e)
      {
        console.error('Error parsing user data:', e);
      }
    }
  }

  loadEvent(id: string) {
    this.isLoading = true;

    this.client.eventGET2(id).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.event = res.data;
          this.loadParticipantCount(id);
          this.loadParticipants(id);
          this.checkIfParticipant(id);
        } else {
          this.notification.error('Event not found');
          this.router.navigate(['/events']);
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading event:', err);
        this.notification.error('Failed to load event');
        this.isLoading = false;
        this.router.navigate(['/events']);
      }
    });
  }

  loadParticipants(eventId: string) {
    this.client.followers(eventId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.participants = response.data.map((p: any) => p.user);
        }
      },
      error: (err) => {
        console.error('Error loading participants:', err);
      }
    });
  }

  loadParticipantCount(eventId: string) {
    this.client.count(eventId).subscribe({
      next: (response) => {
        if (response.success && response.data !== undefined) {
          this.participantCount = response.data;
        }
      },
      error: (err) => {
        console.error('Error loading participant count:', err);
      }
    });
  }

  checkIfParticipant(eventId: string) {
    if (!this.isLoggedIn || !this.currentUserId)
    {
      this.isParticipant = false;
      return;
    }

    this.client.isFollowing(eventId).subscribe(
      {
        next: (response) =>
        {
          if (response.success)
          {
          this.isParticipant = response.data === true;
          }
          else
          {
          this.isParticipant = false;
        }
      },
        error: (err) =>
        {
          console.error('Error checking participant status:', err);
          this.isParticipant = false;
        }
    });
  }

  join() {
    if (!this.isLoggedIn) {
      this.notification.warning('Please log in to join this event');
      this.router.navigate(['/login']);
      return;
    }

    if (!this.event?.id) return;

    if (this.event.capacity && this.event.capacity > 0 && this.participantCount >= this.event.capacity) {
      this.notification.error('This event is full');
      return;
    }

    this.isLoadingAction = true;

    this.client.join(this.event.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.notification.success('Successfully joined the event!');
          this.isParticipant = true;
          this.participantCount++;
        } else {
          this.notification.error(response.message || 'Failed to join event');
        }
        this.isLoadingAction = false;
      },
      error: (err) => {
        console.error('Error joining event:', err);

        if (err.response) {
          try {
            const errorData = JSON.parse(err.response);
            this.notification.error(errorData.message || 'Failed to join event');
          } catch {
            this.notification.error('Failed to join event');
          }
        } else {
          this.notification.error('Failed to join event');
        }

        this.isLoadingAction = false;
      }
    });
  }

  leave() {
    if (!this.event?.id) return;

    this.isLoadingAction = true;

    this.client.leave(this.event.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.notification.success('Successfully left the event');
          this.isParticipant = false;
          this.participantCount--;
        } else {
          this.notification.error(response.message || 'Failed to leave event');
        }
        this.isLoadingAction = false;
      },
      error: (err) => {
        console.error('Error leaving event:', err);
        this.notification.error('Failed to leave event');
        this.isLoadingAction = false;
      }
    });
  }

  formatDate(date: Date | undefined): string {
    if (!date) return '';
    const dateObj = new Date(date);
    return dateObj.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getEventStatus(): 'upcoming' | 'ongoing' | 'past' {
    if (!this.event) return 'upcoming';

    const now = new Date();
    const start = this.event.startDate ? new Date(this.event.startDate) : new Date();
    const end = this.event.endDate ? new Date(this.event.endDate) : new Date();

    if (now < start) return 'upcoming';
    if (now > end) return 'past';
    return 'ongoing';
  }

  isFull(): boolean {
    if (!this.event?.capacity || this.event.capacity === 0) return false;
    return this.participantCount >= this.event.capacity;
  }

  getCapacityPercentage(): number {
    if (!this.event?.capacity || this.event.capacity === 0) return 0;
    return (this.participantCount / this.event.capacity) * 100;
  }

  canJoin(): boolean {
    if (!this.isLoggedIn) return false;
    if (this.isParticipant) return false;
    if (this.getEventStatus() === 'past') return false;
    if (this.isFull()) return false;
    return true;
  }

  canLeave(): boolean {
    if (!this.isLoggedIn) return false;
    if (!this.isParticipant) return false;
    if (this.getEventStatus() === 'past') return false;
    return true;
  }

  isOrganizer(): boolean {
    if (!this.currentUserId || !this.event) return false;
    return this.currentUserId === this.event.organizerId;
  }

  editEvent() {
    if (this.event?.id) {
      this.router.navigate(['/events', this.event.id, 'edit']);
    }
  }

  deleteEvent() {
    if (!this.event?.id) return;

    if (!confirm('Are you sure you want to delete this event? This action cannot be undone.')) {
      return;
    }

    this.client.eventDELETE(this.event.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.notification.success('Event deleted successfully');
          this.router.navigate(['/events']);
        } else {
          this.notification.error('Failed to delete event');
        }
      },
      error: (err) => {
        console.error('Error deleting event:', err);
        this.notification.error('Failed to delete event');
      }
    });
  }
}
