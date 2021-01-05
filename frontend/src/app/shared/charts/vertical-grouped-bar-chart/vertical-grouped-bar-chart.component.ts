import { Component, OnInit } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { VerticalGroupedBarDto } from '../../api-generated/api-generated';
import * as moment from 'moment';

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

  public setDataWithDates(inData: VerticalGroupedBarDto[], start: string, end: string) {
    this.startDate = new Date(start);
    this.endDate = new Date(end);
    alert(start);
    alert(this.startDate);
    this.allData = inData;
    this.checkForUpdate();
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
      this.visibleData = this.allData.filter(date => moment(date.name, 'DD.MM.YYYY')
        .isBetween(moment(this.startDate, 'DD.MM.YYYY'), moment(this.endDate, 'DD.MM.YYYY'), null, '[]'));
      this.setTheView();
      this.changeFunction(this.visibleData);
    }
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
