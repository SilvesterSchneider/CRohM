import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EventsAddComponent } from './events-add.component';
import { AppRoutingModule } from '../../app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from 'src/app/shared/material.module';

describe('EventsAddComponent', () => {
  let component: EventsAddComponent;
  let fixture: ComponentFixture<EventsAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EventsAddComponent ],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule, MaterialModule, BrowserAnimationsModule, HttpClientModule, SharedModule],
      providers: [{
        provide: MatDialogRef,
        useValue: {}
      }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EventsAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
