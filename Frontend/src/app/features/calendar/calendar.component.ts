import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Client, EventDto } from '../../core/api/generated-api';
import { NotificationService } from '../../core/services/notification.service';
import { AuthService } from '../../core/services/auth.service';
import { FullCalendarModule, FullCalendarComponent } from '@fullcalendar/angular';
import { CalendarOptions, EventClickArg } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { constants } from '../../shared/constants';

interface CalendarEvent {
  id: string;
  title: string;
  start: Date;
  end: Date;
  backgroundColor: string;
  borderColor: string;
  textColor: string;
  extendedProps: {
    location?: string;
    isOrganizer: boolean;
  };
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, FullCalendarModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  @ViewChild('calendar') calendarComponent!: FullCalendarComponent;

  isLoading = true;
  isLoggedIn = false;
  currentView: 'month' | 'week' = 'month';
  currentUserId: string | null = null;

  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek'
    },
    height: 'auto',
    contentHeight: 'auto',
    aspectRatio: 1.8,
    eventDisplay: 'block',
    eventTimeFormat: {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false,
      meridiem: false
    },
    displayEventTime: true,
    displayEventEnd: true,
    eventClick: this.handleEventClick.bind(this),
    events: [],
    buttonText: {
      today: 'Today',
      month: 'Month',
      week: 'Week'
    },
    views: {
      dayGridMonth: {
        titleFormat: { year: 'numeric', month: 'long' }
      },
      timeGridWeek: {
        titleFormat: { year: 'numeric', month: 'short', day: 'numeric' },
        slotMinTime: '06:00:00',
        slotMaxTime: '22:00:00',
        slotDuration: '01:00:00',
        slotLabelInterval: '01:00:00',
        slotLabelFormat: {
          hour: '2-digit',
          minute: '2-digit',
          hour12: false
        },
        allDaySlot: true
      }
    }
  };

  constructor(
    private client: Client,
    private authService: AuthService,
    private notification: NotificationService,
    private router: Router
  ) { }

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;

      if (!isLoggedIn) {
        this.notification.warning('Please log in to view your calendar');
        this.router.navigate(['/login']);
        return;
      }

      this.loadCurrentUser();
      this.loadCalendarEvents();
    });
  }

  loadCurrentUser() {
    const userString = localStorage.getItem(constants.USER_KEY) || sessionStorage.getItem(constants.USER_KEY);
    if (userString) {
      try {
        const user = JSON.parse(userString);
        this.currentUserId = user.id || user.userId;
      } catch (e) {
        console.error('Error parsing user data:', e);
      }
    }
  }

  loadCalendarEvents() {
    this.isLoading = true;

    const myEvents$ = this.client.mine().pipe(
      catchError((err) => {
        console.error('Error loading my events:', err);
        return of(null);
      })
    );

    const userString = localStorage.getItem(constants.USER_KEY) || sessionStorage.getItem(constants.USER_KEY);
    const userId = userString ? JSON.parse(userString)?.id || JSON.parse(userString)?.userId : null;

    const participantEvents$ = userId
      ? this.client.following(userId).pipe(
        catchError((err) => {
          console.error('Error loading followed events:', err);
          return of(null);
        })
      )
      : of(null);

    forkJoin([myEvents$, participantEvents$]).subscribe({
      next: async ([myEventsRes, participantEventsRes]) => {
        const myEvents: EventDto[] = Array.isArray(myEventsRes?.data)
          ? myEventsRes.data.filter((e): e is EventDto => !!e)
          : [];

        const subscriptions = Array.isArray(participantEventsRes?.data)
          ? participantEventsRes.data
          : [];

        const eventRequests = subscriptions
          .map((sub: any) => sub.eventId)
          .filter((id: string) => !!id)
          .map((id: string) =>
            this.client.eventGET2(id).pipe(
              catchError((err) => {
                console.error(`Error loading event ${id}:`, err);
                return of(null);
              })
            )
          );

        const participantEventsResponses = await Promise.all(eventRequests.map(r => r.toPromise()));
        const participantEvents: EventDto[] = participantEventsResponses
          .filter(res => res && res.success && res.data)
          .map(res => EventDto.fromJS(res!.data));

        const allEventsMap = new Map<string, EventDto>();
        [...myEvents, ...participantEvents].forEach(event => {
          if (event.id) allEventsMap.set(event.id, event);
        });

        const calendarEvents = Array.from(allEventsMap.values()).map(event => {
          const isOrganizer = event.organizerId === this.currentUserId;
          return {
            id: event.id!,
            title: event.title || 'Untitled Event',
            start: new Date(event.startDate!),
            end: new Date(event.endDate!),
            backgroundColor: isOrganizer ? '#6F8C42' : '#3B82F6',
            borderColor: isOrganizer ? '#5A7035' : '#2563EB',
            textColor: '#ffffff',
            extendedProps: {
              location: event.location,
              isOrganizer
            }
          };
        });

        this.calendarOptions = {
          ...this.calendarOptions,
          events: calendarEvents
        };

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading calendar events:', err);
        this.notification.error('Failed to load calendar events');
        this.isLoading = false;
      }
    });
  }


  handleEventClick(clickInfo: EventClickArg) {
    const eventId = clickInfo.event.id;
    if (eventId) {
      this.router.navigate(['/events', eventId]);
    }
  }

  switchView(view: 'month' | 'week') {
    this.currentView = view;
    const calendarApi = this.calendarComponent?.getApi();
    if (!calendarApi) return;

    if (view === 'month') {
      calendarApi.changeView('dayGridMonth');
    } else {
      calendarApi.changeView('timeGridWeek');
    }
  }

  get hasNoEvents(): boolean {
    const events = this.calendarOptions.events;
    return Array.isArray(events) && events.length === 0;
  }

  refreshCalendar() {
    this.loadCalendarEvents();
    this.notification.success('Calendar refreshed');
  }
}
