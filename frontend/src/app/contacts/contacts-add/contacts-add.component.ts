import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { Contact } from '../contacts.model';
import { ContactsService } from '../contacts.service';

@Component({
  selector: 'app-contacts-add',
  templateUrl: './contacts-add.component.html',
  styleUrls: ['./contacts-add.component.scss']
})
export class ContactsAddComponent implements OnInit {
  contact: Contact;

  contactsForm = this.fb.group({
    vorname: ['', Validators.required],
    nachname: [''],
    adresse: this.fb.group({
      strasse: [''],
      plz: ['', Validators.pattern('^[0-9]{5}$')],
      ort: [''],
    })
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
