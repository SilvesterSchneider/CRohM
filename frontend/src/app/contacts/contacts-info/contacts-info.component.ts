import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import {
  ContactDto, HistoryElementType, ModificationEntryDto, ModificationEntryService,
  MODEL_TYPE, DATA_TYPE, UserDto, GenderTypes, ContactService, HistoryElementDto, EventDto
} from '../../shared/api-generated/api-generated';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';
import { sortDatesDesc } from '../../shared/util/sort';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-contacts-info',
  templateUrl: './contacts-info.component.html',
  styleUrls: ['./contacts-info.component.scss']
})

export class ContactsInfoComponent extends BaseDialogInput implements OnInit {
  contact: ContactDto;
  public genderTypes: string[] = ['Männlich', 'Weiblich', 'Divers'];
  contactsForm: FormGroup;
  history: (EventDto | HistoryElementDto)[] = [];
  dataHistory: ModificationEntryDto[] = new Array<ModificationEntryDto>();
  displayedColumns = ['icon', 'datum', 'name', 'kommentar'];
  displayedColumnsOrganizations = ['name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];
  displayedColumnsDataChangeHistory = ['datum', 'bearbeiter', 'feldname', 'alterWert', 'neuerWert'];

  constructor(
    public dialogRef: MatDialogRef<ContactsInfoComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: ContactDto,
    private fb: FormBuilder,
    private modService: ModificationEntryService,
    private contactService: ContactService) {
    super(dialogRef, dialog);
    this.contact = data;
  }

  hasChanged() {
    return !this.contactsForm.pristine;
  }

  ngOnInit(): void {
    // Load dataHistory entries
    this.modService.getSortedListByTypeAndId(this.contact.id, MODEL_TYPE.CONTACT)
      .pipe(map(list => list
        .filter(element => element.dataType !== DATA_TYPE.NONE)
        .sort((obj1, obj2) => sortDatesDesc(obj1.dateTime, obj2.dateTime))))
      .subscribe(modificationEntries => {
        this.dataHistory = modificationEntries;
      });

    // Load history; TODO: Pagination
    this.contactService.getHistory(this.contact.id)
      .pipe(map(history => history.sort((obj1, obj2) => sortDatesDesc(obj1.date, obj2.date))))
      .subscribe(history => {
        this.history = this.history.concat(history);
      });

    // Merge history with events
    this.history = this.history.concat(this.contact.events);

    this.initForm();
    this.contactsForm.patchValue(this.contact);
    this.contactsForm.get('gender').setValue(this.getGenderText(this.contact.gender));
  }

  private getGenderText(value: number): string {
    if (value === GenderTypes.MALE) {
      return this.genderTypes[0];
    } else if (value === GenderTypes.FEMALE) {
      return this.genderTypes[1];
    } else {
      return this.genderTypes[2];
    }
  }

  getUsername(user: UserDto) {
    return user ? `${user.firstName} ${user.lastName}` : 'Benutzer gelöscht';
  }

  initForm() {
    this.contactsForm = this.fb.group({
      id: [''],
      name: [''],
      preName: [''],
      gender: [this.genderTypes[0]],
      contactPartner: [''],
      address: this.fb.group({
        id: [''],
        name: [''],
        description: [''],
        street: [''],
        streetNumber: [''],
        zipcode: [''],
        city: [''],
        country: ['']
      }),
      contactPossibilities: this.fb.group({
        // Validiert auf korrektes E-Mail-Format
        mail: [''],
        // Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
        phoneNumber: [''],
        fax: ['']
      })
    });
  }

  closeDialog() {
    this.dialogRef.close();
  }

  eventParticipated(element: EventDto): boolean {
    return !!element.participated && element.participated?.some(part => part.contactId === this.contact.id);
  }

  eventNotParticipated(element: EventDto): boolean {
    return !!element.participated && !element.participated?.some(part => part.contactId === this.contact.id);
  }

  isLocalPhone(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.PHONE_CALL;
  }

  isNote(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.NOTE;
  }

  isMail(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.MAIL;
  }
}

