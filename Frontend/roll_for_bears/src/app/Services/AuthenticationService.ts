import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { request } from 'node:http';


export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResult {
  uuid: string;
  username: string;
  accessToken: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5204/api/users';
  private accesToken?: string;

  register(request: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<LoginResult> {
    return this.http.post<LoginResult>(`${this.apiUrl}/login`, request,
        {
          withCredentials: true,
        }
    );
  }

  refresh(): Observable<LoginResult> {
    return this.http.post<LoginResult>(`${this.apiUrl}/refresh`,{},{
      withCredentials: true,
    });
  }

  restoreSession(): void {
    this.refresh().subscribe({
      next: (next) => {
        this.setAccessToken(next.accessToken);
      },
      error: ()=> {
        this.accesToken = undefined;
      }
    });
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/logout`, {},{
      withCredentials: true,
    })
  }

  getAccessToken() {
    return this.accesToken;
  }

  setAccessToken(accessToken: string) {
    this.accesToken = accessToken;
  }
}
