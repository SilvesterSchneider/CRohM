import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import {
  ContactDto, HistoryElementType, ModificationEntryDto, ModificationEntryService,
  MODEL_TYPE, UserDto, GenderTypes, ContactService, HistoryElementDto, EventDto, ParticipatedStatus
} from '../../shared/api-generated/api-generated';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-contacts-info',
  templateUrl: './contacts-info.component.html',
  styleUrls: ['./contacts-info.component.scss']
})

export class ContactsInfoComponent extends BaseDialogInput implements OnInit {
  genderTypes: string[] = ['Männlich', 'Weiblich', 'Divers'];
  contactsForm: FormGroup;

  history: (EventDto | HistoryElementDto)[] = [];
  historyPaginationLength: number;

  modifications: ModificationEntryDto[] = new Array<ModificationEntryDto>();
  modificationsPaginationLength: number;

  displayedColumns = ['icon', 'datum', 'name', 'kommentar'];
  displayedColumnsOrganizations = ['name'];
  displayedColumnsContactPossibilities = ['name', 'kontakt'];
  displayedColumnsDataChangeHistory = ['datum', 'bearbeiter', 'feldname', 'alterWert', 'neuerWert'];

  constructor(public dialogRef: MatDialogRef<ContactsInfoComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public contact: ContactDto,
    private fb: FormBuilder,
    private modService: ModificationEntryService,
    private contactService: ContactService) {
    super(dialogRef, dialog);
  }

  hasChanged() {
    return !this.contactsForm.pristine;
  }

  onPaginationChangedModification(event: PageEvent) {
    this.loadModifications((event.pageIndex * event.pageSize), event.pageSize);
  }

  onPaginationChangedHistory(event: PageEvent) {
    this.loadHistory((event.pageIndex * event.pageSize), event.pageSize);
  }

  ngOnInit(): void {
    // Load initial modification entries
    this.loadModifications(0, 5);
    // Load initial history
    this.loadHistory(0, 5);

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

  eventParticipated(element: EventDto): boolean {
    return !!element.participated && element.participated?.some(part => part.modelType ===
      MODEL_TYPE.CONTACT && part.objectId === this.contact.id && part.hasParticipated);
  }

  eventNotParticipated(element: EventDto): boolean {
    return !!element.participated && element.participated?.some(part => part.modelType ===
      MODEL_TYPE.CONTACT && part.objectId === this.contact.id && !part.hasParticipated);
  }

  eventNotInvited(element: EventDto): boolean {
    return element.participated.some(part => part.objectId === this.contact.id && part.eventStatus === ParticipatedStatus.NOT_INVITED);
  }

  eventInvited(element: EventDto): boolean {
    return element.participated.some(part => part.objectId === this.contact.id && part.eventStatus === ParticipatedStatus.INVITED);
  }

  eventAgreed(element: EventDto): boolean {
    return element.participated.some(part => part.objectId === this.contact.id && part.eventStatus === ParticipatedStatus.AGREED);
  }

  eventCancelled(element: EventDto): boolean {
    return element.participated.some(part => part.objectId === this.contact.id && part.eventStatus === ParticipatedStatus.CANCELLED);
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

  isVisit(element: HistoryElementDto): boolean {
    return element.type === HistoryElementType.VISIT;
  }

  private loadHistory(pageStart: number, pageSize: number) {
    this.contactService.getHistory(this.contact.id, pageStart, pageSize)
      .subscribe(result => {
        this.history = result.data;
        this.historyPaginationLength = result.totalRecords;
      });
  }

  private loadModifications(pageStart: number, pageSize: number) {
    this.modService.getSortedListByTypeAndId(this.contact.id, MODEL_TYPE.CONTACT, pageStart, pageSize)
      .subscribe(result => {
        this.modifications = result.data;
        this.modificationsPaginationLength = result.totalRecords;
      });

  }
}
