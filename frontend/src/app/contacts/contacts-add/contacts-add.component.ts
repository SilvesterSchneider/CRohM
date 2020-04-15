import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { ContactCreateDto } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-contacts-add',
  templateUrl: './contacts-add.component.html',
  styleUrls: ['./contacts-add.component.scss']
})
export class ContactsAddComponent implements OnInit {
  contact: ContactCreateDto;

  contactsForm = this.fb.group({
    name: ['', Validators.required],
    preName: ['', Validators.required],
    address: this.fb.group({
      country: ['', Validators.required],
      street: ['', Validators.required],
      streetNumber: ['', Validators.required],
      zipcode: ['', Validators.pattern('^[0-9]{5}$')],
      city: ['', Validators.required],
    }),
    contactPossibilities: this.fb.group({
      // Validiert auf korrektes E-Mail-Format
      mail: ['', Validators.email],
      // Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
      phoneNumber: ['', Validators.pattern('^0[0-9\- ]*$')],
      fax: ['', Validators.pattern('^0[0-9\- ]*$')]
    })
  });
  constructor(private fb: FormBuilder, private service: ContactService) { }

  ngOnInit(): void {
  }

  addContact() {
    // Take values from Input-Form and fits them into api-dto's.
    this.contactCreateDto.name = this.contactsForm.value.nachname;
    this.contactCreateDto.preName = this.contactsForm.value.name;
    
    this.adressCreateDto.country = this.contactsForm.value.adresse.value.land;
    this.adressCreateDto.city = this.contactsForm.value.adresse.value.city;
    this.adressCreateDto.zipcode = this.contactsForm.value.adresse.value.plz;
    this.adressCreateDto.street = this.contactsForm.value.adresse.value.strasse;
    this.adressCreateDto.streetNumber = this.contactsForm.value.adresse.value.hausnr;

    this.contactPossibilitiesCreateDto.email = this.contactsForm.value.mail;
    this.contactPossibilitiesCreateDto.phoneNumber = this.contactsForm.value.phone;

    this.contactCreateDto.address = this.adressCreateDto;
    this.contactCreateDto.contactPossibilities = this.contactPossibilitiesCreateDto;

    // And add a new Contact with the service
    this.service.post(this.contact).subscribe();
  }
}
