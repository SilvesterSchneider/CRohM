import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DpDisclaimerDialogComponent } from './dp-disclaimer-dialog.component';

describe('DpDisclaimerDialogComponent', () => {
  let component: DpDisclaimerDialogComponent;
  let fixture: ComponentFixture<DpDisclaimerDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DpDisclaimerDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DpDisclaimerDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
