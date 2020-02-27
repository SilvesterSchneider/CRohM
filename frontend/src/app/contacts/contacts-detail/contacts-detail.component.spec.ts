import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactsDetailComponent } from './contacts-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../../app-routing.module';
import { ActivatedRoute } from '@angular/router';
import { CONTACTS } from '../mock-contacts';

describe('ContactsDetailComponent', () => {
  let component: ContactsDetailComponent;
  let fixture: ComponentFixture<ContactsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ContactsDetailComponent],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: {
                contact: CONTACTS[0]
              },
            }
          },
        }]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactsDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show vorname', () => {
    expect(component.contactsForm.get('vorname').value).toEqual('Max');
  });

  it('should show nachname', () => {
    expect(component.contactsForm.get('nachname').value).toEqual('Mustermann');
  });

  it('should show adress', () => {
    expect(component.contactsForm.get('adresse.strasse').value).toEqual('Teststrasse 1');
  });

  it('should show plz', () => {
    expect(component.contactsForm.get('adresse.plz').value).toEqual('12345');
  });

  it('should show ort', () => {
    expect(component.contactsForm.get('adresse.ort').value).toEqual('Nürnberg');
  });
});
