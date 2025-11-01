import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = '/api';

  constructor(private http: HttpClient) { }

  getTest(): Observable<any> {
    return this.http.get(`${this.baseUrl}/user`);
  }
}
