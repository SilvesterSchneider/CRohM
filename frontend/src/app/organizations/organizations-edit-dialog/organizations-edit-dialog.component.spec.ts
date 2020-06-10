import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OrganizationsEditDialogComponent } from './organizations-edit-dialog.component';
import { AppRoutingModule } from '../../app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

describe('OrganizationsEditDialogComponent', () => {
	let component: OrganizationsEditDialogComponent;
	let fixture: ComponentFixture<OrganizationsEditDialogComponent>;

	beforeEach(
		async(() => {
			TestBed.configureTestingModule({
				declarations: [ OrganizationsEditDialogComponent ],
				imports: [
					FormsModule,
					ReactiveFormsModule,
					AppRoutingModule,
					BrowserAnimationsModule,
					HttpClientModule,
					SharedModule
				],
				providers: [{
					provide: MatDialogRef,
					useValue: {},
				  },
				  {
					provide: MAT_DIALOG_DATA,
					useValue: {},
				  }]
			}).compileComponents();
		})
	);

	beforeEach(() => {
		fixture = TestBed.createComponent(OrganizationsEditDialogComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
