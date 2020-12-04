import { Component, OnInit } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { VerticalGroupedBarDto } from '../../api-generated/api-generated';

@Component({
  selector: 'app-vertical-grouped-bar-chart',
  templateUrl: './vertical-grouped-bar-chart.component.html',
  styleUrls: ['./vertical-grouped-bar-chart.component.scss']
})
export class VerticalGroupedBarChartComponent implements OnInit {
  xLabel: string;
  yLabel: string;
  legendLabel: string;
  startDate: Date;
  showDates = true;
  endDate: Date;
  widthPerObject = 100;
  widthGeneral = 400;
  height = 400;
  view: any[] = [3 * this.widthPerObject + this.widthGeneral, this.height];
  allData: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
  visibleData: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
  changeFunction: (visibleData: VerticalGroupedBarDto[]) => void;

  constructor() { }

  ngOnInit(): void {
  }

  public setLabels(nameX: string, nameY: string, nameL: string) {
    this.xLabel = nameX;
    this.yLabel = nameY;
    this.legendLabel = nameL;
  }

  public setData(inData: VerticalGroupedBarDto[]) {
    this.allData = inData;
    this.visibleData = inData;
    this.setTheView();
  }

  public setChangeCallback(funct: (visibleData: VerticalGroupedBarDto[]) => void) {
    this.changeFunction = funct;
  }

  public setSizes(perObjectWidth: number, generalWidth: number, heightValue: number) {
    this.widthPerObject = perObjectWidth;
    this.widthGeneral = generalWidth;
    this.height = heightValue;
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
      this.changeFunction(this.visibleData);
    }
  }

  isDateSmallerThan(dateOne: Date, dateTwo: Date): boolean {
    const one: number = dateOne.getFullYear() * 1000 + dateOne.getMonth() * 100 + dateOne.getDate();
    const two: number = dateTwo.getFullYear() * 1000 + dateTwo.getMonth() * 100 + dateTwo.getDate();
    return one <= two;
  }

  isDateGreaterThan(dateOne: Date, dateTwo: Date): boolean {
    const one: number = dateOne.getFullYear() * 1000 + dateOne.getMonth() * 100 + dateOne.getDate();
    const two: number = dateTwo.getFullYear() * 1000 + dateTwo.getMonth() * 100 + dateTwo.getDate();
    return one >= two;
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
    if (this.visibleData.length > 0) {
      this.view = [this.visibleData.length * this.widthPerObject + this.widthGeneral, this.height];
    } else {
      this.view = [700, this.height];
    }
  }

  shouldShowDates(value: boolean) {
    this.showDates = value;
  }
}
