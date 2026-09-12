import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { AuthenticationService } from '../../Services/AuthenticationService';
import { Router } from '@angular/router';


@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-register',
  styleUrl: './register.css',
  templateUrl: './register.html',
})
export class Register {
  private _location = inject(Location);
  private readonly _authenticationService = inject(AuthenticationService);
  private readonly _router = inject(Router);

  registerForm = new FormGroup({
    username: new FormControl('', { nonNullable: true,
      validators: [Validators.required,
        Validators.minLength(3),
        Validators.maxLength(20)]}),
    email: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.email]}),
    password: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(6)]}),
    repeatPassword: new FormControl('', {nonNullable: true, validators: [Validators.required]})
  });

  passwordDoNotMatch(): boolean {
    const password = this.registerForm.controls.password.value;
    const repeatPassword = this.registerForm.controls.repeatPassword.value;

    return password !== repeatPassword;
  }

  onSubmit() {
    if (this.registerForm.invalid || this.passwordDoNotMatch()) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const username = this.registerForm.controls.username.value;
    const password = this.registerForm.controls.password.value;
    const email = this.registerForm.controls.email.value;

    this._authenticationService.register({
      username,
      email,
      password
    }).subscribe({
      next: (result) => {this._router.navigate(['/login'])},
      error: (error) => {console.log("Registration failed: ",error);}
    })
  }

  goBack(): void {
    this._location.back();
  }
}

