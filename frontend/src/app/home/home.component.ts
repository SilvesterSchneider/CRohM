import { Component, OnInit } from '@angular/core';
import { ContactService, OrganizationService, ContactDto, OrganizationDto, EventDto, EventService,
  ModificationEntryService, MODEL_TYPE, MODIFICATION, ModificationEntryDto, AddressDto,
  ContactPossibilitiesDto } from '../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsInfoComponent } from '../contacts/contacts-info/contacts-info.component';
import { EventsInfoComponent } from '../events/events-info/events-info.component';

export class ContactExtended {
  name?: string;
  preName?: string;
  address?: AddressDto;
  contactPossibilities?: ContactPossibilitiesDto;
  userName: string;
  created: boolean;
}

export class OrganizationExtended {
  name?: string;
  description?: string;
  address?: AddressDto;
  contact?: ContactPossibilitiesDto;
  userName: string;
  created: boolean;
}

export class EventExtended {
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
        userName: entry.userName,
        created: entry.modificationType === MODIFICATION.CREATED
    }));
  }

  openContactDetails(contact: ContactDto) {
    this.dialog.open(ContactsInfoComponent, {data: contact});
  }

  openOrganizationDetails(organization: OrganizationDto) {
    // TO DO: open dialog of organizations detail!
  }

  openEventDetails(event: EventDto) {
    this.dialog.open(EventsInfoComponent, {data: event});
  }
}
