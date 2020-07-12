import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ContactsAddDialogComponent } from './contacts-add-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../../app-routing.module';
import { MaterialModule } from '../../shared/material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from '../../shared/shared.module';
import { MatDialogRef } from '@angular/material/dialog';

describe('ContactsAddDialogComponent', () => {
	let component: ContactsAddDialogComponent;
	let fixture: ComponentFixture<ContactsAddDialogComponent>;

	beforeEach(
		async(() => {
			TestBed.configureTestingModule({
				declarations: [ ContactsAddDialogComponent ],
				imports: [
					FormsModule,
					ReactiveFormsModule,
					AppRoutingModule,
					MaterialModule,
					BrowserAnimationsModule,
					HttpClientModule,
					SharedModule
				],
				providers: [{
					provide: MatDialogRef,
					useValue: {}
				  }]
			}).compileComponents();
		})
	);

	beforeEach(() => {
		fixture = TestBed.createComponent(ContactsAddDialogComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
