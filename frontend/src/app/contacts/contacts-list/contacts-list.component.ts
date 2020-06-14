import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsAddHistoryComponent } from '../contacts-add-history/contacts-add-history.component';
import { ContactsInfoComponent } from '../contacts-info/contacts-info.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { ContactsEditDialogComponent } from '../contacts-edit-dialog/contacts-edit-dialog.component';
import { ContactsAddDialogComponent } from '../contacts-add-dialog/contacts-add-dialog.component';

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
    this.getData();
  }

  private getData() {
    this.contacts = this.service.getAll();
    this.changeDetectorRefs.detectChanges(); // mdie comment from raz: This is not needed if implemented properly
    // this.contacts.subscribe((x) => (this.contacts = x)); // mdie TODO implement working change subscription
    // this.contacts = this.serviceMock.getContacts();
  }

  openAddDialog() {
    const dialogRef = this.dialog.open(ContactsAddDialogComponent, {
      disableClose: true
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.contacts = this.service.getAll();
    });
  }

  openEditDialog(contact: ContactDto) {
    const dialogRef = this.dialog.open(ContactsEditDialogComponent, { data: contact, disableClose: true });

    dialogRef.afterClosed().subscribe((result) => {
      if (result.delete) {
        this.deleteContact(result.id);
      }
      this.getData();
    });
  }

  deleteContact(id: number) {
    const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
      data: 'Kontakt',
      disableClose: true
    });

    deleteDialogRef.afterClosed().subscribe((deleteResult) => {
      if (deleteResult?.delete) {
        this.service.delete(id).subscribe(x => this.getData());
      }
    });
  }

  addNote(id: number) {
    const dialogRef = this.dialog.open(ContactsAddHistoryComponent, { data: id });
    dialogRef.afterClosed().subscribe((y) => this.getData());
  }

  openInfo(id: number) {
    this.service.getById(id).subscribe((x) => this.dialog.open(ContactsInfoComponent, { data: x }));
  }
}
