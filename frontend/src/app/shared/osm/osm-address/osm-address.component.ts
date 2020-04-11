import { Component, OnInit } from '@angular/core';
import { OsmService } from '../osm.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AddressDto } from '../../api-generated/api-generated';

@Component({
  selector: 'app-osm-address',
  templateUrl: './osm-address.component.html',
  styleUrls: ['./osm-address.component.scss']
})
export class OsmAddressComponent implements OnInit {
  addressSuggestions: AddressDto[] = [];
  modelChanged: Subject<string> = new Subject<string>();

  constructor(private osmService: OsmService) { }

  ngOnInit(): void {
    this.modelChanged.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => this.onSearch(value));
  }

  changed($event: any) {
    this.modelChanged.next($event.target.value);
  }

  onSearch(term) {
    this.addressSuggestions = [];
    if (term !== '' || term.trim() !== '') {
      this.osmService.getResults(term)
        .subscribe((results: any) => {
          this.addressSuggestions = results.features.map(element => OsmService.parseAddress(element));
        });
    }

  }
}
