import { Component, OnInit, Inject } from '@angular/core';
import { ContactDto,
  ContactPossibilitiesEntryDto, OrganizationDto, HistoryElementType, HistoryElementDto} from '../../shared/api-generated/api-generated';
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
  history: HistoryElementDto[] = new Array<HistoryElementDto>();
  employees: ContactDto[] = new Array<ContactDto>();
  displayedColumnsEmployees = ['vorname', 'name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];
  displayedColumnsHistory = ['icon', 'datum', 'name', 'kommentar'];

  constructor(
    public dialogRef: MatDialogRef<OrganizationsInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: OrganizationDto,
    private fb: FormBuilder) {
      this.organization = data;
    }

  ngOnInit(): void {
    this.initForm();
    if (this.organization.employees != null) {
      this.organization.employees.forEach(x => this.employees.push(x));
    }
    if (this.organization.contact.contactEntries != null) {
      this.organization.contact.contactEntries.forEach(x => this.contactPossibilitiesEntries.push(x));
    }
    this.history = this.organization.history;
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

  closeDialog() {
    this.dialogRef.close();
  }

  isLocalPhone(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.PHONE_CALL;
  }

  isNote(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.NOTE;
  }

  getDate(date: string): string {
    const dateUsed = new Date(date);
    return dateUsed.getFullYear().toString() + '-' + (+dateUsed.getMonth() + 1).toString() + '-' + dateUsed.getDate().toString();
  }

  isMail(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.MAIL;
  }
}
