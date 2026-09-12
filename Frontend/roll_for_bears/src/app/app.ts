import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthenticationService } from './Services/AuthenticationService';


@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private readonly _authenticationService = inject(AuthenticationService);
  ngOnInit(): void {
    this._authenticationService.restoreSession();
  }
  protected readonly title = signal('roll_for_bears');
}
