import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  imports: [RouterLink],
  selector: 'app-nav-bar',
  styleUrl: './nav-bar.css',
  templateUrl: './nav-bar.html',
})
export class NavBar {

  isCreateDropDownOpen = false;

  changeDropDown(): void {
    this.isCreateDropDownOpen = !this.isCreateDropDownOpen;
  }

  logout(): void {
    console.log('logout');
  }
}
