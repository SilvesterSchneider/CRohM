import { Component, OnInit, ChangeDetectorRef, AfterViewInit, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { EventService } from '../../shared/api-generated/api-generated';
import { EventDto } from '../../shared/api-generated/api-generated';
import { EventsServiceMock } from '../events.service-mock';
import { MatDialog } from '@angular/material/dialog';
import { EventsAddComponent } from '../events-add/events-add.component';
import { ActivatedRouteSnapshot } from '@angular/router';
import { EventsDetailComponent } from '../events-detail/events-detail.component';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { EventsInfoComponent } from '../events-info/events-info.component';

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss']
})

export class EventsListComponent implements OnInit {
  @ViewChild(MatSort) sort: MatSort;
  events: Observable<EventDto[]>;
  displayedColumns = ['bezeichnung', 'datum', 'uhrzeit', 'action'];
  public dataSource: EventDto[];
  checkboxSelected = false;

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
      this.dataSource = xSort;
      if (this.checkboxSelected) {
        this.filterValues();
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
    const justNewEvents: EventDto[] = this.dataSource.filter(x => new Date(x.date).getTime() >= Date.now());
    this.dataSource = justNewEvents;
  }

  openInfo(id: number) {
    this.service.getById(id).subscribe(x => {
      this.dialog.open(EventsInfoComponent, { data: x, height: '600px' });
    });
  }
}
