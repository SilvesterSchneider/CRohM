import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrganizationsListComponent } from './organizations-list/organizations-list.component';
import { OrganizationsDetailComponent } from './organizations-detail/organizations-detail.component';
import { OrganizationsDetailResolverService } from './organizations-detail-resolver.service';

const organizationsRoutes: Routes = [
	{
		path: '',
		component: OrganizationsListComponent
	},
	{
		path: ':id',
		component: OrganizationsDetailComponent,
		resolve: {
			organization: OrganizationsDetailResolverService
		}
	}
];

@NgModule({
	imports: [ RouterModule.forChild(organizationsRoutes) ],
	exports: [ RouterModule ],
	providers: [ OrganizationsDetailResolverService ]
})
export class OrganizationsRoutingModule {}
