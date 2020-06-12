import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsAddHistoryComponent } from '../contacts-add-history/contacts-add-history.component';
import { ContactsInfoComponent } from '../contacts-info/contacts-info.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})

export class ContactsListComponent implements OnInit {
  contacts: Observable<ContactDto[]>;
  displayedColumns = ['vorname', 'nachname', 'stasse', 'hausnummer', 'plz', 'ort', 'land', 'telefon', 'fax', 'mail', 'action'];
  service: ContactService;

  constructor(service: ContactService, private changeDetectorRefs: ChangeDetectorRef, private dialog: MatDialog) {
    this.service = service;
  }

  ngOnInit() {
    this.init();
  }

  private init() {
    this.contacts = this.service.getAll();
    this.contacts.subscribe();
    this.changeDetectorRefs.detectChanges();
    // this.contacts = this.serviceMock.getContacts();
  }

  addContact() {
    console.log('addContact');
  }

  deleteContact(id: number) {
    const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
      data: 'Kontakt',
      disableClose: true
    });

    deleteDialogRef.afterClosed().subscribe((deleteResult) => {
      if (deleteResult.delete) {
        this.service.delete(id).subscribe(x => this.init());
      }
    });

  }

  addNote(id: number) {
    const dialogRef = this.dialog.open(ContactsAddHistoryComponent, { data: id, disableClose: true });
    dialogRef.afterClosed().subscribe(y => this.init());
  }

  openInfo(id: number) {
    this.service.getById(id).subscribe(x => this.dialog.open(ContactsInfoComponent, { data: x, disableClose: true }));
  }
}
