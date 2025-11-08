import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';
import { jwtInterceptor } from './app/core/api/jwt.interceptor';

import { Client, API_BASE_URL } from './app/core/api/generated-api';
import { environment } from './app/environments/environment';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor])),

    Client,
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl }
  ]
}).catch(err => console.error(err));
