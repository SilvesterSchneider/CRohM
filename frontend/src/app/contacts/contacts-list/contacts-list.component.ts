import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ContactService } from '../../shared/api-generated/api-generated';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsAddHistoryComponent } from '../contacts-add-history/contacts-add-history.component';
import { ContactsInfoComponent } from '../contacts-info/contacts-info.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { ContactsEditDialogComponent } from '../contacts-edit-dialog/contacts-edit-dialog.component';
import { ContactsAddDialogComponent } from '../contacts-add-dialog/contacts-add-dialog.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})

export class ContactsListComponent implements OnInit, OnDestroy {
  contacts: Observable<ContactDto[]>;
  displayedColumns = [];
  service: ContactService;
  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;
  constructor(service: ContactService, private changeDetectorRefs: ChangeDetectorRef, private dialog: MatDialog, private mediaObserver: MediaObserver) {
    this.service = service;
    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
        if (change.mqAlias !== this.currentScreenWidth) {
            this.currentScreenWidth = change.mqAlias;
            this.setupTable();
        }
      });
  }

  ngOnInit() {
    this.getData();
  }
  
  ngOnDestroy(): void {
    this.flexMediaWatcher.unsubscribe();
  }

  setupTable() {
    if (this.currentScreenWidth === 'xs') { 
      // only display prename and name on larger screens
      this.displayedColumns = ['vorname', 'nachname', 'action'];}
      else{
      this.displayedColumns = ['vorname', 'nachname', 'stasse', 'hausnummer', 'plz', 'ort', 'land', 'telefon', 'fax', 'mail', 'action'];
    }
  }

  private getData() {
    this.contacts = this.service.getAll();
    this.changeDetectorRefs.detectChanges();
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
