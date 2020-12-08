import { NgModule } from '@angular/core';
import { EventsListComponent } from './events-list/events-list.component';
import { EventsDetailComponent } from './events-detail/events-detail.component';
import { EventsRoutingModule } from './events-routing.module';
import { EventsAddComponent } from './events-add/events-add.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../shared/material.module';
import { EventsInfoComponent } from './events-info/events-info.component';
import { EventsInvitationComponent } from './events-invitation/events-invitation.component';
import { DeletionConfirmationDialogComponent } from './deletion-confirmation-dialog/deletion-confirmation-dialog.component';
import { ConfirmationInvitationDialogComponent } from './confirmation-invitation-dialog/confirmation-invitation-dialog.component';

@NgModule({
  imports: [
    SharedModule,
    EventsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule
  ],
  declarations: [
    EventsListComponent,
    EventsDetailComponent,
    EventsAddComponent,
    EventsInfoComponent,
    EventsInvitationComponent,
    DeletionConfirmationDialogComponent,
    ConfirmationInvitationDialogComponent
  ],
})
export class EventsModule { }
