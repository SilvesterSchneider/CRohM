import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ObjectsCreationComponent } from './objects-creation.component';

describe('ObjectsCreationComponent', () => {
  let component: ObjectsCreationComponent;
  let fixture: ComponentFixture<ObjectsCreationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ObjectsCreationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ObjectsCreationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
