import { TestBed } from '@angular/core/testing';

import { AuthGuard } from './auth.guard';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from '../../app-routing.module';
import { JwtService } from '../jwt.service';

describe('AuthGuard', () => {
  let guard: AuthGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule, AppRoutingModule]
    });
    guard = TestBed.inject(AuthGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });

  it('should be activated', () => {
    localStorage.setItem(JwtService.LS_KEY, 'test');
    expect(guard.canActivate()).toBeTruthy();
  });

  it('should not activate', () => {
    localStorage.clear();
    return (guard.canActivate() as Promise<boolean>).then(res => {
      expect(res).toBeFalsy();
    });
  });
});
