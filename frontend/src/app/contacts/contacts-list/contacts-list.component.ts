import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ContactService, UsersService, DataProtectionService, GenderTypes } from '../../shared/api-generated/api-generated';
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
import { DataProtectionHelperService, DpUpdatePopupComponent } from 'src/app/shared/data-protection';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ContactsDisclosureDialogComponent } from '../contacts-disclosure-dialog/contacts-disclosure-dialog.component';
import { TagsFilterComponent } from 'src/app/shared/tags-filter/tags-filter.component';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})

export class ContactsListComponent implements OnInit, OnDestroy {
  @ViewChild(TagsFilterComponent, { static: true })
  tagsFilter: TagsFilterComponent;
  contacts: Observable<ContactDto[]>;
  displayedColumns = [];
  isAdminUserLoggedIn = false;
  permissionAdd = false;
  permissionModify = false;
  permissionDelete = false;
  permissionAddHistory = false;
  permissionInformContact = false;

  length = 0;
  currentScreenWidth = '';
  flexMediaWatcher: Subscription;
  allContacts: ContactDto[];
  dataSource = new MatTableDataSource<ContactDto>();

  constructor(
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

  applyTagFilter() {
    this.dataSource = new MatTableDataSource<ContactDto>();
    this.allContacts.forEach(x => {
      if (this.tagsFilter.areAllTagsIncluded(x.tags)) {
        this.dataSource.data.push(x);
      }
    });
  }

  ngOnInit() {
    this.isAdminUserLoggedIn = this.jwt.getUserId() === 1;
    this.tagsFilter.setRefreshTableFunction(() => this.applyTagFilter());
    this.getData();
    this.permissionAdd = this.jwt.hasPermission('Anlegen eines Kontakts');
    this.permissionModify = this.jwt.hasPermission('Einsehen und Bearbeiten aller Kontakte');
    this.permissionDelete = this.jwt.hasPermission('Löschen eines Kontakts');
    this.permissionAddHistory = this.jwt.hasPermission('Hinzufügen eines Historieneintrags bei Kontakt oder Organisation');
    this.permissionInformContact = this.jwt.hasPermission('Auskunft gegenüber eines Kontakts zu dessen Daten');
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
      this.dataSource.data = x;
      this.allContacts = x;
      this.tagsFilter.updateTagsInAutofill(this.allContacts);
      this.applyTagFilter();
    });
    this.changeDetectorRefs.detectChanges();
  }

  openDisclosureDialog(id: number) {
    this.service.getById(id).subscribe((x) => {
      const dialogRef = this.dialog.open(ContactsDisclosureDialogComponent, { data: x, disableClose: true });

      dialogRef.afterClosed().subscribe((result) => {
        this.contacts = this.service.getAll();
      });
    });
  }

  openAddDialog() {
    const dialogRef = this.dialog.open(ContactsAddDialogComponent, {
      disableClose: true
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.getData();
    });
  }

  openEditDialog(contact: ContactDto) {
    const dialogRef = this.dialog.open(ContactsEditDialogComponent, { data: contact, disableClose: true });

    dialogRef.afterClosed().subscribe((editDialogResult) => {
      if (editDialogResult.delete) {
        this.deleteContact(contact);
      } else {
        if (editDialogResult.newContact && editDialogResult.oldContact && this.jwt.isDatenschutzbeauftragter()) {
          const dialogDSGVORef = this.dialog.open(DpUpdatePopupComponent, { disableClose: true });

          dialogDSGVORef.afterClosed().subscribe(sendMessage => {
            if (sendMessage) {
              const diff = this.dsgvoService.getDiffOfObjects(editDialogResult.newContact, editDialogResult.oldContact, ['unchanged']);
              this.dataProtectionService.sendUpdateMessage({ delete: false, contactChanges: diff, contact }).subscribe({
                error: err => {
                  this.snackBar.open('oops, something went wrong', '🤷‍♂️', {
                    duration: 2000,
                  });
                }
              });
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
      if (deleteResult?.delete) {
        this.service.delete(contact.id).subscribe(x => {
          this.service.getAll().subscribe(fu => {
            this.dataSource.data = fu;
          });
        });
        if (this.jwt.isDatenschutzbeauftragter()) {
          const dialogDSGVORef = this.dialog.open(DpUpdatePopupComponent, { disableClose: true });

          dialogDSGVORef.afterClosed().subscribe(sendMessage => {
            if (sendMessage) {
              this.dataProtectionService.sendUpdateMessage({ delete: true, contactChanges: null, contact }).subscribe({
                error: err => {
                  this.snackBar.open('oops, something went wrong', '🤷‍♂️', {
                    duration: 3000,
                  });
                }
              });
            }
          });
        }
      }
    });
  }

  addNote(id: number) {
    const dialogRef = this.dialog.open(AddHistoryComponent);
    dialogRef.afterClosed().subscribe((y) => {
      if (y) {
        this.service.postHistoryElement(y, id).subscribe(x => this.getData());
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
      gender: GenderTypes.MALE,
      address: {
        city: 'Stadt',
        country: 'Land',
        street: 'Strasse',
        streetNumber: this.length.toString(),
        zipcode: '12345'
      },
      contactPossibilities: {
        fax: '01234-123' + this.length,
        mail: 'a.b@fu.com',
        phoneNumber: '0172-9344333' + this.length,
        contactEntries: []
      }
    }).subscribe(x => this.getData());
  }
}
