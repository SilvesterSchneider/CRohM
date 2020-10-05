import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OverviewComponent } from './overview/overview.component';

const statisticsRoutes: Routes = [
  {
      path: '',
      component: OverviewComponent
  }
];

@NgModule({
  imports: [
      RouterModule.forChild(statisticsRoutes)
  ],
  exports: [
      RouterModule
  ],
  providers: [
  ]
})
export class StatisticsRoutingModule { }
