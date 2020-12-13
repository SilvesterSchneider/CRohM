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
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';

@NgModule({
  imports: [
    SharedModule,
    EventsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    NgxMaterialTimepickerModule
  ],
  declarations: [
    EventsListComponent,
    EventsDetailComponent,
    EventsAddComponent,
    EventsInfoComponent,
    EventsInvitationComponent
  ],
})
export class EventsModule { }
