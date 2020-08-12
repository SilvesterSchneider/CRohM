import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactsDisclosureDialogComponent } from './contacts-disclosure-dialog.component';

describe('ContactsDisclosureDialogComponent', () => {
  let component: ContactsDisclosureDialogComponent;
  let fixture: ComponentFixture<ContactsDisclosureDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContactsDisclosureDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactsDisclosureDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
