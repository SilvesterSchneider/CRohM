import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import {
	ContactCreateDto,
	AddressCreateDto,
	ContactPossibilitiesCreateDto, GenderTypes
} from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';
import { JwtService } from 'src/app/shared/jwt.service';
import { OsmAddressComponent } from 'src/app/shared/osm/osm-address/osm-address.component';

/// <summary>
/// RAM: 60%
/// </summary>
@Component({
	selector: 'app-contacts-add-dialog',
	templateUrl: './contacts-add-dialog.component.html',
	styleUrls: ['./contacts-add-dialog.component.scss']
})
export class ContactsAddDialogComponent extends BaseDialogInput<ContactsAddDialogComponent> implements OnInit {
	public genderTypes = [
		{
			translate: 'common.male',
			type: GenderTypes.MALE
		},
		{
			translate: 'common.female',
			type: GenderTypes.FEMALE
		},
		{
			translate: 'common.diverse',
			type: GenderTypes.DIVERS
		}];
	contactCreateDto: ContactCreateDto = { name: 'n', preName: 'n', gender: GenderTypes.MALE };
	adressCreateDto: AddressCreateDto = { country: '', street: '', streetNumber: '', zipcode: '', city: '' };
	contactPossibilitiesCreateDto: ContactPossibilitiesCreateDto = { mail: '', phoneNumber: '', fax: '' };
	@ViewChild(ContactPossibilitiesComponent, { static: true })
	contactPossibilitiesEntries: ContactPossibilitiesComponent;
	contactPossibilitiesEntriesFormGroup: FormGroup;
	contactsForm: FormGroup;
	@ViewChild(OsmAddressComponent, { static: true })
	addressGroup: OsmAddressComponent;
	addressForm: FormGroup;

	constructor(
		public dialogRef: MatDialogRef<ContactsAddDialogComponent>,
		public dialog: MatDialog,
		private fb: FormBuilder,
		private service: ContactService,
		private jwt: JwtService
	) {
		super(dialogRef, dialog);
		this.dialogRef.backdropClick().subscribe(() => {
			// Close the dialog
			dialogRef.close();
		});
	}

	ngOnInit(): void {
		this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
		this.addressForm = this.addressGroup.getAddressForm();
		this.createForm();
	}

	createForm() {
		this.contactsForm = this.fb.group({
			name: ['', Validators.required],
			preName: ['', Validators.required],
			gender: [this.genderTypes[0], Validators.required],
			contactPartner: [''],
			address: this.addressForm,
			contactPossibilities: this.fb.group({
				// Validiert auf korrektes E-Mail-Format
				mail: ['', [Validators.email, Validators.required]],
				// Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
				phoneNumber: ['', Validators.pattern('^0[0-9- ]*$')],
				fax: ['', Validators.pattern('^0[0-9- ]*$')],
				contactEntries: this.contactPossibilitiesEntriesFormGroup
			})
		});
	}

	onApprove() {
		// Take values from Input-Form and fits them into api-dto's.
		this.contactCreateDto.name = this.contactsForm.value.name;
		this.contactCreateDto.preName = this.contactsForm.value.preName;
		this.contactCreateDto.contactPartner = this.contactsForm.value.contactPartner;
		this.contactCreateDto.gender = this.contactsForm.get('gender').value;
		this.adressCreateDto.country = this.contactsForm.value.address.country;
		this.adressCreateDto.city = this.contactsForm.value.address.city;
		this.adressCreateDto.zipcode = this.contactsForm.value.address.zipcode;
		this.adressCreateDto.street = this.contactsForm.value.address.street;
		this.adressCreateDto.streetNumber = this.contactsForm.value.address.streetNumber;

		this.contactPossibilitiesCreateDto.mail = this.contactsForm.value.contactPossibilities.mail;
		this.contactPossibilitiesCreateDto.phoneNumber = this.contactsForm.value.contactPossibilities.phoneNumber;
		this.contactPossibilitiesCreateDto.fax = this.contactsForm.value.contactPossibilities.fax;
		this.contactPossibilitiesCreateDto.contactEntries = this.contactsForm.value.contactPossibilities.contactEntries;

		this.contactCreateDto.address = this.adressCreateDto;
		this.contactCreateDto.contactPossibilities = this.contactPossibilitiesCreateDto;

		// And add a new Contact with the service
		this.service.post(this.contactCreateDto).subscribe(x => this.dialogRef.close());
	}

	onCancel() {
		super.confirmDialog();
	}

	hasChanged(): boolean {
		return !this.contactsForm.pristine;	// pristine means no data was filled inside dialog
	}
}
