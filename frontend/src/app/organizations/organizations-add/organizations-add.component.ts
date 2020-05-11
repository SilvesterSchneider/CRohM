import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrganizationService, OrganizationCreateDto, ContactService } from 'src/app/shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';

@Component({
  selector: 'app-organizations-add',
  templateUrl: './organizations-add.component.html',
  styleUrls: ['./organizations-add.component.scss']
})
export class OrganizationsAddComponent implements OnInit {
  @ViewChild(ContactPossibilitiesComponent, {static: true})
  contactPossibilitiesEntries: ContactPossibilitiesComponent;
  contactPossibilitiesEntriesFormGroup: FormGroup;
  public organizationForm: FormGroup;
  private organization: OrganizationCreateDto;

  constructor(private readonly fb: FormBuilder,
              private readonly organizationsService: OrganizationService,
              private readonly contactService: ContactService) { }

  public ngOnInit(): void {
    this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
    this.organizationForm = this.createOrganizationForm();
  }

  public onAddOrganization(): void {
    this.organization = this.organizationForm.value;
    this.organization.contact.contactEntries = this.contactPossibilitiesEntries.getContactPossibilitiesEntriesAsCreateDto();
    this.organizationsService.post(this.organization).subscribe();
    this.sleep(500);
  }

   sleep(milliseconds) {
    const date = Date.now();
    let currentDate = null;
    do {
      currentDate = Date.now();
    } while (currentDate - date < milliseconds);
  }

  private createOrganizationForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      description: [''],
      address: this.fb.control(''),
      contact: this.createContactForm()
    });
  }

  private createContactForm(): FormGroup {
    return this.fb.group({
      phoneNumber: ['', Validators.pattern('^0[0-9\- ]*$')],
      fax: ['', Validators.pattern('^0[0-9\- ]*$')],
      mail: ['', Validators.email],
      contactEntries: this.contactPossibilitiesEntriesFormGroup
    });
  }
}

