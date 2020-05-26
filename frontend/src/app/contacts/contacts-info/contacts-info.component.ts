import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ContactDto,
  ContactPossibilitiesEntryDto,
  EventDto,
  ParticipatedDto,
  EventService,
  HistoryElementType,
  OrganizationDto} from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { timeInterval } from 'rxjs/operators';
import { getLocaleDateFormat } from '@angular/common';
import { getType } from '@angular/flex-layout/extended/typings/style/style-transforms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export class EventDtoCustomized {
  id: number;
  name: string;
  date: string;
  type: number;
  participated: boolean;
}

export enum TYPE {
  EVENT = 3,
  PHONE_CALL = HistoryElementType.PHONE_CALL,
  NOTE = HistoryElementType.NOTE,
  MAIL = HistoryElementType.MAIL
}

@Component({
  selector: 'app-contacts-info',
  templateUrl: './contacts-info.component.html',
  styleUrls: ['./contacts-info.component.scss']
})

export class ContactsInfoComponent implements OnInit {
  contact: ContactDto;
  organizations: OrganizationDto[];
  contactPossibilitiesEntries: ContactPossibilitiesEntryDto[];
  contactsForm: FormGroup;
  events: EventDtoCustomized[] = new Array<EventDtoCustomized>();
  displayedColumns = ['icon', 'datum', 'name'];
  displayedColumnsOrganizations = ['name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];

  constructor(
    public dialogRef: MatDialogRef<ContactsInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ContactDto,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private service: ContactService,
    private eventService: EventService) {
      this.contact = data;
    }

  ngOnInit(): void {
    this.contact.events.forEach(x => {
      this.events.push({
        id: x.id,
        name: x.name,
        date: this.getDate(x.date),
        type: TYPE.EVENT,
        participated: false
      });
      this.eventService.getById(x.id).subscribe(y => {
        this.updateParticipation(x.id, y.participated);
      });
    });
    this.contact.history.forEach(x => {
      this.events.push({
        id: 0,
        date: this.getDate(x.date),
        name: x.name,
        type: x.type,
        participated: false
      });
    });
    this.sortEvents();
    this.initForm();
    this.organizations = this.contact.organizations;
    this.contactPossibilitiesEntries = this.contact.contactPossibilities.contactEntries;
    this.contactsForm.patchValue(this.contact);
  }

  sortEvents() {
    const sortedEvents: EventDtoCustomized[] = this.events.sort(this.getSortFunction);
    this.events = sortedEvents;
  }

  getSortFunction(a: EventDtoCustomized, b: EventDtoCustomized): number {
    return new Date(a.date).getTime() - new Date(b.date).getTime();
  }

  updateParticipation(id: number, participated: ParticipatedDto[]) {
    const update: EventDtoCustomized = this.events.find(x => x.id === id);
    if (update != null) {
      update.participated = this.getParticipation(participated);
    }
  }

  getParticipation(participations: ParticipatedDto[]): boolean {
    let participatedReal = false;
    const part: ParticipatedDto = participations.find(x => x.contactId === this.contact.id);
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
    this.contactsForm = this.fb.group({
      id: [''],
      name: [''],
      preName: [''],
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
      contactPossibilities: this.fb.group({
        // Validiert auf korrektes E-Mail-Format
        mail: [''],
        // Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
        phoneNumber: [''],
        fax: ['']
      })
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
}