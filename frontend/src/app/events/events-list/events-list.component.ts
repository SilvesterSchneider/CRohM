import { Component, OnInit, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { EventService, ParticipatedDto, ContactDto } from '../../shared/api-generated/api-generated';
import { EventDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { EventsAddComponent } from '../events-add/events-add.component';
import { EventsDetailComponent } from '../events-detail/events-detail.component';
import { MatSort } from '@angular/material/sort';
import { EventsInfoComponent } from '../events-info/events-info.component';

export class EventDtoGroup implements EventDto {
  id: number;
  date: string;
  time: string;
  name?: string;
  duration: number;
  contacts?: ContactDto[];
  participated?: ParticipatedDto[];
  weekNumber: number;
  isGroupBy: boolean;
}

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss']
})

export class EventsListComponent implements OnInit {
  @ViewChild(MatSort) sort: MatSort;
  events: Observable<EventDto[]>;
  displayedColumns = ['bezeichnung', 'datum', 'uhrzeit', 'action'];
  public dataSource: EventDtoGroup[] = new Array<EventDtoGroup>();
  checkboxSelected = true;
  weekNumber = 0;

  constructor(
    private service: EventService,
    private dialog: MatDialog) {
   }

  ngOnInit() {
    this.init();
  }

  private init() {
    this.events = this.service.get();
    this.events.subscribe(y => {
      const xSort: EventDto[] = y.sort(this.funtionGetSortedData);
      this.filterValues(xSort);
    });
  }

  funtionGetSortedData(a: EventDto, b: EventDto): number {
    const dateA = new Date(a.date);
    const timeA = new Date(a.time);
    dateA.setHours(timeA.getHours());
    dateA.setMinutes(timeA.getMinutes());
    const dateB = new Date(b.date);
    const timeB = new Date(b.time);
    dateB.setHours(timeB.getHours());
    dateB.setMinutes(timeB.getMinutes());
    return (dateA.getTime() - dateB.getTime());
  }

  addContact() {
    const dialogRef = this.dialog.open(EventsAddComponent);
    dialogRef.afterClosed().subscribe(x => this.init());
  }

  callEdit(id: number) {
    this.service.getById(id).subscribe(x => {
      const dialogRef = this.dialog.open(EventsDetailComponent, { data: x });
      dialogRef.afterClosed().subscribe(y => this.init());
    });
  }

  isToday(element: EventDtoGroup): boolean {
    const date: Date = new Date(element.date);
    if (date.getDate() === new Date(Date.now()).getDate()) {
      return true;
    } else {
      return false;
    }
  }

  deleteEvent(id: number) {
    this.service.delete(id).subscribe(x => this.init());
  }

  toggleSelection() {
    this.checkboxSelected = !this.checkboxSelected;
    this.init();
  }

  filterValues(events: EventDto[]) {
    this.dataSource = [];
    this.weekNumber = 0;
    let eventsFiltered: EventDto[] = new Array<EventDto>();
    if (this.checkboxSelected) {
      eventsFiltered = events.filter(x => this.getDate(x.date, x.time) >= new Date(Date.now()).getTime());
    } else {
      eventsFiltered = events;
    }
    eventsFiltered.forEach(x => {
        const week = this.getWeekNumber(new Date(x.date));
        if (week > this.weekNumber) {
          this.weekNumber = week;
          this.dataSource.push({
            date: x.date,
            duration: x.duration,
            id: x.id,
            time: x.time,
            contacts: x.contacts,
            name: x.name,
            participated: x.participated,
            weekNumber: week,
            isGroupBy: true
          });
        }
        this.dataSource.push({
          date: x.date,
          duration: x.duration,
          id: x.id,
          time: x.time,
          contacts: x.contacts,
          name: x.name,
          participated: x.participated,
          weekNumber: 0,
          isGroupBy: false
        });
    });
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
      this.dialog.open(EventsInfoComponent, { data: x });
    });
  }

  isGroup(index: number, item: EventDtoGroup): boolean {
    return item.isGroupBy;
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
