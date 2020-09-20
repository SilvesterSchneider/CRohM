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
    this.mailService.getSendInvitationText(this.data).subscribe(x => {
      this.textForm.get('text').setValue(x);
    });
  }

  private createTextForm(): FormGroup {
    return this.fb.group({
      text: ['', Validators.required]
    });
  }

  sendMail(shouldSend: boolean) {
    if (!shouldSend) {
      this.dialogRef.close({ send: false });
    } else {
      this.dialogRef.close({ send: true, text: this.textForm.get('text').value });
    }
  }
}
