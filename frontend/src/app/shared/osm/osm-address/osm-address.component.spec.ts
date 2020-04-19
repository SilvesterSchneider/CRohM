import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OsmAddressComponent } from './osm-address.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MaterialModule } from '../../material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('OsmAddressComponent', () => {
  let component: OsmAddressComponent;
  let fixture: ComponentFixture<OsmAddressComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OsmAddressComponent ],
      imports: [FormsModule, ReactiveFormsModule, HttpClientModule, MaterialModule, BrowserAnimationsModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OsmAddressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
