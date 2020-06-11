import {
  ElementRef, HostBinding, Component, OnInit, ViewChild, Input, Optional, Self,
  ChangeDetectorRef, OnDestroy, Inject
} from '@angular/core';
import { NgControl, FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { FocusMonitor } from '@angular/cdk/a11y';
import {
  MatAutocompleteTrigger} from '@angular/material/autocomplete';
import { MatFormFieldControl } from '@angular/material/form-field';
import { ContactDto, EventDto, ParticipatedDto } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';

export class EventContactConnection {
  contactId: number;
  selected: boolean;
  name: string;
  preName: string;
  participated: boolean;
}

@Component({
  selector: 'app-events-detail',
  templateUrl: './events-detail.component.html',
  styleUrls: ['./events-detail.component.scss']
})

export class EventsDetailComponent extends BaseDialogInput<EventsDetailComponent>
  implements OnInit, OnDestroy, MatFormFieldControl<EventContactConnection> {
  static nextId = 0;
  @ViewChild('inputTrigger', { read: MatAutocompleteTrigger }) inputTrigger: MatAutocompleteTrigger;
  @HostBinding() id = `input-ac-${EventsDetailComponent.nextId++}`;
  @HostBinding('attr.aria-describedby') describedBy = '';
  public selectable = true;
  contacts: ContactDto[];
  items: EventContactConnection[] = new Array<EventContactConnection>();
  selectedItems: EventContactConnection[] = new Array<EventContactConnection>();
  filteredItems: EventContactConnection[] = new Array<EventContactConnection>();
  public eventsForm: FormGroup;
  private event: EventDto;
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
  columnsEvent: ['participated', 'prename', 'name'];

  constructor(
    public dialogRef: MatDialogRef<EventsDetailComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: EventDto,
    @Optional() @Self() public ngControl: NgControl,
    private fm: FocusMonitor,
    private elRef: ElementRef<HTMLElement>,
    private cd: ChangeDetectorRef,
    private contactService: ContactService,
    private eventService: EventService,
    private fb: FormBuilder  ) {
    super(dialogRef, dialog);
    if (this.ngControl != null) {
      this.ngControl.valueAccessor = this;
    }
    fm.monitor(elRef.nativeElement, true).subscribe(origin => {
      this.focused = !!origin;
      this.stateChanges.next();
    });
    this.event = data;
  }

  ngOnInit() {
    this.eventsForm = this.createEventsForm();
    this.contactService.getAll().subscribe(y => {
      this.contacts = y;
      y.forEach(x => {
        let participatedReal = false;
        this.event.participated.forEach(z => {
          if (z.contactId === x.id) {
            participatedReal = z.hasParticipated;
          }
        });
        this.filteredItems.push(
          {
            contactId: x.id,
            name: x.name,
            preName: x.preName,
            selected: false,
            participated: participatedReal
          }
        );
      });
      this.finishInit();
    }
    );
  }

  private finishInit() {
    this.itemControl.valueChanges.pipe(
      startWith<string | EventContactConnection[]>(''),
      map(value => typeof value === 'string' ? value : this.lastFilter),
      map(filter => this.filter(filter))
    ).subscribe();

    if (this.event.contacts.length > 0) {
      this.event.contacts.forEach(x => {
        const cont = this.filteredItems.find(y => y.contactId === x.id);
        if (cont != null) {
          this.toggleSelection(cont);
        }
      });
    }
    this.eventsForm.patchValue(this.event);
    this.eventsForm.get('time').patchValue(this.formatTime(this.event.time));
  }

  private formatTime(date) {
    const d = new Date(date);
    let hours = '' + (d.getHours());
    let minutes = '' + d.getMinutes();
    if (hours.length < 2) {
      hours = '0' + hours;
    }
    if (minutes.length < 2) {
      minutes = '0' + minutes;
    }
    return [hours, minutes].join(':');
  }

  private createEventsForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      date: [new FormControl(new Date(this.event.date)), Validators.required],
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
      this.selectedItems[i].participated = false;
      this.selectedItems.splice(i, 1);
    }
    if (this.changeCallback) {
      this.changeCallback(this.selectedItems);
    }
  }

  toggleParticipated(item: EventContactConnection) {
    this.selectedItems.forEach(x => {
      if (x.contactId === item.contactId) {
        x.participated = !x.participated;
      }
    });
  }

  ngOnDestroy() {
    this.fm.stopMonitoring(this.elRef.nativeElement);
    this.stateChanges.complete();
  }

  saveValues() {
    const eventToSave: EventDto = this.eventsForm.value;
    this.event.date = new Date(new Date(eventToSave.date).getTime()).toDateString();
    this.event.duration = eventToSave.duration;
    this.event.time = eventToSave.time;
    const contacts: ContactDto[] = new Array<ContactDto>();
    const participants: ParticipatedDto[] = new Array<ParticipatedDto>();
    this.selectedItems.forEach(x => {
      const contact = this.contacts.find(y => y.id === x.contactId);
      if (contact != null) {
        contacts.push(contact);
        const partExistend: ParticipatedDto = this.event.participated.find(z => z.contactId === x.contactId);
        if (partExistend == null) {
          participants.push(
            {
              contactId: x.contactId,
              hasParticipated: x.participated,
              id: 0
            }
          );
        } else {
          partExistend.hasParticipated = x.participated;
          participants.push(partExistend);
        }
      }
    });
    this.event.contacts = contacts;
    this.event.participated = participants;
    this.eventService.put(this.event, this.event.id).subscribe(() => this.dialogRef.close());
  }

  close() {
    super.confirmDialog();
  }
}


