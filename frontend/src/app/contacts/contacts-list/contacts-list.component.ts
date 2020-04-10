import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ContactsServiceMock } from '../contacts.service-mock';
import { ContactService } from '../../shared/api-generated/api-generated'
import { ContactDto } from '../../shared/api-generated/api-generated'

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})
export class ContactsListComponent implements OnInit {
  contacts: Observable<ContactDto[]>;
  displayedColumns = ['vorname', 'nachname', 'stasse', 'hausnummer', 'plz', 'ort', 'land', 'telefon', 'fax', 'mail', 'action'];
  serviceMock: ContactsServiceMock;
  service: ContactService;

  constructor(serviceMock: ContactsServiceMock, service: ContactService) {
    this.serviceMock = serviceMock;
    this.service = service;
   }

  ngOnInit() {
    this.init();
  }

  private init() {
    this.contacts = this.service.get();
    this.contacts.subscribe();
   // this.contacts = this.serviceMock.getContacts();
  }

  addContact() {
    console.log('addContact');
  }

  deleteContact(id: number) {
    this.service.delete(id);
    this.serviceMock.delete(id);
    this.init();
  }
}
