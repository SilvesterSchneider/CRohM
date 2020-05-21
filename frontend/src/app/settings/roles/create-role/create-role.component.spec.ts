import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateRoleDialogComponent } from './create-role.component';

describe('CreateRoleComponent', () => {
  let component: CreateRoleDialogComponent;
  let fixture: ComponentFixture<CreateRoleDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateRoleDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateRoleDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
