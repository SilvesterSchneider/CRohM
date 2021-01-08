import {
  ElementRef, HostBinding, Component, OnInit, ViewChild, Input, Optional, Self,
  ChangeDetectorRef, OnDestroy, Inject
} from '@angular/core';
import { NgControl, FormControl, AbstractControl, ValidatorFn } from '@angular/forms';
import { Subject } from 'rxjs';
import { FocusMonitor } from '@angular/cdk/a11y';
import {
  MatAutocompleteTrigger
} from '@angular/material/autocomplete';
import { MatFormFieldControl } from '@angular/material/form-field';
import { EventCreateDto, MODEL_TYPE, OrganizationService } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';
import { OsmAddressComponent } from '../../shared/osm/osm-address/osm-address.component';
import { getTranslationDeclStmts } from '@angular/compiler/src/render3/view/template';

export class ItemList {
  constructor(public item: string, public selected?: boolean) {
    if (selected === undefined) {
      selected = false;
    }
  }
}

export class EventContactConnection {
  objectId: number;
  selected: boolean;
  name: string;
  preName: string;
  participated: boolean;
  wasInvited: boolean;
  modelType: MODEL_TYPE;
}

@Component({
  selector: 'app-events-add',
  templateUrl: './events-add.component.html',
  styleUrls: ['./events-add.component.scss']
})
/// <summary>
/// RAM: 90%
/// </summary>
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
  preselectedContactsOrgas: Array<number> = new Array<number>();

  constructor(
    public dialogRef: MatDialogRef<EventsAddComponent>,
    @Optional() @Self() public ngControl: NgControl,
    private fm: FocusMonitor,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private elRef: ElementRef<HTMLElement>,
    private cd: ChangeDetectorRef,
    private contactService: ContactService,
    private eventService: EventService,
    private fb: FormBuilder,
    public dialog: MatDialog,
    private orgaService: OrganizationService
  ) {
    super(dialogRef, dialog);
    this.dialogRef.backdropClick().subscribe(() => {
      // Close the dialog
      dialogRef.close();
    });
    if (this.ngControl != null) {
      this.ngControl.valueAccessor = this;
    }
    fm.monitor(elRef.nativeElement, true).subscribe(origin => {
      this.focused = !!origin;
      this.stateChanges.next();
    });
    if (data != null && data.list.length > 0) {
      this.preselectedContactsOrgas = data.list;
    }
  }

  userAriaDescribedBy?: string;

  hasChanged() {
    return !this.eventsForm.pristine;
  }

  ngOnInit() {
    this.eventsForm = this.fb.group({
      name: ['', Validators.required],
      date: [new FormControl('')],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      description: ['', Validators.maxLength(300)],
      location: ['']
    });

    this.contactService.getAll().subscribe(y => {
      y.forEach(x => {
        this.filteredItems.push(
          {
            objectId: x.id,
            name: x.name,
            preName: x.preName,
            selected: false,
            modelType: MODEL_TYPE.CONTACT,
            participated: false,
            wasInvited: false
          });
        });
      this.orgaService.get().subscribe(a => {
        a.forEach(b => {
          this.filteredItems.push(
            {
              objectId: b.id,
              name: b.name,
              preName: b.description,
              selected: false,
              modelType: MODEL_TYPE.ORGANIZATION,
              participated: false,
              wasInvited: false
            }
          );
        });
        if (this.preselectedContactsOrgas?.length > 0) {
          this.preselectedContactsOrgas.forEach(s => {
            let cont: EventContactConnection;
            if (this.data.useOrgas != null && this.data.useOrgas) {
              cont = this.filteredItems.find(z => z.objectId === s && z.modelType === MODEL_TYPE.ORGANIZATION);
            } else {
              cont = this.filteredItems.find(z => z.objectId === s && z.modelType === MODEL_TYPE.CONTACT);
            }
            if (cont != null) {
              this.toggleSelection(cont);
            }
          });
        }
      });
    });
  }

  getTheStartTime(): string {
    if (this.eventsForm != null && this.eventsForm.get('startTime') != null
      && this.eventsForm.get('startTime').value != null) {
      return this.eventsForm.get('startTime').value;
    }
    return '';
  }

  getEndTime(): string {
    if (this.eventsForm != null && this.eventsForm.get('endTime') != null
      && this.eventsForm.get('endTime').value != null) {
      return this.eventsForm.get('endTime').value;
    }
    return '';
  }

  formIsValid(): boolean {
    const startTime = this.getTheStartTime();
    const endTime = this.getEndTime();
    return this.eventsForm.valid && startTime.length > 0 && endTime.length > 0 && this.isEndTimeGreaterThanStarttime(startTime, endTime);
  }

  isEndTimeGreaterThanStarttime(startTime: string, endTime: string): boolean {
    if (endTime.indexOf(':') > 0 && startTime.indexOf(':') > 0) {
      const hoursEnd = +endTime.substr(0, endTime.indexOf(':'));
      const minutesEnd = +endTime.substr(endTime.indexOf(':') + 1);
      const hoursStart = +startTime.substr(0, endTime.indexOf(':'));
      const minutesStart = +startTime.substr(endTime.indexOf(':') + 1);
      if (hoursEnd > hoursStart || hoursEnd === hoursStart && minutesEnd > minutesStart) {
        return true;
      }
    }
    return false;
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
    this.filteredItems.forEach(x => this.toggleSelectionAll(x, this.isAllSelected));
  }

  toggleSelectionAll(item: EventContactConnection, isSelected: boolean) {
    item.selected = isSelected;
    if (item.selected) {
      if (this.selectedItems.find(a => a.objectId === item.objectId && a.modelType === item.modelType) == null) {
        this.selectedItems.push(item);
      }
    } else {
      const i = this.selectedItems.findIndex(value => value.objectId === item.objectId && value.modelType === item.modelType);
      if (i > -1) {
        this.selectedItems[i].participated = false;
        this.selectedItems.splice(i, 1);
      }
    }
    if (this.changeCallback) {
      this.changeCallback(this.selectedItems);
    }
  }

  toggleSelection(item: EventContactConnection) {
    item.selected = !item.selected;
    if (item.selected) {
      if (this.selectedItems.find(a => a.objectId === item.objectId && a.modelType === item.modelType) == null) {
        this.selectedItems.push(item);
      }
    } else {
      const i = this.selectedItems.findIndex(value => value.objectId === item.objectId && value.modelType === item.modelType);
      if (i > -1) {
        this.selectedItems[i].participated = false;
        this.selectedItems.splice(i, 1);
      }
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
    eventToSave.date = new Date(new Date(eventToSave.date).getTime()).toDateString();
    eventToSave.contacts = new Array<number>();
    eventToSave.organizations = new Array<number>();
    this.selectedItems.forEach(x => {
      if (x.modelType === MODEL_TYPE.CONTACT) {
        eventToSave.contacts.push(x.objectId);
      } else {
        eventToSave.organizations.push(x.objectId);
      }
    });
    this.eventService.post(eventToSave).subscribe(() => this.dialogRef.close({save: true}));
  }

  exit() {
    this.dialogRef.close({save: false})
  }
}
