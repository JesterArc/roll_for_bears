import { Component } from '@angular/core';
import { NavBar } from '../../shared/nav-bar/nav-bar';
import { Location } from '@angular/common';
import { SessionInfo } from '../../shared/session-info/session-info';
import { SessionDto } from '../../shared/dtos/sessoion.dto';
import { SessionRole } from '../../shared/dtos/session-player.dto';
import { SessionPlayer } from './session-player/session-player';
import { SessionReviewDto } from '../../shared/dtos/session-review.dto';
import { SessionReview } from './session-review/session-review';

@Component({
  imports: [NavBar, SessionInfo, SessionPlayer, SessionReview],
  selector: 'app-session-details',
  styleUrl: './session-details.css',
  templateUrl: './session-details.html',
})
export class SessionDetails {
  session: SessionDto = {
    id: '1',
    name: 'Sesja3',
    gameSystem: 'D&D',
    gameMode: 'Campaign',
    creatorRole: 'GM',
    description: 'Bring good vibes and lucky dice',
    playerSlots: '5-10',
    language: 'English',
    date: '2026-11-01',
    players: [
      {
        id: '1',
        name: 'RealUser123',
        role: 'GM',
        is_owner: true,
      },
      {
        id: '2',
        name: 'Olo1',
        role: 'Player',
        is_owner: false,
      },
    ],
  };

  reviews: SessionReviewDto[] = [
    {
      id: '1',
      reviewer_id: '3',
      reviewer_name: 'DefoNotGoku',
      rating: 4,
      review_title: 'Great session',
      review_content:
        "Session was super fun. I think the GM did a fabulous job.\nCan't wait to play with them another time.",
    }
  ];


  sendJoinRequest(role: SessionRole): void {
    console.log('Session:', this.session.id, 'Role:', role);
  }

  constructor(private location: Location) {}

  goBack(): void {
    this.location.back();
  }

  reviewPlayer(playerId: string): void {
    console.log('player:', playerId);
  }
}
