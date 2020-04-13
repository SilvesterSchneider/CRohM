import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrganizationsAddComponent } from './organizations-add/organizations-add.component';
import { OrganizationsComponent } from './organizations-list/organizations-list.component';



const organizationsRoutes: Routes = [
    {
        path: '',
        component: OrganizationsComponent
    },
    // 'add' needs to be before ':id' to be recognized
    {
        path: 'add',
        component: OrganizationsAddComponent,
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(organizationsRoutes)
    ],
    exports: [
        RouterModule
    ],
    providers: [

    ]
})
export class OrganizationsRoutingModule { }
