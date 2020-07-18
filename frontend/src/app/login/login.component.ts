import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { JwtService } from '../shared/jwt.service';
import { UserDto } from '../shared/api-generated/api-generated';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ChangePasswordComponent } from './change-password-dialog/change-password-dialog.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  userNameOrEmail = new FormControl('', Validators.required);
  password = new FormControl('', Validators.required);
  errorText: string;
  user: UserDto;
  matDialog: MatDialogRef<ChangePasswordComponent>;

  constructor(private jwtService: JwtService, private router: Router, private route: ActivatedRoute, private dialog: MatDialog) { }

  login() {
    this.jwtService.login({
      userNameOrEmail: this.userNameOrEmail.value,
      password: this.password.value
    }).subscribe((x) => {
      this.user = x;
      this.redirect(); },
      error => this.errorText = error
    );
  }

  redirect() {
    if (this.user.hasPasswordChanged) {
      this.router.navigate([''], { relativeTo: this.route });
    } else {
      this.matDialog = this.dialog.open(ChangePasswordComponent, { data: this.user, disableClose: true });
      this.matDialog.afterClosed().subscribe(x => this.router.navigate([''], { relativeTo: this.route }));
    }
  }

  loginWithProof() {
    if (this.userNameOrEmail.valid && this.password.valid) {
      this.login();
    }
  }
}
