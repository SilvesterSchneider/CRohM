import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateRoleDialogComponent } from './update-role.component';

describe('UpdateRoleComponent', () => {
  let component: UpdateRoleDialogComponent;
  let fixture: ComponentFixture<UpdateRoleDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UpdateRoleDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateRoleDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
