import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs';
import { ContactService, UsersService } from '../../shared/api-generated/api-generated';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsAddHistoryComponent } from '../contacts-add-history/contacts-add-history.component';
import { ContactsInfoComponent } from '../contacts-info/contacts-info.component';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})

export class ContactsListComponent implements OnInit {
  contacts: Observable<ContactDto[]>;
  displayedColumns = ['vorname', 'nachname', 'stasse', 'hausnummer', 'plz', 'ort', 'land', 'telefon', 'fax', 'mail', 'action'];
  service: ContactService;
  isAdminUserLoggedIn: boolean = false;
  length: number = 0;

  constructor(
    service: ContactService,
    private changeDetectorRefs: ChangeDetectorRef,
    private dialog: MatDialog,
    private userService: UsersService) {
    this.service = service;
   }

  ngOnInit() {
    this.init();
    this.userService.getLoggedInUser(1).subscribe(x => {
      this.isAdminUserLoggedIn = x.id === 1;
    });
  }

  private init() {
    this.contacts = this.service.getAll();
    this.contacts.subscribe(x => this.length = x.length);
    this.changeDetectorRefs.detectChanges();
   // this.contacts = this.serviceMock.getContacts();
  }

  addContact() {
    console.log('addContact');
  }

  deleteContact(id: number) {
    this.service.delete(id).subscribe(x => this.init());
  }

  addNote(id: number) {
    const dialogRef = this.dialog.open(ContactsAddHistoryComponent, { data: id });
    dialogRef.afterClosed().subscribe(y => this.init());
  }

  openInfo(id: number) {
    this.service.getById(id).subscribe(x => this.dialog.open(ContactsInfoComponent, { data: x }));
  }

  addDummyContact() {
    this.service.post({
      name: 'Nachname' + this.length,
      preName: 'Vorname' + this.length,
      address: {
        city: 'Stadt' + this.length,
        country: 'Land' + this.length,
        street: 'Strasse' + this.length,
        streetNumber: this.length.toString(),
        zipcode: '12345'
      },
      contactPossibilities: {
        fax: '01234-123' + this.length,
        mail: 'info@test' + this.length + '.de',
        phoneNumber: '0172-9344333' + this.length,
        contactEntries: []
      }
    }).subscribe(x => this.init());
  }
}
