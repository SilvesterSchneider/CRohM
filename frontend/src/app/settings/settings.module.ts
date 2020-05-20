import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserComponent } from './user/user.component';
import { SettingsRoutingModule } from './settings-routing.module';
import { MaterialModule } from '../shared/material.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { OverviewComponent } from './overview/overview.component';
import {MatTabsModule} from '@angular/material/tabs';
import { RolesComponent } from './roles/roles.component';



@NgModule({
  declarations: [UserComponent, OverviewComponent, RolesComponent],
  imports: [
    CommonModule,
    MatTabsModule,
    SettingsRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
    FormsModule,
  ]
})
export class SettingsModule { }
