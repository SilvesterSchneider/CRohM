import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteEntryDialogComponent } from './delete-entry-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { MaterialModule } from '../material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from '../shared.module';

describe('DeleteEntryDialogComponent', () => {
  let component: DeleteEntryDialogComponent;
  let fixture: ComponentFixture<DeleteEntryDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeleteEntryDialogComponent ],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule, MaterialModule, BrowserAnimationsModule,
        HttpClientModule, SharedModule],
      providers: [{
        provide: MatDialogRef,
        useValue: {},
      },
      {
        provide: MAT_DIALOG_DATA,
        useValue: {
          date: ''
        },
      }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteEntryDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
