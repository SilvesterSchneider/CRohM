import { Component, OnInit, ChangeDetectorRef, Injectable } from '@angular/core';
import { OrganizationService, UsersService } from '../../shared/api-generated/api-generated';
import { Observable } from 'rxjs';
import { OrganizationDto } from '../../shared/api-generated/api-generated';
import { OrganizationsMockService } from '../organizations-mock-service';

@Component({
  selector: 'app-organizations-list',
  templateUrl: './organizations-list.component.html',
  styleUrls: ['./organizations-list.component.scss']
})

@Injectable({
  providedIn: 'root',
})
export class OrganizationsListComponent implements OnInit {
  orga: OrganizationService;
  orgaMock: OrganizationsMockService;
  organizations: Observable<OrganizationDto[]>;
  organizationMock: Observable<OrganizationDto[]>;
  displayedColumns = ['Name', 'Beschreibung', 'Strasse', 'Hausnummer', 'PLZ', 'Stadt', 'Telefonnummer',
   'E-Mail', 'Faxnummer', 'Zugehörige', 'Action'];
   isAdminUserLoggedIn: boolean = false;
   length: number = 0;

  constructor(
    organizationServive: OrganizationService,
    mock: OrganizationsMockService,
    private changeDetectorRefs: ChangeDetectorRef,
    private userService: UsersService) {
    this.orga = organizationServive;
    this.orgaMock = mock;
  }

  ngOnInit(): void {
    this.loadData();
    this.userService.getLoggedInUser(1).subscribe(x => {
      this.isAdminUserLoggedIn = x.id === 1;
    });
  }

  private loadData() {
    this.organizations = this.orga.get();
    this.organizations.subscribe(x => this.length = x.length);
    this.changeDetectorRefs.detectChanges();
   // this.organizationMock = this.orgaMock.getOrganizationsMock();
  }

  deleteOrganization(id: number) {
    this.orga.delete(id).subscribe(x => this.loadData());
  }

  addDummyOrganization() {
    this.orga.post({
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
        fax: '0234234-234' +this.length,
        mail: 'info@testorga' + this.length + '.de',
        phoneNumber: '02342-234234' + this.length
      }
    }).subscribe(x => this.loadData());
  }
}

