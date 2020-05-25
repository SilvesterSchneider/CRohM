import { NgModule } from '@angular/core';
import { EventsListComponent } from './events-list/Events-list.component';
import { EventsDetailComponent } from './events-detail/Events-detail.component';
import { EventsRoutingModule } from './events-routing.module';
import { EventsAddComponent } from './events-add/Events-add.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../shared/material.module';
import { EventsInfoComponent } from './events-info/events-info.component';

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
    EventsInfoComponent
  ],
})
export class EventsModule { }
