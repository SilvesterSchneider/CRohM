import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrganizationsAddDialogComponent } from './organizations-add-dialog/organizations-add-dialog.component';
import { OrganizationsRoutingModule } from './organizations-routing.module';
import { OrganizationsListComponent } from './organizations-list/organizations-list.component';
import { OrganizationsDetailComponent } from './organizations-detail/organizations-detail.component';
import { MaterialModule } from '../shared/material.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';

@NgModule({
	declarations: [ OrganizationsAddDialogComponent, OrganizationsListComponent, OrganizationsDetailComponent ],
	imports: [
		CommonModule,
		OrganizationsRoutingModule,
		MaterialModule,
		ReactiveFormsModule,
		FormsModule,
		SharedModule
	]
})
export class OrganizationsModule {}
