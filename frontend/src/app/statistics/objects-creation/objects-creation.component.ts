import { Component, OnInit, ViewChild } from '@angular/core';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';
import { VerticalGroupedBarChartComponent } from 'src/app/shared/charts/vertical-grouped-bar-chart/vertical-grouped-bar-chart.component';

@Component({
  selector: 'app-objects-creation',
  templateUrl: './objects-creation.component.html',
  styleUrls: ['./objects-creation.component.scss']
})
export class ObjectsCreationComponent implements OnInit {
  @ViewChild(VerticalGroupedBarChartComponent, { static: true })
	chart: VerticalGroupedBarChartComponent;
  totalContacts = 0;
  totalOrganizations = 0;
  totalEvents = 0;

  constructor(
    private statistics: StatisticsService) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.ALL_CREATED_OBJECTS).subscribe(x => {
      this.chart.setSizes(80, 400, 400);
      this.chart.setChangeCallback((visibleData: VerticalGroupedBarDto[]) => this.calculateTheAmounts(visibleData));
      this.chart.setLabels('Daten', 'Anzahl erzeugter Objekte', 'Objekte');
      this.chart.setData(x);
      if (x.length === 0) {
        this.totalContacts = 0;
        this.totalOrganizations = 0;
        this.totalEvents = 0;
      } else {
        this.calculateTheAmounts(x);
      }
    });
  }

  calculateTheAmounts(visibleData: VerticalGroupedBarDto[]) {
    this.totalContacts = 0;
    this.totalOrganizations = 0;
    this.totalEvents = 0;
    visibleData.forEach(x => {
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
}