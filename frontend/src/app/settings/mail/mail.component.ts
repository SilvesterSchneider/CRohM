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
  mailCredentials: MailCredentialsSerializableDto;

  constructor(
    private readonly fb: FormBuilder,
    private readonly mailService: MailService) { }

  ngOnInit(): void {
    this.mailsForm = this.createForm();
    this.mailService.getEmailCredentials('1').subscribe(x => {
      this.mailsForm.patchValue(x);
    });
  }

  createForm(): FormGroup {
    return this.fb.group({
			mailAddress: ['', Validators.required],
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
}
