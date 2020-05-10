import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder, FormControl } from '@angular/forms';
import { ContactCreateDto, AddressCreateDto, ContactPossibilitiesCreateDto } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-contacts-add',
  templateUrl: './contacts-add.component.html',
  styleUrls: ['./contacts-add.component.scss']
})
export class ContactsAddComponent implements OnInit {
  contactCreateDto: ContactCreateDto = {name: 'n', preName: 'n'};
  adressCreateDto: AddressCreateDto = {country: '', street: '', streetNumber: '', zipcode: '', city: ''};
  contactPossibilitiesCreateDto: ContactPossibilitiesCreateDto = {mail: '', phoneNumber: '', fax: ''};

  contactsForm = this.fb.group({
    name: ['', Validators.required],
    preName: ['', Validators.required],
    address: this.fb.control(''),
    contactPossibilities: this.fb.group({
      // Validiert auf korrektes E-Mail-Format
      mail: ['', Validators.email],
      // Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
      phoneNumber: ['', Validators.pattern('^0[0-9\- ]*$')],
      fax: ['', Validators.pattern('^0[0-9\- ]*$')],
      contactEntries: this.fb.control('')
    })
  });
  constructor(private fb: FormBuilder, private service: ContactService) { }

  ngOnInit(): void {
  }

  addContact() {
    // Take values from Input-Form and fits them into api-dto's.
    this.contactCreateDto.name = this.contactsForm.value.name;
    this.contactCreateDto.preName = this.contactsForm.value.preName;

    this.adressCreateDto.country = this.contactsForm.value.address.country;
    this.adressCreateDto.city = this.contactsForm.value.address.city;
    this.adressCreateDto.zipcode = this.contactsForm.value.address.zipcode;
    this.adressCreateDto.street = this.contactsForm.value.address.street;
    this.adressCreateDto.streetNumber = this.contactsForm.value.address.streetNumber;

    this.contactPossibilitiesCreateDto.mail = this.contactsForm.value.contactPossibilities.mail;
    this.contactPossibilitiesCreateDto.phoneNumber = this.contactsForm.value.contactPossibilities.phoneNumber;
    this.contactPossibilitiesCreateDto.fax = this.contactsForm.value.contactPossibilities.fax;
    this.contactPossibilitiesCreateDto.contactEntries = this.contactsForm.value.contactPossibilities.contactEntries;

    this.contactCreateDto.address = this.adressCreateDto;
    this.contactCreateDto.contactPossibilities = this.contactPossibilitiesCreateDto;

    // And add a new Contact with the service
    this.service.post(this.contactCreateDto).subscribe();
  }
}



