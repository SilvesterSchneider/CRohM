import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrganizationsAddComponent } from './organizations-add/organizations-add.component';



const organizationsRoutes: Routes = [
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
