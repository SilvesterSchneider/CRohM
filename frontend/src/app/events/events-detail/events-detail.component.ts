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
import { ContactDto, EventDto, MailService, MODEL_TYPE, OrganizationDto, OrganizationService,
   ParticipatedDto, ParticipatedStatus, TagDto } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';
import { EventsInvitationComponent } from '../events-invitation/events-invitation.component';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmDialogComponent, ConfirmDialogModel } from 'src/app/shared/form/confirmdialog/confirmdialog.component';

export class EventContactConnection {
  objectId: number;
  selected: boolean;
  name: string;
  preName: string;
  participated: boolean;
  eventStatus: ParticipatedStatus;
  modelType: MODEL_TYPE;
}

@Component({
  selector: 'app-events-detail',
  templateUrl: './events-detail.component.html',
  styleUrls: ['./events-detail.component.scss']
})
/// <summary>
/// RAM: 90%
/// </summary>
export class EventsDetailComponent extends BaseDialogInput<EventsDetailComponent>
  implements OnInit, OnDestroy, MatFormFieldControl<EventContactConnection> {
  static nextId = 0;
  @ViewChild('inputTrigger', { read: MatAutocompleteTrigger }) inputTrigger: MatAutocompleteTrigger;
  @HostBinding() id = `input-ac-${EventsDetailComponent.nextId++}`;
  @HostBinding('attr.aria-describedby') describedBy = '';
  public selectable = true;
  contacts: ContactDto[];
  orgas: OrganizationDto[];
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
    private orgaService: OrganizationService,
    private eventService: EventService,
    private fb: FormBuilder,
    private mailService: MailService,
    private translate: TranslateService) {
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
    this.eventsForm = this.fb.group({
      name: ['', Validators.required],
      date: [new FormControl(new Date(this.event.date)), Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      description: ['', Validators.maxLength(300)],
      location: ['']
    });

    this.contactService.getAll().subscribe(y => {
      this.contacts = y;
      y.forEach(x => {
        let participatedReal = false;
        let eventState = ParticipatedStatus.NOT_INVITED;
        this.event.participated.forEach(z => {
          if (z.objectId === x.id && z.modelType === MODEL_TYPE.CONTACT) {
            participatedReal = z.hasParticipated;
            eventState = z.eventStatus;
          }
        });
        this.filteredItems.push(
          {
            objectId: x.id,
            name: x.name,
            preName: x.preName,
            selected: false,
            participated: participatedReal,
            eventStatus: eventState,
            modelType: MODEL_TYPE.CONTACT
          }
        );
      });
      this.addOrgas();
    });
  }

  addOrgas() {
    this.orgaService.get().subscribe(x => {
      this.orgas = x;
      x.forEach(y => {
        let participatedReal = false;
        let eventState = ParticipatedStatus.NOT_INVITED;
        this.event.participated.forEach(z => {
          if (z.objectId === y.id && z.modelType === MODEL_TYPE.ORGANIZATION) {
            participatedReal = z.hasParticipated;
            eventState = z.eventStatus;
          }
        });
        this.filteredItems.push(
          {
            objectId: y.id,
            name: y.name,
            preName: '',
            selected: false,
            participated: participatedReal,
            eventStatus: eventState,
            modelType: MODEL_TYPE.ORGANIZATION
          }
        );
      });
      this.finishInit();
    });
  }

  private finishInit() {
    this.itemControl.valueChanges.pipe(
      startWith<string | EventContactConnection[]>(''),
      map(value => typeof value === 'string' ? value : this.lastFilter),
      map(filter => this.filter(filter))
    ).subscribe();

    if (this.event.contacts.length > 0) {
      this.event.contacts.forEach(x => {
        const cont = this.filteredItems.find(y => y.objectId === x.id && y.modelType === MODEL_TYPE.CONTACT);
        if (cont != null) {
          this.toggleSelection(cont);
        }
      });
    }
    if (this.event.organizations.length > 0) {
      this.event.organizations.forEach(x => {
        const org = this.filteredItems.find(y => y.objectId === x.id && y.modelType === MODEL_TYPE.ORGANIZATION);
        if (org != null) {
          this.toggleSelection(org);
        }
      });
    }
    this.eventsForm.patchValue(this.event);
    this.eventsForm.get('startTime').patchValue(this.formatTime(this.event.starttime));
    this.eventsForm.get('endTime').patchValue(this.formatTime(this.event.endtime));
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

  formIsValid(): boolean {
    const startTime = this.getTheStartTime();
    const endTime = this.getEndTime();
    return this.eventsForm.valid && startTime.length > 0 && endTime.length > 0 && this.isEndTimeGreaterThanStarttime(startTime, endTime);
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

  callInvitation(save: boolean, useFinishSave: boolean): boolean {
    const dialogRef = this.dialog.open(EventsInvitationComponent, { data: this.event, disableClose: true, minWidth: '450px', minHeight: '400px' });
    dialogRef.afterClosed().subscribe(x => {
      if (x.send && x.text != null) {
        const listOfContactIds: number[] = new Array<number>();
        const listOfOrgaIds: number[] = new Array<number>();
        this.selectedItems.forEach(y => {
          if (y.modelType === MODEL_TYPE.CONTACT && y.eventStatus === ParticipatedStatus.NOT_INVITED) {
            listOfContactIds.push(y.objectId);
          } else if (y.modelType === MODEL_TYPE.ORGANIZATION && y.eventStatus === ParticipatedStatus.NOT_INVITED) {
            listOfOrgaIds.push(y.objectId);
          }
          if (y.eventStatus === ParticipatedStatus.NOT_INVITED) {
            y.eventStatus = ParticipatedStatus.INVITED;
          }
        });

        this.mailService.sendInvitationMails(listOfContactIds, listOfOrgaIds, x.text, this.event.id).subscribe(x => {
          if (useFinishSave) {
            this.finishSave(save);
          }
        });
        return true;
      } else {
        return false;
      }
    });
    return false;
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

  isDateOk(): boolean {
    const dateOfEvent = new Date(this.event.date);
    dateOfEvent.setHours(new Date(this.event.starttime).getHours());
    dateOfEvent.setMinutes(new Date(this.event.starttime).getMinutes());
    const dateNow = new Date(Date.now());
    return (dateNow.getFullYear() > dateOfEvent.getFullYear()) ||
      (dateNow.getFullYear() === dateNow.getFullYear() &&
      dateNow.getMonth() > dateOfEvent.getMonth()) ||
      (dateNow.getFullYear() === dateNow.getFullYear() &&
      dateNow.getMonth() === dateOfEvent.getMonth() &&
      dateNow.getDate() > dateOfEvent.getDate()) ||
      (dateNow.getFullYear() === dateNow.getFullYear() &&
      dateNow.getMonth() === dateOfEvent.getMonth() &&
      dateNow.getDate() === dateOfEvent.getDate() &&
      dateNow.getHours() > dateOfEvent.getHours()) ||
      (dateNow.getFullYear() === dateNow.getFullYear() &&
      dateNow.getMonth() === dateOfEvent.getMonth() &&
      dateNow.getDate() === dateOfEvent.getDate() &&
      dateNow.getHours() === dateOfEvent.getHours() &&
      dateNow.getMinutes() >= dateOfEvent.getMinutes());
  }

  isItemConditionOk(status: ParticipatedStatus): boolean {
    return status !== ParticipatedStatus.CANCELLED;
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

  toggleParticipated(item: EventContactConnection) {
    this.selectedItems.forEach(x => {
      if (x.objectId === item.objectId && x.modelType === item.modelType) {
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
    eventToSave.date = new Date(new Date(eventToSave.date)).toDateString();
    eventToSave.starttime = this.eventsForm.get('startTime').value;
    eventToSave.endtime = this.eventsForm.get('endTime').value;
    const saveNewValues = this.isSameDate(new Date(this.event.date).toString(), eventToSave.date)
     && this.isSameTime(this.event.starttime, eventToSave.starttime)
     && this.isSameTime(this.event.endtime, eventToSave.endtime);
    let callInvitation = false;
    if (saveNewValues) {
      this.selectedItems.forEach(a => {
        if (a.eventStatus === ParticipatedStatus.NOT_INVITED) {
          callInvitation = true;
        }
      });
    }
    this.event.date = eventToSave.date;
    this.event.starttime = eventToSave.starttime;
    this.event.endtime = eventToSave.endtime;
    this.event.description = eventToSave.description;
    this.event.location = eventToSave.location;
    if (callInvitation) {
      const data = new ConfirmDialogModel('event.sendInvitation', 'event.sendInvitation');
      const dialogYesNo = this.dialog.open(ConfirmDialogComponent, {data});
      dialogYesNo.afterClosed().subscribe(y => {
        if (y) {
          if (!this.callInvitation(saveNewValues, true)) {
            this.finishSave(saveNewValues);
          }
        } else {
          this.finishSave(saveNewValues);
        }
      });
    } else {
      this.finishSave(saveNewValues);
    }
  }

  finishSave(saveNewValues: boolean) {
    const contacts: ContactDto[] = new Array<ContactDto>();
    const organizations: OrganizationDto[] = new Array<OrganizationDto>();
    const participants: ParticipatedDto[] = new Array<ParticipatedDto>();
    this.selectedItems.forEach(x => {
      if (x.modelType === MODEL_TYPE.CONTACT) {
        const contact = this.contacts.find(y => y.id === x.objectId);
        if (contact != null) {
          contacts.push(contact);
          this.addParticipatedStates(MODEL_TYPE.CONTACT, saveNewValues, participants, x);
        }
      } else if (x.modelType === MODEL_TYPE.ORGANIZATION) {
        const orga = this.orgas.find(y => y.id === x.objectId);
        if (orga != null) {
          organizations.push(orga);
          this.addParticipatedStates(MODEL_TYPE.ORGANIZATION, saveNewValues, participants, x);
        }
      }
    });
    this.event.contacts = contacts;
    this.event.organizations = organizations;
    this.event.participated = participants;
    this.event.tags = this.selectedTags;
    this.eventService.put(this.event, this.event.id).subscribe(x => this.dialogRef.close({save: true}));
  }

  addParticipatedStates(modelType: MODEL_TYPE, saveNewValues: boolean, participants: ParticipatedDto[], x: EventContactConnection) {
    const partExistend: ParticipatedDto = this.event.participated.find(z => z.objectId === x.objectId && z.modelType ===
      modelType);
    let newState = ParticipatedStatus.NOT_INVITED;
    let participatedState = false;
    if (saveNewValues) {
      newState = x.eventStatus;
      participatedState = x.participated;
    }
    if (partExistend == null) {
      participants.push(
        {
          objectId: x.objectId,
          hasParticipated: participatedState,
          eventStatus: newState,
          id: 0,
          modelType
        }
      );
    } else {
      partExistend.hasParticipated = participatedState;
      partExistend.eventStatus = newState;
      participants.push(partExistend);
    }
  }

  isSameDate(dateOld: string, dateNew: string): boolean {
    const dateOldObj = new Date(dateOld);
    const dateNewObj = new Date(dateNew);
    return dateOldObj.getFullYear() === dateNewObj.getFullYear() && dateOldObj.getMonth() === dateNewObj.getMonth() &&
      dateOldObj.getDate() === dateNewObj.getDate();
  }

  isSameTime(timeOld: string, timeNew: string): boolean {
    const timeOldObj = new Date(timeOld);
    let min = timeOldObj.getMinutes().toString();
    if (min.length === 1) {
      min = '0' + min;
    }
    let hours = timeOldObj.getHours().toString();
    if (hours.length === 1) {
      hours = '0' + hours;
    }
    const oldTime = hours + ':' + min;
    return oldTime === timeNew;
  }

  disableInvitation(): boolean {
    let state = true;
    this.selectedItems.forEach(x => {
      if (x.eventStatus === ParticipatedStatus.NOT_INVITED) {
        state = false;
      }
    });
    return state;
  }

  getEventState(state: ParticipatedStatus): string {
    if (state === ParticipatedStatus.NOT_INVITED) {
      return this.translate.instant('event.notInvited');
    } else if (state === ParticipatedStatus.INVITED) {
      return this.translate.instant('event.invited');
    } else if (state === ParticipatedStatus.AGREED) {
      return this.translate.instant('event.agreed');
    } else {
      return this.translate.instant('event.cancelled');
    }
  }

  close() {
    super.confirmDialog();
  }

  showParticipated(): boolean {
    return this.isDateOk();
  }
}
