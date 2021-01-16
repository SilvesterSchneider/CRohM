import { Component, OnInit, Inject, ModuleWithComponentFactories } from '@angular/core';
import {
  ContactDto,
  ModificationEntryService, MODEL_TYPE, MODIFICATION, DATA_TYPE,
  ContactPossibilitiesEntryDto, OrganizationDto, ModificationEntryDto, TagDto,
  HistoryElementDto,
  HistoryElementType, OrganizationService, EventDto, ParticipatedStatus
} from '../../shared/api-generated/api-generated';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';
import { sortDatesDesc } from '../../shared/util/sort';
import { PageEvent } from '@angular/material/paginator';
import { JwtService } from 'src/app/shared/jwt.service';
import { OrganizationsEditDialogComponent } from '../organizations-edit-dialog/organizations-edit-dialog.component';

@Component({
  selector: 'app-organizations-info',
  templateUrl: './organizations-info.component.html',
  styleUrls: ['./organizations-info.component.scss']
})

export class OrganizationsInfoComponent implements OnInit {
  organizationsForm: FormGroup;

  modifications: ModificationEntryDto[] = [];
  modificationsPaginationLength: number;
  permissionModify = false;
  history: (EventDto | HistoryElementDto)[] = [];
  historyPaginationLength: number;

  displayedColumnsEmployees = ['vorname', 'name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];
  tags: TagDto[] = new Array<TagDto>();
  displayedColumnsHistory = ['icon', 'datum', 'name', 'kommentar'];
  displayedColumnsDataChangeHistory = ['datum', 'bearbeiter', 'feldname', 'alterWert', 'neuerWert'];

  constructor(
              public dialogRef: MatDialogRef<OrganizationsInfoComponent>,
              public dialog: MatDialog,
              @Inject(MAT_DIALOG_DATA) public organization: OrganizationDto,
              private fb: FormBuilder,
              private modService: ModificationEntryService,
              private organisationService: OrganizationService,
              private jwt: JwtService) {
    this.tags = this.organization.tags;
  }

  ngOnInit(): void {
    this.permissionModify = this.jwt.hasPermission('Einsehen und Bearbeiten aller Organisationen');
    this.initLoad();
  }

  initLoad() {
    this.initForm();
    // Initialize modifications
    this.loadModifications(0, 5);
    // Initialize history
    this.loadHistory(0, 5);
    this.organizationsForm.patchValue(this.organization);
  }

  initForm() {
    this.organizationsForm = this.fb.group({
      id: [''],
      name: [''],
      description: [''],
      address: this.fb.group({
        id: [''],
        name: [''],
        description: [''],
        street: [''],
        streetNumber: [''],
        zipcode: [''],
        city: [''],
        country: ['']
      }),
      contact: this.fb.group({
        mail: [''],
        phoneNumber: [''],
        fax: [''],
        contactEntries: ['']
      }),
      employees: ['']
    });
  }

  onPaginationChangedModification(event: PageEvent) {
    this.loadModifications((event.pageIndex * event.pageSize), event.pageSize);
  }

  onPaginationChangedHistory(event: PageEvent) {
    this.loadHistory((event.pageIndex * event.pageSize), event.pageSize);
  }

  isLocalPhone(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.PHONE_CALL;
  }

  isNote(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.NOTE;
  }

  isMail(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.MAIL;
  }

  eventParticipated(element: EventDto): boolean {
    return !!element.participated && element.participated?.some(part => part.modelType === MODEL_TYPE.ORGANIZATION &&
       part.objectId === this.organization.id && part.hasParticipated);
  }

  eventNotParticipated(element: EventDto): boolean {
    return !!element.participated && element.participated?.some(part => part.modelType === MODEL_TYPE.ORGANIZATION &&
       part.objectId === this.organization.id && !part.hasParticipated);
  }

  private loadModifications(pageStart: number, pageSize: number) {
    this.modService.getSortedListByTypeAndId(this.organization.id, MODEL_TYPE.ORGANIZATION, pageStart, pageSize)
      .subscribe(result => {
        this.modifications = result.data;
        this.modificationsPaginationLength = result.totalRecords;
      });
  }

  private loadHistory(pageStart: number, pageSize: number) {
    this.organisationService.getHistory(this.organization.id, pageStart, pageSize)
      .subscribe(result => {
        this.history = result.data;
        this.historyPaginationLength = result.totalRecords;
      });
  }

  callEdit() {
    const dialogRef = this.dialog.open(OrganizationsEditDialogComponent, {
      data: this.organization,
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(x => {
      this.organisationService.getById(this.organization.id).subscribe(y => {
        this.organization = y;
        this.initLoad();
      });
    });
  }
}
