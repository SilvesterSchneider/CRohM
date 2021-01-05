import { Component, OnInit, forwardRef } from '@angular/core';
import { OsmService } from '../osm.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AddressDto } from '../../api-generated/api-generated';
import {
  FormBuilder, Validators, NG_VALUE_ACCESSOR, NG_VALIDATORS,
  ControlValueAccessor, Validator, AbstractControl, ValidationErrors, FormGroup
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
    { value: 'Deutschland', viewValue: 'common.germany' },
    { value: 'Schweiz', viewValue: 'common.switzerland' },
    { value: 'Österreich', viewValue: 'common.austria' }
  ];

  addressForm = this.fb.group({
    id: [''],
    name: [''],
    description: [''],
    country: [this.countries[0].value, Validators.required],
    street: ['', Validators.pattern('^[a-zA-Z äüöÄÜÖß.-]*')],
    streetNumber: ['', Validators.pattern('^[a-zA-Z0-9äüöÄÜÖß.-]*')],
    zipcode: ['', Validators.pattern('^[0-9]{4,5}$')],
    city: ['', Validators.pattern('^[a-zA-ZäüöÄÜÖß.-]*')],
  });

  isValid():boolean {
    return this.addressForm.get('country').value.length > 0 && this.addressForm.get('street').value.length > 0
      && this.addressForm.get('streetNumber').value.length > 0 && this.addressForm.get('zipcode').value.length > 0
      && this.addressForm.get('city').value.length > 0 && !this.addressForm.get('street').value.startsWith(' ');
  }

  constructor(private fb: FormBuilder, private osmService: OsmService) { }

  validate(control: AbstractControl): ValidationErrors | null {
    return this.addressForm.valid ? null : this.addressForm.errors;
  }

  public getAddressForm(): FormGroup {
    return this.addressForm;
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

