import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrganizationsAddComponent } from './organizations-add/organizations-add.component';
import { OrganizationsRoutingModule } from './organizations-routing.module';
import { OrganizationsComponent } from './organizations-list/organizations-list.component';
import { MaterialModule } from '../shared/material.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';



@NgModule({
  declarations: [OrganizationsAddComponent, OrganizationsComponent],
  imports: [
    CommonModule, OrganizationsRoutingModule, MaterialModule, ReactiveFormsModule, FormsModule
  ]
})
export class OrganizationsModule { }
