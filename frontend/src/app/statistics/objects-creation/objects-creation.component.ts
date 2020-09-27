import { Component, OnInit } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-objects-creation',
  templateUrl: './objects-creation.component.html',
  styleUrls: ['./objects-creation.component.scss']
})
export class ObjectsCreationComponent implements OnInit {
  view: any[] = [3 * 120 + 600, 400];
  allData: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
  visibleData: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
  totalContacts = 0;
  totalOrganizations = 0;
  totalEvents = 0;
  startDate: Date;
  endDate: Date;

  constructor(
    private statistics: StatisticsService) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.ALL_CREATED_OBJECTS).subscribe(x => {
      this.visibleData = x;
      this.allData = x;
      if (x.length === 0) {
        this.view = [700, 400];
        this.totalContacts = 0;
        this.totalOrganizations = 0;
        this.totalEvents = 0;
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
    this.view = [this.visibleData.length * 120 + 600, 400];
  }

  calculateTheAmounts() {
    this.totalContacts = 0;
    this.totalOrganizations = 0;
    this.totalEvents = 0;
    this.visibleData.forEach(x => {
      const contactSet = x.series.find(a => a.name === 'Kontakte');
      if (contactSet != null) {
        this.totalContacts += contactSet.value;
      }
      const orgaSet = x.series.find(a => a.name === 'Organisationen');
      if (orgaSet != null) {
        this.totalOrganizations += orgaSet.value;
      }
      const eventSet = x.series.find(a => a.name === 'Veranstaltungen');
      if (eventSet != null) {
        this.totalEvents += eventSet.value;
      }
    });
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