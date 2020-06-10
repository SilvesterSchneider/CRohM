import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';

@Component({
	selector: 'app-contacts-edit-dialog',
	templateUrl: './contacts-edit-dialog.component.html',
	styleUrls: [ './contacts-edit-dialog.component.scss' ]
})
export class ContactsEditDialogComponent implements OnInit {
	@ViewChild(ContactPossibilitiesComponent, { static: true })
	contactPossibilitiesEntries: ContactPossibilitiesComponent;
	contactPossibilitiesEntriesFormGroup: FormGroup;
	contactsForm: FormGroup;

	constructor(
		private fb: FormBuilder,
		public dialogRef: MatDialogRef<ContactsEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public contact: ContactDto,
		private service: ContactService
	) {}

	ngOnInit(): void {
		this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
		this.contactPossibilitiesEntries.patchExistingValuesToForm(this.contact.contactPossibilities.contactEntries);
		this.initForm();
		this.contactsForm.patchValue(this.contact);
	}

	initForm() {
		this.contactsForm = this.fb.group({
			id: [ '', Validators.required ],
			name: [ '', Validators.required ],
			preName: [ '', Validators.required ],
			address: this.fb.control(''),
			contactPossibilities: this.fb.group({
				// Validiert auf korrektes E-Mail-Format
				mail: [ '', Validators.email ],
				// Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
				phoneNumber: [ '', Validators.pattern('^0[0-9- ]*$') ],
				fax: [ '', Validators.pattern('^0[0-9- ]*$') ],
				contactEntries: this.contactPossibilitiesEntriesFormGroup
			})
		});
	}

	onApprove() {
		const idAddress = this.contact.address.id;
		const idContactPossibilities = this.contact.contactPossibilities.id;
		const newContact: ContactDto = this.contactsForm.value;
		this.contact.address = newContact.address;
		this.contact.preName = newContact.preName;
		this.contact.name = newContact.name;
		this.contact.contactPossibilities = newContact.contactPossibilities;
		this.contact.address.id = idAddress;
		this.contact.contactPossibilities.id = idContactPossibilities;
		this.service.put(this.contact, this.contact.id); // subscribe neccessary?
		this.dialogRef.close();
	}

	onCancel() {
		this.dialogRef.close();
	}
}
