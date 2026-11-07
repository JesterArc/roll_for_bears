import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthenticationService } from '../../Services/AuthenticationService';

@Component({
  imports:  [RouterLink, RouterLinkActive],
  selector: 'app-nav-bar',
  styleUrl: './nav-bar.css',
  templateUrl: './nav-bar.html',
})
export class NavBar {
  isCreateDropDownOpen = false;
  private readonly _authenticationService = inject(AuthenticationService);
  private readonly _router = inject(Router);

  changeDropDown(): void {
    this.isCreateDropDownOpen = !this.isCreateDropDownOpen;
  }

  logout(): void {
    this._authenticationService.logout().subscribe({
      next: () => {
        this._router.navigate(['/login']);
      }, error: (err) => {
        console.error("nie udalo sie wylogowac: ", err)
      }
    });
  }
}
