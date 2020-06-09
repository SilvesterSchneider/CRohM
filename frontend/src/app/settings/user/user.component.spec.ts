import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserComponent } from './user.component';
import { MaterialModule } from '../../shared/material.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';

describe('UserComponent', () => {
  let component: UserComponent;
  let fixture: ComponentFixture<UserComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [UserComponent],
      imports: [MaterialModule, ReactiveFormsModule, FormsModule, BrowserAnimationsModule, HttpClientModule]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should disable addUser on invalid fields', () => {
    component.userForm.get('email').setValue('');
    component.userForm.get('firstName').setValue('');
    component.userForm.get('lastName').setValue('');

    const form: HTMLElement = fixture.nativeElement;
    const buttons = form.getElementsByTagName('button');
    expect(buttons.item(buttons.length - 1).getAttribute('disabled')).toBeTruthy();
  });


  it('should validate email correctly', () => {
    component.userForm.get('email').setValue('wrongmail');
    expect(component.userForm.get('email').valid).toBeFalsy();
  });

  it('should enable button on valid fields', () => {
    component.userForm.get('email').setValue('test@mail.com');
    component.userForm.get('firstName').setValue('FirstName');
    component.userForm.get('lastName').setValue('LastName');

    fixture.detectChanges();

    const form: HTMLElement = fixture.nativeElement;
    const buttons = form.getElementsByTagName('button');
    expect(buttons.item(buttons.length - 1).getAttribute('disabled')).toBeFalsy();
  });
});
