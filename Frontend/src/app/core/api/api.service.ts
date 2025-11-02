import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  readonly baseUrl = environment.apiBaseUrl;

  url(endpoint: string): string {
    return `${this.baseUrl}${endpoint}`;
  }
}
