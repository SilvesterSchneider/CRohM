import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../../app-routing.module';
import { MaterialModule } from '../../shared/material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { OrganizationsListComponent } from './organizations-list.component';
import { HttpClientModule } from '@angular/common/http';

describe('OrganizationsComponent', () => {
	let component: OrganizationsListComponent;
	let fixture: ComponentFixture<OrganizationsListComponent>;

	beforeEach(
		async(() => {
			TestBed.configureTestingModule({
				declarations: [ OrganizationsListComponent ],
				imports: [
					FormsModule,
					ReactiveFormsModule,
					AppRoutingModule,
					MaterialModule,
					BrowserAnimationsModule,
					HttpClientModule
				]
			}).compileComponents();
		})
	);

	beforeEach(() => {
		fixture = TestBed.createComponent(OrganizationsListComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
