import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrganizationService, OrganizationCreateDto, ContactService } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-organizations-add',
  templateUrl: './organizations-add.component.html',
  styleUrls: ['./organizations-add.component.scss']
})
export class OrganizationsAddComponent implements OnInit {
  public organizationForm: FormGroup;
  private organization: OrganizationCreateDto;

  constructor(private readonly fb: FormBuilder,
              private readonly organizationsService: OrganizationService,
              private readonly contactService: ContactService) { }

  public ngOnInit(): void {
    this.organizationForm = this.createOrganizationForm();
  }

  public onAddOrganization(): void {
    this.organization = this.organizationForm.value;
    this.organizationsService.post(this.organization).subscribe(oragnization => {
      console.log(oragnization);
    });
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
      mail: ['', Validators.email]
    });
  }
}

