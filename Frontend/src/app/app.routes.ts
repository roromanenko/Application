import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { CreateEventComponent } from './features/events/create-event/create-event.component'
import { EventsListComponent } from './features/events/event-list/event-list.component'
import { ProfileComponent } from './features/profile/profile.component'
import { EventDetailComponent } from './features/events/event-detail/event-detail.component';
import { EditEventComponent } from './features/events/edit-event/edit-event.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'events/create', component: CreateEventComponent },
  { path: 'events/:id', component: EventDetailComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'events', component: EventsListComponent },
  { path: 'events/:id/edit', component: EditEventComponent }
];
