import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent implements OnInit {
  selectedIndex = 0;

  constructor() { }

  ngOnInit(): void {
  }

  tabChange(index: number){
    this.selectedIndex = index;
  }
}
