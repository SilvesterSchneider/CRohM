import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContactsListComponent } from './contacts-list/contacts-list.component';
import { ContactsDetailComponent } from './contacts-detail/contacts-detail.component';
import { ContactsDetailResolverService } from './contacts-detail-resolver.service';
import { ContactsAddComponent } from './contacts-add/contacts-add.component';

const contactsRoutes: Routes = [
    {
        path: '',
        component: ContactsListComponent
    },
    // 'add' needs to be before ':id' to be recognized
    {
        path: 'add',
        component: ContactsAddComponent,
    },
    {
        path: ':id',
        component: ContactsDetailComponent,
        resolve: {
            contact: ContactsDetailResolverService
        }
    }    
];

@NgModule({
    imports: [
        RouterModule.forChild(contactsRoutes)
    ],
    exports: [
        RouterModule
    ],
     providers: [
         ContactsDetailResolverService
     ]
})
export class ContactsRoutingModule { }
