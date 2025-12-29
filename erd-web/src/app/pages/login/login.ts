import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  onLogin() {
    console.log('LOGIN CLICKED', this.email, this.password);
    this.error = '';
    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
        console.log('LOGIN SUCCESS', res);
        this.auth.setToken(res.token);
        this.router.navigate(['/assets']);
      },
      error: (err) => {
        console.error('LOGIN FAILED', err);
        this.error = 'Login failed';
      }
    });
  }
}