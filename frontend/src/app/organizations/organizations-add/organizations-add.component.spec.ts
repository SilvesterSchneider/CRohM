import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationsAddComponent } from './organizations-add.component';

describe('OrganizationsAddComponent', () => {
  let component: OrganizationsAddComponent;
  let fixture: ComponentFixture<OrganizationsAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationsAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationsAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
