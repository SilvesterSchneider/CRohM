import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ContactService, UsersService } from '../../shared/api-generated/api-generated';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { ContactsInfoComponent } from '../contacts-info/contacts-info.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { ContactsEditDialogComponent } from '../contacts-edit-dialog/contacts-edit-dialog.component';
import { ContactsAddDialogComponent } from '../contacts-add-dialog/contacts-add-dialog.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { JwtService } from 'src/app/shared/jwt.service';
import { AddHistoryComponent } from 'src/app/shared/add-history/add-history.component';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})

export class ContactsListComponent implements OnInit, OnDestroy {
  contacts: Observable<ContactDto[]>;
  displayedColumns = [];
  isAdminUserLoggedIn = false;
  length = 0;
  currentScreenWidth = '';
  flexMediaWatcher: Subscription;
  dataSource = new MatTableDataSource<ContactDto>();

  constructor(
    private service: ContactService,
    private changeDetectorRefs: ChangeDetectorRef,
    private dialog: MatDialog,
    private mediaObserver: MediaObserver,
    private jwt: JwtService) {
      this.flexMediaWatcher = mediaObserver.asObservable().subscribe((change: MediaChange[]) => {
      if (change[0].mqAlias !== this.currentScreenWidth) {
        this.currentScreenWidth = change[0].mqAlias;
        this.setupTable();
      }
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    this.dataSource.filterPredicate = ((data, filter) => {
      if (data.preName.trim().toLowerCase().includes(filter) || data.name.trim().toLowerCase().includes(filter) ||
        data.address.street.trim().toLowerCase().includes(filter) ||
        data.address.streetNumber.trim().toLowerCase().includes(filter) || data.address.zipcode.trim().toLowerCase().includes(filter) ||
        data.address.city.trim().toLowerCase().includes(filter) || data.address.country.trim().toLowerCase().includes(filter) ||
        data.contactPossibilities.phoneNumber.trim().toLowerCase().includes(filter) ||
        data.contactPossibilities.fax.trim().toLowerCase().includes(filter) ||
        data.contactPossibilities.mail.trim().toLowerCase().includes(filter)) {
        return true;
      } else {
        return false;
      }
    });
  }

  ngOnInit() {
    this.isAdminUserLoggedIn = this.jwt.getUserId() === 1;
    this.getData();
  }

  ngOnDestroy(): void {
    this.flexMediaWatcher.unsubscribe();
  }

  setupTable() {
    if (this.currentScreenWidth === 'xs') {
      // only display prename and name on larger screens
      this.displayedColumns = ['vorname', 'nachname', 'action'];
    } else {
      this.displayedColumns = ['vorname', 'nachname', 'stasse', 'hausnummer', 'plz', 'ort', 'land', 'telefon', 'fax', 'mail', 'action'];
    }
  }

  private getData() {
    this.contacts = this.service.getAll();
    this.contacts.subscribe(x => {
       this.length = x.length;
       x.forEach(y => this.dataSource.data = x);
    });
    this.changeDetectorRefs.detectChanges();
  }

  openDisclosureDialog() {
    // mdie todo: create dialog
    
		;
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
    const dialogRef = this.dialog.open(AddHistoryComponent);
    dialogRef.afterClosed().subscribe((y) => {
      if (y) {
        this.service.postHistoryElement(y, id, this.jwt.getUserId()).subscribe(x => this.getData());
      }
    });
  }

  openInfo(id: number) {
    this.service.getById(id).subscribe((x) => this.dialog.open(ContactsInfoComponent, { data: x }));
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
    }, this.jwt.getUserId()).subscribe(x => this.getData());
  }
}
