import { Component, OnInit, ViewChild } from '@angular/core';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';
import { VerticalGroupedBarChartComponent } from 'src/app/shared/charts/vertical-grouped-bar-chart/vertical-grouped-bar-chart.component';

@Component({
  selector: 'app-event-visits',
  templateUrl: './event-visits.component.html',
  styleUrls: ['./event-visits.component.scss']
})
export class EventVisitsComponent implements OnInit {
  @ViewChild(VerticalGroupedBarChartComponent, { static: true })
	chart: VerticalGroupedBarChartComponent;
  totalInvitations = 0;
  totalParticipations = 0;
  relation = 0;
  startDate: Date;
  endDate: Date;

  constructor(
    private statistics: StatisticsService) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.INVITED_AND_PARTICIPATED_EVENT_PERSONS).subscribe(x => {
      this.chart.setSizes(80, 400, 400);
      this.chart.setChangeCallback((visibleData: VerticalGroupedBarDto[]) => this.calculateTheAmounts(visibleData));
      this.chart.setLabels('Veranstaltungen', 'Anzahl Teilnehmer', 'Teilnehmer');
      this.chart.setData(x);
      if (x.length === 0) {
        this.totalInvitations = 0;
        this.totalParticipations = 0;
        this.relation = 0;
      } else {
        this.calculateTheAmounts(x);
      }
    });
  }

  calculateTheAmounts(visibleData: VerticalGroupedBarDto[]) {
    this.totalInvitations = 0;
    this.totalParticipations = 0;
    visibleData.forEach(x => {
      this.totalInvitations += x.series.find(a => a.name === 'Eingeladen').value;
      this.totalParticipations += x.series.find(a => a.name === 'Teilgenommen').value;
    });
    this.relation = this.totalParticipations / this.totalInvitations * 100;
  }
}
