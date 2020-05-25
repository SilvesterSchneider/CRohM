import {
  Component, OnInit, Inject
} from '@angular/core';
import { ContactDto, EventDto } from '../../shared/api-generated/api-generated';
import { EventService } from '../../shared/api-generated/api-generated';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-events-info',
  templateUrl: './events-info.component.html',
  styleUrls: ['./events-info.component.scss']
})

export class EventsInfoComponent implements OnInit {
  contacts: ContactDto[];
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
    this.contacts = this.event.contacts;
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

  participated(contactId: number): boolean {
    this.event.participated.forEach(x => {
      if (x.contactId === contactId) {
        return x.hasParticipated;
      }
    });
    return false;
  }
}
