import { TestBed } from '@angular/core/testing';

import { JwtService } from './jwt.service';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from '../app-routing.module';
import { AuthService } from './api-generated/api-generated';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('JwtService', () => {
  let service: JwtService;
  let authServiceMock;
  let router: Router;



  beforeEach(() => {
    authServiceMock = jasmine.createSpyObj('AuthService', ['login']);
    authServiceMock.login.and.returnValue(of({ accessToken: 'accesstoken' }));

    localStorage.clear();

    TestBed.configureTestingModule({
      imports: [HttpClientModule, AppRoutingModule, RouterTestingModule.withRoutes([])],
      providers: [{
        provide: AuthService, useValue: authServiceMock
      }]

    });
    service = TestBed.inject(JwtService);
    router = TestBed.inject(Router);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login', () => {
    return service.login(null).subscribe(() => {
      expect(localStorage.getItem(JwtService.LS_KEY)).toEqual('accesstoken');
    });
  });

  it('should logout', () => {
    localStorage.setItem(JwtService.LS_KEY, 'test');
    service.logout();
    expect(localStorage.getItem(JwtService.LS_KEY)).toBeNull();
  });

  it('should be loggedin', () => {
    localStorage.setItem(JwtService.LS_KEY, 'test');

    expect(service.isLoggedIn()).toBeTruthy();
  });

  it('should not be loggedin', () => {
    expect(service.isLoggedIn()).toBeFalsy();
  });

  it('should go to login', () => {
    const navigateSpy = spyOn(router, 'navigate');

    service.goToLogin();
    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });


});
