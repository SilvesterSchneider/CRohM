import { Component, OnInit,
  ViewChild,
  TemplateRef} from '@angular/core';

import {
  isSameDay,
  isSameMonth,
} from 'date-fns';
import { Subject } from 'rxjs';
import {
  CalendarEvent,
  CalendarEventAction,
  CalendarMonthViewComponent,
  CalendarView,
} from 'angular-calendar';
import { EventDto, EventService } from '../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { EventColor } from 'calendar-utils';
import { EventsDetailComponent } from '../events/events-detail/events-detail.component';
import { EventsAddComponent } from '../events/events-add/events-add.component';
import { JwtService } from '../shared/jwt.service';
import { min } from 'rxjs/operators';

const colors: any = {
  cyan: {
    primary: '#00FFFF',
    secondary: '#FAE3E3',
  },
  blue: {
    primary: '#1e90ff',
    secondary: '#D1E8FF',
  },
  yellow: {
    primary: '#e3bc08',
    secondary: '#FDF1BA',
  },
};

class CalendarEventExtended implements CalendarEvent {
  start: Date;
  end?: Date;
  title: string;
  color?: EventColor;
  actions?: CalendarEventAction[];
  allDay?: boolean;
  cssClass?: string;
  resizable?: { beforeStart?: boolean; afterEnd?: boolean; };
  draggable?: boolean;
  meta?: any;
  id: number;
}

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  @ViewChild('modalContent', { static: true }) modalContent: TemplateRef<any>;

  view: CalendarView = CalendarView.Month;

  CalendarView = CalendarView;

  viewDate: Date = new Date();
  permissionAdd = false;
  modalData: {
    action: string;
    event: CalendarEventExtended;
  };

  actions: CalendarEventAction[] = [
    {
      label: '<i class="fas fa-fw fa-pencil-alt"></i>',
      a11yLabel: 'Anzeigen',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.handleEvent('Anzeigen', event as CalendarEventExtended);
      },
    }
  ];

  refresh: Subject<any> = new Subject();

  events: CalendarEventExtended[] = new Array<CalendarEventExtended>();

  constructor(private eventService: EventService, private dialog: MatDialog, private jwt: JwtService) {}

  activeDayIsOpen = true;

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
      this.viewDate = date;
    }
  }

  addEvent() {
    const dialogRef = this.dialog.open(EventsAddComponent, { disableClose: true });
    dialogRef.afterClosed().subscribe(x => this.init());
  }

  handleEvent(action: string, event: CalendarEventExtended): void {
    this.modalData = { event, action };
    this.eventService.getById(event.id).subscribe(x => {
      const dialogRef = this.dialog.open(EventsDetailComponent, { data: x, disableClose: true, minWidth: '450px', height: '600px' });
      dialogRef.afterClosed().subscribe(y => this.init());
    });
  }

  setView(view: CalendarView) {
    this.view = view;
  }

  closeOpenMonthViewDay() {
    this.activeDayIsOpen = false;
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

  init() {
    this.events = new Array<CalendarEventExtended>();
    this.eventService.get().subscribe(x => {
      const xSort: EventDto[] = x.sort(this.funtionGetSortedData);
      let idx = 0;
      xSort.forEach(a => {
        this.events.push(
          {
            start: this.getStartDate(a.date, a.time),
            end: this.getEndDate(a.date, a.time, a.duration),
            title: this.getTime(a.time) + ' ' + a.name,
            color: idx % 3 === 0 ? colors.cyan : (idx % 2 === 0 ? colors.blue : colors.yellow),
            actions: this.actions,
            allDay: false,
            draggable: false,
            id: a.id
          }
        );
        idx++;
      });
    });
  }

  getTime(time: string) {
    const date = new Date(time);
    let minutes = date.getMinutes().toString();
    if (minutes.length === 1) {
      minutes = '0' + minutes;
    }
    return date.getHours() + ':' + minutes;
  }

  getEndDate(date: string, time: string, duration: number): Date {
    const dateNew = this.getStartDate(date, time);
    let timeNew = dateNew.getHours() * 60 + dateNew.getMinutes();
    timeNew += duration * 60;
    dateNew.setHours(timeNew / 60);
    dateNew.setMinutes(timeNew % 60);
    return dateNew;
  }

  getStartDate(date: string, time: string): Date {
    const dateNew = new Date(date);
    const timeNew = new Date(time);
    dateNew.setHours(timeNew.getHours());
    dateNew.setMinutes(timeNew.getMinutes());
    return dateNew;
  }

  ngOnInit(): void {
    this.permissionAdd = this.jwt.hasPermission('Anlegen einer Veranstaltung');
    this.init();
  }

  getKw(date: Date): number {
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
