import { Component, OnInit, Input } from '@angular/core';
import { OsmService } from '../osm.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AddressDto } from '../../api-generated/api-generated';
import { Country } from '../../../contacts/contacts.model';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-osm-address',
  templateUrl: './osm-address.component.html',
  styleUrls: ['./osm-address.component.scss']
})
export class OsmAddressComponent implements OnInit {
  @Input() parentForm: FormGroup;

  addressSuggestions: AddressDto[] = [];
  modelChanged: Subject<string> = new Subject<string>();

  // Liste der im Dropdown angezeigten Laender
  countries: Country[] = [
    { value: 'Deutschland', viewValue: 'Deutschland' },
    { value: 'Schweiz', viewValue: 'Schweiz' },
    { value: 'Österreich', viewValue: 'Österreich' }
  ];

  constructor(private fb: FormBuilder, private osmService: OsmService) { }

  ngOnInit(): void {
    this.modelChanged.pipe(
      debounceTime(200),
      distinctUntilChanged()
    ).subscribe(value => this.onSearch(value));
  }

  onChange($event: any) {
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

  onSelection(selected: MatAutocompleteSelectedEvent) {
    // const address: AddressDto = OsmService.parseAddress(selected.option.value);
    const address: AddressDto = selected.option.value;
    const adressControl = this.parentForm.get('adresse');

    console.log(adressControl);

    adressControl.reset();

    if (address.street) {
      adressControl.get('street').patchValue(address.street);
    }
    if (address.streetNumber) {
      adressControl.get('streetNumber').patchValue(address.streetNumber.toString());
    }
    if (address.zipcode) {
      adressControl.get('zipcode').patchValue(address.zipcode);
    }
    if (address.city) {
      adressControl.get('city').patchValue(address.city);
    }
    if (address.country) {
      adressControl.get('country').patchValue(address.country);
    }
  }

}
