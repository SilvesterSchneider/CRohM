import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { Country } from '../contacts.model';
import { ContactsService } from '../contacts.service';
import { ContactService, ContactCreateDto, ContactPossibilitiesCreateDto, AddressCreateDto } from '../../shared/api-generated/api-generated';




@Component({
  selector: 'app-contacts-add',
  templateUrl: './contacts-add.component.html',
  styleUrls: ['./contacts-add.component.scss']
})
export class ContactsAddComponent implements OnInit {
  adressCreateDto : AddressCreateDto =
  {
    name: '',
    description: '',
    city: '',
    street: '',
    streetNumber: 0,
    zipcode: '',
    country: ''
  };
  contactPossibilitiesCreateDto : ContactPossibilitiesCreateDto = 
  {
    mail: '',
    phoneNumber: '',
    fax: ''
  };
  contactCreateDto : ContactCreateDto = 
  {
    name: '',
    preName: '',
    address: this.adressCreateDto,
    contactPossibilities: this.contactPossibilitiesCreateDto
  };
  

  // Liste der im Dropdown angezeigten Laender
  countries: Country[] = [
    {value: 'Deutschland', viewValue: 'Deutschland'},
    {value: 'Schweiz', viewValue: 'Schweiz'},
    {value: 'Österreich', viewValue: 'Österreich'}
  ];

  contactsForm = this.fb.group({
    vorname: ['', Validators.required],
    nachname: [''],
    adresse: this.fb.group({
      land: [''],
      strasse: [''],
      hausnr: [''],
      plz: ['', Validators.pattern('^[0-9]{5}$')],
      ort: [''],
    }),
    // Validiert auf korrektes E-Mail-Format
    mail: ['', Validators.email],
    // Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
    phone: ['', Validators.pattern('^0[0-9\- ]*$')]
    });

  constructor(private fb: FormBuilder, 
    private service: ContactsService,
    private contactService: ContactService) { }

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
    this.contactService.post(this.contactCreateDto);
  }

}
