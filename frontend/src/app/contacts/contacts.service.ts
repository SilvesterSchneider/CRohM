import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Contact } from './contacts.model';
import { CONTACTS } from './mock-contacts';

@Injectable({
    providedIn: 'root',
})
export class ContactsService {
    static nextId = 4;
    private contacts$: BehaviorSubject<Contact[]> = new BehaviorSubject<Contact[]>(CONTACTS);

    constructor() { }

    getContacts() {
        return this.contacts$.asObservable();
    }

    getContact(id: number | string) {
        return this.getContacts().pipe(
            map(contacts => contacts.find(contact => contact.id === +id))
        );
    }

    addContact(contact: Contact) {
        contact.id = ContactsService.nextId++;
        CONTACTS.push(contact);
        this.contacts$.next(CONTACTS);
    }
}
