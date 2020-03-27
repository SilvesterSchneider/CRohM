import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { Contact, Country } from '../contacts.model';
import { ContactsService } from '../contacts.service';




@Component({
  selector: 'app-contacts-add',
  templateUrl: './contacts-add.component.html',
  styleUrls: ['./contacts-add.component.scss']
})
export class ContactsAddComponent implements OnInit {
  contact: Contact;

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
      plz: ['', Validators.pattern('^[0-9]{5}$')],
      ort: [''],
    }),
    // Validiert auf korrektes E-Mail-Format
    mail: ['', Validators.email],
    // Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
    phone: ['', Validators.pattern('^0[0-9\- ]*$')]
    });

  constructor(private fb: FormBuilder, private service: ContactsService) { }

  ngOnInit(): void {
  }

  addContact() {
    // Take values from Input-Form
    this.contact = this.contactsForm.value;
    // And add a new Contact with the service
    this.service.addContact(this.contact);
  }

}
