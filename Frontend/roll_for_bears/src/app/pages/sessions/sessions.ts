import { Component } from '@angular/core';
import { NavBar } from '../../shared/nav-bar/nav-bar';
import { SessionDto } from '../../shared/dtos/sessoion.dto';
import { Router } from '@angular/router';

@Component({
  imports: [NavBar],
  selector: 'app-sessions',
  styleUrl: './sessions.css',
  templateUrl: './sessions.html',
})
export class Sessions {
  recomendations: SessionDto[] = [
    {
      id: '1',
      name: 'Sesja1',
      gameMode: 'Campaign',
      description: 'D&D and Chill',
      playerSlots: '6/10',
      language: 'English',
      date: '20.04.2026'
    }
  ];

  otherSessions: SessionDto[] = [
    {
      id: '2',
      name: 'Sesja2',
      gameMode: 'Campaign',
      description: 'D&D and hardcore roleplay',
      playerSlots: '7/10',
      language: 'English',
      date: '01.01.2027'
    }
  ];

  constructor(private router: Router) {

  }

  sendRequest(sessionId: string): void {
    console.log('send request to session:', sessionId);
  }

  goToYourSessions(): void {
    this.router.navigate(['/your-sessions']);
  }

  goToCreateSession(): void {
    this.router.navigate(['/create-session']);
  }
}
