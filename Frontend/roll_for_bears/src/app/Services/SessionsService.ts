import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5204/api/sessions';

  createSession(formData: FormData): Observable<string> {
    return this.http.post<string>(this.apiUrl, formData);
  }
}
