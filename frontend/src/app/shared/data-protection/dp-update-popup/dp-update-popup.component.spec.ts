import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DpUpdatePopupComponent } from './dp-update-popup.component';

describe('DpUpdatePopupComponent', () => {
  let component: DpUpdatePopupComponent;
  let fixture: ComponentFixture<DpUpdatePopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DpUpdatePopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DpUpdatePopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
