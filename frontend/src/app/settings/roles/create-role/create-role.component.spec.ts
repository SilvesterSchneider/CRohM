import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateRoleDialogComponent } from './create-role.component';
import {  MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { MaterialModule } from 'src/app/shared/material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from 'src/app/shared/shared.module';

describe('CreateRoleComponent', () => {
  let component: CreateRoleDialogComponent;
  let fixture: ComponentFixture<CreateRoleDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateRoleDialogComponent ],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule, MaterialModule, BrowserAnimationsModule,
        HttpClientModule, SharedModule],
      providers: [{
        provide: MatDialogRef,
        useValue: {},
      },
      {
        provide: MAT_DIALOG_DATA,
        useValue: {
          permissions: [
            {
              name: 'test1'
            },
            {
              name: 'test2'
            }
          ]
        }
      }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateRoleDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
