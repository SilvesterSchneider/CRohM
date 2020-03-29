import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserComponent } from './user/user.component';


const settingsRoutes: Routes = [
    {
        path: '',
        component: UserComponent
    },
];

@NgModule({
    imports: [
        RouterModule.forChild(settingsRoutes)
    ],
    exports: [
        RouterModule
    ],
    providers: []
})
export class SettingsRoutingModule { }
