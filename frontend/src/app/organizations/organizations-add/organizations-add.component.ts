import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrganizationService } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-organizations-add',
  templateUrl: './organizations-add.component.html',
  styleUrls: ['./organizations-add.component.scss']
})
export class OrganizationsAddComponent implements OnInit {
  public organizationForm: FormGroup;

    // TODO: sollten die möglichen Länder aus dem Backend laden
    // Liste der im Dropdown angezeigten Laender
    public countries: Country[] = [
      {value: 'Deutschland', viewValue: 'Deutschland'},
      {value: 'Schweiz', viewValue: 'Schweiz'},
      {value: 'Österreich', viewValue: 'Österreich'}
    ];

  constructor(private readonly fb: FormBuilder,
              private readonly organizationsService: OrganizationService) { }

  public ngOnInit(): void {
    this.organizationForm = this.createOrganizationForm();
  }

  public onAddOrganization(): void {
    this.organizationsService.post(this.organizationForm.value).subscribe(oragnization => {
      console.log(oragnization);
    });
  }

  private createOrganizationForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      description: [''],
      address: this.createAddressForm()
    });
  }

  private createAddressForm(): FormGroup {
    return this.fb.group({
      country: [''],
      street: [''],
      zipcode: ['', Validators.pattern('^[0-9]{5}$')],
      streetNumber: [''],
      city: ['']
    });
  }

}

interface Country {
  value: string;
  viewValue: string;
}
