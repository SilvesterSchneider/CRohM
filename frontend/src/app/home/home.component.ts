import { Component, OnInit } from '@angular/core';
import {
    ContactService, OrganizationService, ContactDto, OrganizationDto, EventDto, EventService,
    ModificationEntryService, MODEL_TYPE, MODIFICATION, ModificationEntryDto, AddressDto,
    ContactPossibilitiesDto,
    ParticipatedDto,
    HistoryElementDto,
    UserLoginService
} from '../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsInfoComponent } from '../contacts/contacts-info/contacts-info.component';
import { EventsInfoComponent } from '../events/events-info/events-info.component';
import { OrganizationsInfoComponent } from '../organizations/organizations-info/organizations-info.component';
import { JwtService } from '../shared/jwt.service';
import { ActivatedRoute } from '@angular/router';

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
    AMOUNT_OF_DATASETS = 2;
    public lastLogin: string;
    private weekDays: string[] = ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'];

    constructor(
        private readonly contactsService: ContactService,
        private readonly organizationService: OrganizationService,
        private readonly eventsService: EventService,
        private readonly modificationEntryService: ModificationEntryService,
        private readonly dialog: MatDialog,
        private readonly userLoginService: UserLoginService,
        private readonly jwt: JwtService,
        private readonly route: ActivatedRoute) { }

    public ngOnInit() {
        this.modificationEntryService.getSortedListByType(MODEL_TYPE.CONTACT).subscribe(x => {
            let modelId = -1;
            let idx = 0;
            x.forEach(a => {
                if (a.dataModelId !== modelId && idx < this.AMOUNT_OF_DATASETS) {
                    modelId = a.dataModelId;
                    idx++;
                    this.addContact(a);
                }
            });
        });
        this.modificationEntryService.getSortedListByType(MODEL_TYPE.ORGANIZATION).subscribe(x => {
            let modelId = -1;
            let idx = 0;
            x.forEach(a => {
                if (a.dataModelId !== modelId && idx < this.AMOUNT_OF_DATASETS) {
                    modelId = a.dataModelId;
                    idx++;
                    this.addOrganization(a);
                }
            });
        });
        this.modificationEntryService.getSortedListByType(MODEL_TYPE.EVENT).subscribe(x => {
            let modelId = -1;
            let idx = 0;
            x.forEach(a => {
                if (a.dataModelId !== modelId && idx < this.AMOUNT_OF_DATASETS) {
                    modelId = a.dataModelId;
                    idx++;
                    this.addEvent(a);
                }
            });
        });
        this.userLoginService.getTheLastLoginTimeOfUserById(this.jwt.getUserId())
            .subscribe(lastLogin => this.lastLogin = this.getDate(lastLogin));
    }

    public openContactDetails(contactId: number) {
        this.contactsService.getById(contactId).subscribe(x => this.dialog.open(ContactsInfoComponent, { data: x }));
    }

    public openOrganizationDetails(organizationId: number) {
        this.organizationService.getById(organizationId).subscribe(x => this.dialog.open(OrganizationsInfoComponent, { data: x }));
    }

    public openEventDetails(eventId: number) {
        this.eventsService.getById(eventId).subscribe(x => this.dialog.open(EventsInfoComponent, { data: x }));
    }

    private getDate(lastLogin: string): string {
        const date = new Date(lastLogin);
        return this.weekDays[date.getDay()] + ' den ' + date.getDate().toString() + '-' + (date.getMonth() + 1).toString()
            + '-' + date.getFullYear().toString() + ' um ' + date.getHours().toString() + ':' + date.getMinutes().toString()
            + '.' + date.getSeconds().toString() + ' Uhr';
    }

    private addEvent(entry: ModificationEntryDto) {
        this.eventsService.getById(entry.dataModelId).subscribe(event => {
            this.events.push({
                date: event.date,
                time: event.time,
                duration: event.duration,
                name: event.name,
                id: event.id,
                contacts: event.contacts,
                participated: event.participated,
                userName: entry.user.userName,
                created: entry.modificationType === MODIFICATION.CREATED
            });
        }
        );
    }

    private addOrganization(entry: ModificationEntryDto) {
        this.organizationService.getById(entry.dataModelId).subscribe(y => {
            this.organizations.push({
                address: y.address,
                contact: y.contact,
                description: y.description,
                name: y.name,
                id: y.id,
                employees: y.employees,
                userName: entry.user?.userName,
                created: entry.modificationType === MODIFICATION.CREATED
            });
        }
        );
    }

    private addContact(entry: ModificationEntryDto) {
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
                userName: entry.user?.userName,
                created: entry.modificationType === MODIFICATION.CREATED
            }));
    }
}