import {
  ElementRef, HostBinding, Component, OnInit, ViewChild, Input, Optional, Self,
  ChangeDetectorRef, OnDestroy
} from '@angular/core';
import { NgControl, FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { FocusMonitor } from '@angular/cdk/a11y';
import {
  MatAutocompleteTrigger} from '@angular/material/autocomplete';
import { MatFormFieldControl } from '@angular/material/form-field';
import { EventCreateDto } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';

export class ItemList {
  constructor(public item: string, public selected?: boolean) {
    if (selected === undefined) {
      selected = false;
    }
  }
}

export class EventContactConnection {
  contactId: number;
  selected: boolean;
  name: string;
  preName: string;
}

@Component({
  selector: 'app-events-add',
  templateUrl: './events-add.component.html',
  styleUrls: ['./events-add.component.scss']
})

export class EventsAddComponent extends BaseDialogInput<EventsAddComponent>
  implements OnInit, OnDestroy, MatFormFieldControl<EventContactConnection> {
  static nextId = 0;
  @ViewChild('inputTrigger', { read: MatAutocompleteTrigger }) inputTrigger: MatAutocompleteTrigger;
  @HostBinding() id = `input-ac-${EventsAddComponent.nextId++}`;
  @HostBinding('attr.aria-describedby') describedBy = '';
  public selectable = true;
  items: EventContactConnection[];
  selectedItems: EventContactConnection[] = new Array<EventContactConnection>();
  filteredItems: EventContactConnection[] = new Array<EventContactConnection>();
  itemsToDelete: EventContactConnection[] = new Array<EventContactConnection>();
  itemsToInsert: EventContactConnection[] = new Array<EventContactConnection>();
  public eventsForm: FormGroup;
  private changeCallback: (input: EventContactConnection[]) => void;
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
    public dialogRef: MatDialogRef<EventsAddComponent>,
    @Optional() @Self() public ngControl: NgControl,
    private fm: FocusMonitor,
    private elRef: ElementRef<HTMLElement>,
    private cd: ChangeDetectorRef,
    private contactService: ContactService,
    private eventService: EventService,
    private fb: FormBuilder,
    public dialog: MatDialog
  ) {
    super(dialogRef, dialog);
    if (this.ngControl != null) {
      this.ngControl.valueAccessor = this;
    }
    fm.monitor(elRef.nativeElement, true).subscribe(origin => {
      this.focused = !!origin;
      this.stateChanges.next();
    });
  }

  hasChanged() {
    return !this.eventsForm.pristine;
  }


  ngOnInit() {
    this.eventsForm = this.createOrganizationForm();
    this.contactService.getAll().subscribe(y => {
      y.forEach(x => {
        this.filteredItems.push(
          {
            contactId: x.id,
            name: x.name,
            preName: x.preName,
            selected: false
          }
        );
      });
    }
    );
  }

  private createOrganizationForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      date: ['', Validators.required],
      time: ['', Validators.required],
      duration: ['', Validators.required]
    });
  }

  setDescribedByIds(ids: string[]) {
    this.describedBy = ids.join(' ');
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

  onContainerClick(event: MouseEvent): void {
    throw new Error('Method not implemented.');
  }

  writeValue() {
  }
  registerOnChange(fn: (input: EventContactConnection[]) => void) {
    this.changeCallback = fn;
  }
  registerOnTouched() {
  }

  clicker() {
    this.inputTrigger.openPanel();
  }

  filter(filter: string): EventContactConnection[] {
    this.lastFilter = filter;
    if (filter) {
      return this.items.filter(option => {
        return option.name.toLowerCase().indexOf(filter.toLowerCase()) >= 0;
      });
    } else {
      return this.items.slice();
    }
  }

  optionClicked(event: Event, item: EventContactConnection) {
    event.stopPropagation();
    this.toggleSelection(item);
  }

  toggleSelectAll() {
    this.isAllSelected = !this.isAllSelected;
    const len = this.filteredItems.length;
    if (this.isAllSelected) {
      for (let i = 0; i++; i < len) {
        this.filteredItems[i].selected = true;
      }
      this.selectedItems = this.filteredItems;
      this.changeCallback(this.selectedItems);
      this.cd.markForCheck();
    } else {
      this.selectedItems = [];
    }
    this.changeCallback(this.selectedItems);
  }

  toggleSelection(item: EventContactConnection) {
    item.selected = !item.selected;
    if (item.selected) {
      this.selectedItems.push(item);
    } else {
      const i = this.selectedItems.findIndex(value => value.contactId === item.contactId);
      this.selectedItems.splice(i, 1);
    }
    if (this.changeCallback) {
      this.changeCallback(this.selectedItems);
    }
  }

  ngOnDestroy() {
    this.fm.stopMonitoring(this.elRef.nativeElement);
    this.stateChanges.complete();
  }

  saveValues() {
    const eventToSave: EventCreateDto = this.eventsForm.value;
    eventToSave.contacts = new Array<number>();
    this.selectedItems.forEach(x => eventToSave.contacts.push(x.contactId));
    this.eventService.post(eventToSave).subscribe(() => this.dialogRef.close());
  }

  exit() {
    super.confirmDialog();
  }
}
