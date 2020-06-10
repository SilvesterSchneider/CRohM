import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EventsListComponent } from './events-list/events-list.component';
import { EventsDetailComponent } from './events-detail/events-detail.component';
import { EventsAddComponent } from './events-add/events-add.component';

const eventsRoutes: Routes = [
    {
        path: '',
        component: EventsListComponent
    },
    // 'add' needs to be before ':id' to be recognized
    {
        path: 'add',
        component: EventsAddComponent,
    },
    {
        path: ':id',
        component: EventsDetailComponent,
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(eventsRoutes)
    ],
    exports: [
        RouterModule
    ],
    providers: [
    ]
})
export class EventsRoutingModule { }
