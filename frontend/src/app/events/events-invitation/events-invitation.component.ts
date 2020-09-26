import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EventDto, MailService } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-events-invitation',
  templateUrl: './events-invitation.component.html',
  styleUrls: ['./events-invitation.component.scss']
})
export class EventsInvitationComponent implements OnInit {
  textForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private mailService: MailService,
    public dialogRef: MatDialogRef<EventsInvitationComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EventDto,
    public dialog: MatDialog) { }

  ngOnInit(): void {
    this.textForm = this.createTextForm();
    this.mailService.getSendInvitationText(this.data.name, this.getDate(this.data.date), this.getTime(this.data.time)).subscribe(x => {
      this.textForm.get('text').setValue(x);
    });
  }

  private createTextForm(): FormGroup {
    return this.fb.group({
      text: ['', Validators.required]
    });
  }

  private getTime(time: string): string {
    const date: Date = new Date(time);
    let result = '';
    let hours = date.getHours().toString();
    if (hours.length === 1) {
      hours = '0' + hours;
    }
    let minutes = date.getMinutes().toString();
    if (minutes.length === 1) {
      minutes = '0' + minutes;
    }
    let seconds = date.getSeconds().toString();
    if (seconds.length === 1) {
      seconds = '0' + seconds;
    }
    result = hours + ':' + minutes + ':' + seconds;
    return result;
  }

  private getDate(date: string): string {
    let result = '';
    const dt: Date = new Date(date);
    let month = (dt.getMonth() + 1).toString();
    if (month.length === 1) {
      month = '0' + month;
    }
    let day = dt.getDate().toString();
    if (day.length === 1) {
      day = '0' + day;
    }
    result = dt.getFullYear().toString() + '-' + month + '-' + day;
    return result;
  }

  sendMail(shouldSend: boolean) {
    if (!shouldSend) {
      this.dialogRef.close({ send: false });
    } else {
      this.dialogRef.close({ send: true, text: this.textForm.get('text').value });
    }
  }
}