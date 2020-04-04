import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserMenuComponent } from './user-menu.component';
import { HttpClientModule } from '@angular/common/http';
import { RouterTestingModule } from '@angular/router/testing';
import { AppRoutingModule } from '../../../app-routing.module';
import { MaterialModule } from '../../material.module';
import { Router } from '@angular/router';

describe('UserMenuComponent', () => {
  let component: UserMenuComponent;
  let fixture: ComponentFixture<UserMenuComponent>;
  let router: Router;


  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [UserMenuComponent],
      imports: [HttpClientModule, AppRoutingModule, MaterialModule, RouterTestingModule.withRoutes([])]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserMenuComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should logout', () => {
    const navigateSpy = spyOn(router, 'navigate');

    component.logout();
    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });

});
