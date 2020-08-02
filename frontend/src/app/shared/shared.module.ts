import { NgModule } from '@angular/core';
import { OsmAddressComponent } from './osm/osm-address/osm-address.component';
import { MaterialModule } from './material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';
import { CommonModule } from '@angular/common';
import { ContactPossibilitiesComponent } from './contactPossibilities/contact-possibilities.component';
import { AddHistoryComponent } from './add-history/add-history.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MaterialModule,
    FlexLayoutModule],
  declarations: [OsmAddressComponent, ContactPossibilitiesComponent, AddHistoryComponent],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MaterialModule,
    FlexLayoutModule,
    OsmAddressComponent,
    ContactPossibilitiesComponent,
    AddHistoryComponent
    ]
})
export class SharedModule { }
