import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SessionPlayerDto } from '../../../shared/dtos/session-player.dto';

@Component({
  imports: [],
  selector: 'app-session-player',
  styleUrl: './session-player.css',
  templateUrl: './session-player.html',
})
export class SessionPlayer {
  @Input({required: true}) player!: SessionPlayerDto;

  @Output() reviewRequested = new EventEmitter<string>();

  reviewPlayer(): void {
    this.reviewRequested.emit(this.player.id)
  }
}
