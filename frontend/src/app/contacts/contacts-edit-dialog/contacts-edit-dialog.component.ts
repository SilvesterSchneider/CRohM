import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { ContactDto, GenderTypes, TagDto } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';
import { JwtService } from 'src/app/shared/jwt.service';
import {COMMA, ENTER} from '@angular/cdk/keycodes';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { OsmAddressComponent } from 'src/app/shared/osm/osm-address/osm-address.component';

@Component({
	selector: 'app-contacts-edit-dialog',
	templateUrl: './contacts-edit-dialog.component.html',
	styleUrls: ['./contacts-edit-dialog.component.scss']
})
export class ContactsEditDialogComponent extends BaseDialogInput implements OnInit {
	@ViewChild(ContactPossibilitiesComponent, { static: true })
	contactPossibilitiesEntries: ContactPossibilitiesComponent;
	public genderTypes: string[] = ['Männlich', 'Weiblich', 'Divers'];
	contactPossibilitiesEntriesFormGroup: FormGroup;
	contactsForm: FormGroup;
	contact: ContactDto;
	private oldContact: ContactDto;
	private newContact: ContactDto;
	private copy;
	@ViewChild(OsmAddressComponent, { static: true })
	addressGroup: OsmAddressComponent;
	addressForm: FormGroup;
	@ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;
	tagsControl = new FormControl();
	selectedTags: TagDto[] = new Array<TagDto>();
	separatorKeysCodes: number[] = [ENTER, COMMA];
	filteredTagsObservable: Observable<string[]>;
	allTags: string[] = [ 'Lehrbeauftragter', 'Kunde', 'Politiker', 'Unternehmen', 'Behörde', 'Bildungseinrichtung',
	 'Institute', 'Ministerium', 'Emeriti', 'Alumni'];
	removable = true;
	selectable = true;

	constructor(
		public dialogRef: MatDialogRef<ContactsEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: ContactDto,
		public dialog: MatDialog,
		private readonly fb: FormBuilder,
		private readonly contactService: ContactService,
		private readonly jwtService: JwtService,
	) {
		super(dialogRef, dialog);
		this.contact = data;
		this.contact.tags.forEach(x => this.selectedTags.push(x));
		this.filteredTagsObservable = this.tagsControl.valueChanges.pipe(
			map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
	}

	public ngOnInit(): void {
		this.oldContact = this.data;
		this.copy = (JSON.parse(JSON.stringify(this.data)));
		this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
		this.contactPossibilitiesEntries.patchExistingValuesToForm(this.contact.contactPossibilities.contactEntries);
		this.addressForm = this.addressGroup.getAddressForm();
		this.initForm();
		this.contactsForm.patchValue(this.contact);
		this.contactsForm.get('gender').setValue(this.getGenderText(this.contact.gender));
	}

	private getGenderText(value: number): string {
		if (value === GenderTypes.MALE) {
			return this.genderTypes[0];
		} else if (value === GenderTypes.FEMALE) {
			return this.genderTypes[1];
		} else {
			return this.genderTypes[2];
		}
	}

	private _filter(value: string): string[] {
		const tagValue = value.toLowerCase();

		return this.allTags.filter(tag => tag.toLowerCase().indexOf(tagValue) === 0);
	  }

	addTag(event: Event) {
		const value = (event.target as HTMLInputElement).value;
		if (value.length > 0 && this.selectedTags.find(a => a.name === value) == null) {
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

	remove(tag: TagDto) {
		const index = this.selectedTags.indexOf(tag);
		if (index >= 0) {
			this.selectedTags.splice(index, 1);
		}
	}

	selected(event: MatAutocompleteSelectedEvent): void {
		if (this.selectedTags.find(a => a.name === event.option.viewValue) == null) {
			this.selectedTags.push({
			  id: 0,
			  name: event.option.viewValue
			});
			this.tagInput.nativeElement.value = '';
			this.tagsControl.setValue(null);
		  }
	  }

	initForm() {
		this.contactsForm = this.fb.group({
			id: ['', Validators.required],
			description: [],
			events: [[]],
			history: [[]],
			organizations: [[]],
			name: ['', Validators.required],
			preName: ['', Validators.required],
			gender: [this.genderTypes[0], Validators.required],
			contactPartner: [''],
			address: this.addressForm,
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

		const idAddress = this.contact.address.id;
		const idContactPossibilities = this.contact.contactPossibilities.id;
		const newContact: ContactDto = this.contactsForm.value;
		this.contact.address = newContact.address;
		this.contact.preName = newContact.preName;
		this.contact.name = newContact.name;
		this.contact.contactPartner = newContact.contactPartner;
		this.contact.contactPossibilities = newContact.contactPossibilities;
		this.contact.address.id = idAddress;
		this.contact.contactPossibilities.id = idContactPossibilities;
		this.contact.tags = this.selectedTags;
		const genderText: string = this.contactsForm.get('gender').value;
		let gender: GenderTypes = GenderTypes.MALE;
		if (genderText === this.genderTypes[1]) {
			gender = GenderTypes.FEMALE;
		} else if (genderText === this.genderTypes[2]) {
			gender = GenderTypes.DIVERS;
		}
		this.contact.gender = gender;
		this.newContact.tags = this.selectedTags;
		this.contactService.put(this.contact.id, this.contact).subscribe(x => {
			this.dialogRef.close({ delete: false, id: 0, oldContact: this.copy, newContact: this.newContact });
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
