import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { NavBar } from '../../../shared/nav-bar/nav-bar';
import { Location } from '@angular/common';

@Component({
  imports: [NavBar],
  selector: 'app-choose-game-system',
  styleUrl: './choose-game-system.css',
  templateUrl: './choose-game-system.html',
})
export class ChooseGameSystem {
  private readonly location = inject(Location);
  private readonly router = inject(Router);

  private returnToForm(gameSystemName?: string): void {
    this.router.navigate(['/create-session'], {
      state: {

      }
    });
  }

  goBack(): void {
    this.location.back();
  }
}
