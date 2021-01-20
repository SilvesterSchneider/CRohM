import { Injectable } from '@angular/core';
import { AuthService, CredentialsDto, UserDto, UsersService } from '../shared/api-generated/api-generated';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class JwtService {

  public static LS_KEY = 'access_token';
  public static LS_SUPERADMIN = 'superadmin';

  constructor(private authService: AuthService, private router: Router, private userService: UsersService) { }

  public login(credentials: CredentialsDto) {
    return this.authService.login(credentials).pipe(tap(user => {
      localStorage.setItem(JwtService.LS_KEY, user.accessToken);
      localStorage.setItem(JwtService.LS_SUPERADMIN, `${user.isSuperAdmin}`);
    }));
  }

  public logout() {
    localStorage.removeItem(JwtService.LS_KEY);
  }

  public isLoggedIn(): boolean {
    return localStorage.getItem(JwtService.LS_KEY) !== null;
  }

  public goToLogin() {
    return this.router.navigate(['/login']);
  }

  public getUserId(): number {
    const jwt = localStorage.getItem(JwtService.LS_KEY);
    if (jwt != null && jwt.length > 0 && jwt.indexOf('.') < jwt.lastIndexOf('.')) {
      let text = jwt.substr(jwt.indexOf('.') + 1);
      text = text.substr(0, text.indexOf('.'));
      const decodedText = atob(text);
      const obj = JSON.parse(decodedText);
      return parseInt(obj.Id, 6);
    } else {
      return 0;
    }
  }

  public isSuperAdmin(): boolean {
    const value = localStorage.getItem(JwtService.LS_SUPERADMIN);
    return value !== null && value === 'true';
  }

  //
  public hasPermission(permission: string): boolean {
    const jwt = localStorage.getItem(JwtService.LS_KEY);
    if (jwt != null && jwt.length > 0 && jwt.indexOf('.') < jwt.lastIndexOf('.')) {
      let text = jwt.substr(jwt.indexOf('.') + 1);
      text = text.substr(0, text.indexOf('.'));
      let decodedText = atob(text);
      decodedText = decodeURIComponent(escape(window.atob(text)));
      permission = decodeURIComponent(permission);
      decodedText = decodedText.substr(decodedText.indexOf('[') + 1);
      decodedText = decodedText.substr(0, decodedText.indexOf(']'));
      // TODO: Remove before merging, just used for testing
      // alert(permission + ':' + decodedText);
      return decodedText.includes(permission);
    }

    return false;
  }

  public isDatenschutzbeauftragter(): boolean {
    return this.isInRole('Datenschutzbeauftragter');
  }

  public isAdmin(): boolean {
    return this.isInRole('Admin');
  }

  public isInRole(roleToCheck: string): boolean {
    const jwt = localStorage.getItem(JwtService.LS_KEY);

    if (jwt != null && jwt.length > 0 && jwt.indexOf('.') < jwt.lastIndexOf('.')) {
      let text = jwt.substr(jwt.indexOf('.') + 1);
      text = text.substr(0, text.indexOf('.'));
      const decodedText = atob(text);
      const obj = JSON.parse(decodedText);
      return obj.role?.includes(roleToCheck);
    } else {
      return false;
    }
  }
}
