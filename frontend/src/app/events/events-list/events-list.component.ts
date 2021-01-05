import { Component, OnInit, ViewChild, ModuleWithComponentFactories } from '@angular/core';
import { Observable } from 'rxjs';
import { EventService, ParticipatedDto, ContactDto, TagDto, MailService, ParticipatedStatus, OrganizationDto } from '../../shared/api-generated/api-generated';
import { EventDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { EventsAddComponent } from '../events-add/events-add.component';
import { EventsDetailComponent } from '../events-detail/events-detail.component';
import { MatSort } from '@angular/material/sort';
import { EventsInfoComponent } from '../events-info/events-info.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { JwtService } from 'src/app/shared/jwt.service';
import { MatTableDataSource } from '@angular/material/table';
import { TagsFilterComponent } from 'src/app/shared/tags-filter/tags-filter.component';
import { BlockScrollStrategy, ScrollStrategyOptions } from '@angular/cdk/overlay';
import { DeletionConfirmationDialogComponent } from '../deletion-confirmation-dialog/deletion-confirmation-dialog.component';
import { ConfirmDialogComponent, ConfirmDialogModel } from 'src/app/shared/form/confirmdialog/confirmdialog.component';
import { EventsInvitationComponent } from '../events-invitation/events-invitation.component';

export class EventDtoGroup implements EventDto {
  id: number;
  date: string;
  starttime: string;
  name?: string;
  tags: TagDto[];
  endtime: string;
  location: string;
  contacts?: ContactDto[];
  participated?: ParticipatedDto[];
  weekNumber: number;
  isGroupBy: boolean;
  organizations: OrganizationDto[];
}

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss']
})

export class EventsListComponent implements OnInit {
  @ViewChild(TagsFilterComponent, { static: true })
  tagsFilter: TagsFilterComponent;
  @ViewChild(MatSort) sort: MatSort;
  events: Observable<EventDto[]>;
  allEvents: EventDtoGroup[] = new Array<EventDtoGroup>();
  displayedColumns = ['bezeichnung', 'datum', 'uhrzeit', 'ort', 'action'];
  public dataSource: EventDtoGroup[] = new Array<EventDtoGroup>();
  checkboxSelected = true;
  weekNumber = 0;
  isAdminUserLoggedIn = false;
  length = 0;
  dataSourceFiltered = new MatTableDataSource<EventDtoGroup>();
  permissionAdd = false;
  permissionModify = false;
  permissionDelete = false;

  constructor(
    private service: EventService,
    private dialog: MatDialog,
    private jwt: JwtService,
    private mailService: MailService) {
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSourceFiltered.filter = filterValue.trim().toLowerCase();
    this.dataSourceFiltered.filterPredicate = ((data, filter) => {
      if (data.name.trim().toLowerCase().includes(filter) || data.date.trim().toLowerCase().includes(filter) ||
        data.starttime.trim().toLowerCase().includes(filter)) {
        return true;
      } else {
        return false;
      }
    });
  }

  ngOnInit() {
    this.tagsFilter.setRefreshTableFunction(() => this.applyTagFilter());
    this.init();
    this.isAdminUserLoggedIn = this.jwt.getUserId() === 1;
    this.permissionAdd = this.jwt.hasPermission('Anlegen einer Veranstaltung');
    this.permissionModify = this.jwt.hasPermission('Einsehen und Bearbeiten einer Veranstaltung');
    this.permissionDelete = this.jwt.hasPermission('Löschen einer Veranstaltung');
  }

  applyTagFilter() {
    this.dataSourceFiltered = new MatTableDataSource<EventDtoGroup>();
    this.allEvents.forEach(x => {
      if (this.tagsFilter.areAllTagsIncluded(x.tags)) {
        this.dataSourceFiltered.data.push(x);
      }
    });
  }

  private init() {
    this.events = this.service.get();
    this.events.subscribe(y => {
      const xSort: EventDto[] = y.sort(this.funtionGetSortedData);
      this.filterValues(xSort);
      this.length = y.length;
      this.applyTagFilter();
    });
  }

