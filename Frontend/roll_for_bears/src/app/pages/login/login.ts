import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Location } from '@angular/common';
import { AuthenticationService } from '../../Services/AuthenticationService';

@Component({
  imports: [ReactiveFormsModule, RouterLink],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {
  private readonly location = inject(Location);
  private readonly _authenticationService = inject(AuthenticationService);
  private readonly _router = inject(Router);

  loginForm = new FormGroup({
    username: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(6)],
    }),
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const username = this.loginForm.controls.username.value;
    const password = this.loginForm.controls.password.value;
    this._authenticationService.login({
      username,
      password
    }).subscribe({
      next: (result) => {
        this._authenticationService.setAccessToken(
          result.accessToken
        );

        this._router.navigate(['/']);
      },
      error: (error) => {
        console.log("Login Failed: ", error);
    }
    })
  }

  goBack(): void {
    this.location.back();
  }
}
