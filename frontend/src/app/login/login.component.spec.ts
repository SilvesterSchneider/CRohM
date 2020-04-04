import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
import { MaterialModule } from '../shared/material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from '../app-routing.module';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [MaterialModule, BrowserAnimationsModule, HttpClientModule, AppRoutingModule]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should disable login on invalid fields', () => {
    component.userNameOrEmail.setValue('');
    component.password.setValue('');

    const form: HTMLElement = fixture.nativeElement;
    expect(form.getElementsByTagName('button').item(0).getAttribute('disabled')).toBeTruthy();
  });


  it('should validate fields correctly', () => {
    component.userNameOrEmail.setValue('userNameOrEmail');
    component.password.setValue('Password');
    expect(component.userNameOrEmail.valid).toBeTruthy();
    expect(component.password.valid).toBeTruthy();
  });

  it('should enable login on valid fields', () => {
    component.userNameOrEmail.setValue('userNameOrEmail');
    component.password.setValue('Password');

    fixture.detectChanges();

    const form: HTMLElement = fixture.nativeElement;
    expect(form.getElementsByTagName('button').item(0).getAttribute('disabled')).toBeFalsy();
  });


});
