import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../shared/material.module';
import { MatDialogModule } from '@angular/material/dialog';
import { ContactsRoutingModule } from './contacts-routing.module';
import { ContactsListComponent } from './contacts-list/contacts-list.component';
import { ContactsAddDialogComponent } from './contacts-add-dialog/contacts-add-dialog.component';
import { ContactsEditDialogComponent } from './contacts-edit-dialog/contacts-edit-dialog.component';
import { ContactsDetailComponent } from './contacts-detail/contacts-detail.component';
import { ContactsInfoComponent } from './contacts-info/contacts-info.component';
import { ContactsDisclosureDialogComponent } from './contacts-disclosure-dialog/contacts-disclosure-dialog.component';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { ContactsSendMailDialogComponent } from './contacts-send-mail-dialog/contacts-send-mail-dialog.component';

@NgModule({
	imports: [
		SharedModule,
		FormsModule,
		ReactiveFormsModule, MaterialModule, MatDialogModule, ContactsRoutingModule, MatSnackBarModule ],
	declarations: [
		ContactsListComponent,
		ContactsAddDialogComponent,
		ContactsEditDialogComponent,
		ContactsDetailComponent,
		ContactsInfoComponent,
		ContactsDisclosureDialogComponent,
		ContactsSendMailDialogComponent
	]
})
export class ContactsModule {}
