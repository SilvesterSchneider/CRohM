import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OrganizationsDetailComponent } from './organizations-detail.component';
import { AppRoutingModule } from '../../app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';

describe('OrganizationsAddComponent', () => {
  let component: OrganizationsDetailComponent;
  let fixture: ComponentFixture<OrganizationsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationsDetailComponent ],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule, BrowserAnimationsModule, HttpClientModule, SharedModule],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: {
                organization: {
                  id: 0,
                  description: 'mcu',
                  name: 'promik',
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
                    fax: '0157-00231223344',
                    contactEntries: []
                  },
                  employees: []
                },
              },
            }
          },
        }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationsDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
