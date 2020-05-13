import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactPossibilitiesComponent } from './contact-possibilities.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MaterialModule } from '../material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('ContactPossibilitiesComponent', () => {
  let component: ContactPossibilitiesComponent;
  let fixture: ComponentFixture<ContactPossibilitiesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContactPossibilitiesComponent ],
      imports: [FormsModule, ReactiveFormsModule, HttpClientModule, MaterialModule, BrowserAnimationsModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactPossibilitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
