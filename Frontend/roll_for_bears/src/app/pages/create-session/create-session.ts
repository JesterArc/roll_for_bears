import { Component } from '@angular/core';
import { NavBar } from '../../shared/nav-bar/nav-bar';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SessionPlayerDto, SessionRole } from '../../shared/dtos/session-player.dto';
import { Router } from '@angular/router';

@Component({
  imports: [NavBar, ReactiveFormsModule],
  selector: 'app-create-session',
  styleUrl: './create-session.css',
  templateUrl: './create-session.html',
})
export class CreateSession {
  gameModes = [
    'Campaign',
    'One-shot'
  ];

  players: SessionPlayerDto [] = [
    {
      id: '1',
      name: 'RealUser123',
      role: 'GM',
      isOwner: true
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

    game_mode: new FormControl ('', {
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

    console.log(this.sessionForm.value);
    console.log(this.players);
  }


}
