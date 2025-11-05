import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { Client, EventDto } from '../../../core/api/generated-api';
import { NotificationService } from '../../../core/services/notification.service';
import { constants } from '../../../shared/constants';

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.scss']
})
export class EventsListComponent implements OnInit {
  events: EventDto[] = [];
  participantCounts = new Map<string, number>();
  isLoading = false;

  searchTitle = '';
  sortBy: 'createdAt' | 'startDate' | 'participants' = 'startDate';
  sortDescending = false;
  currentPage = 1;
  pageSize = 12;
  activeFilter: 'all' | 'upcoming' | 'popular' = 'all';

  isLoggedIn$ = new BehaviorSubject<boolean>(!!localStorage.getItem(constants.TOKEN_KEY));

  constructor(
    private client: Client,
    private notification: NotificationService
  ) { }

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoading = true;

    const loadFn =
      this.activeFilter === 'upcoming' ? this.loadUpcomingEvents.bind(this) :
        this.activeFilter === 'popular' ? this.loadPopularEvents.bind(this) :
          this.loadAllEvents.bind(this);

    loadFn();
  }

  private loadAllEvents(): void {
    this.client.eventGET(
      this.currentPage,
      this.pageSize,
      this.sortBy,
      this.sortDescending,
      this.searchTitle || undefined
    ).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.events = this.normalizeEvents(res.data);
          this.loadParticipantCounts();
        }
        this.isLoading = false;
      },
      error: err => this.handleError('Failed to load events', err)
    });
  }

  private loadUpcomingEvents(): void {
    this.client.upcoming(30, this.currentPage, this.pageSize).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.events = this.normalizeEvents(res.data);
          this.loadParticipantCounts();
        }
        this.isLoading = false;
      },
      error: err => this.handleError('Failed to load upcoming events', err)
    });
  }

  private loadPopularEvents(): void {
    this.client.popular(this.currentPage, this.pageSize).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.events = this.normalizeEvents(res.data);
          this.loadParticipantCounts();
        }
        this.isLoading = false;
      },
      error: err => this.handleError('Failed to load popular events', err)
    });
  }

  private loadParticipantCounts(): void {
    this.participantCounts.clear();

    for (const event of this.events) {
      if (!event.id) continue;

      this.client.count(event.id!).subscribe({
        next: res => {
          if (res.success && typeof res.data === 'number') {
            this.participantCounts.set(event.id!, res.data);
          } else {
            this.participantCounts.set(event.id!, 0);
          }
        },
        error: err => {
          console.error(`Error loading count for event ${event.id}:`, err);
          this.participantCounts.set(event.id!, 0);
        }
      });
    }
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadEvents();
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadEvents();
  }

  setFilter(filter: 'all' | 'upcoming' | 'popular'): void {
    this.activeFilter = filter;
    this.currentPage = 1;
    this.loadEvents();
  }

  nextPage(): void {
    this.currentPage++;
    this.loadEvents();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadEvents();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  private normalizeEvents(events: EventDto[]): EventDto[] {
    return events.map(e => {
      const dto = EventDto.fromJS(e);
      if (dto.startDate) dto.startDate = new Date(dto.startDate);
      if (dto.endDate) dto.endDate = new Date(dto.endDate);
      return dto;
    });
  }

  private handleError(defaultMsg: string, err: any): void {
    console.error(defaultMsg, err);
    const msg = err?.message || err?.response || defaultMsg;
    this.notification.error(msg);
    this.isLoading = false;
  }

  getParticipantCount(eventId?: string): number {
    return eventId ? this.participantCounts.get(eventId) ?? 0 : 0;
  }

  isFull(event: EventDto): boolean {
    const capacity = event.capacity ?? 0;
    if (capacity === 0) return false;
    return this.getParticipantCount(event.id) >= capacity;
  }

  getCapacityPercentage(event: EventDto): number {
    const capacity = event.capacity ?? 0;
    if (capacity === 0) return 0;
    const count = this.getParticipantCount(event.id);
    return Math.round((count / capacity) * 100);
  }

  formatDate(date?: Date): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getEventStatus(event: EventDto): 'upcoming' | 'ongoing' | 'past' {
    const now = new Date();
    const start = event.startDate ? new Date(event.startDate) : now;
    const end = event.endDate ? new Date(event.endDate) : now;

    if (now < start) return 'upcoming';
    if (now > end) return 'past';
    return 'ongoing';
  }
}
