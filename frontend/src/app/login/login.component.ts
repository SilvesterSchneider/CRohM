import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { AuthService } from '../shared/api-generated/api-generated';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  userNameOrEmail = new FormControl('', Validators.required);
  password = new FormControl('', Validators.required);
  errorText: string;

  constructor(private authService: AuthService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
  }

  login() {
    this.authService.login({
      userNameOrEmail: this.userNameOrEmail.value,
      password: this.password.value
    }).subscribe(result => this.redirect(), error => this.errorText = error);
  }

  redirect() {
    this.router.navigate(['/contacts'], { relativeTo: this.route });
  }

}
