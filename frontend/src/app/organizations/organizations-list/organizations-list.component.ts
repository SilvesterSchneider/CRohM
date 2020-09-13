import { Component, OnInit, ChangeDetectorRef, Injectable, OnDestroy, ViewChild } from '@angular/core';
import { OrganizationService, UsersService } from '../../shared/api-generated/api-generated';
import { Observable, Subscription } from 'rxjs';
import { OrganizationDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { OrganizationsInfoComponent } from '../organizations-info/organizations-info.component';
import { OrganizationsAddDialogComponent } from '../organizations-add-dialog/organizations-add-dialog.component';
import { OrganizationsEditDialogComponent } from '../organizations-edit-dialog/organizations-edit-dialog.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { JwtService } from 'src/app/shared/jwt.service';
import { AddHistoryComponent } from 'src/app/shared/add-history/add-history.component';
import { MatTableDataSource } from '@angular/material/table';
import { TagsFilterComponent } from 'src/app/shared/tags-filter/tags-filter.component';

@Component({
	selector: 'app-organizations-list',
	templateUrl: './organizations-list.component.html',
	styleUrls: ['./organizations-list.component.scss']
})
@Injectable({
	providedIn: 'root'
})

export class OrganizationsListComponent implements OnInit, OnDestroy {
	@ViewChild(TagsFilterComponent, { static: true })
	tagsFilter: TagsFilterComponent;
	service: OrganizationService;
	organizations: Observable<OrganizationDto[]>;
	allOrganizations: OrganizationDto[];
	displayedColumns = [];
	currentScreenWidth = '';
	flexMediaWatcher: Subscription;
	length = 0;
	isAdminUserLoggedIn = false;
	dataSource = new MatTableDataSource<OrganizationDto>();

	constructor(public dialog: MatDialog, service: OrganizationService,
		           private changeDetectorRefs: ChangeDetectorRef, private mediaObserver: MediaObserver, private jwt: JwtService) {
		this.service = service;
		this.flexMediaWatcher = mediaObserver.asObservable().subscribe((change: MediaChange[]) => {
			if (change[0].mqAlias !== this.currentScreenWidth) {
				this.currentScreenWidth = change[0].mqAlias;
				this.setupTable();
			}
		});
	}

	applyFilter(event: Event) {
		const filterValue = (event.target as HTMLInputElement).value;
		this.dataSource.filter = filterValue.trim().toLowerCase();
		this.dataSource.filterPredicate = ((data, filter) => {
		  if (data.name.trim().toLowerCase().includes(filter) || data.description.trim().toLowerCase().includes(filter) ||
			data.address.street.trim().toLowerCase().includes(filter) || data.address.streetNumber.trim().toLowerCase().includes(filter) ||
			data.address.zipcode.trim().toLowerCase().includes(filter) || data.address.city.trim().toLowerCase().includes(filter) ||
			data.address.country.trim().toLowerCase().includes(filter) || data.contact.phoneNumber.trim().toLowerCase().includes(filter) ||
			data.contact.mail.trim().toLowerCase().includes(filter) || data.contact.fax.trim().toLowerCase().includes(filter)) {
			return true;
		  } else {
			return false;
		  }
		});
	  }

	ngOnInit(): void {
		this.isAdminUserLoggedIn = this.jwt.getUserId() === 1;
		this.tagsFilter.setRefreshTableFunction(() => this.applyTagFilter());
		this.getData();
	}

	applyTagFilter() {
		this.dataSource = new MatTableDataSource<OrganizationDto>();
		this.allOrganizations.forEach(x => {
		  if (this.tagsFilter.areAllTagsIncluded(x.tags)) {
			this.dataSource.data.push(x);
		  }
		});
	  }

	ngOnDestroy(): void {
		this.flexMediaWatcher.unsubscribe();
	}

	setupTable() {
		if (this.currentScreenWidth === 'xs') {
			// only display prename and name on larger screens
			this.displayedColumns = ['Name', 'Zugehörige', 'Action'];
		} else {
			this.displayedColumns = [
				'Name',
				'Beschreibung',
				'Strasse',
				'Hausnummer',
				'PLZ',
				'Stadt',
				'Land',
				'Telefonnummer',
				'E-Mail',
				'Faxnummer',
				'Zugehörige',
				'Action'
			];
		}
	}

	private getData() {
		this.organizations = this.service.get();
		this.organizations.subscribe(x => {
			 this.length = x.length;
			 this.dataSource.data = x;
			 this.allOrganizations = x;
			 this.tagsFilter.updateTagsInAutofill(this.allOrganizations);
		});
		this.changeDetectorRefs.detectChanges();
		// this.organizationMock = this.orgaMock.getOrganizationsMock();
	}

	openAddDialog() {
		console.log('openedAddDialog');
		const dialogRef = this.dialog.open(OrganizationsAddDialogComponent, {
			disableClose: true
		});
		dialogRef.afterClosed().subscribe((result) => {
			this.getData();
		});
	}

	openEditDialog(id: number) {
		this.service.getById(id).subscribe(x => {
			const dialogRef = this.dialog.open(OrganizationsEditDialogComponent, {
				data: x,
				disableClose: true
			});

			dialogRef.afterClosed().subscribe((result) => {
				if (result.delete) {
					this.deleteOrganization(result.id);
				}
				this.getData();
			});
		});
	}

	addNote(id: number) {
		const dialogRef = this.dialog.open(AddHistoryComponent);
		dialogRef.afterClosed().subscribe((y) => {
			if (y) {
				this.service.postHistoryElement(y, id).subscribe(x => this.getData());
			}
		});
	  }

	openInfo(id: number) {
		this.service.getById(id).subscribe(x => this.dialog.open(OrganizationsInfoComponent, { data: x }));
	  }

	deleteOrganization(id: number) {
		const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
			data: 'Organisation',
			disableClose: true
		});

		deleteDialogRef.afterClosed().subscribe((deleteResult) => {
			if (deleteResult.delete) {
				this.service.delete(id).subscribe((x) => this.getData());
			}
		});
	}

	addDummyOrganization() {
		this.service.post({
		  name: 'Organisation' + this.length,
		  description: 'Bezeichnung' + this.length,
		  employees: [],
		  address: {
			city: 'Statd' + this.length,
			country: 'Land' + this.length,
			street: 'Strasse' + this.length,
			streetNumber: this.length.toString(),
			zipcode: '12345'
		  },
		  contact: {
			contactEntries: [],
			fax: '0234234-234' + this.length,
			mail: 'info@testorga' + this.length + '.de',
			phoneNumber: '02342-234234' + this.length
		  }
		}).subscribe(x => this.getData());
	}
}
