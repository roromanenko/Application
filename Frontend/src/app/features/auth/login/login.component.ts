import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  usernameOrEmail = '';
  password = '';
  rememberMe = false;

  onSubmit() {
    console.log('Login attempt:', {
      usernameOrEmail: this.usernameOrEmail,
      password: this.password,
      rememberMe: this.rememberMe
    });

    // Позже добавим AuthService.login(...)
    alert('Login successful! (demo)');
  }
}
