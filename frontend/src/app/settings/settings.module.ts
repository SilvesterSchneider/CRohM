import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserComponent } from './user/user.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTabsModule } from '@angular/material/tabs';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SettingsRoutingModule } from './settings-routing.module';
import { MaterialModule } from '../shared/material.module';
import { OverviewComponent } from './overview/overview.component';
import { RolesComponent } from './roles/roles.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role.component';
import { UpdateRoleDialogComponent } from './roles/update-role/update-role.component';
import { AddUserDialogComponent } from './user/add-user/add-user.component';
import { EditUserDialogComponent } from './user/edit-user/edit-user.component';
import { DeleteEntryDialogModule } from '../shared/form/delete-entry-dialog/delete-entry-dialog.module';
import { MailComponent } from './mail/mail.component';



@NgModule({
  declarations: [UserComponent, OverviewComponent, RolesComponent, CreateRoleDialogComponent, UpdateRoleDialogComponent,
    EditUserDialogComponent, AddUserDialogComponent, MailComponent],
  imports: [
    DeleteEntryDialogModule,
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
