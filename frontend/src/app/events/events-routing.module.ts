import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EventsListComponent } from './Events-list/Events-list.component';
import { EventsDetailComponent } from './Events-detail/Events-detail.component';
import { EventsDetailResolverService } from './events-detail-resolver.service';
import { EventsAddComponent } from './Events-add/Events-add.component';

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
        resolve: {
            event: EventsDetailResolverService
        }
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
        EventsDetailResolverService
    ]
})
export class EventsRoutingModule { }
