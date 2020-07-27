import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { ContactDto, TagDto } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';
import { JwtService } from 'src/app/shared/jwt.service';

@Component({
	selector: 'app-contacts-edit-dialog',
	templateUrl: './contacts-edit-dialog.component.html',
	styleUrls: ['./contacts-edit-dialog.component.scss']
})
export class ContactsEditDialogComponent extends BaseDialogInput implements OnInit {
	@ViewChild(ContactPossibilitiesComponent, { static: true })
	contactPossibilitiesEntries: ContactPossibilitiesComponent;
	contactPossibilitiesEntriesFormGroup: FormGroup;
	contactsForm: FormGroup;
	contact: ContactDto;
	tagsControl = new FormControl();
	selectedTags: TagDto[] = new Array<TagDto>();

	constructor(
		public dialogRef: MatDialogRef<ContactsEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: ContactDto,
		public dialog: MatDialog,
		private fb: FormBuilder,
		private service: ContactService,
		private jwt: JwtService
	) {
		super(dialogRef, dialog);
		this.contact = data;
		this.contact.tags.forEach(x => this.selectedTags.push(x));
	}

	ngOnInit(): void {
		this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
		this.contactPossibilitiesEntries.patchExistingValuesToForm(this.contact.contactPossibilities.contactEntries);
		this.initForm();
		this.contactsForm.patchValue(this.contact);
	}

	addTag(event: Event) {
		const value = (event.target as HTMLInputElement).value;
		if (this.selectedTags.find(a => a.name === value) == null) {
			this.selectedTags.push({
				id: 0,
				name: value
			});
		}
		this.tagsControl.setValue('');
	}

	removeTag() {
		if (this.selectedTags.length > 0) {
			this.selectedTags.splice(this.selectedTags.length - 1, 1);
		}
	}

	initForm() {
		this.contactsForm = this.fb.group({
			id: ['', Validators.required],
			name: ['', Validators.required],
			preName: ['', Validators.required],
			address: this.fb.control(''),
			contactPossibilities: this.fb.group({
				// Validiert auf korrektes E-Mail-Format
				mail: ['', Validators.email],
				// Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
				phoneNumber: ['', Validators.pattern('^0[0-9- ]*$')],
				fax: ['', Validators.pattern('^0[0-9- ]*$')],
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
		this.contact.tags = this.selectedTags;
		this.service.put(this.contact, this.contact.id, this.jwt.getUserId()).subscribe(x => {
			this.dialogRef.close({ delete: false, id: 0 });
		});
	}

	onCancel() {
		super.confirmDialog({ delete: false, id: 0 });
	}

	onDelete() {
		super.confirmDialog({ delete: true, id: this.contact.id });
	}

	hasChanged(): boolean {
		return !this.contactsForm.pristine;
	}
}
