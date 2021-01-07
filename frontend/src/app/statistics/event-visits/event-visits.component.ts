import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';
import { VerticalGroupedBarChartComponent } from 'src/app/shared/charts/vertical-grouped-bar-chart/vertical-grouped-bar-chart.component';

@Component({
  selector: 'app-event-visits',
  templateUrl: './event-visits.component.html',
  styleUrls: ['./event-visits.component.scss']
})
export class EventVisitsComponent implements OnInit {
  @ViewChild(VerticalGroupedBarChartComponent, { static: false })
  chart: VerticalGroupedBarChartComponent;
  totalInvitations = 0;
  totalParticipations = 0;
  relation = 0;
  totalAgrees = 0;
  startDate: Date;
  endDate: Date;

  constructor(private statistics: StatisticsService, private translate: TranslateService) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.INVITED_AND_PARTICIPATED_EVENT_PERSONS).subscribe(stats => {
      stats.forEach(res => res.series.map(series => {
        switch (series.name) {
          case 'Eingeladen': series.name = this.translate.instant('event.invited'); break;
          case 'Teilgenommen': series.name = this.translate.instant('event.participated'); break;
          case 'Zugesagt': series.name = this.translate.instant('event.agreed'); break;
        }
        return series;
      }));

      this.chart.setSizes(80, 400, 400);
      this.chart.setChangeCallback((visibleData: VerticalGroupedBarDto[]) => this.calculateTheAmounts(visibleData));
      this.chart.setLabels(this.translate.instant('event.events'),
        this.translate.instant('statistic.numberParticipants'),
        this.translate.instant('event.participant'));
      this.chart.setData(stats);
      if (stats.length === 0) {
        this.totalInvitations = 0;
        this.totalParticipations = 0;
        this.relation = 0;
      } else {
        this.calculateTheAmounts(stats);
      }
    });
  }

  calculateTheAmounts(visibleData: VerticalGroupedBarDto[]) {
    this.totalInvitations = 0;
    this.totalParticipations = 0;
    this.totalAgrees = 0;
    visibleData.forEach(x => {
      this.totalInvitations += x.series.find(a => a.name === this.translate.instant('event.invited')).value;
      this.totalParticipations += x.series.find(a => a.name === this.translate.instant('event.participated')).value;
      this.totalAgrees += x.series.find(a => a.name === this.translate.instant('event.agreed')).value;
    });
    this.relation = this.totalParticipations / this.totalInvitations * 100;
  }
}
