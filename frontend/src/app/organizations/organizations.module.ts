import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrganizationsAddComponent } from './organizations-add/organizations-add.component';
import { OrganizationsRoutingModule } from './organizations-routing.module';




@NgModule({
  declarations: [OrganizationsAddComponent],
  imports: [
    CommonModule, OrganizationsRoutingModule
  ]
})
export class OrganizationsModule { }
