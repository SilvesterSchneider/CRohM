import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserComponent } from './user/user.component';
import { SettingsRoutingModule } from './settings-routing.module';
import { MaterialModule } from '../shared/material.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { OverviewComponent } from './overview/overview.component';
import {MatTabsModule} from '@angular/material/tabs';
import { RolesComponent } from './roles/roles.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role.component';
import { MatDialogModule } from '@angular/material/dialog';
import { UpdateRoleDialogComponent } from './roles/update-role/update-role.component';



@NgModule({
  declarations: [UserComponent, OverviewComponent, RolesComponent, CreateRoleDialogComponent, UpdateRoleDialogComponent],
  imports: [
    CommonModule,
    MatTabsModule,
    SettingsRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
    MatDialogModule,
    FormsModule,
  ], entryComponents: [CreateRoleDialogComponent, UpdateRoleDialogComponent]
})
export class SettingsModule { }
