import { Component, OnInit, ViewChild } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ContactDto,
  ContactPossibilitiesEntryDto,
  EventDto,
  ParticipatedDto,
  EventService } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactPossibilitiesComponent } from 'src/app/shared/contactPossibilities/contact-possibilities.component';
import { timeInterval } from 'rxjs/operators';
import { getLocaleDateFormat } from '@angular/common';
import { getType } from '@angular/flex-layout/extended/typings/style/style-transforms';

export class EventDtoCustomized {
  id: number;
  name: string;
  date: string;
  type: number;
  participated: boolean;
}

export enum TYPE {
  EVENT = 0,
  PHONE_CALL = 1,
  NOTE = 2
}

@Component({
  selector: 'app-contacts-detail',
  templateUrl: './contacts-detail.component.html',
  styleUrls: ['./contacts-detail.component.scss']
})

export class ContactsDetailComponent implements OnInit {
  @ViewChild(ContactPossibilitiesComponent, {static: true})
  contactPossibilitiesEntries: ContactPossibilitiesComponent;
  contactPossibilitiesEntriesFormGroup: FormGroup;
  contact: ContactDto;
  contactsForm: FormGroup;
  events: EventDtoCustomized[] = new Array<EventDtoCustomized>();
  displayedColumns = ['icon', 'datum', 'name'];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private service: ContactService,
    private eventService: EventService) { }

  ngOnInit(): void {
    this.contact = this.route.snapshot.data.contact;
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
    this.contactPossibilitiesEntriesFormGroup = this.contactPossibilitiesEntries.getFormGroup();
    this.contactPossibilitiesEntries.patchExistingValuesToForm(this.contact.contactPossibilities.contactEntries);
    this.initForm();
    this.contactsForm.patchValue(this.contact);
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
      id: ['', Validators.required],
      name: ['', Validators.required],
      preName: ['', Validators.required],
      address: this.fb.control(''),
      contactPossibilities: this.fb.group({
        // Validiert auf korrektes E-Mail-Format
        mail: ['', Validators.email],
        // Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
        phoneNumber: ['', Validators.pattern('^0[0-9\- ]*$')],
        fax: ['', Validators.pattern('^0[0-9\- ]*$')],
        contactEntries: this.contactPossibilitiesEntriesFormGroup
      })
    });
  }

  updateContact() {
    const idAddress = this.contact.address.id;
    const idContactPossibilities = this.contact.contactPossibilities.id;
    const idContact = this.contact.id;
    this.contact = this.contactsForm.value;
    this.contact.id = idContact;
    this.contact.address.id = idAddress;
    this.contact.contactPossibilities.id = idContactPossibilities;
    this.service.put(this.contact, this.contact.id).subscribe();
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
}
