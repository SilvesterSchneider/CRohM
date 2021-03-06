import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../shared/material.module';
import { EventVisitsComponent } from './event-visits/event-visits.component';
import { StatisticsRoutingModule } from './statistics-routing.module';
import { OverviewComponent } from './overview/overview.component';
import { ObjectsCreationComponent } from './objects-creation/objects-creation.component';
import { TagsComponent } from './tags/tags.component';


@NgModule({
  imports: [
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    StatisticsRoutingModule
  ],
  declarations: [
    EventVisitsComponent,
    OverviewComponent,
    ObjectsCreationComponent,
    TagsComponent
  ],
})
export class StatisticsModule { }
