import { NgModule } from '@angular/core';
import { ContactsListComponent } from './contacts-list/contacts-list.component';
import { ContactsDetailComponent } from './contacts-detail/contacts-detail.component';
import { ContactsRoutingModule } from './contacts-routing.module';
import { ContactsAddComponent } from './contacts-add/contacts-add.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../shared/material.module';
import { ContactsAddHistoryComponent } from './contacts-add-history/contacts-add-history.component';
import { ContactsInfoComponent } from './contacts-info/contacts-info.component';

@NgModule({
  imports: [
    SharedModule,
    ContactsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule
  ],
  declarations: [
    ContactsListComponent,
    ContactsDetailComponent,
    ContactsAddComponent,
    ContactsInfoComponent,
    ContactsAddHistoryComponent
  ],
})
export class ContactsModule { }
