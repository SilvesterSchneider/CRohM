import { Component, OnInit } from '@angular/core';
import { ContactDtoExtended } from 'src/app/events/events-info/events-info.component';
import { StatisticsService, STATISTICS_VALUES, VerticalGroupedBarDto } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-event-visits',
  templateUrl: './event-visits.component.html',
  styleUrls: ['./event-visits.component.scss']
})
export class EventVisitsComponent implements OnInit {
  view: any[] = [3 *100 + 400, 500];
  data: VerticalGroupedBarDto[] = new Array<VerticalGroupedBarDto>();
  totalInvitations: number = 0;
  totalParticipations: number = 0;
  relation: number = 0;
  
  constructor(private statistics: StatisticsService ) { }

  ngOnInit(): void {
    this.statistics.getVerticalGroupedBarDataByType(STATISTICS_VALUES.INVITED_AND_PARTICIPATED_EVENT_PERSONS).subscribe(x => {
      this.data = x
      if (x.length === 0) {
        this.view = [700, 500];
        this.totalInvitations = 0;
        this.totalParticipations = 0;
        this.relation = 0;
      } else {
        this.view = [x.length * 80 + 400, 500];
        this.data.forEach(x => {
          this.totalInvitations += x.series.find(a => a.name === 'Eingeladen').value;
          this.totalParticipations += x.series.find(a => a.name === 'Teilgenommen').value;
        });
        this.relation = this.totalParticipations / this.totalInvitations * 100;
      }
    });
  }
}
