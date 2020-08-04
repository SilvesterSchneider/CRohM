import { Component, OnInit, Inject } from '@angular/core';
import { ContactDto,
  ContactPossibilitiesEntryDto, OrganizationDto, ModificationEntryDto, TagDto,
  ModificationEntryService, MODEL_TYPE, DATA_TYPE, HistoryElementType,
  HistoryElementDto} from '../../shared/api-generated/api-generated';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-organizations-info',
  templateUrl: './organizations-info.component.html',
  styleUrls: ['./organizations-info.component.scss']
})

export class OrganizationsInfoComponent implements OnInit {
  organization: OrganizationDto;
  contactPossibilitiesEntries: ContactPossibilitiesEntryDto[] = new Array<ContactPossibilitiesEntryDto>();
  organizationsForm: FormGroup;
  dataHistory: ModificationEntryDto[] = new Array<ModificationEntryDto>();
  employees: ContactDto[] = new Array<ContactDto>();
  displayedColumnsEmployees = ['vorname', 'name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];
  tags: TagDto[] = new Array<TagDto>();
  displayedColumnsHistory = ['icon', 'datum', 'name', 'kommentar'];
  displayedColumnsDataChangeHistory = ['datum', 'bearbeiter', 'feldname', 'alterWert', 'neuerWert'];
  history: HistoryElementDto[] = new Array<HistoryElementDto>();

  constructor(
    public dialogRef: MatDialogRef<OrganizationsInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: OrganizationDto,
    private fb: FormBuilder,
    private modService: ModificationEntryService) {
      this.organization = data;
      this.tags = this.organization.tags;
    }

  ngOnInit(): void {
    this.initForm();
    if (this.organization.employees != null) {
      this.organization.employees.forEach(x => this.employees.push(x));
    }
    if (this.organization.contact.contactEntries != null) {
      this.organization.contact.contactEntries.forEach(x => this.contactPossibilitiesEntries.push(x));
    }
    this.modService.getSortedListByTypeAndId(this.organization.id, MODEL_TYPE.ORGANIZATION).subscribe(x => {
      x.forEach(a => {
        if (a.dataType !== DATA_TYPE.NONE) {
          this.dataHistory.push(a);
        }
      });
      this.dataHistory.sort(this.getSortHistoryFunction);
    });
    this.history = this.organization.history;
    this.organizationsForm.patchValue(this.organization);
  }

  getSortHistoryFunction(a: ModificationEntryDto, b: ModificationEntryDto) {
    return new Date(b.dateTime).getTime() - new Date(a.dateTime).getTime();
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

  closeDialog() {
    this.dialogRef.close();
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
}
