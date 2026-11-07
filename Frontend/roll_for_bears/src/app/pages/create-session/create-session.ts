import { Component, inject } from '@angular/core';
import { NavBar } from '../../shared/nav-bar/nav-bar';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SessionPlayerDto, SessionRole } from '../../shared/dtos/session-player.dto';
import { Router } from '@angular/router';
import { SessionService } from '../../Services/SessionsService';

@Component({
  imports: [NavBar, ReactiveFormsModule],
  selector: 'app-create-session',
  styleUrl: './create-session.css',
  templateUrl: './create-session.html',
})
export class CreateSession {
  private readonly _sessionService = inject(SessionService);
  gameModes = [
    'Campaign',
    'One-shot'
  ];

  players: SessionPlayerDto [] = [
    {
      id: '1',
      name: 'RealUser123',
      role: 'GM',
      is_owner: true
    }
  ]

  sessionForm = new FormGroup({
    session_name: new FormControl ('', {
      validators: [Validators.required],
      nonNullable: true
    }),

    session_tagline: new FormControl ('', {
      nonNullable: true
    }),

    generate_notes: new FormControl (false, {
      nonNullable: true
    }),

    game_system: new FormControl ('', {
      validators: [Validators.required],
      nonNullable: true
    }),

    game_type: new FormControl ('', {
      validators: [Validators.required],
      nonNullable: true
    }),

    language: new FormControl ('', {
      validators: [Validators.required],
      nonNullable: true
    }),

    session_date: new FormControl<string | null>(null ),

    min_players: new FormControl<number | null> (null, {
      validators: [Validators.required, Validators.min(1)]
    }),

    max_players: new FormControl<number | null> (null, {
      validators: [Validators.required, Validators.min(1)]
    }),

    description: new FormControl ('')
  });

  constructor(private router: Router) {

  }

  changePlayerRole(playerId: string, role: SessionRole) {
    const player = this.players.find(p => p.id === playerId);

    if (!player) {
      return;
    }

    player.role = role;
  }

  cancelForm() {
    this.router.navigate(['/sessions']);
  }

  onSubmit() {
    if (this.sessionForm.invalid) {
      this.sessionForm.markAllAsTouched();
      return;
    }

    const values = this.sessionForm.getRawValue();
    const formData = new FormData();

    formData.append("Name", values.session_name);
    formData.append("Tagline", values.session_tagline);
    formData.append("IsGeneratingNotes", String(values.generate_notes))
    formData.append("GameSystem", values.game_system);
    formData.append("GameType", values.game_type === "Campaign" ? "Campaign" : "Oneshot");
    formData.append("MinimalPlayers", String(values.min_players));
    formData.append("MaximalPlayers", String(values.max_players));
    formData.append("GameLanguage", values.language);
    formData.append("GameDescription", String(values.description) ?? "");

    if (values.session_date) {
      formData.append("NextSessionDate", `${values.session_date} 19:00:00+1`);
    }

    var owner = this.players.find(player => player.is_owner);

    if (!owner) {
      return;
    }

    formData.append("PlayerRole", owner.role === "GM" ? "Gamemaster" : "Player");

    this._sessionService.createSession(formData).subscribe({
      next: (sessionUuid) => {
        this.router.navigate(['/sessions-details', sessionUuid]);
      },
      error: (error) => {
        console.log("nie udało sie utworzyć sesji: ", error);
      }
    })
  }


}
