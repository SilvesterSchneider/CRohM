import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EventDto, MailService } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-events-invitation',
  templateUrl: './events-invitation.component.html',
  styleUrls: ['./events-invitation.component.scss']
})
/// <summary>
/// RAM: 100%
/// </summary>
export class EventsInvitationComponent implements OnInit {
  textForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private mailService: MailService,
    public dialogRef: MatDialogRef<EventsInvitationComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EventDto,
    public dialog: MatDialog) {
      this.dialogRef.backdropClick().subscribe(() => {
        // Close the dialog
        dialogRef.close();
      });
    }

  ngOnInit(): void {
    this.textForm = this.createTextForm();
    this.mailService.getSendInvitationText(this.data.name, this.getDate(this.data.date), this.data.starttime).subscribe(x => {
      this.textForm.get('text').setValue(x);
    });
  }

  private createTextForm(): FormGroup {
    return this.fb.group({
      text: ['', Validators.required]
    });
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
