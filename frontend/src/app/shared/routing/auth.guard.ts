import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild } from '@angular/router';
import { JwtService } from '../jwt.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild {

  constructor(private jwtService: JwtService) { }

  canActivate() {

    const loggedin = this.jwtService.isLoggedIn();

    if (!loggedin) {
      return this.jwtService.goToLogin().then(() => false);
    }

    // TODO: check roles once we have role-based access
    return true;
  }
  canActivateChild() {
    return this.canActivate();
  }
}
