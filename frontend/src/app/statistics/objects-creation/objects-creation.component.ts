import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';
import { VerticalGroupedBarChartComponent } from 'src/app/shared/charts/vertical-grouped-bar-chart/vertical-grouped-bar-chart.component';

@Component({
  selector: 'app-objects-creation',
  templateUrl: './objects-creation.component.html',
  styleUrls: ['./objects-creation.component.scss']
})
export class ObjectsCreationComponent implements OnInit {
  @ViewChild(VerticalGroupedBarChartComponent, { static: false })
  chart: VerticalGroupedBarChartComponent;
  totalContacts = 0;
  totalOrganizations = 0;
  totalEvents = 0;

  constructor(private statistics: StatisticsService, private translate: TranslateService) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.ALL_CREATED_OBJECTS).subscribe(stats => {

      stats.forEach(res => res.series.map(series => {
        switch (series.name) {
          case 'Kontakte': series.name = this.translate.instant('contact.contacts'); break;
          case 'Organisationen': series.name = this.translate.instant('organization.organizations'); break;
          case 'Veranstaltungen': series.name = this.translate.instant('event.events'); break;
        }
        return series;
      }));

      this.chart.setSizes(80, 400, 400);
      this.chart.setChangeCallback((visibleData: VerticalGroupedBarDto[]) => this.calculateTheAmounts(visibleData));
      this.chart.setLabels(this.translate.instant('statistic.data'),
        this.translate.instant('statistic.numberObjects'),
        this.translate.instant('statistic.objects'));
      this.chart.setData(stats);

      if (stats.length === 0) {
        this.totalContacts = 0;
        this.totalOrganizations = 0;
        this.totalEvents = 0;
      } else {
        this.calculateTheAmounts(stats);
      }
    });
  }

  calculateTheAmounts(visibleData: VerticalGroupedBarDto[]) {
    this.totalContacts = 0;
    this.totalOrganizations = 0;
    this.totalEvents = 0;
    visibleData.forEach(x => {
      const contactSet = x.series.find(a => a.name === this.translate.instant('contact.contacts'));
      if (contactSet != null) {
        this.totalContacts += contactSet.value;
      }
      const orgaSet = x.series.find(a => a.name === this.translate.instant('organization.organizations'));
      if (orgaSet != null) {
        this.totalOrganizations += orgaSet.value;
      }
      const eventSet = x.series.find(a => a.name === this.translate.instant('event.events'));
      if (eventSet != null) {
        this.totalEvents += eventSet.value;
      }
    });
  }
}
