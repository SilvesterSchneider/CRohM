import { Component, OnInit } from '@angular/core';
import { OrganizationService } from '../../shared/api-generated/api-generated'
import { Observable } from 'rxjs';
import { OrganizationDto } from '../../shared/api-generated/api-generated'
import { OrganizationsMockService } from '../organizations-mock-service'

@Component({
  selector: 'app-organizations-list',
  templateUrl: './organizations-list.component.html',
  styleUrls: ['./organizations-list.component.scss']
})
export class OrganizationsComponent implements OnInit {
  orga: OrganizationService;
  orgaMock: OrganizationsMockService;
  organizations: Observable<OrganizationDto[]>;
  organizationMock: Observable<OrganizationDto[]>;
  displayedColumns = ['Name', 'Beschreibung', 'Strasse', 'Hausnummer', 'PLZ', 'Stadt', 'Telefonnummer', 'E-Mail', 'Faxnummer', 'Zugehörige', 'Action'];

  constructor(_organizationServive: OrganizationService, _mock: OrganizationsMockService) { 
    this.orga = _organizationServive;
    this.orgaMock = _mock;
  }

  ngOnInit(): void {
    this.loadData();
  }

  private loadData() {
    this.organizations = this.orga.get();
    this.organizations.subscribe();
    this.organizationMock = this.orgaMock.getOrganizationsMock();
  }

  deleteOrganization(id: number) {
    this.orga.delete(id).subscribe();
    this.orgaMock.delete(id);
    this.loadData();
  }
}
