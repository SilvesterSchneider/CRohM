import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrganizationsAddComponent } from './organizations-add/organizations-add.component';
import { OrganizationsRoutingModule } from './organizations-routing.module';
import { OrganizationsListComponent } from './organizations-list/organizations-list.component';
import { OrganizationsDetailComponent } from './organizations-detail/organizations-detail.component';
import { MaterialModule } from '../shared/material.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { OrganizationsInfoComponent } from './organizations-info/organizations-info.component';

@NgModule({
  declarations: [
    OrganizationsAddComponent,
    OrganizationsListComponent,
    OrganizationsDetailComponent,
    OrganizationsInfoComponent
  ],
  imports: [
    CommonModule,
    OrganizationsRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule
  ]
})
export class OrganizationsModule { }
