import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './shared/routing/auth.guard';
import { OrganizationsListComponent } from './organizations/organizations-list/organizations-list.component';
import { ApproveContactComponent } from './approve-contact/approve-contact.component';
import { ContactsIdResolver2Service } from './approve-contact/contacts-id-resolver2.service';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    component: HomeComponent
  },
  {
    path: 'contacts',
    canActivate: [AuthGuard],
    loadChildren: () => import('./contacts/contacts.module').then(mod => mod.ContactsModule)
  },
  {
    path: 'organizations',
    canActivate: [AuthGuard],
    loadChildren: () => import('./organizations/organizations.module').then(mod => mod.OrganizationsModule)
  },
  { path: 'login', component: LoginComponent },
  {
    path: 'settings',
    canActivate: [AuthGuard],
    loadChildren: () => import('./settings/settings.module').then(mod => mod.SettingsModule)
  },
  {
    path: 'events',
    canActivate: [AuthGuard],
    loadChildren: () => import('./events/events.module').then(mod => mod.EventsModule)
  },
  {
    path: 'ApproveContacte/:id',
    component: ApproveContactComponent,
    resolve: {
			contact: ContactsIdResolver2Service
		}
  },
  },
  {
    path: 'statistics',
    canActivate: [AuthGuard],
    loadChildren: () => import('./statistics/statistics.module').then(mod => mod.StatisticsModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [ ContactsIdResolver2Service ]
})
export class AppRoutingModule { }
