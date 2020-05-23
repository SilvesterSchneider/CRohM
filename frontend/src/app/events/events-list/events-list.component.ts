import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs';
import { EventService } from '../../shared/api-generated/api-generated';
import { EventDto } from '../../shared/api-generated/api-generated';
import { EventsServiceMock } from '../events.service-mock';
import { MatDialog } from '@angular/material/dialog';
import { EventsAddComponent } from '../Events-add/Events-add.component';
import { EventsDetailResolverService } from '../events-detail-resolver.service';
import { ActivatedRouteSnapshot } from '@angular/router';
import { EventsDetailComponent } from '../Events-detail/Events-detail.component';

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss']
})

export class EventsListComponent implements OnInit {
  events: Observable<EventDto[]>;
  displayedColumns = ['bezeichnung', 'datum', 'uhrzeit', 'action'];
  service: EventService;

  constructor(
    service: EventService,
    private changeDetectorRefs: ChangeDetectorRef,
    private serviceMock: EventsServiceMock,
    private dialog: MatDialog) {
      this.service = service;
   }

  ngOnInit() {
    this.init();
  }

  private init() {
    this.events = this.service.get();
    this.events.subscribe();
    this.changeDetectorRefs.detectChanges();
   // this.events = this.serviceMock.getEvents();
  }

  addContact() {
    const dialogRef = this.dialog.open(EventsAddComponent);
    dialogRef.afterClosed().subscribe(x => this.init());
  }

  callEdit(id: number) {
    this.service.getById(id).subscribe(x => {
      const dialogRef = this.dialog.open(EventsDetailComponent, { data: x });
      dialogRef.afterClosed().subscribe(x => this.init());
    });
  }

  deleteEvent(id: number) {
    this.service.delete(id).subscribe(x => this.init());
  }
}
