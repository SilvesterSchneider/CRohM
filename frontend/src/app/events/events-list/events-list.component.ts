import { Component, OnInit, ChangeDetectorRef, AfterViewInit, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { EventService, ParticipatedDto, ContactDto } from '../../shared/api-generated/api-generated';
import { EventDto } from '../../shared/api-generated/api-generated';
import { EventsServiceMock } from '../events.service-mock';
import { MatDialog } from '@angular/material/dialog';
import { EventsAddComponent } from '../events-add/events-add.component';
import { ActivatedRouteSnapshot } from '@angular/router';
import { EventsDetailComponent } from '../events-detail/events-detail.component';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { EventsInfoComponent } from '../events-info/events-info.component';

export interface GroupBy {
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
  public dataSource: (EventDto | GroupBy)[] = new Array<(EventDto | GroupBy)>();
  checkboxSelected = true;
  weekNumber: number = 0;

  constructor(
    private service: EventService,
    private changeDetectorRefs: ChangeDetectorRef,
    private serviceMock: EventsServiceMock,
    private dialog: MatDialog) {
   }

  ngOnInit() {
    this.init();
  }

  private init() {
    this.events = this.service.get();
    this.events.subscribe(x => {
      const xSort: EventDto[] = x.sort(this.funtionGetSortedData);
      xSort.forEach(x => 
        {
          const week = this.getWeekNumber(new Date(x.date));
          if (week > this.weekNumber) {
            this.weekNumber = week;
            this.dataSource.push({
              weekNumber: week,
              isGroupBy: true
            });
          }
          this.dataSource.push(x);
        });          
      if (this.checkboxSelected) {
    //    this.filterValues();
      }
    });    
    this.changeDetectorRefs.detectChanges();
   // this.events = this.serviceMock.getEvents();
  }

  funtionGetSortedData(a: EventDto, b: EventDto): number {
    const dateA = new Date(a.date);
    let dateTimeA = dateA.getTime();
    const timeA = new Date(a.time);
    dateTimeA += timeA.getHours();
    dateTimeA += timeA.getMinutes();
    dateTimeA += timeA.getSeconds();
    const dateB = new Date(b.date);
    let dateTimeB = dateB.getTime();
    const timeB = new Date(a.time);
    dateTimeB += timeB.getHours();
    dateTimeB += timeB.getMinutes();
    dateTimeB += timeB.getSeconds();
    return (dateTimeA - dateTimeB);
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

  deleteEvent(id: number) {
    this.service.delete(id).subscribe(x => this.init());
  }

  toggleSelection() {
    this.checkboxSelected = !this.checkboxSelected;
    this.init();
  }

  filterValues() {
    this.dataSource = [];
    this.weekNumber = 0;
    const justNewEvents: (EventDto | GroupBy)[] = new Array<(EventDto | GroupBy)>();
    let eventsFiltered: EventDto[] = new Array<EventDto>();
    this.events.subscribe(y => eventsFiltered = y.filter(x => new Date(x.date).getTime() >= Date.now()));
    eventsFiltered.forEach(x => 
      {
        const week = this.getWeekNumber(new Date(x.date));
        if (week > this.weekNumber) {
          this.weekNumber = week;
          this.dataSource.push({
            weekNumber: week,
            isGroupBy: true
          });
        }
        this.dataSource.push(x)});
  }

  openInfo(id: number) {
    this.service.getById(id).subscribe(x => {
      this.dialog.open(EventsInfoComponent, { data: x });
    });
  }

  isGroup(index, item): boolean{
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
    const currentThursday = new Date(date.getTime() +(3-((date.getDay()+6) % 7)) * 86400000);

    // At the beginnig or end of a year the thursday could be in another year.
    const yearOfThursday = currentThursday.getFullYear();

    // Get first Thursday of the year
    const firstThursday = new Date(new Date(yearOfThursday,0,4).getTime() +(3-((new Date(yearOfThursday,0,4).getDay()+6) % 7)) * 86400000);

    // +1 we start with week number 1
    // +0.5 an easy and dirty way to round result (in combinationen with Math.floor)
    const weekNumber = Math.floor(1 + 0.5 + (currentThursday.getTime() - firstThursday.getTime()) / 86400000/7);
    return weekNumber;
  }
}
