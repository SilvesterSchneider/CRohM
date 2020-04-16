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
    address: this.fb.control(''),
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
    // Take values from Input-Form
    this.contact = this.contactsForm.value;
    // And add a new Contact with the service
    this.service.post(this.contact).subscribe();
  }
}