  funtionGetSortedData(a: EventDto, b: EventDto): number {
    const dateA = new Date(a.date);
    const timeA = new Date(a.starttime);
    dateA.setHours(timeA.getHours());
    dateA.setMinutes(timeA.getMinutes());
    const dateB = new Date(b.date);
    const timeB = new Date(b.starttime);
    dateB.setHours(timeB.getHours());
    dateB.setMinutes(timeB.getMinutes());
    return (dateA.getTime() - dateB.getTime());
  }

  addEvent() {
    const dialogRef = this.dialog.open(EventsAddComponent, { disableClose: true });
    dialogRef.afterClosed().subscribe(x => {
      this.init();
      let lastEvent: EventDto = null;
      if (x.save) {
        const newEvent = this.events.subscribe(b => {
          const sortedList = b.sort(this.getSortFunctionForNewestEvent);
          lastEvent = sortedList[sortedList.length - 1];
          if (lastEvent.contacts.length > 0 || lastEvent.organizations.length > 0) {
            const data = new ConfirmDialogModel('event.sendInvitation', 'event.sendInvitation');
            const diagSendInvitation = this.dialog.open(ConfirmDialogComponent, {data});
            diagSendInvitation.afterClosed().subscribe(a => {
              if (a) {
                this.callInvitation(lastEvent);
              }
            });
          }
        });
      }
    });
  }

  callInvitation(event: EventDto) {
    const dialogRef = this.dialog.open(EventsInvitationComponent, { data: event, disableClose: true,
       minWidth: '450px', minHeight: '400px' });
    dialogRef.afterClosed().subscribe(x => {
      if (x.send && x.text != null) {
        const listOfContactIds: number[] = new Array<number>();
        const listOfOrgaIds: number[] = new Array<number>();
        event.contacts.forEach(a => listOfContactIds.push(a.id));
        event.organizations.forEach(a => listOfOrgaIds.push(a.id));
        this.mailService.sendInvitationMails(listOfContactIds, listOfOrgaIds, x.text, event.id).subscribe();
        event.participated.forEach(p => p.eventStatus = ParticipatedStatus.INVITED);
        this.service.put(event, event.id).subscribe();
      }
    });
  }

  getSortFunctionForNewestEvent(a: EventDto, b: EventDto): number {
    if (a.id === b.id) {
      return 0;
    } else if (a.id > b.id) {
      return 1;
    } else {
      return -1;
    }
  }

  callEdit(id: number) {
    this.service.getById(id).subscribe(x => {
      const dialogRef = this.dialog.open(EventsDetailComponent, { data: x, disableClose: true, width: '680px', height: '700px' });
      dialogRef.afterClosed().subscribe(y => this.init());
    });
  }

  isToday(element: EventDtoGroup): boolean {
    const date: Date = new Date(element.date);
    const now: Date = new Date(Date.now());
    if (date.getDate() === now.getDate() && date.getMonth() === now.getMonth() && date.getFullYear() === now.getFullYear()) {
      return true;
    } else {
      return false;
    }
  }

