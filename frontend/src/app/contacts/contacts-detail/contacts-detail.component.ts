import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { Contact } from '../contacts.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-contacts-detail',
  templateUrl: './contacts-detail.component.html',
  styleUrls: ['./contacts-detail.component.scss']
})
export class ContactsDetailComponent implements OnInit {
  contact: Contact;

  contactsForm = this.fb.group({
    vorname: ['', Validators.required],
    nachname: [''],
    adresse: this.fb.group({
      strasse: [''],
      plz: ['', Validators.pattern('^[0-9]{5}$')],
      ort: [''],
    })
  });

  constructor(private fb: FormBuilder, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.contact = this.route.snapshot.data.contact;
    this.contactsForm.patchValue(this.contact);
  }



  onSubmit() {
    // TODO: Use EventEmitter with form value
    console.warn(this.contactsForm.value);
  }

}
