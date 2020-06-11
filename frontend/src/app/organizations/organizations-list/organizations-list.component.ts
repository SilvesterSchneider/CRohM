import { Component, OnInit, ChangeDetectorRef, Injectable } from '@angular/core';
import { OrganizationService } from '../../shared/api-generated/api-generated';
import { Observable } from 'rxjs';
import { OrganizationDto } from '../../shared/api-generated/api-generated';
import { OrganizationsMockService } from '../organizations-mock-service';
import { MatDialog } from '@angular/material/dialog';
import { OrganizationsInfoComponent } from '../organizations-info/organizations-info.component';

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

  constructor(
    organizationServive: OrganizationService,
    mock: OrganizationsMockService,
    private changeDetectorRefs: ChangeDetectorRef,
    private dialog: MatDialog) {
    this.orga = organizationServive;
    this.orgaMock = mock;
  }

  ngOnInit(): void {
    this.loadData();
  }

  private loadData() {
    this.organizations = this.orga.get();
    this.organizations.subscribe();
    this.changeDetectorRefs.detectChanges();
   // this.organizationMock = this.orgaMock.getOrganizationsMock();
  }

  deleteOrganization(id: number) {
    this.orga.delete(id).subscribe(x => this.loadData());
  }

  openInfo(id: number) {
    this.orga.getById(id).subscribe(x => this.dialog.open(OrganizationsInfoComponent, { data: x }));
  }
}

