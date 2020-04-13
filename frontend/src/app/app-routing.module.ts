import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './shared/routing/auth.guard';
import { OrganizationsComponent } from './organizations/organizations-list/organizations-list.component';

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
  { path: 'login', component: LoginComponent },
  {
    path: 'settings',
    canActivate: [AuthGuard],
    loadChildren: () => import('./settings/settings.module').then(mod => mod.SettingsModule)
  },
  {
    path: 'organizations',
    component: OrganizationsComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
