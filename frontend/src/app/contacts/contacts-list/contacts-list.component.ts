import { Component, OnInit } from '@angular/core';
import { Contact } from '../contacts.model';
import { Observable } from 'rxjs';
import { ContactsService } from '../contacts.service';
import { ROUTES } from '@angular/router';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})
export class ContactsListComponent implements OnInit {
  contacts$: Observable<Contact[]>;
  displayedColumns = ['vorname', 'nachname', 'adresse', 'action'];

  constructor(private service: ContactsService) { }

  ngOnInit() {
    this.contacts$ = this.service.getContacts();
  }

  addContact() {
    console.log('addContact');
  }

  deleteContact(id: number) {
    // TODO
  }

}
