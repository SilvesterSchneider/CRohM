import { Component, OnInit, Inject } from '@angular/core';
import { EventDto, ModificationEntryDto, ModificationEntryService, MODEL_TYPE, DATA_TYPE } from '../../shared/api-generated/api-generated';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';
import { map } from 'rxjs/operators';

export class ContactOrganizationDtoExtended {
  id: number;
  preName: string;
  name: string;
  modelType: MODEL_TYPE;
  participated: boolean;
  wasInvited: boolean;
}

@Component({
  selector: 'app-events-info',
  templateUrl: './events-info.component.html',
  styleUrls: ['./events-info.component.scss']
})

export class EventsInfoComponent extends BaseDialogInput<EventsInfoComponent> implements OnInit {
  contactsOrganizations: ContactOrganizationDtoExtended[] = new Array<ContactOrganizationDtoExtended>();
  eventsForm: FormGroup;
  dataHistory: ModificationEntryDto[] = new Array<ModificationEntryDto>();
  columnsContacts = ['wasInvited', 'participated', 'prename', 'name'];
  displayedColumnsDataChangeHistory = ['datum', 'bearbeiter', 'feldname', 'alterWert', 'neuerWert'];

  constructor(public dialogRef: MatDialogRef<EventsInfoComponent>,
              public dialog: MatDialog,
              @Inject(MAT_DIALOG_DATA) public event: EventDto,
              private fb: FormBuilder,
              private modService: ModificationEntryService
  ) {
    super(dialogRef, dialog);
  }

  getDate(date: string): string {
    const dateUsed = new Date(date);
    return dateUsed.getFullYear().toString() + '-' + (+dateUsed.getMonth() + 1).toString() + '-' + dateUsed.getDate().toString();
  }

  getSortHistoryFunction(a: ModificationEntryDto, b: ModificationEntryDto) {
    return new Date(b.dateTime).getTime() - new Date(a.dateTime).getTime();
  }

  hasChanged() {
    return !this.eventsForm.pristine;
  }

  ngOnInit() {
    this.eventsForm = this.createEventsForm();
    if (this.event.contacts != null) {
      this.event.contacts.forEach(x => {
        this.contactsOrganizations.push({
          id: x.id,
          preName: x.preName,
          name: x.name,
          participated: false,
          wasInvited: false,
          modelType: MODEL_TYPE.CONTACT
        });
      });
    }
    if (this.event.organizations != null) {
      this.event.organizations.forEach(x => {
        this.contactsOrganizations.push({
          id: x.id,
          preName: x.name,
          name: x.description,
          participated: false,
          wasInvited: false,
          modelType: MODEL_TYPE.ORGANIZATION
        });
      });
    }
    if (this.event.participated != null) {
      this.event.participated.forEach(x => {
        let cont: ContactOrganizationDtoExtended = null;
        if (x.modelType === MODEL_TYPE.CONTACT) {
          cont = this.contactsOrganizations.find(y => y.modelType === MODEL_TYPE.CONTACT && y.id === x.objectId);
        } else {
          cont = this.contactsOrganizations.find(y => y.modelType === MODEL_TYPE.ORGANIZATION && y.id === x.objectId);
        }
        if (cont != null) {
          cont.participated = x.hasParticipated;
          cont.wasInvited = x.wasInvited;
        }
      });
    }
    this.modService.getSortedListByTypeAndId(this.event.id, MODEL_TYPE.EVENT)
      .pipe(map(mod => mod.data), map(mod => mod.filter(el => el.dataType !== DATA_TYPE.NONE).sort(this.getSortHistoryFunction)))
      .subscribe(result => {
        this.dataHistory = result;
      });
    this.eventsForm.patchValue(this.event);
    this.eventsForm.get('date').patchValue(this.formatDate(this.event.date));
    this.eventsForm.get('time').patchValue(this.formatTime(this.event.time));
  }

  formatDate(date: string): any {
    const d = new Date(date);
    let days = '' + (d.getDate());
    let months = '' + (d.getMonth() + 1);
    const year = '' + d.getFullYear();
    if (days.length < 2) {
      days = '0' + days;
    }
    if (months.length < 2) {
      months = '0' + months;
    }
    return [year, months, days].join('-');
  }

  private formatTime(date) {
    const d = new Date(date);
    let hours = '' + (d.getHours());
    let minutes = '' + d.getMinutes();
    if (hours.length < 2) {
      hours = '0' + hours;
    }
    if (minutes.length < 2) {
      minutes = '0' + minutes;
    }
    return [hours, minutes].join(':');
  }

  private createEventsForm(): FormGroup {
    return this.fb.group({
      name: [''],
      date: [''],
      time: [''],
      duration: ['']
    });
  }
}
