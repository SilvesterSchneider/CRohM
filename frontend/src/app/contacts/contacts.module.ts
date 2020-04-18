import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactsListComponent } from './contacts-list/contacts-list.component';
import { ContactsDetailComponent } from './contacts-detail/contacts-detail.component';
import { MaterialModule } from '../shared/material.module';
import { ContactsRoutingModule } from './contacts-routing.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ContactsAddComponent } from './contacts-add/contacts-add.component';
import { MatSelectModule } from '@angular/material/select';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MaterialModule,
    ContactsRoutingModule,
    MatSelectModule
  ],
  declarations: [
    ContactsListComponent,
    ContactsDetailComponent,
    ContactsAddComponent,
  ],
})
export class ContactsModule { }
