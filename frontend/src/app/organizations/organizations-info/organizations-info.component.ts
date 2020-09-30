import { Component, OnInit, Inject } from '@angular/core';
import { ContactDto,
  ModificationEntryService, MODEL_TYPE, MODIFICATION, DATA_TYPE,
  ContactPossibilitiesEntryDto, OrganizationDto, ModificationEntryDto, TagDto,
  HistoryElementDto,
  HistoryElementType, EventService, ParticipatedDto} from '../../shared/api-generated/api-generated';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';

export class EventDtoCustomized {
  id: number;
  name: string;
  date: string;
  type: number;
  comment: string;
  participated: boolean;
}

export enum TYPE {
  EVENT = 4,
  PHONE_CALL = HistoryElementType.PHONE_CALL,
  NOTE = HistoryElementType.NOTE,
  MAIL = HistoryElementType.MAIL,
  VISIT = HistoryElementType.VISIT
}

@Component({
  selector: 'app-organizations-info',
  templateUrl: './organizations-info.component.html',
  styleUrls: ['./organizations-info.component.scss']
})

export class OrganizationsInfoComponent implements OnInit {
  organization: OrganizationDto;
  events: EventDtoCustomized[] = new Array<EventDtoCustomized>();
  contactPossibilitiesEntries: ContactPossibilitiesEntryDto[] = new Array<ContactPossibilitiesEntryDto>();
  organizationsForm: FormGroup;
  dataHistory: ModificationEntryDto[] = new Array<ModificationEntryDto>();
  employees: ContactDto[] = new Array<ContactDto>();
  displayedColumnsEmployees = ['vorname', 'name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];
  tags: TagDto[] = new Array<TagDto>();
  displayedColumnsHistory = ['icon', 'datum', 'name', 'kommentar'];
  displayedColumnsDataChangeHistory = ['datum', 'bearbeiter', 'feldname', 'alterWert', 'neuerWert'];
  history: HistoryElementDto[] = new Array<HistoryElementDto>();

  constructor(
    public dialogRef: MatDialogRef<OrganizationsInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: OrganizationDto,
    private fb: FormBuilder,
    private eventService: EventService,
    private modService: ModificationEntryService) {
      this.organization = data;
      this.tags = this.organization.tags;
    }

  ngOnInit(): void {
    if (this.organization.events != null && this.organization.events.length > 0) {
      this.organization.events.forEach(x => {
        this.events.push({
          id: x.id,
          name: x.name,
          date: this.getDate(x.date),
          type: TYPE.EVENT,
          participated: false,
          comment: ''
        });
        this.eventService.getById(x.id).subscribe(y => {
          this.updateParticipation(x.id, y.participated);
        });
      });
    }
    if (this.organization.history != null && this.organization.history.length > 0) {
      this.organization.history.forEach(x => {
        this.events.push({
          id: 0,
          date: this.getDate(x.date),
          name: x.name,
          type: x.type,
          participated: false,
          comment: x.comment
        });
      });
    }
    this.sortEvents();
    this.initForm();
    if (this.organization.employees != null) {
      this.organization.employees.forEach(x => this.employees.push(x));
    }
    if (this.organization.contact.contactEntries != null) {
      this.organization.contact.contactEntries.forEach(x => this.contactPossibilitiesEntries.push(x));
    }
    this.modService.getSortedListByTypeAndId(this.organization.id, MODEL_TYPE.ORGANIZATION).subscribe(x => {
      x.forEach(a => {
        this.dataHistory.push(a);
      });
      this.dataHistory.sort(this.getSortHistoryFunction);
    });
    this.organizationsForm.patchValue(this.organization);
  }

  sortEvents() {
    const sortedEvents: EventDtoCustomized[] = this.events.sort(this.getSortFunction);
    this.events = sortedEvents;
  }

  getSortFunction(a: EventDtoCustomized, b: EventDtoCustomized): number {
    return new Date(a.date).getTime() - new Date(b.date).getTime();
  }

  getSortHistoryFunction(a: ModificationEntryDto, b: ModificationEntryDto) {
    return new Date(b.dateTime).getTime() - new Date(a.dateTime).getTime();
  }

  updateParticipation(id: number, participated: ParticipatedDto[]) {
    const update: EventDtoCustomized = this.events.find(x => x.id === id);
    if (update != null) {
      update.participated = this.getParticipation(participated);
    }
  }

  getParticipation(participations: ParticipatedDto[]): boolean {
    let participatedReal = false;
    const part: ParticipatedDto = participations.find(x => x.objectId === this.organization.id && x.modelType === MODEL_TYPE.ORGANIZATION);
    if (part != null) {
      participatedReal = part.hasParticipated;
    }
    return participatedReal;
  }

  getDate(date: string): string {
    const dateUsed = new Date(date);
    return dateUsed.getFullYear().toString() + '-' + (+dateUsed.getMonth() + 1).toString() + '-' + dateUsed.getDate().toString();
  }

  initForm() {
    this.organizationsForm = this.fb.group({
      id: [''],
      name: [''],
      description: [''],
      events: [''],
      address: this.fb.group({
        id: [''],
        name: [''],
        description: [''],
        street: [''],
        streetNumber: [''],
        zipcode: [''],
        city: [''],
        country: ['']
       }),
      contact: this.fb.group({
        mail: [''],
        phoneNumber: [''],
        fax: [''],
        contactEntries: ['']
      }),
      employees: ['']
    });
  }

  closeDialog() {
    this.dialogRef.close();
  }

  eventParticipated(element: EventDtoCustomized): boolean {
    return element.type === TYPE.EVENT && element.participated;
  }

  eventNotParticipated(element: EventDtoCustomized): boolean {
    return element.type === TYPE.EVENT && !element.participated;
  }

  isLocalPhone(element: EventDtoCustomized): boolean {
    return element.type === TYPE.PHONE_CALL;
  }

  isNote(element: EventDtoCustomized): boolean {
    return element.type === TYPE.NOTE;
  }

  isMail(element: EventDtoCustomized): boolean {
    return element.type === TYPE.MAIL;
  }

  isVisit(element: EventDtoCustomized): boolean {
    return element.type === TYPE.VISIT;
  }
}
