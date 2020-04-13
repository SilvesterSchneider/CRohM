import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { ContactDto } from '../shared/api-generated/api-generated';
import { Injectable } from '@angular/core';
import { CONTACTS } from './mock-contacts';
import { ContactCreateDto } from '../shared/api-generated/api-generated';
import { ContactsDetailComponent } from './contacts-detail/contacts-detail.component';

@Injectable({
    providedIn: 'root',
})
export class ContactsServiceMock {
    static nextId = 4;
    private contacts$: BehaviorSubject<ContactDto[]> = new BehaviorSubject<ContactDto[]>(CONTACTS);
    private contactInner: ContactDto;
    constructor() { }

    getContacts() {
        return this.contacts$.asObservable();
    }

    getById(id: number | string) {
        return this.getContacts().pipe(
            map(contacts => contacts.find(contact => contact.id === +id))
        );
    }

    addContact(contact: ContactCreateDto) {
        this.contactInner = {
            id: ContactsServiceMock.nextId++,
            name: contact.name,
            preName: contact.preName,
            address: {
                city: contact.address.city,
                country: contact.address.country,
                street: contact.address.street,
                streetNumber: contact.address.streetNumber,
                zipcode: contact.address.zipcode
            },
            contactPossibilities: {
                id: 0,
                fax: contact.contactPossibilities.fax,
                mail: contact.contactPossibilities.mail,
                phoneNumber: contact.contactPossibilities.phoneNumber
            }
        };
        CONTACTS.push(this.contactInner);
        this.contacts$.next(CONTACTS);
    }

    delete(id: number) {
        const index: number = CONTACTS.findIndex(x => x.id === id);
        if (index > -1) {
            CONTACTS.splice(index, 1);
        }
    }

    update(contact: ContactDto, id: number) {
        this.delete(id);
        CONTACTS.push(contact);
        this.contacts$.next(CONTACTS);
    }
}
