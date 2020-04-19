import { Component, OnInit, forwardRef } from '@angular/core';
import { OsmService } from '../osm.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AddressDto } from '../../api-generated/api-generated';
import {
  FormBuilder, Validators, NG_VALUE_ACCESSOR, NG_VALIDATORS,
  ControlValueAccessor, Validator, AbstractControl, ValidationErrors
} from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-osm-address',
  templateUrl: './osm-address.component.html',
  styleUrls: ['./osm-address.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OsmAddressComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => OsmAddressComponent),
      multi: true
    }
  ]
})
export class OsmAddressComponent implements OnInit, ControlValueAccessor, Validator {
  addressSuggestions: AddressDto[] = [];
  addressSearchString: Subject<string> = new Subject<string>();

  // TODO: sollten die möglichen Länder aus dem Backend laden
  // Liste der im Dropdown angezeigten Laender
  public countries: Country[] = [
    { value: 'Deutschland', viewValue: 'Deutschland' },
    { value: 'Schweiz', viewValue: 'Schweiz' },
    { value: 'Österreich', viewValue: 'Österreich' }
  ];


  addressForm = this.fb.group({
    country: ['', Validators.required],
    street: ['', Validators.required],
    streetNumber: ['', Validators.required],
    zipcode: ['', Validators.pattern('^[0-9]{5}$')],
    city: ['', Validators.required],
  });


  constructor(private fb: FormBuilder, private osmService: OsmService) { }

  validate(control: AbstractControl): ValidationErrors | null {
    return this.addressForm.valid ? null : this.addressForm.errors;
  }

  public onTouched: () => void = () => { };

  writeValue(val: any): void {
    if (val) {
      this.addressForm.setValue(val, { emitEvent: false });
    }
  }
  registerOnChange(fn: any): void {
    this.addressForm.valueChanges.subscribe(fn);
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;

  }
  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.addressForm.disable() : this.addressForm.enable();
  }

  ngOnInit(): void {
    this.addressSearchString.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(value => this.onSearch(value));
  }

  onChange($event: any) {
    this.addressSearchString.next($event.target.value);
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
    const address: AddressDto = selected.option.value;

    this.addressForm.reset();

    if (address.street) {
      this.addressForm.get('street').patchValue(address.street);
    }
    if (address.streetNumber) {
      this.addressForm.get('streetNumber').patchValue(address.streetNumber.toString());
    }
    if (address.zipcode) {
      this.addressForm.get('zipcode').patchValue(address.zipcode);
    }
    if (address.city) {
      this.addressForm.get('city').patchValue(address.city);
    }
    if (address.country) {
      this.addressForm.get('country').patchValue(address.country);
    }
  }

  displayFn(selected: AddressDto) {
    return `${selected.street ?? ''} ${selected.streetNumber ?? ''} ` +
      `${selected.zipcode ?? ''} ${selected.city ?? ''} ${selected.country ?? ''}`;
  }


}


interface Country {
  value: string;
  viewValue: string;
}

