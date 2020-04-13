import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OrganizationsAddComponent } from './organizations-add.component';
import { AppRoutingModule } from '../../app-routing.module';
import { MaterialModule } from '../../shared/material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';

describe('OrganizationsAddComponent', () => {
  let component: OrganizationsAddComponent;
  let fixture: ComponentFixture<OrganizationsAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationsAddComponent ],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule, MaterialModule, BrowserAnimationsModule, HttpClientModule]
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
