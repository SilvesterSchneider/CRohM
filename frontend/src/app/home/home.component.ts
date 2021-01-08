import { Component, OnInit, OnDestroy } from '@angular/core';
import { ContactService, OrganizationService, ContactDto, OrganizationDto, EventDto, EventService,
  ModificationEntryService, MODEL_TYPE, MODIFICATION, ModificationEntryDto, AddressDto,
  ContactPossibilitiesDto,
  ParticipatedDto,
  HistoryElementDto,
  UserLoginService, GenderTypes, DataProtectionService, UsersService} from '../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsInfoComponent } from '../contacts/contacts-info/contacts-info.component';
import { EventsInfoComponent } from '../events/events-info/events-info.component';
import { OrganizationsInfoComponent } from '../organizations/organizations-info/organizations-info.component';
import { JwtService } from '../shared/jwt.service';
import { ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs/operators';
import { DpDisclaimerDialogComponent } from '../shared/data-protection/dp-disclaimer-dialog/dp-disclaimer-dialog.component';

export class ContactExtended implements ContactDto {
  id: number;
  description?: string;
  organizations?: OrganizationDto[];
  events?: EventDto[];
  gender: GenderTypes;
  contactPartner: string;
  history?: HistoryElementDto[];
  name?: string;
  preName?: string;
  address?: AddressDto;
  contactPossibilities?: ContactPossibilitiesDto;
  userName: string;
  created: boolean;
  isApproved: boolean;
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
  starttime: string;
  name?: string;
  endtime: string;
  userName: string;
  created: boolean;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
/// <summary>
/// RAM: 90%
/// </summary>
export class HomeComponent implements OnInit {
  public contacts: ContactExtended[] = new Array<ContactExtended>();
  public organizations: OrganizationExtended[] = new Array<OrganizationExtended>();
  public events: EventExtended[] = new Array<EventExtended>();
  AMOUNT_OF_DATASETS = 2;
  public lastLogin: Date;
  loggedInUser: string;
  permissionToAddRoles = false;

  private weekDays: string[] = ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'];

  constructor(
    private readonly contactsService: ContactService,
    private readonly organizationService: OrganizationService,
    private readonly eventsService: EventService,
    private readonly modificationEntryService: ModificationEntryService,
    private readonly dialog: MatDialog,
    private readonly userLoginService: UserLoginService,
    private readonly jwt: JwtService,
    private readonly route: ActivatedRoute,
    private readonly dataProtectionService: DataProtectionService,
    private readonly userService: UsersService) { }

  public ngOnInit(): void {
    this.permissionToAddRoles = this.jwt.hasPermission('Zuweisung einer neuen Rolle zu einem Benutzer');
    this.checkIfComingFromLogin().then(isLogin => {
      if (isLogin){
        this.dataProtectionService.isThereAnyDataProtectionOfficerInTheSystem()
          .subscribe(x => {
            if (!x) {
              this.dialog.open(DpDisclaimerDialogComponent, { data: this.permissionToAddRoles });
            }
          })
       }
    });

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
    const userId = this.jwt.getUserId();
    this.userLoginService.getTheLastLoginTimeOfUserById(userId)
      .subscribe(lastLogin => this.lastLogin = new Date(lastLogin));
    this.userService.get().subscribe(x => {
      x.forEach(y => {
        if (y.id === userId) {
          this.loggedInUser = y.firstName + ' ' + y.lastName;
          return;
        }
      });
    });
  }

  public openContactDetails(contactId: number) {
    this.contactsService.getById(contactId).subscribe(x => this.dialog.open(ContactsInfoComponent, {data: x}));
  }

  public openOrganizationDetails(organizationId: number) {
    this.organizationService.getById(organizationId).subscribe(x => this.dialog.open(OrganizationsInfoComponent, {data: x}));
  }

  public openEventDetails(eventId: number) {
    this.eventsService.getById(eventId).subscribe(x => this.dialog.open(EventsInfoComponent, {data: x}));
  }

  private checkIfComingFromLogin(): Promise<boolean>{
    return new Promise(resolve => {
      this.route.queryParams.pipe(
        filter(params => params.from))
      .subscribe(params => resolve(params.from === 'login'));
    });
  }

  private addEvent(entry: ModificationEntryDto) {
    this.eventsService.getById(entry.dataModelId).subscribe(event => {
          this.events.push({
            date: event.date,
            starttime: event.starttime,
            endtime: event.endtime,
            name: event.name,
            id: event.id,
            contacts: event.contacts,
            participated: event.participated,
            userName: entry.user?.userName,
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
        gender: y.gender,
        contactPartner: y.contactPartner,
        preName: y.preName,
        id: y.id,
        description: y.description,
        events: y.events,
        history: y.history,
        organizations: y.organizations,
        userName: entry.user?.userName,
        created: entry.modificationType === MODIFICATION.CREATED,
        isApproved: y.isApproved
    }));
  }
}
