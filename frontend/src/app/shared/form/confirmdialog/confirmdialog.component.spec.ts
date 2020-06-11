import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmDialogComponent, ConfirmDialogModel } from './confirmdialog.component';

describe('ConfirmDialogComponent', () => {
  let component: ConfirmDialogComponent;
  let fixture: ComponentFixture<ConfirmDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    // Angezeigte Ueberschrift bzw. Nachricht im Confirm-Dialog
    const message = `Testnachricht`;
    const dialogData = new ConfirmDialogModel('Testueberschrift', message);

    fixture = TestBed.createComponent(ConfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

/*   it('should create', () => {
    expect(component).toBeTruthy();
  }); */
});

