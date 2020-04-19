import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ContactDto } from '../../shared/api-generated/api-generated';
import { ContactService } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-contacts-detail',
  templateUrl: './contacts-detail.component.html',
  styleUrls: ['./contacts-detail.component.scss']
})
export class ContactsDetailComponent implements OnInit {
  contact: ContactDto;

  contactsForm = this.fb.group({
    id: ['', Validators.required],
    name: ['', Validators.required],
    preName: ['', Validators.required],
    address: this.fb.group({
      country: [''],
      street: [''],
      streetNumber: [''],
      zipcode: ['', Validators.pattern('^[0-9]{5}$')],
      city: [''],
    }),
    contactPossibilities: this.fb.group({
// Validiert auf korrektes E-Mail-Format
      mail: ['', Validators.email],
// Laesst beliebige Anzahl an Ziffern, Leerzeichen und Bindestrichen zu, Muss mit 0 beginnen
      phoneNumber: ['', Validators.pattern('^0[0-9\- ]*$')],
      fax: ['', Validators.pattern('^0[0-9\- ]*$')]
    })
  });

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private service: ContactService) { }

  ngOnInit(): void {
    this.contact = this.route.snapshot.data.contact;
    this.contactsForm.patchValue(this.contact);
  }

  updateContact() {
    const idAddress = this.contact.address.id;
    const idContactPossibilities = this.contact.contactPossibilities.id;
    const idContact = this.contact.id;
    this.contact = this.contactsForm.value;
    this.contact.id = idContact;
    this.contact.address.id = idAddress;
    this.contact.contactPossibilities.id = idContactPossibilities;
    this.service.put(this.contact, this.contact.id).subscribe();
  }
}
