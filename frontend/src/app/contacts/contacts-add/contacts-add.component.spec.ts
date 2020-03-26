import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ContactsAddComponent } from './contacts-add.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../../app-routing.module';

describe('ContactsAddComponent', () => {
  let component: ContactsAddComponent;
  let fixture: ComponentFixture<ContactsAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ContactsAddComponent],
      imports: [FormsModule, ReactiveFormsModule, AppRoutingModule]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactsAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
