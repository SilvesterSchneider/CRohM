import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SidenavComponent } from './sidenav.component';
import { JwtService } from '../../jwt.service';
import { MaterialModule } from '../../material.module';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from '../../../app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('SidenavComponent', () => {
  let component: SidenavComponent;
  let fixture: ComponentFixture<SidenavComponent>;

  let jwtServiceMock;

  beforeEach(async(() => {
    jwtServiceMock = jasmine.createSpyObj('JwtService', ['isLoggedIn']);
    jwtServiceMock.isLoggedIn.and.returnValue(false);

    TestBed.configureTestingModule({
      declarations: [SidenavComponent],
      imports: [MaterialModule, HttpClientModule, AppRoutingModule, BrowserAnimationsModule],
      providers: [{
        provide: JwtService, useValue: jwtServiceMock
      }]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SidenavComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not be loggedin', () => {
    const mockJWTService = TestBed.inject(JwtService) as any;
    mockJWTService.isLoggedIn.and.returnValue(false);
    expect(component.isLoggedIn()).toBeFalsy();
  });

  it('should be loggedin', () => {
    const mockJWTService = TestBed.inject(JwtService) as any;
    mockJWTService.isLoggedIn.and.returnValue(true);
    expect(component.isLoggedIn()).toBeTruthy();
  });

  it('should not show navigation items', () => {
    const form: HTMLElement = fixture.nativeElement;
    expect(form.getElementsByClassName('navigation-items').length).toBe(0);
  });

  it('should not show user-menu', () => {
    const form: HTMLElement = fixture.nativeElement;
    expect(form.getElementsByTagName('app-user-menu').length).toBe(0);
  });
});
