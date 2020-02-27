import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
<<<<<<< Updated upstream
=======
import { MaterialModule } from '../shared/material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { isNgTemplate } from '@angular/compiler';
>>>>>>> Stashed changes

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
<<<<<<< Updated upstream
      declarations: [ LoginComponent ]
    })
    .compileComponents();
=======
      declarations: [LoginComponent],
      imports: [MaterialModule, BrowserAnimationsModule]
    })
      .compileComponents();
>>>>>>> Stashed changes
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
    component.username.setValue('');
    component.password.setValue('');

    const form: HTMLElement = fixture.nativeElement;
    expect(form.getElementsByTagName('button').item(0).getAttribute('disabled')).toBeTruthy();
  });


  it('should validate fields correctly', () => {
    component.username.setValue('Username');
    component.password.setValue('Password');
    expect(component.username.valid).toBeTruthy();
    expect(component.password.valid).toBeTruthy();
  });

  it('should enable login on valid fields', () => {
    component.username.setValue('Username');
    component.password.setValue('Password');

    fixture.detectChanges();

    const form: HTMLElement = fixture.nativeElement;
    expect(form.getElementsByTagName('button').item(0).getAttribute('disabled')).toBeFalsy();
  });


});
