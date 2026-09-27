import { Component } from '@angular/core';
import { NavBar } from '../../shared/nav-bar/nav-bar';
import { Location } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { SessionDto } from '../../shared/dtos/sessoion.dto'


@Component({
  imports: [NavBar],
  selector: 'app-your-sessions',
  styleUrl: './your-sessions.css',
  templateUrl: './your-sessions.html',
})
export class YourSessions {
  sessions: SessionDto[] = [
    {
      id: '1',
      name: 'Sesja3',
      gameMode: 'Campaign',
      description: 'D&D with friends',
      playerSlots: '2/10',
      language: 'English',
      date: '21.06.2026',
    },
  ];
  constructor(
    private location: Location,
    private router: Router,
  ) {}

  goBack(): void {
    this.location.back();
  }

  goToSessionDetails(sessionId: string): void {
    this.router.navigate(['/session-details', sessionId]);
  }

  goToVtt(sessionId: string): void {
    this.router.navigate(['/vtt', sessionId]);
  }

  protected readonly ongotpointercapture = ongotpointercapture;
  protected readonly onkeydown = onkeydown;
}
