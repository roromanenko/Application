import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface EventDto {
  id: string;
  title: string;
  description?: string;
  startDate: string;
  endDate: string;
  location?: string;
  isPublic: boolean;
  organizerId: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateEventRequest {
  title: string;
  description?: string | null;
  startDate: string;
  endDate: string;
  location?: string | null;
  isPublic: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly baseUrl = '/api/event';

  constructor(private http: HttpClient) { }

  create(event: CreateEventRequest): Observable<ApiResponse<EventDto>> {
    return this.http.post<ApiResponse<EventDto>>(this.baseUrl, event);
  }

  getById(id: string): Observable<ApiResponse<EventDto>> {
    return this.http.get<ApiResponse<EventDto>>(`${this.baseUrl}/${id}`);
  }

  getAll(): Observable<ApiResponse<EventDto[]>> {
    return this.http.get<ApiResponse<EventDto[]>>(this.baseUrl);
  }

  update(id: string, event: CreateEventRequest): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.baseUrl}/${id}`, event);
  }

  delete(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}
