import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MailService } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-contacts-send-mail-dialog',
  templateUrl: './contacts-send-mail-dialog.component.html',
  styleUrls: ['./contacts-send-mail-dialog.component.scss']
})
export class ContactsSendMailDialogComponent implements OnInit {
  textForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private mailService: MailService,
    public dialogRef: MatDialogRef<ContactsSendMailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public address: string,
    public dialog: MatDialog) { }

  ngOnInit(): void {
    this.textForm = this.createTextForm();
  }

  private createTextForm(): FormGroup {
    return this.fb.group({
      text: ['', Validators.required]
    });
  }

  sendMail(shouldSend: boolean) {
    if (!shouldSend) {
      this.dialogRef.close();
    } else {
      this.mailService.sendMail('1', 'Mail', this.address, this.textForm.get('text').value)
      this.dialogRef.close();
    }
  }
}