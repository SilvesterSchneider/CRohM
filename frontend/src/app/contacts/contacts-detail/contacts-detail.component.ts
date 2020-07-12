import { Component, OnInit, ViewChild } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ContactDto, EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { JwtService } from 'src/app/shared/jwt.service';

@Component({
	selector: 'app-contacts-detail',
	templateUrl: './contacts-detail.component.html',
	styleUrls: [ './contacts-detail.component.scss' ]
})
export class ContactsDetailComponent implements OnInit {
	@ViewChild(ContactPossibilitiesComponent, { static: true })
	contactPossibilitiesEntries: ContactPossibilitiesComponent;
	contactPossibilitiesEntriesFormGroup: FormGroup;
	contact: ContactDto;
	contactsForm: FormGroup;

	constructor(
		private fb: FormBuilder,
		private route: ActivatedRoute,
		private service: ContactService,
		private eventService: EventService,
		private jwt: JwtService
	) {}

	ngOnInit(): void {
		this.contact = this.route.snapshot.data.contact;
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

	updateContact() {
		const idAddress = this.contact.address.id;
		const idContactPossibilities = this.contact.contactPossibilities.id;
		const newContact: ContactDto = this.contactsForm.value;
		this.contact.address = newContact.address;
		this.contact.preName = newContact.preName;
		this.contact.name = newContact.name;
		this.contact.contactPossibilities = newContact.contactPossibilities;
		this.contact.address.id = idAddress;
		this.contact.contactPossibilities.id = idContactPossibilities;
		this.service.put(this.contact, this.contact.id, this.jwt.getUserId()).subscribe();
	}
}
