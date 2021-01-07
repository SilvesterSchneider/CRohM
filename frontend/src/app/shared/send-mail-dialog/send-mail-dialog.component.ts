import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MailService } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-send-mail-dialog',
  templateUrl: './send-mail-dialog.component.html',
  styleUrls: ['./send-mail-dialog.component.scss']
})
export class SendMailDialogComponent implements OnInit {
  textForm: FormGroup;
  text: string;
  showReplaceInfo = false;

  constructor(
    private fb: FormBuilder,
    private mailService: MailService,
    public dialogRef: MatDialogRef<SendMailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: MailData,
    public dialog: MatDialog) {
      this.text = data.title;
      this.showReplaceInfo = data.showReplaceInfo;
    }

  ngOnInit(): void {
    this.textForm = this.createTextForm();
    this.textForm.get('text').setValue(this.data.initText);
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
      if (this.data.subjects.length <= 1) {
        this.mailService.sendMail('1', 'Mail', this.data.subjects[0].adress, this.textForm.get('text').value,
         this.data.subjects[0].preName, this.data.subjects[0].name).subscribe(x =>
          this.dialogRef.close({ send: true }));
      } else {
        this.sendMailLoop(0);
      }
    }
  }

  sendMailLoop(index: number) {
    if (index < this.data.subjects.length) {
      this.mailService.sendMail('1', 'Mail', this.data.subjects[index].adress, this.textForm.get('text').value,
      this.data.subjects[index].preName, this.data.subjects[index].name).subscribe(x => {
        this.sendMailLoop(++index);
      });
    } else {
      this.dialogRef.close({ send: true });
    }
  }
}

export class MailData {
  showReplaceInfo: boolean;
  title: string;
  subjects: Array<MailSubject>;
  initText: string;
}

export class MailSubject {
  adress: string;
  preName: string;
  name: string;
}