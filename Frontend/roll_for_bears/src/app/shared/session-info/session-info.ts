import { Component, Input, Output, EventEmitter } from '@angular/core';
import { SessionDto } from '../dtos/sessoion.dto';
import { SessionRole } from '../dtos/session-player.dto';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-session-info',
  styleUrl: './session-info.css',
  templateUrl: './session-info.html',
})
export class SessionInfo {
  @Input({required: true}) session!: SessionDto;
  @Input() showJoinForm = false;

  @Output() joinRequested = new EventEmitter<SessionRole>();

  joinForm = new FormGroup({
    role: new FormControl<SessionRole>('Player',{
      nonNullable: true,
      validators: [Validators.required]
    })
  });

  onSubmit(): void {
    if (this.joinForm.invalid) {
      this.joinForm.markAllAsTouched();
      return;
    }

    const role = this.joinForm.controls.role.value;

    this.joinRequested.emit(role);
  }
}
