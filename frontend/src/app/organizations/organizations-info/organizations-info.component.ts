import { Component, OnInit, Inject, ModuleWithComponentFactories } from '@angular/core';
import {
  ContactDto,
  ModificationEntryService, MODEL_TYPE, MODIFICATION, DATA_TYPE,
  ContactPossibilitiesEntryDto, OrganizationDto, ModificationEntryDto, TagDto,
  HistoryElementDto,
  HistoryElementType, OrganizationService
} from '../../shared/api-generated/api-generated';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';
import { sortDatesDesc } from '../../shared/util/sort';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-organizations-info',
  templateUrl: './organizations-info.component.html',
  styleUrls: ['./organizations-info.component.scss']
})

export class OrganizationsInfoComponent implements OnInit {
  organizationsForm: FormGroup;

  modifications: ModificationEntryDto[] = [];
  modificationsPaginationLength: number;

  history: HistoryElementDto[] = [];
  historyPaginationLength: number;

  displayedColumnsEmployees = ['vorname', 'name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];
  tags: TagDto[] = new Array<TagDto>();
  displayedColumnsHistory = ['icon', 'datum', 'name', 'kommentar'];
  displayedColumnsDataChangeHistory = ['datum', 'bearbeiter', 'feldname', 'alterWert', 'neuerWert'];

  constructor(public dialogRef: MatDialogRef<OrganizationsInfoComponent>,
              @Inject(MAT_DIALOG_DATA) public organization: OrganizationDto,
              private fb: FormBuilder,
              private modService: ModificationEntryService,
              private organisationService: OrganizationService) {
    this.tags = this.organization.tags;
  }

  ngOnInit(): void {
    this.initForm();
    // Initialize modifications
    this.loadModifications(0, 5);
    // Initialize history
    this.loadHistory(0, 5);
    this.organizationsForm.patchValue(this.organization);
  }

  getDate(date: string): string {
    const dateUsed = new Date(date);
    return dateUsed.getFullYear().toString() + '-' + (+dateUsed.getMonth() + 1).toString() + '-' + dateUsed.getDate().toString();
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



}
