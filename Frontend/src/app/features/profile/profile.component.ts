import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Client, EventDto, UserDto } from '../../core/api/generated-api';
import { NotificationService } from '../../core/services/notification.service';
import { constants } from '../../shared/constants';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user: UserDto | null = null;
  myEvents: EventDto[] = [];
  subscribedEvents: EventDto[] = [];

  participantCounts = new Map<string, number>();

  activeTab: 'subscribed' | 'my-events' = 'subscribed';
  isLoading = true;
  isLoadingEvents = false;

  constructor(
    private client: Client,
    private notification: NotificationService,
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loadUserProfile();
    this.loadSubscribedEvents();
  }

  getParticipantCount(eventId?: string): number {
    return eventId ? this.participantCounts.get(eventId) ?? 0 : 0;
  }

  loadUserProfile() {
    this.isLoading = true;
    this.client.me().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.user = UserDto.fromJS(response.data);
        } else {
          this.notification.error('Failed to load profile');
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading profile:', err);
        this.notification.error('Failed to load profile');
        this.isLoading = false;
      }
    });
  }

  loadSubscribedEvents() {
    this.isLoadingEvents = true;

    const userString = localStorage.getItem(constants.USER_KEY) || sessionStorage.getItem(constants.USER_KEY);
    if (!userString) {
      this.isLoadingEvents = false;
      return;
    }

    const user = JSON.parse(userString);
    const userId = user.id || user.userId;

    this.client.following(userId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const eventIds = response.data.map((sub: any) => sub.eventId);
          this.loadEventDetails(eventIds, 'subscribed');
        } else {
          this.isLoadingEvents = false;
        }
      },
      error: (err) => {
        console.error('Error loading subscribed events:', err);
        this.isLoadingEvents = false;
      }
    });
  }

  loadMyEvents() {
    this.isLoadingEvents = true;

    this.client.mine().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.myEvents = response.data.map(e => EventDto.fromJS(e));

          for (const event of this.myEvents) {
            if (!event.id) continue;
            this.client.count(event.id).subscribe({
              next: (countRes) => {
                if (countRes.success && countRes.data !== undefined) {
                  this.participantCounts.set(event.id!, countRes.data);
                }
              },
              error: (err) => console.error(`Error loading participant count for ${event.id}:`, err)
            });
          }
        } else {
          this.myEvents = [];
        }

        this.isLoadingEvents = false;
      },
      error: (err) => {
        console.error('Error loading my events:', err);
        this.notification.error('Failed to load your events');
        this.isLoadingEvents = false;
      }
    });
  }

  private loadEventDetails(eventIds: string[], type: 'subscribed' | 'my-events') {
    if (eventIds.length === 0) {
      this.isLoadingEvents = false;
      return;
    }

    const requests = eventIds.map(id =>
      this.client.eventGET2(id).pipe(
        catchError((err) => {
          console.error(`Error loading event ${id}:`, err);
          return of(null);
        })
      )
    );

    forkJoin(requests).subscribe({
      next: (responses) => {
        const events: EventDto[] = [];

        for (const res of responses) {
          if (res && res.success && res.data) {
            const event = EventDto.fromJS(res.data);
            events.push(event);

            this.client.count(event.id!).subscribe({
              next: (countRes) => {
                if (countRes.success && countRes.data !== undefined) {
                  this.participantCounts.set(event.id!, countRes.data);
                }
              },
              error: (err) => console.error('Error loading participant count:', err)
            });
          }
        }

        if (type === 'subscribed') {
          this.subscribedEvents = events;
        } else {
          this.myEvents = events;
        }

        this.isLoadingEvents = false;
      },
      error: (err) => {
        console.error('Error loading event details:', err);
        this.isLoadingEvents = false;
      }
    });
  }

  switchTab(tab: 'subscribed' | 'my-events') {
    this.activeTab = tab;

    if (tab === 'my-events' && this.myEvents.length === 0) {
      this.loadMyEvents();
    } else if (tab === 'subscribed' && this.subscribedEvents.length === 0) {
      this.loadSubscribedEvents();
    }
  }

  logout() {
    if (confirm('Are you sure you want to log out?'))
    {
      this.authService.logout(localStorage);
      this.authService.logout(sessionStorage);

      this.notification.success('Logged out successfully');
      this.router.navigate(['/']);
    }
  }

  formatDate(date: Date | undefined): string {
    if (!date) return '';
    const dateObj = new Date(date);
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getEventStatus(event: EventDto): 'upcoming' | 'ongoing' | 'past' {
    const now = new Date();
    const start = event.startDate ? new Date(event.startDate) : new Date();
    const end = event.endDate ? new Date(event.endDate) : new Date();

    if (now < start) return 'upcoming';
    if (now > end) return 'past';
    return 'ongoing';
  }
}
