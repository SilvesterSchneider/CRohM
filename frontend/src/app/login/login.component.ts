import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  username = new FormControl('', Validators.required);
  password = new FormControl('', Validators.required);

  constructor() { }

  ngOnInit() {
  }

  login() {
    console.log('Login');
  }

}
