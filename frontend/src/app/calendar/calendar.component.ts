import { Component, OnInit ,ChangeDetectionStrategy,
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
import { EventService } from '../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { EventColor } from 'calendar-utils';
import { EventsDetailComponent } from '../events/events-detail/events-detail.component';

const colors: any = {
  red: {
    primary: '#ad2121',
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

  constructor(private eventService: EventService, private dialog: MatDialog) {}

  activeDayIsOpen: boolean = true;

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

  init() {
    this.events = new Array<CalendarEventExtended>();
    this.eventService.get().subscribe(x => {
      x.forEach(a => {
        this.events.push(
          {
            start: this.getStartDate(a.date, a.time),
            end: this.getEndDate(a.date, a.time, a.duration),
            title: a.name,
            color: this.events.length % 3 === 0 ? colors.red : this.events.length % 2 === 0 ? colors.blue : colors.yellow,
            actions: this.actions,
            allDay: false,
            draggable: false,
            id: a.id
          }
        );
      });
      this.refresh.subscribe();
    });
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
    this.init();
  }
}
