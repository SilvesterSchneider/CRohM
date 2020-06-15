import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactsEditDialogComponent } from './contacts-edit-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../../app-routing.module';
import { ActivatedRoute } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from '../../shared/shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

describe('ContactsDetailComponent', () => {
  let component: ContactsEditDialogComponent;
  let fixture: ComponentFixture<ContactsEditDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ContactsEditDialogComponent],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule, HttpClientModule, SharedModule, BrowserAnimationsModule],
      providers: [{
        provide: MatDialogRef,
        useValue: {},
      },
      {
        provide: MAT_DIALOG_DATA,
        useValue: {
          contactPossibilities: {
            contactEntries: []
          }
        },
      }]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactsEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
