import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ContactService, UsersService, DataProtectionService } from '../../shared/api-generated/api-generated';
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
import { DpUpdatePopupComponent } from 'src/app/shared/data-protection/dp-update-popup/dp-update-popup.component';
import { DataProtectionHelperService } from 'src/app/shared/data-protection';
import {MatSnackBar} from '@angular/material/snack-bar';

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
    private userService: UsersService,
    private service: ContactService,
    private changeDetectorRefs: ChangeDetectorRef,
    private dialog: MatDialog,
    private mediaObserver: MediaObserver,
    private readonly dataProtectionService: DataProtectionService,
    private readonly dsgvoService: DataProtectionHelperService,
    private readonly snackBar: MatSnackBar,
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

    dialogRef.afterClosed().subscribe((editDialogResult) => {
      if (editDialogResult.delete) {
        this.deleteContact(contact);
      } else {
        if (editDialogResult.newContact && editDialogResult.oldContact && this.jwt.isDatenschutzbeauftragter()) {
        const dialogDSGVORef = this.dialog.open(DpUpdatePopupComponent, {disableClose: true});

        dialogDSGVORef.afterClosed().subscribe(sendMessage => {
          if (sendMessage) {
            const diff = this.dsgvoService.getDiffOfObjects( editDialogResult.newContact, editDialogResult.oldContact, ['unchanged']);
            this.dataProtectionService.sendUpdateMessage({delete: false, contactChanges: diff, contact}).subscribe({error: err => {
              this.snackBar.open('oops, something went wrong', '🤷‍♂️', {
                duration: 2000,
              });
            }});
          }
        });
      }
        this.getData();
      }
    });
  }

  deleteContact(contact: ContactDto) {
    const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
      data: 'Kontakt',
      disableClose: true
    });

    deleteDialogRef.afterClosed().subscribe((deleteResult) => {
      if (deleteResult?.delete && this.jwt.isDatenschutzbeauftragter()) {

        const dialogDSGVORef = this.dialog.open(DpUpdatePopupComponent, {disableClose: true});

        dialogDSGVORef.afterClosed().subscribe(sendMessage => {
          if (sendMessage) {
            this.dataProtectionService.sendUpdateMessage({delete: true, contactChanges: null, contact}).subscribe({error: err => {
              this.snackBar.open('oops, something went wrong', '🤷‍♂️', {
                duration: 3000,
              });
            }});
            this.service.delete(contact.id).subscribe(x => this.getData());
          }
        });
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
        mail: 'a.b@fu.com' ,
        phoneNumber: '0172-9344333' + this.length,
        contactEntries: []
      }
    }).subscribe(x => this.getData());
  }
}
