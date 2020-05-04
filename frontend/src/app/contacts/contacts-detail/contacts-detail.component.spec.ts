import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactsDetailComponent } from './contacts-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../../app-routing.module';
import { ActivatedRoute } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from '../../shared/shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('ContactsDetailComponent', () => {
  let component: ContactsDetailComponent;
  let fixture: ComponentFixture<ContactsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ContactsDetailComponent],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule, HttpClientModule, SharedModule, BrowserAnimationsModule],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: {
                contact: {
                  id: 0,
                  preName: 'silvester',
                  name: 'kracher',
                  address: {
                    country: 'Deutschland',
                    street: 'Teststrasse 1',
                    city: 'Nürnberg',
                    zipcode: '12345',
                    streetNumber: '10'
                  },
                  contactPossibilities: {
                    mail: 'maxmustermann@getMaxListeners.com',
                    phoneNumber: '0157 0011223344',
                    fax: '0157-00231223344'
                  }
                },
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

 /* it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show vorname', () => {
    expect(component.contactsForm.value.preName).toEqual('silvester');
  });

  it('should show nachname', () => {
    expect(component.contactsForm.value.name).toEqual('kracher');
  }); 

  it('should show adress', () => {
    expect(component.contactsForm.value.address.street).toEqual('Teststrasse 1');
  });

  it('should show plz', () => {
    expect(component.contactsForm.value.address.zipcode).toEqual('12345');
  });

  it('should show ort', () => {
    expect(component.contactsForm.value.address.city).toEqual('Nürnberg');
  }); */
});
