import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OrganizationsAddDialogComponent } from './organizations-add-dialog.component';
import { AppRoutingModule } from '../../app-routing.module';
import { MaterialModule } from '../../shared/material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from '../../shared/shared.module';
import { MatDialogRef } from '@angular/material/dialog';

describe('OrganizationsAddComponent', () => {
	let component: OrganizationsAddDialogComponent;
	let fixture: ComponentFixture<OrganizationsAddDialogComponent>;

	beforeEach(
		async(() => {
			TestBed.configureTestingModule({
				declarations: [ OrganizationsAddDialogComponent ],
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
		fixture = TestBed.createComponent(OrganizationsAddDialogComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
