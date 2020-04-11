import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OsmAddressComponent } from './osm-address.component';

describe('OsmAddressComponent', () => {
  let component: OsmAddressComponent;
  let fixture: ComponentFixture<OsmAddressComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OsmAddressComponent ]
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
