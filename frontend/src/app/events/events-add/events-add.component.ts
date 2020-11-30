import {
  ElementRef, HostBinding, Component, OnInit, ViewChild, Input, Optional, Self,
  ChangeDetectorRef, OnDestroy, Inject
} from '@angular/core';
import { NgControl, FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { FocusMonitor } from '@angular/cdk/a11y';
import {
  MatAutocompleteTrigger
} from '@angular/material/autocomplete';
import { MatFormFieldControl } from '@angular/material/form-field';
import { EventCreateDto, MODEL_TYPE, OrganizationService } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup, ValidatorFn } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';

// Validator fuer Start frueher als Ende?
const MyAwesomeRangeValidator: ValidatorFn = (fg: FormGroup) => {
  const start = fg.get('start').value;
  const end = fg.get('end').value;
  return start !== null && end !== null && start < end
    ? null
    : { range: true };
};

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
  preselectedContacts: Array<number> = new Array<number>();

  constructor(
    public dialogRef: MatDialogRef<EventsAddComponent>,
    @Optional() @Self() public ngControl: NgControl,
    private fm: FocusMonitor,
    @Inject(MAT_DIALOG_DATA) public data: Array<number>,
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
    if (data != null && data.length > 0) {
      this.preselectedContacts = data;
    }
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
            objectId: x.id,
            name: x.name,
            preName: x.preName,
            selected: false,
            modelType: MODEL_TYPE.CONTACT,
            participated: false,
            wasInvited: false
          }
        );
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
        if (this.preselectedContacts != null && this.preselectedContacts.length > 0) {
          this.preselectedContacts.forEach(s => {
            const cont: EventContactConnection = this.filteredItems.find(z => z.objectId === s && z.modelType === MODEL_TYPE.CONTACT);
            if (cont != null) {
              this.toggleSelection(cont);
            }
          });
        }
      });
    });
  }



  private createOrganizationForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      date: ['', Validators.required],
      start: ['', Validators.required],
      end: ['', Validators.required]
    }, { validator: MyAwesomeRangeValidator });  // Einbau des Validators der Start/Ende validiert
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
    eventToSave.contacts = new Array<number>();
    eventToSave.organizations = new Array<number>();
    this.selectedItems.forEach(x => {
      if (x.modelType === MODEL_TYPE.CONTACT) {
        eventToSave.contacts.push(x.objectId);
      } else {
        eventToSave.organizations.push(x.objectId);
      }
    });
    this.eventService.post(eventToSave).subscribe(() => this.dialogRef.close());
  }

  exit() {
    super.confirmDialog();
  }
}
