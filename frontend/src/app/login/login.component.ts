import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { JwtService } from '../shared/jwt.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  userNameOrEmail = new FormControl('', Validators.required);
  password = new FormControl('', Validators.required);
  errorText: string;

  constructor(private jwtService: JwtService, private router: Router, private route: ActivatedRoute) { }

  login() {
    this.jwtService.login({
      userNameOrEmail: this.userNameOrEmail.value,
      password: this.password.value
    }).subscribe(() => this.redirect(), error => this.errorText = error);
  }

  redirect() {
    this.router.navigate(['/contacts'], { relativeTo: this.route });
  }

}
