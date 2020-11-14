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
  text: string;

  constructor(
    private fb: FormBuilder,
    private mailService: MailService,
    public dialogRef: MatDialogRef<ContactsSendMailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: string[],
    public dialog: MatDialog) {
      this.text = data[0];
    }

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
      this.dialogRef.close({ send: false });
    } else {
      if (this.data.length <= 2) {
        this.mailService.sendMail('1', 'Mail', this.data[1], this.textForm.get('text').value).subscribe(x =>
          this.dialogRef.close({ send: true }));
      } else {
        this.sendMailLoop(1);
      }
    }
  }

  sendMailLoop(index: number) {
    if (index < this.data.length) {
      this.mailService.sendMail('1', 'Mail', this.data[index], this.textForm.get('text').value).subscribe(x => {
        this.sendMailLoop(++index);
      });
    } else {
      this.dialogRef.close({ send: true });
    }
  }
}
