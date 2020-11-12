import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDataSet, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';
import { VerticalGroupedBarChartComponent } from 'src/app/shared/charts/vertical-grouped-bar-chart/vertical-grouped-bar-chart.component';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.scss']
})
export class TagsComponent implements OnInit {
  @ViewChild(VerticalGroupedBarChartComponent, { static: true })
  chart: VerticalGroupedBarChartComponent;
  totalTags: number;
  valueContacts: boolean;
  valueOrganizations: boolean;
  valueEvents: boolean;
  data: VerticalGroupedBarDto[];

  constructor(private statistics: StatisticsService, private translate: TranslateService) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.ALL_TAGS).subscribe(x => {
      this.data = x;
      this.chart.setSizes(80, 400, 400);
      this.chart.setChangeCallback((visibleData: VerticalGroupedBarDto[]) => this.calculateTheAmounts(visibleData));
      this.chart.setLabels(this.translate.instant('common.tags'), this.translate.instant('statistic.numberTags'), this.translate.instant('statistic.objects'));
      this.chart.setData(x);
      this.chart.shouldShowDates(false);
      if (x.length === 0) {
        this.totalTags = 0;
      } else {
        this.calculateTheAmounts(x);
      }
      this.updateData();
    });
  }

  calculateTheAmounts(visibleData: VerticalGroupedBarDto[]) {
    this.totalTags = 0;
    visibleData.forEach(x => {
      this.totalTags++;
    });
  }

  setClicked(contactsClicked: boolean, organizationsClicked: boolean, eventsClicked: boolean) {
    this.valueContacts = contactsClicked;
    this.valueOrganizations = organizationsClicked;
    this.valueEvents = eventsClicked;
    this.updateData();
  }

  updateData() {
    const sortedData: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
    this.data.forEach(x => sortedData.push(x));
    sortedData.sort((a, b) => this.sortFunction(a, b));
    this.chart.setData(sortedData);
  }

  sortFunction(firstObject: VerticalGroupedBarDto, secondObject: VerticalGroupedBarDto): number {
    let searchText = 'Kontakt';
    if (this.valueOrganizations) {
      searchText = 'Organisation';
    } else if (this.valueEvents) {
      searchText = 'Veranstaltung';
    }
    const vertA: VerticalGroupedBarDataSet = firstObject.series.find(a => a.name === searchText);
    const vertB: VerticalGroupedBarDataSet = secondObject.series.find(a => a.name === searchText);
    if (vertA == null) {
      return 1;
    }
    if (vertB == null) {
      return -1;
    }
    return vertB.value - vertA.value;
  }
}
