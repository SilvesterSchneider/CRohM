import { NgModule } from '@angular/core';
import { OsmAddressComponent } from './osm/osm-address/osm-address.component';
import { MaterialModule } from './material.module';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';
import { CommonModule } from '@angular/common';
import { ContactPossibilitiesComponent } from './contactPossibilities/contact-possibilities.component';
import { AddHistoryComponent } from './add-history/add-history.component';
import { TagsFilterComponent } from './tags-filter/tags-filter.component';
import { VerticalGroupedBarChartComponent } from './charts/vertical-grouped-bar-chart/vertical-grouped-bar-chart.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MaterialModule,
    FlexLayoutModule],
  declarations: [
    OsmAddressComponent,
    ContactPossibilitiesComponent,
    AddHistoryComponent, TagsFilterComponent,
    VerticalGroupedBarChartComponent,
	FlexLayoutModule,
    NgxChartsModule
  ],

  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MaterialModule,
    FlexLayoutModule,
    OsmAddressComponent,
    ContactPossibilitiesComponent,
    AddHistoryComponent,
    TagsFilterComponent,
    VerticalGroupedBarChartComponent,
    NgxChartsModule
  ]
})
export class SharedModule { }
