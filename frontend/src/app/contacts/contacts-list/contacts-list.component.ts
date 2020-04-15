import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactDto } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})
export class ContactsListComponent implements OnInit {
  contacts: Observable<ContactDto[]>;
  displayedColumns = ['vorname', 'nachname', 'stasse', 'hausnummer', 'plz', 'ort', 'land', 'telefon', 'fax', 'mail', 'action'];
  service: ContactService;

  constructor(service: ContactService) {
    this.service = service;
   }

  ngOnInit() {
    this.init();
  }

  private init() {
    this.contacts = this.service.getAll();
    this.contacts.subscribe();
   // this.contacts = this.serviceMock.getContacts();
  }

  addContact() {
    console.log('addContact');
  }

  deleteContact(id: number) {
    this.service.delete(id).subscribe();
    this.init();
  }
}
