import { Injectable } from '@angular/core';
import { AuthService, CredentialsDto } from '../shared/api-generated/api-generated';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class JwtService {

  public static LS_KEY = 'access_token';

  constructor(private authService: AuthService, private router: Router) { }

  public login(credentials: CredentialsDto) {
    return this.authService.login(credentials).pipe(tap(res => {
      localStorage.setItem(JwtService.LS_KEY, res.accessToken);
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

  public isDatenschutzbeauftragter(): boolean {
    const jwt = localStorage.getItem(JwtService.LS_KEY);

    if (jwt != null && jwt.length > 0 && jwt.indexOf('.') < jwt.lastIndexOf('.')) {
      let text = jwt.substr(jwt.indexOf('.') + 1);
      text = text.substr(0, text.indexOf('.'));
      const decodedText = atob(text);
      const obj = JSON.parse(decodedText);
      return obj.role?.includes('Datenschutzbeauftragter');
    } else {
      return false;
    }
  }
}
