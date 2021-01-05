import {
	ElementRef,
	HostBinding,
	Component,
	OnInit,
	ViewChild,
	Input,
	Optional,
	Self,
	ChangeDetectorRef,
	OnDestroy,
	Inject
} from '@angular/core';
import {COMMA, ENTER} from '@angular/cdk/keycodes';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NgControl, FormControl } from '@angular/forms';
import { Subject, Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { FocusMonitor } from '@angular/cdk/a11y';
import { MatAutocompleteTrigger, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatFormFieldControl } from '@angular/material/form-field';
import { OrganizationDto, TagDto } from '../../shared/api-generated/api-generated';
import { OrganizationService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';
import { OsmAddressComponent } from 'src/app/shared/osm/osm-address/osm-address.component';

export class ItemList {
	constructor(public item: string, public selected?: boolean) {
		if (selected === undefined) {
			selected = false;
		}
	}
}

export class OrganizationContactConnection {
	contactId: number;
	selected: boolean;
	name: string;
	preName: string;
}

@Component({
	selector: 'app-organizations-edit-dialog',
	templateUrl: './organizations-edit-dialog.component.html',
	styleUrls: ['./organizations-edit-dialog.component.scss'],
	providers: [{ provide: MatFormFieldControl, useExisting: OrganizationsEditDialogComponent }]
})
export class OrganizationsEditDialogComponent extends BaseDialogInput implements OnInit, OnDestroy {
	static nextId = 0;
	@ViewChild('inputTrigger', { read: MatAutocompleteTrigger })
	inputTrigger: MatAutocompleteTrigger;
	@HostBinding() id = `input-ac-${OrganizationsEditDialogComponent.nextId++}`;
	@HostBinding('attr.aria-describedby') describedBy = '';
	@ViewChild(ContactPossibilitiesComponent, { static: true })
	contactPossibilitiesEntries: ContactPossibilitiesComponent;
	contactPossibilitiesEntriesFormGroup: FormGroup;
	public selectable = true;
	@ViewChild(OsmAddressComponent, { static: true })
	addressGroup: OsmAddressComponent;
	addressForm: FormGroup;
	items: OrganizationContactConnection[];
	selectedItems: OrganizationContactConnection[] = new Array<OrganizationContactConnection>();
	filteredItems: OrganizationContactConnection[] = new Array<OrganizationContactConnection>();
	itemsToDelete: OrganizationContactConnection[] = new Array<OrganizationContactConnection>();
	itemsToInsert: OrganizationContactConnection[] = new Array<OrganizationContactConnection>();
	public organizationForm: FormGroup;
	private organization: OrganizationDto;
	itemControl = new FormControl();
	stateChanges = new Subject<void>();
	private placeholderSecond: string;
	lastFilter = '';
	focused = false;
	isAllSelected = false;
	empty: boolean;
	shouldLabelFloat: boolean;
	required: boolean;
	disabled: boolean;
	errorState: boolean;
	controlType?: string;
	autofilled?: boolean;
	@ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;
	tagsControl = new FormControl();
	selectedTags: TagDto[] = new Array<TagDto>();
	separatorKeysCodes: number[] = [ENTER, COMMA];
	filteredTagsObservable: Observable<string[]>;
	allTags: string[] = [ 'Lehrbeauftragter', 'Kunde', 'Politiker', 'Unternehmen', 'Behörde', 'Bildungseinrichtung',
	 'Institute', 'Ministerium', 'Emeriti', 'Alumni'];
	removable = true;
	selectableTag = true;

	constructor(
		public dialogRef: MatDialogRef<OrganizationsEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: OrganizationDto,
		public dialog: MatDialog,
		@Optional() @Self() public ngControl: NgControl,
		private fm: FocusMonitor,
		private elRef: ElementRef<HTMLElement>,
		private cd: ChangeDetectorRef,
		private contactService: ContactService,
		private organizationService: OrganizationService,
		private fb: FormBuilder
	) {
		super(dialogRef, dialog);
		this.organization = data;
		this.dialogRef.backdropClick().subscribe(() => {
			// Close the dialog
			dialogRef.close({delete: false});
		});
		if (this.organization.tags != null && this.organization.tags.length > 0) {
			this.organization.tags.forEach(x => this.selectedTags.push(x));
		}
		fm.monitor(elRef.nativeElement, true).subscribe((origin) => {
			this.focused = !!origin;
			this.stateChanges.next();
		});
		this.filteredTagsObservable = this.tagsControl.valueChanges.pipe(startWith(''),
			map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
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

	ngOnInit() {
		this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
		this.addressForm = this.addressGroup.getAddressForm();
		this.organizationForm = this.createOrganizationForm();
		this.contactService.getAll().subscribe((y) => {
			y.forEach((x) =>
				this.filteredItems.push({
					contactId: x.id,
					selected: false,
					name: x.name,
					preName: x.preName
				})
			);
			this.items = this.filteredItems;
			this.finishInit();
		});
	}

	finishInit() {
		this.itemControl.valueChanges
			.pipe(
				startWith<string | OrganizationContactConnection[]>(''),
				map((value) => (typeof value === 'string' ? value : this.lastFilter)),
				map((filter) => this.filter(filter))
			)
			.subscribe();

		if (this.organization.employees.length > 0) {
			this.organization.employees.forEach((x) => {
				const cont = this.filteredItems.find((y) => y.contactId === x.id);
				if (cont != null) {
					this.toggleSelection(cont);
				}
			});
		}
		this.contactPossibilitiesEntries.patchExistingValuesToForm(this.organization.contact.contactEntries);
		this.organizationForm.patchValue(this.organization);
		this.organizationForm.markAsPristine();
	}

	private createOrganizationForm(): FormGroup {
		return this.fb.group({
			name: ['', Validators.required],
			description: [''],
			address: this.addressForm,
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

	@Input()
	set value(value: any) {
		if (value) {
			this.selectedItems = value;
		}
		this.stateChanges.next();
	}
	get value() {
		return this.selectedItems;
	}
	@Input()
	get placeholder() {
		return this.placeholderSecond;
	}
	set placeholder(plh) {
		this.placeholderSecond = plh;
		this.stateChanges.next();
	}

	filter(filter: string): OrganizationContactConnection[] {
		this.lastFilter = filter;
		if (filter) {
			return this.items.filter((option) => {
				return option.name.toLowerCase().indexOf(filter.toLowerCase()) >= 0;
			});
		} else {
			return this.items.slice();
		}
	}

	optionClicked(event: Event, item: OrganizationContactConnection) {
		event.stopPropagation();
		this.toggleSelection(item);
	}

	toggleSelectAll() {
		this.isAllSelected = !this.isAllSelected;
		const len = this.filteredItems.length;
		if (this.isAllSelected) {
			this.selectedItems = [];
			for (let i = 0; i < len; i++) {
				this.filteredItems[i].selected = true;
				this.selectedItems.push(this.filteredItems[i]);
			}
		} else {
			for (let i = 0; i < len; i++) {
				this.filteredItems[i].selected = false;
			}
			this.selectedItems = [];
		}
		this.cd.markForCheck();
	}

	toggleSelection(item: OrganizationContactConnection) {
		item.selected = !item.selected;
		if (item.selected) {
			this.selectedItems.push(item);
		} else {
			const i = this.selectedItems.findIndex((value) => value.contactId === item.contactId);
			this.selectedItems.splice(i, 1);
		}
	}

	ngOnDestroy() {
		this.fm.stopMonitoring(this.elRef.nativeElement);
		this.stateChanges.complete();
	}

	onApprove() {
		const idOrganization = this.organization.id;
		const idAddress = this.organization.address.id;
		const idContactPossibilities = this.organization.contact.id;
		this.organization.employees.forEach((x) => {
			const findObj = this.selectedItems.find((y) => y.contactId === x.id);
			if (findObj == null) {
				this.itemsToDelete.push({
					contactId: x.id,
					name: x.name,
					preName: x.preName,
					selected: false
				});
			}
		});
		this.selectedItems.forEach((x) => {
			const contact = this.organization.employees.find((y) => y.id === x.contactId);
			if (contact == null) {
				this.itemsToInsert.push({
					contactId: x.contactId,
					name: x.name,
					preName: x.preName,
					selected: false
				});
			}
		});
		this.organization = this.organizationForm.value;
		this.organization.id = idOrganization;
		this.organization.address.id = idAddress;
		this.organization.contact.id = idContactPossibilities;
		this.organization.tags = this.selectedTags;
		this.organizationService.put(this.organization, this.organization.id).subscribe();
		this.itemsToDelete.forEach((x) =>
			this.organizationService.removeContact(idOrganization, x.contactId).subscribe()
		);
		this.itemsToInsert.forEach((x) => this.organizationService.addContact(idOrganization, x.contactId).subscribe());
		this.dialogRef.close({ delete: false, id: 0 });
	}

	onCancel() {
		super.confirmDialog({ delete: false, id: 0 });
	}

	onDelete() {
		super.confirmDialog({ delete: true, id: this.organization.id });
	}

	hasChanged(): boolean {
		return !this.organizationForm.pristine;
	}

	isValid(): boolean {
		return this.organizationForm.valid && this.addressGroup.isValid();
	}
}
