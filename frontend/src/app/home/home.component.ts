import { Component, OnInit } from '@angular/core';
import { ContactService, OrganizationService, ContactDto, OrganizationDto, EventDto, EventService,
  ModificationEntryService, MODEL_TYPE, MODIFICATION, ModificationEntryDto, AddressDto,
  ContactPossibilitiesDto,
  ParticipatedDto,
  HistoryElementDto} from '../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsInfoComponent } from '../contacts/contacts-info/contacts-info.component';
import { EventsInfoComponent } from '../events/events-info/events-info.component';
import { OrganizationsInfoComponent } from '../organizations/organizations-info/organizations-info.component';

export class ContactExtended implements ContactDto {
  id: number;
  description?: string;
  organizations?: OrganizationDto[];
  events?: EventDto[];
  history?: HistoryElementDto[];
  name?: string;
  preName?: string;
  address?: AddressDto;
  contactPossibilities?: ContactPossibilitiesDto;
  userName: string;
  created: boolean;
}

export class OrganizationExtended implements OrganizationDto {
  id: number;
  employees?: ContactDto[];
  name?: string;
  description?: string;
  address?: AddressDto;
  contact?: ContactPossibilitiesDto;
  userName: string;
  created: boolean;
}

export class EventExtended implements EventDto {
  id: number;
  contacts?: ContactDto[];
  participated?: ParticipatedDto[];
  date: string;
  time: string;
  name?: string;
  duration: number;
  userName: string;
  created: boolean;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  public contacts: ContactExtended[] = new Array<ContactExtended>();
  public organizations: OrganizationExtended[] = new Array<OrganizationExtended>();
  public events: EventExtended[] = new Array<EventExtended>();

  constructor(
    private contactsService: ContactService,
    private organizationService: OrganizationService,
    private eventsService: EventService,
    private modificationEntryService: ModificationEntryService,
    private dialog: MatDialog) { }

  ngOnInit() {
    this.modificationEntryService.getSortedListByType(MODEL_TYPE.CONTACT).subscribe(x => {
      const index = x.length - 1;
      if (index > -1) {
        this.addContact(x[0]);
      }
      if (index > 0) {
        this.addContact(x[1]);
      }
    });
    this.modificationEntryService.getSortedListByType(MODEL_TYPE.ORGANIZATION).subscribe(x => {
      const index = x.length - 1;
      if (index > -1) {
        this.addOrganization(x[0]);
      }
      if (index > 0) {
        this.addOrganization(x[1]);
      }});
    this.modificationEntryService.getSortedListByType(MODEL_TYPE.EVENT).subscribe(x => {
      const index = x.length - 1;
      if (index > -1) {
        this.addEvent(x[0]);
      }
      if (index > 0) {
        this.addEvent(x[1]);
    }});
  }

  addEvent(entry: ModificationEntryDto) {
    this.eventsService.getById(entry.dataModelId).subscribe(y => {
          this.events.push({
            date: y.date,
            time: y.time,
            duration: y.duration,
            name: y.name,
            id: y.id,
            contacts: y.contacts,
            participated: y.participated,
            userName: entry.userName,
            created: entry.modificationType === MODIFICATION.CREATED
          });
        }
      );
  }

  addOrganization(entry: ModificationEntryDto) {
    this.organizationService.getById(entry.dataModelId).subscribe(y => {
          this.organizations.push({
            address: y.address,
            contact: y.contact,
            description: y.description,
            name: y.name,
            id: y.id,
            employees: y.employees,
            userName: entry.userName,
            created: entry.modificationType === MODIFICATION.CREATED
          });
        }
      );
  }

  addContact(entry: ModificationEntryDto) {
    this.contactsService.getById(entry.dataModelId).subscribe(y =>
      this.contacts.push({
        address: y.address,
        contactPossibilities: y.contactPossibilities,
        name: y.name,
        preName: y.preName,
        id: y.id,
        description: y.description,
        events: y.events,
        history: y.history,
        organizations: y.organizations,
        userName: entry.userName,
        created: entry.modificationType === MODIFICATION.CREATED
    }));
  }

  openContactDetails(contact: ContactDto) {
    this.dialog.open(ContactsInfoComponent, {data: contact});
  }

  openOrganizationDetails(organization: OrganizationDto) {
    this.dialog.open(OrganizationsInfoComponent, {data: organization});
  }

  openEventDetails(event: EventDto) {
    this.dialog.open(EventsInfoComponent, {data: event});
  }
}
