import {
  Component, OnInit, Inject
} from '@angular/core';
import { EventDto } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';

export class ContactDtoExtended {
  id: number;
  preName: string;
  name: string;
  participated: boolean;
}

@Component({
  selector: 'app-events-info',
  templateUrl: './events-info.component.html',
  styleUrls: ['./events-info.component.scss']
})

export class EventsInfoComponent implements OnInit {
  contacts: ContactDtoExtended[] = new Array<ContactDtoExtended>();
  event: EventDto;
  eventsForm: FormGroup;
  columnsContacts = ['participated', 'prename', 'name'];

  constructor(
    public dialogRef: MatDialogRef<EventsInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EventDto,
    private eventService: EventService,
    private fb: FormBuilder,
  ) {
    this.event = data;
  }

  ngOnInit() {
    this.eventsForm = this.createEventsForm();
    this.event.contacts.forEach(x => {
      this.contacts.push({
        id: x.id,
        preName: x.preName,
        name: x.name,
        participated: false
      });
    });
    this.event.participated.forEach(x => {
      const cont: ContactDtoExtended = this.contacts.find(y => y.id === x.contactId);
      if (cont != null) {
        cont.participated = x.hasParticipated;
      }
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

  close() {
    this.dialogRef.close();
  }
}
