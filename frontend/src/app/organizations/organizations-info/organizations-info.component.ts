import { Component, OnInit, Inject } from '@angular/core';
import { ContactDto,
  ContactPossibilitiesEntryDto, OrganizationDto} from '../../shared/api-generated/api-generated';
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
  employees: ContactDto[] = new Array<ContactDto>();
  displayedColumnsEmployees = ['vorname', 'name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];

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
}
