import { Component, OnInit } from '@angular/core';
import { ContactService, OrganizationService, ContactDto, OrganizationDto } from '../shared/api-generated/api-generated';

export class EventDto {
  id: number;
    date: string;
    time: string;
    name: string;
    duration: number;
    contacts: ContactDto[];
}

function getDummyEvents(): EventDto[] {
  let events: EventDto[] = new Array<EventDto>();
  events.push({
    date: Date.now().toString(),
    id: 1,
    duration: 2,
    name: 'testEvent1',
    time: Date.now().toString(),
    contacts: []
  });
  events.push({
    date: Date.now().toString(),
    id: 2,
    duration: 4.5,
    name: 'testEvent2',
    time: Date.now().toString(),
    contacts: []
  });
  return events;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  public contacts: ContactDto[] = new Array<ContactDto>();
  public organizations: OrganizationDto[] = new Array<OrganizationDto>();
  public events: EventDto[] = new Array<EventDto>();

  constructor(private contactsService: ContactService,
    private organizationService: OrganizationService) { }

  ngOnInit() {
    this.contactsService.getAll().subscribe(x =>
      {
        let index = x.length - 1;
        if (index > -1) {
          this.contacts.push(x[index]);
        }
        if (index > 0) {
          this.contacts.push(x[index - 1]);
        }
      });
    this.organizationService.get().subscribe(x =>
      {
        let index = x.length - 1;
        if (index > -1) {
          this.organizations.push(x[index]);
        }
        if (index > 0) {
          this.organizations.push(x[index - 1]);
        }
      });
    this.events = getDummyEvents();
  }
}
