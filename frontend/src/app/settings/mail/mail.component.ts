import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MailCredentialsSerializableDto, MailService } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-mail',
  templateUrl: './mail.component.html',
  styleUrls: ['./mail.component.scss']
})
export class MailComponent implements OnInit {
  mailsForm: FormGroup;
  sendMailForm: FormGroup;
  mailCredentials: MailCredentialsSerializableDto;

  constructor(
    private readonly fb: FormBuilder,
    private readonly mailService: MailService) { }

  ngOnInit(): void {
    this.mailsForm = this.createForm();
    this.sendMailForm = this.createSendMailForm();
    this.mailService.getEmailCredentials('1').subscribe(x => {
      this.mailsForm.patchValue(x);
    });
  }

  createSendMailForm(): FormGroup {
    return this.fb.group({
			address: ['', Validators.email],
		});
  }

  createForm(): FormGroup {
    return this.fb.group({
			mailAddress: ['', Validators.email],
			displayName: ['', Validators.required],
			userName: ['', Validators.required],
			password: ['', Validators.required],
			port: ['', Validators.required],
			host: ['', Validators.required]
		});
  }

  saveValues() {
    this.mailCredentials = this.mailsForm.value;
    this.mailService.saveMailCredentials(this.mailCredentials).subscribe();
  }

  sendMail() {
    this.mailService.sendMail('1', 'Test-Email', this.sendMailForm.get('address').value).subscribe();
  }
}
