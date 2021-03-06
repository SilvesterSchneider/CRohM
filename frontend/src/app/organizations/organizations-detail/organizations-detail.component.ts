import {
  ElementRef, HostBinding, Component, OnInit, ViewChild, forwardRef, Input, Optional, Self,
  ChangeDetectorRef, OnDestroy
} from '@angular/core';
import { NgControl, FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { FocusMonitor } from '@angular/cdk/a11y';
import {
  MatAutocompleteTrigger
} from '@angular/material/autocomplete';
import { MatFormFieldControl } from '@angular/material/form-field';
import { OrganizationDto } from '../../shared/api-generated/api-generated';
import { OrganizationService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';

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
  selector: 'app-organizations-detail',
  templateUrl: './organizations-detail.component.html',
  styleUrls: ['./organizations-detail.component.scss'],
  providers: [{ provide: MatFormFieldControl, useExisting: OrganizationsDetailComponent }]
})

export class OrganizationsDetailComponent implements OnInit, OnDestroy {
  static nextId = 0;
  @ViewChild('inputTrigger', { read: MatAutocompleteTrigger }) inputTrigger: MatAutocompleteTrigger;
  @HostBinding() id = `input-ac-${OrganizationsDetailComponent.nextId++}`;
  @HostBinding('attr.aria-describedby') describedBy = '';
  @ViewChild(ContactPossibilitiesComponent, {static: true})
  contactPossibilitiesEntries: ContactPossibilitiesComponent;
  contactPossibilitiesEntriesFormGroup: FormGroup;
  public selectable = true;
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

  constructor(
    @Optional() @Self() public ngControl: NgControl,
    private fm: FocusMonitor,
    private elRef: ElementRef<HTMLElement>,
    private cd: ChangeDetectorRef,
    private contactService: ContactService,
    private organizationService: OrganizationService,
    private fb: FormBuilder,
    private route: ActivatedRoute
  ) {
    fm.monitor(elRef.nativeElement, true).subscribe(origin => {
      this.focused = !!origin;
      this.stateChanges.next();
    });
  }

  ngOnInit() {
    this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
    this.organizationForm = this.createOrganizationForm();
    this.contactService.getAll().subscribe(y => {
      y.forEach(x => this.filteredItems.push(
        {
          contactId: x.id,
          selected: false,
          name: x.name,
          preName: x.preName
        }));
      this.items = this.filteredItems;
      this.finishInit();
    });
  }

  finishInit() {
    this.itemControl.valueChanges.pipe(
      startWith<string | OrganizationContactConnection[]>(''),
      map(value => typeof value === 'string' ? value : this.lastFilter),
      map(filter => this.filter(filter))
    ).subscribe();
    this.organization = this.route.snapshot.data.organization;

    if (this.organization.employees.length > 0) {
      this.organization.employees.forEach(x => {
        const cont = this.filteredItems.find(y => y.contactId === x.id);
        if (cont != null) {
          this.toggleSelection(cont);
        }
      });
    }
    this.contactPossibilitiesEntries.patchExistingValuesToForm(this.organization.contact.contactEntries);
    this.organizationForm.patchValue(this.organization);
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
      phoneNumber: ['', Validators.pattern('^0[0-9\- ]*$')],
      fax: ['', Validators.pattern('^0[0-9\- ]*$')],
      mail: ['', Validators.email],
      contactEntries: this.contactPossibilitiesEntriesFormGroup
    });
  }

  @Input() set value(value: any) {
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
      return this.items.filter(option => {
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
      const i = this.selectedItems.findIndex(value => value.contactId === item.contactId);
      this.selectedItems.splice(i, 1);
    }
  }

  ngOnDestroy() {
    this.fm.stopMonitoring(this.elRef.nativeElement);
    this.stateChanges.complete();
  }

  saveValues() {
    const idOrganization = this.organization.id;
    const idAddress = this.organization.address.id;
    const idContactPossibilities = this.organization.contact.id;
    this.organization.employees.forEach(x => {
      const findObj = this.selectedItems.find(y => y.contactId === x.id);
      if (findObj == null) {
        this.itemsToDelete.push({
          contactId: x.id,
          name: x.name,
          preName: x.preName,
          selected: false
        });
      }
    });
    this.selectedItems.forEach(x => {
      const contact = this.organization.employees.find(y => y.id === x.contactId);
      if (contact == null) {
        this.itemsToInsert.push(
          {
            contactId: x.contactId,
            name: x.name,
            preName: x.preName,
            selected: false
          }
        );
      }
    }
    );
    this.organization = this.organizationForm.value;
    this.organization.id = idOrganization;
    this.organization.address.id = idAddress;
    this.organization.contact.id = idContactPossibilities;
    this.organizationService.put(this.organization, this.organization.id).subscribe();
    this.itemsToDelete.forEach(x => this.organizationService.removeContact(idOrganization, x.contactId).subscribe());
    this.itemsToInsert.forEach(x => this.organizationService.addContact(idOrganization, x.contactId).subscribe());
  }
}
