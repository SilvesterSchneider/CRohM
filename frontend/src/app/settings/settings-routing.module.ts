import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserComponent } from './user/user.component';
import { OverviewComponent } from './overview/overview.component';


const settingsRoutes: Routes = [
    {
        path: '',
        component: OverviewComponent
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
