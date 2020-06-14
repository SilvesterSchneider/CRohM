import { Component, OnInit, ChangeDetectorRef, Injectable } from '@angular/core';
import { OrganizationService } from '../../shared/api-generated/api-generated';
import { Observable } from 'rxjs';
import { OrganizationDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { OrganizationsAddDialogComponent } from '../organizations-add-dialog/organizations-add-dialog.component';
import { OrganizationsEditDialogComponent } from '../organizations-edit-dialog/organizations-edit-dialog.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';

@Component({
  selector: 'app-organizations-list',
  templateUrl: './organizations-list.component.html',
  styleUrls: ['./organizations-list.component.scss']
})
@Injectable({
  providedIn: 'root'
})
export class OrganizationsListComponent implements OnInit {
  service: OrganizationService;
  organizations: Observable<OrganizationDto[]>;
  organizationMock: Observable<OrganizationDto[]>;
  displayedColumns = [
    'Name',
    'Beschreibung',
    'Strasse',
    'Hausnummer',
    'PLZ',
    'Stadt',
    'Telefonnummer',
    'E-Mail',
    'Faxnummer',
    'Zugehörige',
    'Action'
  ];

  constructor(
    public dialog: MatDialog,
    service: OrganizationService,
    private changeDetectorRefs: ChangeDetectorRef
  ) {
    this.service = service;
  }

  ngOnInit(): void {
    this.getData();
  }

  private getData() {
    this.organizations = this.service.get();
    this.organizations.subscribe();
    this.changeDetectorRefs.detectChanges();
    // this.organizationMock = this.orgaMock.getOrganizationsMock();
  }

  openAddDialog() {
    console.log('openedAddDialog');
    const dialogRef = this.dialog.open(OrganizationsAddDialogComponent);
    dialogRef.afterClosed().subscribe((result) => {
      this.getData();
    });
  }

  openEditDialog(organization: OrganizationDto) {
    const dialogRef = this.dialog.open(OrganizationsEditDialogComponent, { data: organization });

    dialogRef.afterClosed().subscribe((result) => {
      if (result.delete) {
        this.deleteOrganization(result.id);
      }
      this.getData();
    });
  }

  deleteOrganization(id: number) {
    const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
      data: 'Organisation',
      disableClose: true
    });

    deleteDialogRef.afterClosed().subscribe((deleteResult) => {
      if (deleteResult.delete) {
        this.service.delete(id).subscribe(x => this.getData());
      }
    });
  }

}
