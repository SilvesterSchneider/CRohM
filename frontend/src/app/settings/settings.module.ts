import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserComponent, DialogAdd } from './user/user.component';
import { SettingsRoutingModule } from './settings-routing.module';
import { MaterialModule } from '../shared/material.module';
import { ReactiveFormsModule } from '@angular/forms';



@NgModule({
  declarations: [UserComponent, DialogAdd],
  imports: [
    CommonModule,
    SettingsRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
  ]
})
export class SettingsModule { }
