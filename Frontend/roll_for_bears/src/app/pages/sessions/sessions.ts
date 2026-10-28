import { Component, ElementRef, ViewChild } from '@angular/core';
import { NavBar } from '../../shared/nav-bar/nav-bar';
import { SessionDto } from '../../shared/dtos/sessoion.dto';
import { Router } from '@angular/router';
import { SessionRole } from '../../shared/dtos/session-player.dto';
import { SessionInfo } from '../../shared/session-info/session-info';

@Component({
  imports: [NavBar, SessionInfo],
  selector: 'app-sessions',
  styleUrl: './sessions.css',
  templateUrl: './sessions.html',
})
export class Sessions {
  selectedSession: SessionDto | null = null;

  @ViewChild('sessionDialog')
  sessionDialog!: ElementRef<HTMLDialogElement>;

  recomendations: SessionDto[] = [
    {
      id: '1',
      name: 'Sesja1',
      gameMode: 'Campaign',
      description: 'D&D and Chill',
      playerSlots: '6/10',
      language: 'English',
      date: '20.04.2026',
    },
  ];

  otherSessions: SessionDto[] = [
    {
      id: '2',
      name: 'Sesja2',
      gameMode: 'Campaign',
      description: 'D&D and hardcore roleplay',
      playerSlots: '7/10',
      language: 'English',
      date: '01.01.2027',
    },
  ];

  constructor(private router: Router) {}

  sendRequest(role: SessionRole): void {
    if (!this.selectedSession) {
      return;
    }

    console.log('Session:', this.selectedSession.id, 'Role:', role);
  }

  openSessionInfo(session: SessionDto): void {
    this.selectedSession = session;
    this.sessionDialog.nativeElement.showModal();
  }

  closeSessionInfo(): void {
    this.sessionDialog.nativeElement.close();
  }

  onSessionDialogClosed(): void {
    this.selectedSession = null;
  }

  goToYourSessions(): void {
    this.router.navigate(['/your-sessions']);
  }

  goToCreateSession(): void {
    this.router.navigate(['/create-session']);
  }
}
