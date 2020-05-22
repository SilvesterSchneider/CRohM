import { NgModule } from '@angular/core';
import { EventsListComponent } from './Events-list/Events-list.component';
import { EventsDetailComponent } from './Events-detail/Events-detail.component';
import { EventsRoutingModule } from './Events-routing.module';
import { EventsAddComponent } from './Events-add/Events-add.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../shared/material.module';

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
  ],
})
export class EventsModule { }
