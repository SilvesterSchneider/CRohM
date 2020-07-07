import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';
import { JwtService } from 'src/app/shared/jwt.service';
import { DpUpdatePopupComponent } from 'src/app/shared/data-protection/dp-update-popup/dp-update-popup.component';

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
	private oldContact: ContactDto;
	private newContact: ContactDto;

	public constructor(
		public dialogRef: MatDialogRef<ContactsEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: ContactDto,
		public dialog: MatDialog,
		private readonly fb: FormBuilder,
		private readonly contactService: ContactService,
		private readonly jwtService: JwtService,
	) {
		super(dialogRef, dialog);
		this.contact = data;

	}

	public ngOnInit(): void {
		this.oldContact = this.data;
		this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
		this.contactPossibilitiesEntries.patchExistingValuesToForm(this.contact.contactPossibilities.contactEntries);
		this.initForm();
		this.contactsForm.patchValue(this.contact);
	}

	public initForm(): void {
		this.contactsForm = this.fb.group({
			id: ['', Validators.required],
			description: [],
			events: [[]],
			history: [[]],
			organizations: [[]],
			name: ['', Validators.required],
			preName: ['', Validators.required],
			address: this.fb.control(''),
			contactPossibilities: this.fb.group({
				// Validiert auf korrektes E-Mail-Format
				mail: ['', Validators.email],
				// Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
				phoneNumber: ['', Validators.pattern('^0[0-9- ]*$')],
				fax: ['', Validators.pattern('^0[0-9- ]*$')],
				name: [],
				id: [],
				description: [],
				contactEntries: this.contactPossibilitiesEntriesFormGroup
			})
		});
	}

	public onApprove(): void {
		this.newContact =  this.contactsForm.value;
		this.contactService.put(this.contact.id, this.contactsForm.value).subscribe(x => {
			this.dialogRef.close({ delete: false, id: 0, oldContact: this.oldContact, newContact: this.newContact });
		});
	}

	public onCancel(): void {
		super.confirmDialog({ delete: false, id: 0 });
	}

	public onDelete(): void {
		super.confirmDialog({ delete: true, id: this.contact.id });
	}

	public hasChanged(): boolean {
		return !this.contactsForm.pristine;
	}
}
