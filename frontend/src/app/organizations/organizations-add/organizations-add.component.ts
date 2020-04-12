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
  constructor(private readonly fb: FormBuilder,
              private readonly organizationsService: OrganizationService
              ) { }

  public ngOnInit(): void {
    this.createForm();
  }

  public onAddOrganization(): void {
    this.organizationsService.post(this.organizationForm.value).subscribe(oragnization => {
      console.log(oragnization);
    });
  }

  private createForm() {
    this.organizationForm = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

}
