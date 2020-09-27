import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventVisitsComponent } from './event-visits.component';

describe('EventVisitsComponent', () => {
  let component: EventVisitsComponent;
  let fixture: ComponentFixture<EventVisitsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EventVisitsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EventVisitsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
