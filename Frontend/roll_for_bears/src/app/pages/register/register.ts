import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Location } from '@angular/common';


@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-register',
  styleUrl: './register.css',
  templateUrl: './register.html',
})
export class Register {
  private location = inject(Location);

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
    this.registerForm.markAllAsTouched();

    if (this.registerForm.invalid) {
      return;
    }

    if (this.passwordDoNotMatch()) {
      return;
    }

    console.log(this.registerForm.value);
  }

  goBack(): void {
    this.location.back();
  }
}

