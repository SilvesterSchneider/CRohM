import { getLocaleDateFormat } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { ContactDtoExtended } from 'src/app/events/events-info/events-info.component';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-event-visits',
  templateUrl: './event-visits.component.html',
  styleUrls: ['./event-visits.component.scss']
})
export class EventVisitsComponent implements OnInit {
  view: any[] = [3 * 100 + 400, 500];
  allData: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
  visibleData: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
  totalInvitations = 0;
  totalParticipations = 0;
  relation = 0;
  startDate: Date;
  endDate: Date;

  constructor(
    private statistics: StatisticsService) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.INVITED_AND_PARTICIPATED_EVENT_PERSONS).subscribe(x => {
      this.visibleData = x;
      this.allData = x;
      if (x.length === 0) {
        this.view = [700, 500];
        this.totalInvitations = 0;
        this.totalParticipations = 0;
        this.relation = 0;
      } else {
        this.setTheView();
        this.calculateTheAmounts();
      }
    });
  }

  addEventStart(type: string, event: MatDatepickerInputEvent<Date>) {
    this.startDate = new Date(event.value);
    this.checkForUpdate();
  }

  addEventEnd(type: string, event: MatDatepickerInputEvent<Date>) {
    this.endDate = new Date(event.value);
    this.checkForUpdate();
  }

  setTheView() {
    this.view = [this.visibleData.length * 80 + 400, 500];
  }

  calculateTheAmounts() {
    this.totalInvitations = 0;
    this.totalParticipations = 0;
    this.visibleData.forEach(x => {
      this.totalInvitations += x.series.find(a => a.name === 'Eingeladen').value;
      this.totalParticipations += x.series.find(a => a.name === 'Teilgenommen').value;
    });
    this.relation = this.totalParticipations / this.totalInvitations * 100;
  }

  checkForUpdate() {
    if (this.startDate != null && this.endDate != null) {
      this.visibleData = new Array<VerticalGroupedBarDto>();
      this.allData.forEach(x => {
        if (this.isDateGreaterThan(new Date(x.name), this.startDate) && this.isDateSmallerThan(new Date(x.name), this.endDate)) {
          this.visibleData.push(x);
        }
      });
      this.setTheView();
      this.calculateTheAmounts();
    }
  }

  isDateSmallerThan(dateOne: Date, dateTwo: Date): boolean {
    const one: number = dateOne.getFullYear() + dateOne.getMonth() + dateOne.getDate();
    const two: number = dateTwo.getFullYear() + dateTwo.getMonth() + dateTwo.getDate();
    return one <= two;
  }

  isDateGreaterThan(dateOne: Date, dateTwo: Date): boolean {
    const one: number = dateOne.getFullYear() + dateOne.getMonth() + dateOne.getDate();
    const two: number = dateTwo.getFullYear() + dateTwo.getMonth() + dateTwo.getDate();
    return one >= two;
  }
}
