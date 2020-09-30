import {
  ElementRef, HostBinding, Component, OnInit, ViewChild, Input, Optional, Self,
  ChangeDetectorRef, OnDestroy, Inject
} from '@angular/core';
import { NgControl, FormControl } from '@angular/forms';
import { Subject, Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { FocusMonitor } from '@angular/cdk/a11y';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import {
  MatAutocompleteTrigger, MatAutocompleteSelectedEvent
} from '@angular/material/autocomplete';
import { MatFormFieldControl } from '@angular/material/form-field';
import { ContactDto, EventDto, MailService, ParticipatedDto, TagDto } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';
import { EventsInvitationComponent } from '../events-invitation/events-invitation.component';

export class EventContactConnection {
  contactId: number;
  selected: boolean;
  name: string;
  preName: string;
  participated: boolean;
  wasInvited: boolean;
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

  @ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;
  tagsControl = new FormControl();
  selectedTags: TagDto[] = new Array<TagDto>();
  separatorKeysCodes: number[] = [ENTER, COMMA];
  filteredTagsObservable: Observable<string[]>;
  allTags: string[] = ['Lehrbeauftragter', 'Kunde', 'Politiker', 'Unternehmen', 'Behörde', 'Bildungseinrichtung',
   'Institute', 'Ministerium', 'Emeriti', 'Alumni'];
  removable = true;
  selectableTag = true;

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
    private fb: FormBuilder,
    private mailService: MailService) {
    super(dialogRef, dialog);
    if (this.ngControl != null) {
      this.ngControl.valueAccessor = this;
    }
    fm.monitor(elRef.nativeElement, true).subscribe(origin => {
      this.focused = !!origin;
      this.stateChanges.next();
    });
    this.event = data;
    this.event.tags.forEach(x => this.selectedTags.push(x));
    this.filteredTagsObservable = this.tagsControl.valueChanges.pipe(startWith(''),
      map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
  }


  hasChanged() {
    return !this.eventsForm.pristine;
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
    this.eventsForm = this.createEventsForm();
    this.contactService.getAll().subscribe(y => {
      this.contacts = y;
      y.forEach(x => {
        let participatedReal = false;
        let wasInvitedReal = false;
        this.event.participated.forEach(z => {
          if (z.contactId === x.id) {
            participatedReal = z.hasParticipated;
            wasInvitedReal = z.wasInvited;
          }
        });
        this.filteredItems.push(
          {
            contactId: x.id,
            name: x.name,
            preName: x.preName,
            selected: false,
            participated: participatedReal,
            wasInvited: wasInvitedReal
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

  callInvitation() {
    const dialogRef = this.dialog.open(EventsInvitationComponent, { data: this.event, disableClose: true, minWidth: '450px', minHeight: '400px' });
    dialogRef.afterClosed().subscribe(x => {
      if (x.send && x.text != null) {
        const listOfIds: number[] = new Array<number>();
        this.selectedItems.forEach(y => {
          listOfIds.push(y.contactId);
          y.wasInvited = true;
        });
        this.mailService.sendInvitationMails(listOfIds, x.text).subscribe();
      }
    });
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
      if (this.selectedItems.find(a => a.contactId === item.contactId) == null) {
        this.selectedItems.push(item);
      }
    } else {
      const i = this.selectedItems.findIndex(value => value.contactId === item.contactId);
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
      if (this.selectedItems.find(a => a.contactId === item.contactId) == null) {
        this.selectedItems.push(item);
      }
    } else {
      const i = this.selectedItems.findIndex(value => value.contactId === item.contactId);
      if (i > -1) {
        this.selectedItems[i].participated = false;
        this.selectedItems.splice(i, 1);
      }
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
              wasInvited: x.wasInvited,
              id: 0
            }
          );
        } else {
          partExistend.hasParticipated = x.participated;
          partExistend.wasInvited = x.wasInvited;
          participants.push(partExistend);
        }
      }
    });
    this.event.contacts = contacts;
    this.event.participated = participants;
    this.event.tags = this.selectedTags;
    this.eventService.put(this.event, this.event.id).subscribe(() => {
      this.dialogRef.close();
    });
  }

  close() {
    super.confirmDialog();
  }
}