  deleteEvent(id: number) {
    const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
      data: 'event.event',
      disableClose: true
    });

    deleteDialogRef.afterClosed().subscribe((deleteResult) => {
      if (deleteResult.delete) {
        const eventToDelete = this.allEvents.find(a => a.id === id);
        if (eventToDelete != null && (eventToDelete.contacts.length > 0 || eventToDelete.organizations.length > 0)) {
          const sendMailResult = this.dialog.open(DeletionConfirmationDialogComponent);
          sendMailResult.afterClosed().subscribe(confirm => {
            this.service.delete(id, confirm.delete).subscribe(x => this.init());
          });
        } else {
          this.service.delete(id, false).subscribe(x => this.init());
        }
      }
    });
  }

  toggleSelection() {
    this.checkboxSelected = !this.checkboxSelected;
    this.init();
  }

  filterValues(events: EventDto[]) {
    this.dataSource = [];
    this.allEvents = new Array<EventDtoGroup>();
    this.weekNumber = 0;
    let eventsFiltered: EventDto[] = new Array<EventDto>();
    if (this.checkboxSelected) {
      eventsFiltered = events.filter(x => this.getDate(x.date, x.starttime) >= new Date(Date.now()).getTime());
    } else {
      eventsFiltered = events;
    }
    eventsFiltered.forEach(x => {
      const week = this.getWeekNumber(new Date(x.date));
      if (week !== this.weekNumber) {
        this.weekNumber = week;
        this.dataSource.push({
          date: x.date,
          endtime: x.endtime,
          id: x.id,
          tags: x.tags,
          location: x.location,
          starttime: x.starttime,
          contacts: x.contacts,
          name: x.name,
          participated: x.participated,
          weekNumber: week,
          isGroupBy: true,
          organizations: x.organizations
        });
        this.allEvents.push({
          date: x.date,
          endtime: x.endtime,
          id: x.id,
          tags: x.tags,
          location: x.location,
          starttime: x.starttime,
          contacts: x.contacts,
          name: x.name,
          participated: x.participated,
          weekNumber: week,
          isGroupBy: true,
          organizations: x.organizations
        });
      }
      this.allEvents.push({
        date: x.date,
        tags: x.tags,
        endtime: x.endtime,
        id: x.id,
        starttime: x.starttime,
        contacts: x.contacts,
        location: x.location,
        name: x.name,
        participated: x.participated,
        weekNumber: 0,
        isGroupBy: false,
        organizations: x.organizations
      });
      this.dataSource.push({
        date: x.date,
        tags: x.tags,
        endtime: x.endtime,
        id: x.id,
        starttime: x.starttime,
        contacts: x.contacts,
        location: x.location,
        name: x.name,
        participated: x.participated,
        weekNumber: 0,
        isGroupBy: false,
        organizations: x.organizations
      });
    });
    this.tagsFilter.updateTagsInAutofill(this.allEvents);
    this.dataSourceFiltered.data = this.dataSource;
  }

  getDate(date: string, time: string): number {
    const dateFinished: Date = new Date(date);
    const timeFinished: Date = new Date(time);
    dateFinished.setHours(timeFinished.getHours());
    dateFinished.setMinutes(timeFinished.getMinutes());
    dateFinished.setSeconds(timeFinished.getSeconds());
    return dateFinished.getTime();
  }

  openInfo(id: number) {
    this.service.getById(id).subscribe(x => {
      this.dialog.open(EventsInfoComponent, { data: x, disableClose: true });
    });
  }

  isGroup(index: number, item: EventDtoGroup): boolean {
    return item.isGroupBy;
  }

  addDummyEvent() {
    const date = new Date(Date.now());
    date.setDate(date.getDate() + 1);
    this.service.post({
      name: 'Veranstaltung' + this.length,
      endtime: '21:' + this.length % 59,
      date: date.getFullYear().toString() + '-' + (date.getMonth() + 1) + '-' + date.getDate().toString(),
      starttime: '20:' + this.length % 59,
      contacts: new Array<number>(),
      organizations: new Array<number>()
    }).subscribe(x => this.init());
  }

  getWeekNumber(date: Date): number {
    // In JavaScript the Sunday has value 0 as return value of getDay() function.
    // So we have to order them first ascending from Monday to Sunday
    // Monday: ((1+6) % 7) = 0
    // Tuesday ((2+6) % 7) = 1
    // Wednesday: ((3+6) % 7) = 2
    // Thursday: ((4+6) % 7) = 3
    // Friday: ((5+6) % 7) = 4
    // Saturday: ((6+6) % 7) = 5
    // Sunday: ((0+6) % 7) = 6
    // (3 - result) is necessary to get the Thursday of the current week.
    // If we want to have Tuesday it would be (1-result)
    const currentThursday = new Date(date.getTime() + (3 - ((date.getDay() + 6) % 7)) * 86400000);

    // At the beginnig or end of a year the thursday could be in another year.
    const yearOfThursday = currentThursday.getFullYear();

    // Get first Thursday of the year
    const firstThursday = new Date(new Date(yearOfThursday, 0, 4).getTime() +
      (3 - ((new Date(yearOfThursday, 0, 4).getDay() + 6) % 7)) * 86400000);

    // +1 we start with week number 1
    // +0.5 an easy and dirty way to round result (in combinationen with Math.floor)
    const weekNumber = Math.floor(1 + 0.5 + (currentThursday.getTime() - firstThursday.getTime()) / 86400000 / 7);

    return weekNumber;
  }
}
