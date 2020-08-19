import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrganizationService, OrganizationCreateDto } from 'src/app/shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';

@Component({
	selector: 'app-organizations-add',
	templateUrl: './organizations-add-dialog.component.html',
	styleUrls: ['./organizations-add-dialog.component.scss']
})
export class OrganizationsAddDialogComponent extends BaseDialogInput implements OnInit {
	@ViewChild(ContactPossibilitiesComponent, { static: true })
	contactPossibilitiesEntries: ContactPossibilitiesComponent;
	contactPossibilitiesEntriesFormGroup: FormGroup;
	public organizationForm: FormGroup;
	private organization: OrganizationCreateDto;

	constructor(
		public dialogRef: MatDialogRef<OrganizationsAddDialogComponent>,
		public dialog: MatDialog,
		private readonly fb: FormBuilder,
		private readonly service: OrganizationService
	) {
		super(dialogRef, dialog);
	}

	public ngOnInit(): void {
		this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
		this.organizationForm = this.createOrganizationForm();
	}

	public onApprove(): void {
		this.organization = this.organizationForm.value;
		this.service.post(this.organization).subscribe((organization) => {
			console.log(organization);
		});
		this.dialogRef.close();
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
			phoneNumber: ['', Validators.pattern('^0[0-9- ]*$')],
			fax: ['', Validators.pattern('^0[0-9- ]*$')],
			mail: ['', Validators.email],
			contactEntries: this.contactPossibilitiesEntriesFormGroup
		});
	}

	onCancel() {
		super.confirmDialog();
	}

	hasChanged(): boolean {
		return !this.organizationForm.pristine;
	}
}
